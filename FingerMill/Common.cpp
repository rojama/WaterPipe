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



