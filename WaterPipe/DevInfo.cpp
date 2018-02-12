// DevInfo.cpp: implementation of the DevInfo class.
//
//////////////////////////////////////////////////////////////////////

//#include "stdafx.h"
//#include "imei.h"
#include "DevInfo.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
//#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

DevInfo::DevInfo()
{
	m_dwAPIVersion = TAPI_CURRENT_VERSION;
	m_hLineApp = 0;
	m_hLine = 0;
	m_dwExtVersion = 0;
}

DevInfo::~DevInfo()
{
	::lineClose(m_hLine);
	::lineShutdown(m_hLineApp);
}

bool DevInfo::Init()
{
	// set the line init params
	LINEINITIALIZEEXPARAMS LineExtParams;
	LineExtParams.dwTotalSize = sizeof(LineExtParams);
	LineExtParams.dwOptions = LINEINITIALIZEEXOPTION_USEEVENT;

	if (::lineInitializeEx(&m_hLineApp, NULL, NULL, NULL,
		&m_dwNumDevs, &m_dwAPIVersion, &LineExtParams))
	{
		//EndDialog(IDOK);
		return false;
	}

	m_dwTSPILineDeviceID = GetTSPLineDeviceID(CELLTSP_LINENAME_STRING);
	if ( m_dwTSPILineDeviceID == (DWORD)-1 )
	{
		::lineShutdown(m_hLineApp);
		//EndDialog(IDOK);
		return false;
	}

	// open the line
	if( ::lineOpen(m_hLineApp, m_dwTSPILineDeviceID,
		&m_hLine, m_dwAPIVersion, 0, 0,
		LINECALLPRIVILEGE_OWNER, LINEMEDIAMODE_DATAMODEM, 0) )
	{
		::lineShutdown(m_hLineApp);
		//EndDialog(IDOK);
		return false;
	}

	// set up ExTAPI
	if ( ::lineNegotiateExtVersion(m_hLineApp, m_dwTSPILineDeviceID,
		m_dwAPIVersion, EXT_API_LOW_VERSION,
		EXT_API_HIGH_VERSION, &m_dwExtVersion) )
	{
		::lineClose(m_hLine);
		::lineShutdown(m_hLineApp);
		//EndDialog(IDOK);
		return false;
	}

	return true;
}

DWORD DevInfo::GetTSPLineDeviceID(const TCHAR *const psTSPLineName)
{
	DWORD dwReturn = -1;
	for(DWORD dwCurrentDevID = 0 ; dwCurrentDevID < m_dwNumDevs ; dwCurrentDevID++)
	{
		LINEEXTENSIONID LineExtensionID;
		if( ::lineNegotiateAPIVersion(m_hLineApp, dwCurrentDevID,
			TAPI_API_LOW_VERSION, TAPI_CURRENT_VERSION,
			&m_dwAPIVersion, &LineExtensionID) == 0 )
		{
			LINEDEVCAPS LineDevCaps;
			LineDevCaps.dwTotalSize = sizeof(LineDevCaps);
			if( ::lineGetDevCaps(m_hLineApp, dwCurrentDevID,
				m_dwAPIVersion, 0, &LineDevCaps) == 0 )
			{
				BYTE* pLineDevCapsBytes = new BYTE[LineDevCaps.dwNeededSize];
				if(0 != pLineDevCapsBytes)
				{
					LINEDEVCAPS* pLineDevCaps = (LINEDEVCAPS*)pLineDevCapsBytes;
					pLineDevCaps->dwTotalSize = LineDevCaps.dwNeededSize;
					if( ::lineGetDevCaps(m_hLineApp, dwCurrentDevID,
						m_dwAPIVersion, 0, pLineDevCaps) == 0 )
					{
						if(0 == _tcscmp((TCHAR*)((BYTE*)pLineDevCaps+pLineDevCaps->dwLineNameOffset),
							psTSPLineName))
						{
							dwReturn = dwCurrentDevID;
						}
					}
					delete[]  pLineDevCapsBytes;
				}
			}
		}
	}
	return dwReturn;
}

