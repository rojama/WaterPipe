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


typedef struct _BoxStatic				//����״̬
{		
	CMzStringW tag;				//����ͼ�α�ʶ
	CMzStringW in_lab;			//��ˮע���
	CMzStringW full_lab;		//��ˮ�ѳ����
} BoxStatic, *PBoxStatic;

typedef struct _BoxAnimo					//������ˮ����
{  
	int box;					//�����
	vector<CMzStringW> from_to;	//��ˮ�ֽ⶯������
} BoxAnimo, *PBoxAnimo; 

typedef struct _ErrBoxAnimo				//������ˮ�������
{
	int box;					//�����
	CMzStringW err_lab;			//��ˮ�����λ
} ErrBoxAnimo, *PErrBoxAnimo;  

typedef struct _ThreadData 
{
	int box_no;
    UiButton_Image* box;				//����
    PBoxStatic box_static;			//����״̬
} ThreadData, *PThreadData;

typedef struct _ErrThreadData 
{
    UiButton_Image* box;				//����
    CMzStringW err_lab;					//�������
} ErrThreadData, *PErrThreadData;

typedef struct _SoundsData 
{
    IPlayerCore_Play *pPlayer;
	IMzSysFile *pFile;
} SoundsData, *PSoundsData;

CMzStringW getMacAddress();
DWORD WINAPI FinalThread(LPVOID lpParam);