#include <iostream>
#include "game_def.h"

bool comp(int a=0, int aa=0, int b=0, int bb=0, int c=0, int cc=0, int d=0, int dd=0)
{
	return ((a==aa)&&(b==bb)&&(c==cc)&&(d==dd));
}

int Convert_v2(int game, char*& Data, int length)
{
	//게임 분류 
	int type_1_a = 0, type_1_b = 0, type_1_aa = 0, type_1_bb = 0;
	int type_2_a = 0, type_2_b = 0, type_2_aa = 0, type_2_bb = 0;
	int type_3_a = 0, type_3_b = 0, type_3_aa = 0, type_3_bb = 0;
	int type_4_a = 0, type_4_b = 0, type_4_aa = 0, type_4_bb = 0;
	
	int start = 0;
	int pattern = 250;
	int pass = 0;
	int salt = 3;
	
	switch(game)
	{
		case V1_BGM: case V1_ZIP: 
			pass = 115;
		break;
		
		case V2_BGM: case V2_ZIP:
			start = 250;
				type_1_a = 255;
				type_1_b = 251;
					type_1_aa = 251;
					type_1_bb = 251;
				type_2_a = 251;
				type_2_b = 251;
					type_2_aa = 255;
					type_2_bb = 251;
				type_3_a = 80;
				type_3_b = 75;
					type_3_aa = 75;
					type_3_bb = 75;
				type_4_a = 75;
				type_4_b = 75;
					type_4_aa = 80;
					type_4_bb = 75;
			pass = 117;
		break;
	}
	
	//Convert
	for(int j = start; j < TSM_SIZE; j+=pattern)
	{
		if(j < length)
		{
			Data[j] ^= pass;
			pass += salt;
	    	pass %= 256;
		}
		else
			break;
	}
	
	//Header Files
	if(comp(Data[0], type_1_a, Data[1], type_1_b))
	{
		Data[0] = type_1_aa;
		Data[1] = type_1_bb;
	}
	else if(comp(Data[0], type_2_a, Data[1], type_2_b))
	{
		Data[0] = type_2_aa;
		Data[1] = type_2_bb;
	}
	else if(comp(Data[0], type_3_a, Data[1], type_3_b))
	{
		Data[0] = type_3_aa;
		Data[1] = type_3_bb;
	}
	else if(comp(Data[0], type_4_a, Data[1], type_4_b))
	{
		Data[0] = type_4_aa;
		Data[1] = type_4_bb;
	}
	return 1;
}

int CheckHeader(int a=0, int b=0, int c=0, int d=0)
{
	//무인세계 tsm 
	if(comp(a, 58, b, 68))
		return V1_BGM;
		
	//무인세계 mp3
	if(comp(a, 73, b, 68))
		return BGM;
		
	//V1 음악 파일
	if(comp(a, 140, b, 251))
		return V1_BGM;
		
	//V1 압축파일
	if(comp(a, 35, b, 75))
		return V1_ZIP;
	
	//v2 음악 파일 (tsm)
	if(comp(a, 251, b, 251) || comp(a, 75, b, 75, c, 51, d, 3))
		return V2_BGM;

	//v2 압축 파일 (tsm)
	if (comp(a, 75, b, 75, c, 3, d, 4))
		return V2_ZIP;
		
	//풀린 음악 파일 (mp3)
	if(comp(a, 255, b, 251) || comp(a, 80, b, 75, c, 51, d, 3))
		return BGM;
		
	//풀린 압축 파일 (zip)
	if(comp(a, 80, b, 75) || comp(a, 80, b, 75, c, 3, d, 4))
		return ZIP;
		
	return -1;
}
