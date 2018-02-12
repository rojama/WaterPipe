#include "Common.h"
//#include <Iphlpapi.h>


Common::Common(void)
{
}

Common::~Common(void)
{
}

//获得的路径
CMzString Common::GetProgramDir(void)
{
	CMzString AppPath(MAX_PATH);

    DWORD dwReturn = GetModuleFileName(NULL, AppPath, MAX_PATH);
    if (dwReturn <= MAX_PATH)
    {
        // Remove filename from path
		LPTSTR tszSlash = _tcsrchr(AppPath.C_Str(), static_cast<int>(_T('\\')));
        if (tszSlash)
            *++tszSlash = _T('\0');
    }

    return AppPath;
}


BOOL Common::SaveBmp(MemoryDC* m_pmemDC, CMzStringW* m_sfilename){
	HDC     hScrDC,   hMemDC;                   
	int     width,   height,startX,startY; 
	startX   =   0; 
	startY   =   0; 
	width   =   m_pmemDC->GetWidth(); 
	height   =   m_pmemDC->GetHeight(); 

	//the   pointer   will   save   all   pixel   point 's   color   value 
	BYTE     *lpBitmapBits   =   NULL;   

	hScrDC   =   m_pmemDC->GetDC(); 

	hMemDC   =   CreateCompatibleDC(hScrDC);   

	//initialise   the   struct   BITMAPINFO   for   the   bimap   infomation，   
	//in   order   to   use   the   function   CreateDIBSection 
	//   on   wince   os,   each   pixel   stored   by   24   bits(biBitCount=24)   
	//and   no   compressing(biCompression=0)   
	BITMAPINFO   RGB16BitsBITMAPINFO;   
	ZeroMemory(&RGB16BitsBITMAPINFO,   sizeof(BITMAPINFO)); 
	RGB16BitsBITMAPINFO.bmiHeader.biSize   =   sizeof(BITMAPINFOHEADER); 
	RGB16BitsBITMAPINFO.bmiHeader.biWidth   =   width; 
	RGB16BitsBITMAPINFO.bmiHeader.biHeight   =   height; 
	RGB16BitsBITMAPINFO.bmiHeader.biPlanes   =   1; 
	RGB16BitsBITMAPINFO.bmiHeader.biBitCount   =   16; 

	//use   the   function   CreateDIBSection   and   SelectObject   
	//in   order   to   get   the   bimap   pointer   :   lpBitmapBits   
	HBITMAP   directBmp   =   CreateDIBSection(hMemDC,   (BITMAPINFO*)&RGB16BitsBITMAPINFO,   
		DIB_RGB_COLORS,   (void   **)&lpBitmapBits,   NULL,   0); 
	HGDIOBJ   previousObject   =   SelectObject(hMemDC,   directBmp);   

	//   copy   the   screen   dc   to   the   memory   dc 
	BitBlt(hMemDC,   0,   0,   width,   height,   hScrDC,   startX,   startY,   SRCCOPY); 

	//if   you   only   want   to   get   the   every   pixel   color   value,   
	//you   can   begin   here   and   the   following   part   of   this   function   will   be   unuseful; 
	//the   following   part   is   in   order   to   write   file;   

	//bimap   file   header   in   order   to   write   bmp   file   
	BITMAPFILEHEADER   bmBITMAPFILEHEADER; 
	ZeroMemory(&bmBITMAPFILEHEADER,   sizeof(BITMAPFILEHEADER)); 
	bmBITMAPFILEHEADER.bfType   =   0x4d42;     //bmp     
	bmBITMAPFILEHEADER.bfOffBits   =   sizeof(BITMAPFILEHEADER)   +   sizeof(BITMAPINFOHEADER); 
	bmBITMAPFILEHEADER.bfSize   =   bmBITMAPFILEHEADER.bfOffBits   +   ((width*height)*2);   ///2=(16   /   8) 

	//write   into   file 
	FILE   *mStream   =   NULL; 
	if((mStream   =   _wfopen(m_sfilename->C_Str(),   L"wb "))) 
	{     
		//write   bitmap   file   header 
		fwrite(&bmBITMAPFILEHEADER,   sizeof(BITMAPFILEHEADER),   1,   mStream); 
		//write   bitmap   info 
		fwrite(&(RGB16BitsBITMAPINFO.bmiHeader),   sizeof(BITMAPINFOHEADER),   1,   mStream); 
		//write   bitmap   pixels   data 
		fwrite(lpBitmapBits,   2*width*height,   1,   mStream); 
		//close   file 
		fclose(mStream);
		delete   lpBitmapBits;
	} 
	else 
	{
		delete   lpBitmapBits;
		return   FALSE; 
	} 
	return   TRUE;
}