//获取手机SIM卡信息(IMEI国际移动设备识别码)国际移动用户识别码(IMSI)
Siminfo DevInfo::GetGeneralInfo()
{
	Siminfo Result;
	memset(Result.IMEI, 0, 128);
	memset(Result.IMSI, 0, 128);
	memset(Result.SN, 0, 128);

	LPBYTE pLineGeneralInfoBytes = NULL;
	const DWORD dwMediaMode = LINEMEDIAMODE_DATAMODEM;
	LINEGENERALINFO lviGeneralInfo;
	LPLINEGENERALINFO plviGeneralInfo;
	LPTSTR tsManufacturer, tsModel, tsRevision, tsSerialNumber, tsSubscriberNumber;
	//string sInfo;

	lviGeneralInfo.dwTotalSize = sizeof(lviGeneralInfo);

	LONG lRes = ::lineGetGeneralInfo(m_hLine, &lviGeneralInfo);
	if (lRes != 0 && lRes != LINEERR_STRUCTURETOOSMALL)
	{
		TCHAR szMsg[255];
		GetTAPIErrorMsg(szMsg,sizeof(szMsg), lRes);
		//AfxMessageBox(szMsg);
		//  return c;
	}

	pLineGeneralInfoBytes = new BYTE[lviGeneralInfo.dwNeededSize];
	plviGeneralInfo = (LPLINEGENERALINFO)pLineGeneralInfoBytes;

	if(pLineGeneralInfoBytes != NULL)
	{
		plviGeneralInfo->dwTotalSize = lviGeneralInfo.dwNeededSize;
		if ( (lRes = ::lineGetGeneralInfo(m_hLine, plviGeneralInfo)) != 0 )
		{
			TCHAR szMsg[255];
			GetTAPIErrorMsg(szMsg,sizeof(szMsg), lRes);
			//AfxMessageBox(szMsg);
		}
		else
		{
			TCHAR szUnavailable[] = L"Unavailable";
			if(plviGeneralInfo->dwManufacturerSize)
			{
				tsManufacturer = (WCHAR*)(((BYTE*)plviGeneralInfo)+plviGeneralInfo->dwManufacturerOffset);
			}
			else
			{
				tsManufacturer = szUnavailable;
			}

			if(plviGeneralInfo->dwModelSize)
			{
				tsModel = (WCHAR*)(((BYTE*)plviGeneralInfo)+plviGeneralInfo->dwModelOffset);
			}
			else
			{
				tsModel = szUnavailable;
			}

			if(plviGeneralInfo->dwRevisionSize)
			{
				tsRevision = (WCHAR*)(((BYTE*)plviGeneralInfo)+plviGeneralInfo->dwRevisionOffset);
			}
			else
			{
				tsRevision = szUnavailable;
			}

			if(plviGeneralInfo->dwSerialNumberSize)
			{
				tsSerialNumber = (WCHAR*)(((BYTE*)plviGeneralInfo)+plviGeneralInfo->dwSerialNumberOffset);
			}
			else
			{
				tsSerialNumber = szUnavailable;
			}

			if(plviGeneralInfo->dwSubscriberNumberSize)
			{
				tsSubscriberNumber = (WCHAR*)(((BYTE*)plviGeneralInfo)+plviGeneralInfo->dwSubscriberNumberOffset);
			}
			else
			{
				tsSubscriberNumber = szUnavailable;
			}

			wprintf(L"Manufacturer: %s\nModel: %s\nRevision: %s\nSerial No: %s\nSubscriber No: %s\n",
				tsManufacturer,
				tsModel,
				tsRevision,
				tsSerialNumber,
				tsSubscriberNumber);
			int a = sizeof(tsSerialNumber);
			int b = sizeof(tsSubscriberNumber);
			int c = sizeof(tsModel);

			int Length =0;
			while(1)
			{
				Result.IMSI[Length++] = (char)*tsSubscriberNumber++;

				if((char)*tsSubscriberNumber<=0 )
					break;
			}
			int Len = 0;
			while(1)
			{
				Result.IMEI[Len++] = (char)*tsSerialNumber++;

				if( (char)*tsSerialNumber<=0)
					break;
			}
			int SnLen = 0;
			while(1)
			{
				Result.SN[SnLen++] = (char)*tsModel++;

				if( (char)*tsModel<=0)
					break;
			}
		}
	}

	delete [] pLineGeneralInfoBytes;

	if( strlen(Result.SN) > 17)
	{
		memcpy(Result.SN,(Result.SN) + (strlen(Result.SN) - 17),17);
		Result.IMEI[17] = '\0';
	}
	if( strlen(Result.IMEI) > 15)
	{
		memcpy(Result.IMEI,(Result.IMEI) + (strlen(Result.IMEI) - 15),15);
		Result.IMEI[15] = '\0';
	}
	if( strlen(Result.IMSI) >  15)
		memcpy(Result.IMSI,(Result.IMSI) + (strlen(Result.IMSI) - 15),15);

	return Result;
}

void DevInfo::GetTAPIErrorMsg(TCHAR *szMsg, int nSize, DWORD dwError)
{
	LPTSTR lpBuffer = 0;
	DWORD dwRet = 0;

	dwRet = ::FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
		NULL,TAPIERROR_FORMATMESSAGE(dwError),MAKELANGID(LANG_NEUTRAL, LANG_NEUTRAL),
		(LPTSTR) &lpBuffer,0,NULL);
	memset(szMsg,0,nSize);
	if (lpBuffer && dwRet)
	{
		_tcscpy(szMsg,lpBuffer);
		LocalFree(lpBuffer);
	}
	else
	{
		_stprintf(szMsg,L"Unknown Error: 0x%X",dwError);
	}
}