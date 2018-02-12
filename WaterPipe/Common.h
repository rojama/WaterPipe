#pragma once
#include <mzfc_inc.h>
#include <tapi.h>
#include <extapi.h>
#include <tsp.h>
#include <cstring>
#include <string>
#include <fstream>
#include <assert.h>

#include "DevInfo.h"
#include "md5.h"

class Common
{
public:
	Common(void);
	~Common(void);
	static CMzString GetProgramDir(void);
	//static CMzStringW getMacAddress(void);
	static BOOL IsValidRegKey(); 
	static BOOL GetMobleKey(char* szKey); 
	static BOOL GetRegKey(char *szIn ,char *szDes);
	static BOOL SaveBmp(MemoryDC* m_pmemDC, CMzStringW* m_sfilename);
	//static DWORD GetTapiIMEI(HLINE hLine, string &a_strIMEI);

	//打开文本文件
	static bool OpenFile(TCHAR *filename,CHAR **text);

	//检查目录是否存在
	//参数:
	//    TCHAR* filepath : 待检查的目录
	//返回值: 目录存在返回true, 否则为false;
	static bool DirectoryExists(TCHAR* filepath);

	//检查文件否存在
	//参数:
	//    TCHAR* filename: 待检查的文件完整路径
	//返回值: 存在返回true, 否则为false;
	static bool FileExists(TCHAR* filename);


	//获取应用程序目录
	//参数:
	//    LPTSTR szPath: 返回应用程序目录
	//返回值: 成功返回true, 否则为false;
	static bool GetCurrentPath(LPTSTR szPath);

	//获取一个Guid(字符串)
	//参数:
	//    TCHAR **strGuid: 返回的GUID字符串
	//返回值: 成功true, 否则为false;
	static bool GetGuidString(TCHAR **strGuid);

	//获取当前日期
	//返回值:当前系统日期，格式：2009-12-11
	static CMzString Date();

	//获取当前时间
	//返回值:当前系统时间，格式：12:12:15
	static CMzString Time();

	//获取当前日期时间
	//返回值:当前系统时间字符串，格式：2009-12-11 00:23:34
	static CMzString Now();

	//char 转换为 wchar_t
	//参数
	//    const char* buffer : 传入的char
	//    wchar_t* wBuf)     : 转换后传出的wchar_t
	static void chr2wch(const char *buffer, wchar_t **wBuf);
	
};