////取得MAC地址
//CMzStringW Common::getMacAddress(void)
//{
//	// will need to use the below include and also link in the following lib
//	// #include "C:\WINCE500\Public\Common\SDK\INC\Iphlpapi.h"
//	// $(_PROJECTROOT)\cesysgen\sdk\lib\$(_CPUINDPATH)\iphlpapi.lib
//	CMzStringW mac(20);
//	UINT i;
//	DWORD result;
//	PIP_ADAPTER_INFO adapterInfo = NULL;
//	ULONG size = 0;
//
//	result = GetAdaptersInfo( adapterInfo, &size );
//	//called with adapterInfo = NULL and size = 0 to get the size of buffer 
//		if( result == ERROR_BUFFER_OVERFLOW )
//		{
//			if( !(adapterInfo = (PIP_ADAPTER_INFO)malloc(size)) )
//			{
//				RETAILMSG(1, (TEXT("mac address error Insufficient Memory\r\n")));
//				return L"";
//			}
//			result = GetAdaptersInfo( adapterInfo, &size);
//		}
//
//		if( result != NO_ERROR )
//		{
//			RETAILMSG(1, (TEXT("mac address error = %d\r\n"), result));
//			return L"";
//		}
//		if( size == 0 )
//			adapterInfo = NULL;
//
//		if( adapterInfo == NULL )
//			RETAILMSG(1,(TEXT("No Interfaces Present.\n")));
//
//		while (adapterInfo != NULL)
//		{
//			RETAILMSG(1,(TEXT("AdapterType: %d\r\n") , adapterInfo->Type));
//			RETAILMSG(1,(TEXT("AdapterName: [%hs]\r\n"), 
//				adapterInfo->AdapterName));
//			RETAILMSG(1,(TEXT("Address: ") , 
//				adapterInfo->AddressLength));
//			CMzStringW temp(4);
//			for(i = 0; i < adapterInfo->AddressLength; ++i)
//			{
//				RETAILMSG(1,(TEXT("%02x "),adapterInfo->Address[i]));
//				if (strcmp(adapterInfo->AdapterName , "USB CABLE:") == 0){
//					wsprintf(temp.C_Str(), L"%02x ", adapterInfo->Address[i]);
//					mac = mac + temp;
//				}
//			}
//			RETAILMSG(1,(TEXT("\r\n")));
//
//			adapterInfo = adapterInfo->Next;
//		}
//		return mac;
//}


void kmp_init(const char *patn, int len, int *next) 
{ 
	int i, j; 
	assert(patn != NULL && len > 0 && next != NULL); 
	next[0] = 0; 
	for (i = 1, j = 0; i < len; i ++) { 
		while (j > 0 && patn[j] != patn[i]) 
			j = next[j - 1]; 
		if (patn[j] == patn[i]) 
			j ++; 
		next[i] = j; 
	} 
} 

int kmp_find(const char *text, int text_len, const char *patn, 
			 int patn_len, int *next) 
{ 
	int i, j; 
	assert(text != NULL && text_len > 0 && patn != NULL && patn_len > 0 
		&& next != NULL); 
	for (i = 0, j = 0; i < text_len; i ++ ) { 
		while (j > 0 && text[i] != patn[j]) 
			j = next[j - 1]; 
		if (text[i] == patn[j]) 
			j ++; 
		if (j == patn_len) 
			return i + 1 - patn_len; 
	} 
	return -1; 
} 

bool isFind(char * text, char * pattern) 
{ 
	int *next; 
	int pos, len = strlen(pattern); 

	next = (int *)calloc(strlen(pattern), sizeof(int)); 
	kmp_init(pattern, strlen(pattern), next);
/*
	int i;
	printf("next array:\n"); 
	for (i = 0; i < len; i ++) 
		printf("\t%c", pattern[i]); 
	printf("\n"); 
	for (i = 0; i < len; i ++) 
		printf("\t%d", next[i]); 
	printf("\n"); 
*/
	pos = kmp_find(text, strlen(text), pattern, strlen(pattern), next); 
	//printf("find result:\n"); 
	if (pos < 0) { 
		//printf("None found!\n"); 
		return false; 
	} else { 
		/*
		printf("%s\n", text); 
		for (i = 0; i < pos; i ++) 
			printf(" "); 
		printf("^\n"); 
		*/
		return true; 
	} 

}


