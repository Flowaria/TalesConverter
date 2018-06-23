#include <math.h>
#include <vector>
#include <iostream>
#include <iomanip>
#include <fstream>
#include <windows.h>

#include "convert.h"
#include "game_def.h"

using namespace std;

bool replace(std::string& str, const std::string& from, const std::string& to) {
    size_t start_pos = str.find(from);
    if(start_pos == std::string::npos)
        return false;
    str.replace(start_pos, from.length(), to);
    return true;
}

bool hasEnding (std::string const &fullString, std::string const &ending) {
    if (fullString.length() >= ending.length()) {
        return (0 == fullString.compare (fullString.length() - ending.length(), ending.length(), ending));
    } else {
        return false;
    }
}

void Process(char* filename)
{
	ifstream file;
	int gameid = 0;
	string nFilepath = filename;

	file.open(filename, ios::in | ios::binary);
	if (!file.is_open())
	{
		file.close();
		return;
	}

	if (hasEnding(filename, ".tsm"))
	{
		//게임 확인용 : 2자리 매직넘버
		file.seekg(0, ios::beg);

		int a = file.get();
		int b = file.get();
		int c = file.get();
		int d = file.get();

		gameid = CheckHeader(a, b, c, d);

		if (gameid == V1_ZIP || gameid == V2_ZIP || gameid == ZIP)
		{
			replace(nFilepath, ".tsm", ".zip");
		}
		else if (gameid == V1_BGM || gameid == V2_BGM || gameid == BGM)
		{
			replace(nFilepath, ".tsm", ".mp3");
		}
	}

	//변환
	if (gameid > 0)
	{
		file.seekg(0, ios::end);
		size_t fsize = file.tellg();

		char* Data = new char[fsize];

		file.seekg(0, ios::beg);
		file.read(Data, fsize);

		Convert_v2(gameid, Data, fsize);

		//Write File
		ofstream o(nFilepath.c_str(), ios::binary);
		o.write(Data, fsize);
		o.close();

		delete[] Data;
	}
	//이름 변경
	else if (gameid < 0)
	{
		//Rename File
		CopyFile(filename, nFilepath.c_str(), true);
	}
	file.close();
}

int main(int argc, char** argv)
{
	if(argc > 4)
	{
		string outputdir;
		for(int i = 0 ; i<argc; i++)
		{
			string arg = argv[i];
			if (outputdir[0] != 0)
			{
				Process(argv[i]);
			}
			else if (i < argc - 1)
			{
				if (arg == "-output")
					outputdir = argv[i + 1];
			}
		}
	}
	return 0;
}
