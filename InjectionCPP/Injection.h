#pragma once
/*
Разработка временно замороженна
*/
#include<string>
#include <cstring>
#include <winsock2.h>
#include<regex>
#include"cryptlib.h"
#include<rsa.h>
#include<osrng.h>
#include<cryptlib.h>
#include<base64.h>
#include<hex.h>
#include<files.h>
#include<queue.h>
#include<pem.h>
#include<aes.h>
#include<modes.h>


#pragma comment(lib, "ws2_32.lib")
class PacketDevice {
public:
	std::string PublicKey;
	std::string Date;
	PacketDevice(std::string PublicKey) {
		this->PublicKey = PublicKey;
	}
	PacketDevice(std::string PublicKey, std::string Date) {
		this->PublicKey = PublicKey;
		this->Date = Date;
	}
	static PacketDevice ConvertFromString(std::string input)
	{
		std::string pattern = R"(\[(.*?)\])";
		std::regex reg(pattern);

		std::vector<std::string> matches;
		auto matchBegin = std::sregex_iterator(input.begin(), input.end(), reg);
		auto matchEnd = std::sregex_iterator();

		for (std::sregex_iterator it = matchBegin; it != matchEnd; ++it) {
			if (matches.size() >= 2) {
				break;
			}
			std::smatch match = *it;
			matches.push_back(match[1]);
		}

		if (matches.size() == 2) {
			return PacketDevice(matches[0], matches[1]);
		}

		return PacketDevice("", "");
	}
	std::string ToString() {
		return (std::string)"[" + this->PublicKey + "]" + "[" + this->Date + "]";
	}

};
class Injection {
private:
	std::string Ip;
	int Port;

	std::string Login;
	std::string Password;

	SOCKET Connect;
public:
	Injection(std::string Login, std::string Password, std::string Ip = "127.0.0.1", int Port = 2745) {
		this->Ip = Ip;
		this->Port = Port;

		this->Login = Login;
		this->Password = Password;
	}

	void Start() {
		InilizeWinSock();
		ConnectionToServer();

	}
private:
	void Send(std::string Date) {

	}

	void ConnectionToServer() {
		struct sockaddr_in serverAddress;
		serverAddress.sin_family = AF_INET;
		serverAddress.sin_port = htons(Port);
		serverAddress.sin_addr.s_addr = inet_addr(Ip.c_str());

		if (connect(Connect, (struct sockaddr*)&serverAddress, sizeof(serverAddress)) == SOCKET_ERROR) {
			closesocket(Connect);
			WSACleanup();
			throw std::exception("Ошибка при подключении к серверу");

		}
	}

	void InilizeWinSock() {
		WSADATA wsaData;
		if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
			throw std::exception("Ошибка при инициализации WinSock");
		}
	}

	void GenerateKeys(std::string& publicKeyXML, std::string& privateKeyXML)
	{
		CryptoPP::AutoSeededRandomPool rng;
		CryptoPP::RSA::PrivateKey privateKey;
		CryptoPP::RSA::PublicKey publicKey;

		CryptoPP::InvertibleRSAFunction params;
		params.GenerateRandomWithKeySize(rng, 2048);

		privateKey = CryptoPP::RSA::PrivateKey(params);
		publicKey = CryptoPP::RSA::PublicKey(params);

		CryptoPP::ByteQueue privateKeyQueue, publicKeyQueue;

		privateKey.Save(privateKeyQueue);
		publicKey.Save(publicKeyQueue);

		privateKeyXML.resize(privateKeyQueue.MaxRetrievable());
		publicKeyXML.resize(publicKeyQueue.MaxRetrievable());

		privateKeyQueue.Get(reinterpret_cast<CryptoPP::byte*>(&privateKeyXML[0]), privateKeyXML.size());
		publicKeyQueue.Get(reinterpret_cast<CryptoPP::byte*>(&publicKeyXML[0]), publicKeyXML.size());
	}

	std::string EncryptWithPublicKey(const std::string& plainText, const std::string& publicKeyXML)
	{
		CryptoPP::RSA::PublicKey publicKey;
		publicKey.Load(CryptoPP::BERSequenceDecoder(reinterpret_cast<const CryptoPP::byte*>(publicKeyXML.data()), publicKeyXML.size()));

		CryptoPP::AutoSeededRandomPool rng;

		std::string cipher;
		CryptoPP::RSAES_OAEP_SHA_Encryptor e(publicKey);
		CryptoPP::StringSource(plainText, true, new CryptoPP::PK_EncryptorFilter(rng, e, new CryptoPP::StringSink(cipher)));

		std::string encodedCipher;
		CryptoPP::StringSource(cipher, true, new CryptoPP::Base64Encoder(new CryptoPP::StringSink(encodedCipher)));

		return encodedCipher;
	}

	std::string DecryptWithPrivateKey(const std::string& encryptedText, const std::string& privateKeyXML)
	{
		CryptoPP::RSA::PrivateKey privateKey;
		privateKey.Load(CryptoPP::BERSequenceDecoder(reinterpret_cast<const CryptoPP::byte*>(privateKeyXML.data()), privateKeyXML.size()));

		CryptoPP::AutoSeededRandomPool rng;

		std::string encodedCipher;
		CryptoPP::StringSource(encryptedText, true, new CryptoPP::Base64Decoder(new CryptoPP::StringSink(encodedCipher)));

		std::string decrypted;
		CryptoPP::RSAES_OAEP_SHA_Decryptor d(privateKey);
		CryptoPP::StringSource(encodedCipher, true, new CryptoPP::PK_DecryptorFilter(rng, d, new CryptoPP::StringSink(decrypted)));

		return decrypted;
	}



};