BOOL Common::IsValidRegKey()   
{   	
	bool rtnValue = false;
	CHAR *text;;
	if (Common::OpenFile(Common::GetProgramDir()+L"key", &text))
	{
		char szKey[20];
		if (Common::GetMobleKey(szKey)){
			char szOutKey[31];
			if (Common::GetRegKey(szKey ,szOutKey)){
				if (isFind(text, szOutKey)){
					rtnValue = true;
				}				
			}
		}      
	}   
    return rtnValue;
}   

BOOL Common::GetRegKey(char *szIn ,char *szKey)   
{      
	char szTemp[31];     
	CMd5 md5_1, md5_2, md5_3;
	LPCSTR out_1_md5, out_2_md5, out_3_md5;
	
	md5_1.TargetStr(szIn);
	out_1_md5 = md5_1.GetDigestKey();
	//printf("out_1_md5 = %s\n",out_1_md5) ;

	szTemp[0] = out_1_md5[15];   
	szTemp[1] = out_1_md5[18];   
	szTemp[2] = out_1_md5[17];   
	szTemp[3] = out_1_md5[27];   
	szTemp[4] = out_1_md5[4];  
	szTemp[5] = out_1_md5[14];   
	szTemp[6] = out_1_md5[11];   
	szTemp[7] = out_1_md5[28];   
	szTemp[8] = out_1_md5[7];   
	szTemp[9] = out_1_md5[31];  
	szTemp[10] = out_1_md5[1];   
	szTemp[11] = out_1_md5[6];   
	szTemp[12] = out_1_md5[16];   
	szTemp[13] = out_1_md5[22];   
	szTemp[14] = out_1_md5[2];   
	szTemp[15] = out_1_md5[13];   
	szTemp[16] = out_1_md5[29];   
	szTemp[17] = out_1_md5[8];   
	szTemp[18] = out_1_md5[5];   
	szTemp[19] = out_1_md5[10];   
	szTemp[20] = out_1_md5[3];   
	szTemp[21] = out_1_md5[9];   
	szTemp[22] = out_1_md5[12];   
	szTemp[23] = out_1_md5[19];   
	szTemp[24] = out_1_md5[21];      
	szTemp[25] = out_1_md5[23];   
	szTemp[26] = out_1_md5[24];   
	szTemp[27] = out_1_md5[25];   
	szTemp[28] = out_1_md5[30];   
	szTemp[29] = out_1_md5[26];  
	szTemp[30] = '\0'; 

	md5_2.TargetStr(szTemp);
	out_2_md5 = md5_2.GetDigestKey();
	//printf("out_2_md5 = %s\n",out_2_md5) ;

	szTemp[0] = out_2_md5[25];   
	szTemp[1] = out_2_md5[17];   
	szTemp[2] = out_2_md5[22];   
	szTemp[3] = out_2_md5[1];   
	szTemp[4] = out_2_md5[9];   
	szTemp[5] = out_2_md5[15];   
	szTemp[6] = out_2_md5[23];   
	szTemp[7] = out_2_md5[30];   
	szTemp[8] = out_2_md5[19];   
	szTemp[9] = out_2_md5[12];    
	szTemp[10] = out_2_md5[11];   
	szTemp[11] = out_2_md5[4];   
	szTemp[12] = out_2_md5[6];   
	szTemp[13] = out_2_md5[27];   
	szTemp[14] = out_2_md5[14];    
	szTemp[15] = out_2_md5[3];   
	szTemp[16] = out_2_md5[2];   
	szTemp[17] = out_2_md5[18];   
	szTemp[18] = out_2_md5[5];   
	szTemp[19] = out_2_md5[0];   
	szTemp[20] = out_2_md5[8];   
	szTemp[21] = out_2_md5[7];   
	szTemp[22] = out_2_md5[10];   
	szTemp[23] = out_2_md5[13];   
	szTemp[24] = out_2_md5[16];      
	szTemp[25] = out_2_md5[24];   
	szTemp[26] = out_2_md5[20];   
	szTemp[27] = out_2_md5[21];   
	szTemp[28] = out_2_md5[26];   
	szTemp[29] = out_2_md5[29];  
	szTemp[30] = '\0'; 

	md5_3.TargetStr(szTemp);
	out_3_md5 = md5_3.GetDigestKey();	
	//printf("out_3_md5 = %s\n",out_3_md5) ;
	
	szKey[0] = out_3_md5[9];   
	szKey[1] = out_3_md5[6];   
	szKey[2] = out_3_md5[4];   
	szKey[3] = out_3_md5[1];   
	szKey[4] = out_3_md5[7];      
	szKey[5] = out_3_md5[2];   
	szKey[6] = out_3_md5[3];   
	szKey[7] = out_3_md5[5];   
	szKey[8] = out_3_md5[0];   
	szKey[9] = out_3_md5[8];     
	szKey[10] = out_3_md5[11];   
	szKey[11] = out_3_md5[14];   
	szKey[12] = out_3_md5[16];   
	szKey[13] = out_3_md5[13];   
	szKey[14] = out_3_md5[19];    
	szKey[15] = out_3_md5[12];   
	szKey[16] = out_3_md5[17];   
	szKey[17] = out_3_md5[18];   
	szKey[18] = out_3_md5[15];   
	szKey[19] = out_3_md5[10];   
	szKey[20] = out_3_md5[28];   
	szKey[21] = out_3_md5[25];   
	szKey[22] = out_3_md5[26];   
	szKey[23] = out_3_md5[21];   
	szKey[24] = out_3_md5[27];      
	szKey[25] = out_3_md5[29];   
	szKey[26] = out_3_md5[24];   
	szKey[27] = out_3_md5[23];   
	szKey[28] = out_3_md5[20];   
	szKey[29] = out_3_md5[22];  
	szKey[30] = '\0'; 

	printf("Out sn = %s\n",szKey) ;
    return TRUE;   
}   
   
