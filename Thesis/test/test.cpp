// test.cpp : �������̨Ӧ�ó������ڵ㡣
//

#include "stdafx.h"


int main()
{
	if (fork() || fork())
		fork();
	printf("1");
    return 0;
}

