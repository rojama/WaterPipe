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

	//���ı��ļ�
	static bool OpenFile(TCHAR *filename,CHAR **text);

	//���Ŀ¼�Ƿ����
	//����:
	//    TCHAR* filepath : ������Ŀ¼
	//����ֵ: Ŀ¼���ڷ���true, ����Ϊfalse;
	static bool DirectoryExists(TCHAR* filepath);

	//����ļ������
	//����:
	//    TCHAR* filename: �������ļ�����·��
	//����ֵ: ���ڷ���true, ����Ϊfalse;
	static bool FileExists(TCHAR* filename);


	//��ȡӦ�ó���Ŀ¼
	//����:
	//    LPTSTR szPath: ����Ӧ�ó���Ŀ¼
	//����ֵ: �ɹ�����true, ����Ϊfalse;
	static bool GetCurrentPath(LPTSTR szPath);

	//��ȡһ��Guid(�ַ���)
	//����:
	//    TCHAR **strGuid: ���ص�GUID�ַ���
	//����ֵ: �ɹ�true, ����Ϊfalse;
	static bool GetGuidString(TCHAR **strGuid);

	//��ȡ��ǰ����
	//����ֵ:��ǰϵͳ���ڣ���ʽ��2009-12-11
	static CMzString Date();

	//��ȡ��ǰʱ��
	//����ֵ:��ǰϵͳʱ�䣬��ʽ��12:12:15
	static CMzString Time();

	//��ȡ��ǰ����ʱ��
	//����ֵ:��ǰϵͳʱ���ַ�������ʽ��2009-12-11 00:23:34
	static CMzString Now();

	//char ת��Ϊ wchar_t
	//����
	//    const char* buffer : �����char
	//    wchar_t* wBuf)     : ת���󴫳���wchar_t
	static void chr2wch(const char *buffer, wchar_t **wBuf);
	
};
