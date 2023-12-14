using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace MyAssistentDLL.Module.Internet
{
    internal class Uploader
    {
       
        public string Log;

        /// <summary>
        /// Send/Receive timeout in seconds
        /// Default is 5 seconds
        /// </summary>
        public int Timeout = 5;

        /// <summary>
        /// Update a device running the Arduino OTA (Over The Air) module with new firmware
        /// </summary>
        /// <param name="deviceAddress">IP address of the device that we are updating</param>
        /// <param name="devicePort">Port number on which ArduinoOTA is set to listen</param>
        /// <param name="password">ArduinoOTA password if requested</param>
        /// <param name="firmware">Stream containing the new firmware</param>
        /// <returns>true if upload was successful</returns>
        /// 
        public bool FirmwareUpload(string deviceAddress, int devicePort, string password, Stream firmware)
        {
            //
            // code adapted from Arduino's espota.py
            //
            bool success = false;
            UdpClient udpClient = new UdpClient();
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            Log = "";

            try
            {
                string hash;

                // get the MD5 hash of the file
                using (var md5Hash = MD5.Create())
                {
                    // Generate hash value(Byte Array) for input data
                    var hashBytes = md5Hash.ComputeHash(firmware);

                    // Convert hash byte array to string
                    hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
                }

                // ArduinoOTA will communicate back to us on 2 different ports
                // The first port is the local port number from which we are sending the data
                // The second port is any port we specify in the first command we send to the device
                // It sounds easier to use the same port for both, which is what we are doing here
                // We need the local port number to send it to the device in the first command
                if (udpClient.Client.LocalEndPoint == null)
                {
                    udpClient.Connect(deviceAddress, devicePort);
                    udpClient.Client.SendTimeout = Timeout * 1000;
                    udpClient.Client.ReceiveTimeout = Timeout * 1000;
                }
                int localPport = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;

                // the first command is U_FLASH (value 0) followed by the local port, the file length and and the file hash
                String command = String.Format("0 {0} {1} {2}\n", localPport, firmware.Length, hash);

                Byte[] sendBytes = Encoding.ASCII.GetBytes(command);
                udpClient.Send(sendBytes, sendBytes.Length);

                // Blocks until a message is returned from the device
                Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);

                string returnData = Encoding.ASCII.GetString(receiveBytes);

                if (returnData.StartsWith("AUTH"))
                {
                    // device is requesting the password in a very specific format
                    string nonce = returnData.Split(' ')[1];
                    string cnonce = Hash(String.Format("{0}{1}{2}", firmware.Length, hash, deviceAddress));      // this hash is not actually used by ArduinoOTA so any random 32 character string will do here
                    string result = Hash(String.Format("{0}:{1}:{2}", Hash(password), nonce, cnonce));

                    string message = String.Format("200 {0} {1}\n", cnonce, result);   // 200 = AUTH

                    // send the hashed password and wait for OK or ERR return from device
                    sendBytes = Encoding.ASCII.GetBytes(message);
                    udpClient.Send(sendBytes, sendBytes.Length);
                    receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                    returnData = Encoding.ASCII.GetString(receiveBytes);

                    if (!returnData.StartsWith("OK"))
                    {
                        Exception e = new Exception("Authentication Failed");
                    }
                }

                // send firmware data to device in small blocks
                const int block_size = 1460;

                // start a TCP listener on the same port as the UDP listener and wait for device to connect back to us
                TcpListener listener = new TcpListener(IPAddress.Any, localPport);
                listener.Server.ReceiveTimeout = listener.Server.SendTimeout = Timeout * 1000;
                listener.Start();
                TcpClient tcpClient = listener.AcceptTcpClient();
                tcpClient.SendBufferSize = block_size;

                // rewind file to beginning, the call to ComputeHash moves the file pointer
                firmware.Seek(0, SeekOrigin.Begin);

                byte[] buf = new byte[block_size];
                string ret = "";

                while (true)
                {
                    int bytes_read = firmware.Read(buf, 0, block_size);
                    if (bytes_read > 0)
                    {
                        tcpClient.GetStream().Write(buf, 0, bytes_read);
                        int len = tcpClient.GetStream().Read(buf, 0, block_size);     // ignore any returned value
                        ret = Encoding.ASCII.GetString(buf, 0, len);
                    }

                    if (bytes_read < block_size)        // end of file
                        break;
                }

                // keep reading until we receive ERR or OK
                while (!ret.Contains("ERR") && !ret.Contains("OK"))
                {
                    int len = tcpClient.GetStream().Read(buf, 0, block_size);     // ignore any returned value
                    ret = Encoding.ASCII.GetString(buf, 0, len);
                }

                success = ret.Contains("OK");

                Log += "========== FLASH ==========\nFrom " + RemoteIpEndPoint.Address.ToString() + ":" + RemoteIpEndPoint.Port.ToString() + "\n";
                Log += ret;

                // close all connections
                tcpClient.Close();
                listener.Stop();
                udpClient.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return success;
        }

        static string Hash(string value)
        {
            string hash = "";

            using (var md5Hash = MD5.Create())
            {
                // Generate hash value(Byte Array) for input data
                var hashBytes = md5Hash.ComputeHash(Encoding.ASCII.GetBytes(value));

                // Convert hash byte array to string
                // ComputeHash returns upper case letters with each byte separated by a dash
                // we need to remove the dashes and convert to lower case for Arduino
                hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
            }

            return hash;
        }
    }
}
