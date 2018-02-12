#pragma once
//#endif // _MSC_VER > 1000

#include <tapi.h>
#include <extapi.h>
#include <tsp.h>
#include <mzfc_inc.h>
#include <cstring>
#include <string>

#define TAPI_API_LOW_VERSION    0x00010003
#define TAPI_CURRENT_VERSION    0x00020000
#define EXT_API_LOW_VERSION     0x00010000
#define EXT_API_HIGH_VERSION    0x00010000

typedef struct Info
{
	char SN[128];
	char IMSI[128];
	char IMEI[128];
}Siminfo;

class DevInfo
{
public:
	DevInfo();
	virtual ~DevInfo();
	static DevInfo Info;
public:
	DWORD m_dwNumDevs;
	DWORD m_dwAPIVersion;
	HLINEAPP m_hLineApp;
	HLINE m_hLine;
	DWORD m_dwExtVersion;
	DWORD m_dwTSPILineDeviceID;

	bool Init();
	Siminfo GetGeneralInfo();


	LPTSTR g_dwImei;


private:
	void GetTAPIErrorMsg(TCHAR *szMsg,int nSize, DWORD dwError);
	DWORD GetTSPLineDeviceID(const TCHAR* const psTSPLineName);

};

//#endif // !defined(AFX_IMEI1_H__84CB8CFD_9DA9_470E_867E_2D689F916CBE__INCLUDED_)
