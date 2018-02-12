#include <mzfc_inc.h>
#include <Mzfc/sound.h>
#include <ReadWriteIni.h>
#include <Mzfc/ConvertHelper.h>
#include <pm.h>
#include <ShellNotifyMsg.h>
#include <Mzfc/MzCommonDlg.h>
#include <SettingApi.h>
#include <MzCommon.h>

#define PIPE_MAIN_BTN_SKIP  101

#define PIPE_MAIN_BTN_1_1  211
#define PIPE_MAIN_BTN_1_2  212
#define PIPE_MAIN_BTN_1_3  213
#define PIPE_MAIN_BTN_1_4  214
#define PIPE_MAIN_BTN_1_5  215
#define PIPE_MAIN_BTN_1_6  216
#define PIPE_MAIN_BTN_2_1  221
#define PIPE_MAIN_BTN_2_2  222
#define PIPE_MAIN_BTN_2_3  223
#define PIPE_MAIN_BTN_2_4  224
#define PIPE_MAIN_BTN_2_5  225
#define PIPE_MAIN_BTN_2_6  226
#define PIPE_MAIN_BTN_3_1  231
#define PIPE_MAIN_BTN_3_2  232
#define PIPE_MAIN_BTN_3_3  233
#define PIPE_MAIN_BTN_3_4  234
#define PIPE_MAIN_BTN_3_5  235
#define PIPE_MAIN_BTN_3_6  236
#define PIPE_MAIN_BTN_4_1  241
#define PIPE_MAIN_BTN_4_2  242
#define PIPE_MAIN_BTN_4_3  243
#define PIPE_MAIN_BTN_4_4  244
#define PIPE_MAIN_BTN_4_5  245
#define PIPE_MAIN_BTN_4_6  246
#define PIPE_MAIN_BTN_5_1  251
#define PIPE_MAIN_BTN_5_2  252
#define PIPE_MAIN_BTN_5_3  253
#define PIPE_MAIN_BTN_5_4  254
#define PIPE_MAIN_BTN_5_5  255
#define PIPE_MAIN_BTN_5_6  256
#define PIPE_MAIN_BTN_6_1  261
#define PIPE_MAIN_BTN_6_2  262
#define PIPE_MAIN_BTN_6_3  263
#define PIPE_MAIN_BTN_6_4  264
#define PIPE_MAIN_BTN_6_5  265
#define PIPE_MAIN_BTN_6_6  266

#define PIPE_TOOLBAR  120
#define PIPE_TOOLBAR_GAME_NEW  121
#define PIPE_TOOLBAR_GAME_GO  122
#define PIPE_TOOLBAR_GAME_EXIT  123
#define PIPE_TOOLBAR_GAME_CONTIUNE  124
#define PIPE_TOOLBAR_SET_SCROLLWIN 130
#define PIPE_TOOLBAR_SET_TOOLBAR 131
#define PIPE_TOOLBAR_COPYRIGHT  132

#define PIPE_MAIN_PRELIST 140
#define PIPE_MAIN_RANKING_LIST 141
#define PIPE_RANKING_LIST_TOOLBAR  142
#define PIPE_RANKING_NAMEEDIT_TOOLBAR  143
#define PIPE_RANKING_NAMEEDIT 144

#define PIPE_SETTING_SOUND 150
#define PIPE_SETTING_DEBUG 151
#define PIPE_SETTING_VOLUME 152
#define PIPE_SETTING_PIPEDIRE 153
#define PIPE_SETTING_README 154
#define PIPE_SETTING_ABOUT 155
#define PIPE_SETTING_README_TOOLBAR 156
#define PIPE_SETTING_ABOUT_TOOLBAR 157
#define PIPE_SETTING_PIPEWHERE 158
#define PIPE_SETTING_SYSVOLUME 159
#define PIPE_SETTING_MKEYACT 160


#define PIPE_RANKING_MAX 20   //��������
#define PIPE_RANKING_HIGHT 55   //����ÿ���ĸ�

#define PIPE_MAIN_LABLE_H 28
#define PIPE_MAIN_SIZE_IMG 75
#define PIPE_MAIN_SIZE_ACR_DESC 20  //���߶���ʹ�õ�HDR��ϲ�Ҫ��С�����Լӿ춯���ٶ�
#define PIPE_MAIN_SIZE_ACR_IMG 55  //���߶���ʹ�õ�HDR���Ҫ��С�����Լӿ춯���ٶ�
#define PIPE_MAIN_SPAN_OUT 5
#define PIPE_MAIN_SPAN_IN 15
#define PIPE_MAIN_SPAN_IMG 1
#define PIPE_HEIGHT_CAPTION 60

#define PIPE_MAIN_NUM_RRELIST_IMG 5  //�¼�������ɼ�
#define PIPE_MAIN_NUM_IMG 45		//���÷���
#define PIPE_MAIN_NUM_IMG_H 5
#define PIPE_MAIN_NUM_IMG_V 8

#define PIPE_MAIN_FIRST_IMG_NO 25

#define PIPE_ANIMO_MAX_THREADS 20
#define PIPE_ANIMO_FRAME_BETWEEN 100        //ÿ֡��ʱ��ms
#define PIPE_ANIMO_FRAME 15		
#define PIPE_ANIMO_HAFE_FRAME 8     //��������֡��

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

CMzStringW getMacAddress();
DWORD WINAPI FinalThread(LPVOID lpParam);
