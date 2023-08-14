#pragma once
/*
Разработка временно замороженна
*/
#include<string>
#include <cstring>
#include <winsock2.h>
#include<regex>


#pragma comment(lib, "ws2_32.lib")
class PacketDevice {
public :
	std::string PublicKey;
	std::string Date;
	PacketDevice(std::string PublicKey) {
		this->PublicKey = PublicKey;
	}
	PacketDevice(std::string PublicKey,std::string Date) {
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

private:

};
class Injection {
private:
	std::string Ip;
	int Port;

	std::string Login;
	std::string Password;

	SOCKET Connect;
public :
	Injection(std::string Login, std::string Password, std::string Ip = "127.0.0.1", int Port = 2745) {
		this->Ip = Ip;
		this->Port = Port;

		this->Login = Login;
		this->Password = Password;
	}

	void Start() {
		InilizeWinSock();

	}
private :
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

	
};
