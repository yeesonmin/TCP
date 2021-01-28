#include <stdio.h>
#include <stdlib.h>
#include <winsock2.h>
#include <Windows.h>

#define SEVER_PORT 4000
#pragma comment(lib,"ws2_32.lib")
#pragma warning(disable:4996)

void ErrorHandling(char* message);

struct Data{
	int X = 100;
	int Y = 200;

}LEE;
POINT pt;

int main()
{
	WSADATA	wsaData;
	SOCKET hServSock, hClntSock;
	SOCKADDR_IN servAddr, clntAddr;

	int szClntAddr;
	if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0)
		ErrorHandling("WSAStartup() error!");

	hServSock = socket(PF_INET, SOCK_STREAM, 0);
	if (hServSock == INVALID_SOCKET)
		ErrorHandling("socket() error");

	memset(&servAddr, 0, sizeof(servAddr));
	servAddr.sin_family = AF_INET;
	servAddr.sin_addr.s_addr = htonl(INADDR_ANY);
	servAddr.sin_port = htons(SEVER_PORT);

	if (bind(hServSock, (SOCKADDR*)&servAddr, sizeof(servAddr)) == SOCKET_ERROR)
		ErrorHandling("bind() error");

	if (listen(hServSock, 5) == SOCKET_ERROR)
		ErrorHandling("listen() error");

	szClntAddr = sizeof(clntAddr);
	hClntSock = accept(hServSock, (SOCKADDR*)&clntAddr, &szClntAddr);
	if (hClntSock == INVALID_SOCKET)
		ErrorHandling("accept() error");


	while (1) {
		//마우스 좌표설정
		GetCursorPos(&pt);
		LEE.X = pt.x;
		LEE.Y = pt.y;

		//데이터 전달
		send(hClntSock, (char*)&LEE, sizeof(LEE), 0);
		printf("데이터를 전달합니다. x : %d y :%d\n", LEE.X, LEE.Y);
		Sleep(10);
	}
	


	closesocket(hClntSock);
	closesocket(hServSock);
	WSACleanup();
	return 0;
}

void ErrorHandling(char* message)
{
	fputs(message, stderr);
	fputc('\n', stderr);
	exit(1);
}