BOOL Common::GetMobleKey(char *szDes)   
{   
    DevInfo def;
	def.Init();
	Siminfo sim = def.GetGeneralInfo();
	def.~DevInfo();

	if(strlen(sim.IMEI)==0 || strlen(sim.SN)==0){
		return FALSE;
	}
     
	CMd5 md5_I, md5_S;
	LPCSTR imei_md5, sn_md5;
	
	md5_I.TargetStr(sim.IMEI);
	imei_md5 = md5_I.GetDigestKey();

	md5_S.TargetStr(sim.SN);
	sn_md5 = md5_S.GetDigestKey();

	szDes[0] = imei_md5[15];   
	szDes[1] = sn_md5[27];   
	szDes[2] = sn_md5[30];   
	szDes[3] = imei_md5[8];   
	szDes[4] = '-';   
	szDes[5] = sn_md5[25];   
	szDes[6] = imei_md5[11];   
	szDes[7] = sn_md5[3];   
	szDes[8] = imei_md5[7];   
	szDes[9] = '-';   
	szDes[10] = imei_md5[1];   
	szDes[11] = imei_md5[14];   
	szDes[12] = imei_md5[16];   
	szDes[13] = sn_md5[23];   
	szDes[14] = '-';   
	szDes[15] = sn_md5[22];   
	szDes[16] = sn_md5[31];   
	szDes[17] = imei_md5[18];   
	szDes[18] = sn_md5[5];   
	szDes[19] = '\0'; 

	printf("moble sn = %s\n",szDes) ;
    return TRUE;   
}   
  
//DWORD Common::GetTapiIMEI(HLINE hLine, string &a_strIMEI)
//{
//    DWORD            rc;
//    LINEGENERALINFO    LineGeneralInfo;
//    TCHAR            lpszSerialNumber[32];
//
//    memset( &LineGeneralInfo, 0, sizeof(LineGeneralInfo) );
//    LineGeneralInfo.dwTotalSize  = sizeof(LineGeneralInfo);
//    rc = lineGetGeneralInfo(hLine, &LineGeneralInfo);
//
//
//    if (0 == rc)
//    {
//        if (LineGeneralInfo.dwTotalSize < LineGeneralInfo.dwNeededSize)
//        {
//            LINEGENERALINFO *lpBuffer = (LINEGENERALINFO *)malloc(LineGeneralInfo.dwNeededSize);
//            lpBuffer->dwTotalSize  = LineGeneralInfo.dwNeededSize;
//
//            rc = lineGetGeneralInfo(hLine, lpBuffer);
//
//            memcpy( lpszSerialNumber, 
//                              (TCHAR*)((lpBuffer)+lpBuffer->dwSerialNumberOffset), 
//                               lpBuffer->dwSerialNumberSize );
//            free(lpBuffer);
//        }
//        else
//        {
//
//            memcpy( lpszSerialNumber, 
//                             (TCHAR*)((&LineGeneralInfo)+LineGeneralInfo.dwSerialNumberOffset), 
//                             LineGeneralInfo.dwSerialNumberSize  );
//        }
//        //a_strIMEI = lpszSerialNumber;
//    }
//
//    return rc;
//}


