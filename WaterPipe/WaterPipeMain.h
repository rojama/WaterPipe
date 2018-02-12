#include <imzsysfile.h>
#include <InitGuid.h>
#include <IMzUnknown.h>
#include <IMzUnknown_IID.h>
//#include <IPhotoViewer.h>
//#include <IPhotoViewer_GUID.h>
//#include <IFileBrowser.h>
//#include <IFileBrowser_GUID.h>
#include <IPlayerCore.h>
#include <PlayerCore_GUID.h>
#include <IPlayerCore_IID.h>


typedef struct _BoxStatic				//方块状态
{		
	CMzStringW tag;				//方块图形标识
	CMzStringW in_lab;			//流水注入口
	CMzStringW full_lab;		//流水已充填口
} BoxStatic, *PBoxStatic;

typedef struct _BoxAnimo					//方块流水动画
{  
	int box;					//方块号
	vector<CMzStringW> from_to;	//流水分解动画方向
} BoxAnimo, *PBoxAnimo; 

typedef struct _ErrBoxAnimo				//方块流水溢出动画
{
	int box;					//方块号
	CMzStringW err_lab;			//流水溢出部位
} ErrBoxAnimo, *PErrBoxAnimo;  

typedef struct _ThreadData 
{
	int box_no;
    UiButton_Image* box;				//方块
    PBoxStatic box_static;			//方块状态
} ThreadData, *PThreadData;

typedef struct _ErrThreadData 
{
    UiButton_Image* box;				//方块
    CMzStringW err_lab;					//方块错误
} ErrThreadData, *PErrThreadData;

typedef struct _SoundsData 
{
    IPlayerCore_Play *pPlayer;
	IMzSysFile *pFile;
} SoundsData, *PSoundsData;

CMzStringW getMacAddress();
DWORD WINAPI FinalThread(LPVOID lpParam);