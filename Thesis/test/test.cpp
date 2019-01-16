// test.cpp : 定义控制台应用程序的入口点。
//

#include "stdafx.h"


int main()
{
	if (fork() || fork())
		fork();
	printf("1");
    return 0;
}