//打开文本文件
bool Common::OpenFile(TCHAR *filename, CHAR **text)
{
	//CMzString m_Text;
	bool rtnValue = false;

	if (Common::FileExists(filename)){
		//读取文本
		//char *ss;
		fstream file1;
		file1.open(filename,  ios::in | ios::binary);
		file1.seekg(0, ios::end);
		int nLen = file1.tellg();
		*text = new char[nLen+1];
		file1.seekg(0, ios::beg);
		file1.read(*text, nLen);
		(*text)[nLen] = '\0';
		file1.close();

		//wchar_t* wszString = new wchar_t[nLen + 1];
		//chr2wch(ss, wszString); 		
		//text = ss;
		rtnValue = true;
	}

	return rtnValue;
}

//目录是否存在的检查
bool Common::DirectoryExists(TCHAR* filepath)
{
	int code = GetFileAttributes(filepath);
	return ((code != -1) && (FILE_ATTRIBUTE_DIRECTORY && code != 0));
}

//检查文件否存在
bool Common::FileExists(TCHAR* filename)
{
  WIN32_FIND_DATA FindFileData;
  HANDLE hFind;

  hFind = FindFirstFile(filename, &FindFileData);
  if (hFind == INVALID_HANDLE_VALUE) 
  {
    return false;
  } 
  else 
  {
    FindClose(hFind);
    return true;
  }
}

//获取应用程序目录
bool Common::GetCurrentPath(LPTSTR szPath)  
{  
	HMODULE handle = GetModuleHandle(NULL);
	DWORD dwRet = GetModuleFileName(handle, szPath, MAX_PATH);
	if (dwRet == 0)
	{
		return false;
	}
	else
	{
		TCHAR* p = szPath;
		while(*p)++p; //let p point to '\0'  
		while('\\' != *p)--p; //let p point to '\\'  
		++p;
		*p = '\0'; //get the path
		return true;  
	}
}

//获取Guid
bool Common::GetGuidString(TCHAR **strGuid)
{
	GUID theGuid;
	LPOLESTR pstrGuid;

	CoCreateGuid(&theGuid);
	if (StringFromCLSID(theGuid, &pstrGuid) == 0)
	{
		*strGuid = pstrGuid;
		//CoTaskMemFree(pstrGuid);
		return true;
	}
	else
		return false;
}

//获取当前日期
//返回值:当前系统日期，格式：2009-12-11
CMzString Common::Date()
{
	SYSTEMTIME sysTime;
	GetLocalTime(&sysTime);
	CMzString sDate;
	wsprintf(sDate.C_Str(), L"%4d-%02d-%02d", sysTime.wYear, sysTime.wMonth, sysTime.wDay);	

	return sDate;
}

//获取当前时间
//返回值:当前系统时间，格式：12:12:15
CMzString Common::Time()
{
	SYSTEMTIME sysTime;
	GetLocalTime(&sysTime);
	CMzString sTime;
	wsprintf(sTime.C_Str(), L"%02d:%02d:%02d.%03d" , sysTime.wHour, sysTime.wMinute,sysTime.wSecond,sysTime.wMilliseconds);	
	return sTime;
}


//获取当前时间
CMzString Common::Now()
{
	CMzString sDateTime;

	//sDateTime = Date() + L" " + Time();
	return sDateTime;
}


/*
  wBuf 为申明指针即可。
*/

void Common::chr2wch(const char *buffer, wchar_t **wBuf)
{
	size_t len = strlen(buffer);
	size_t wlen = MultiByteToWideChar(CP_ACP, 0, (const char*)buffer, int(len), NULL, 0);
	*wBuf = new wchar_t[wlen + 1];
	MultiByteToWideChar(CP_ACP, 0, (const char*)buffer, int(len), *wBuf, int(wlen));
} 