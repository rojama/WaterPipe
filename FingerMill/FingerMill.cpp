#include "FingerMill.h"
#include "Common.h"

int GameMode = 1;    //1��������ϰģʽ    2��30�뾺��ģʽ     3����������ģʽ

RECT rcWork = MzGetWorkArea();
CMzString AppPath = Common::GetProgramDir();

int skip;
int lenght;
int speed;
int time;
bool counttime;
bool isStop;
int nowlenght;  //��ʱ���漴ʱ�ٶ���һ��ʱ��ľ���
int nowspeed;

int oldlen; //��һ����ʱ�ľ���

UiStatic m_len;
UiStatic m_speed;
UiStatic m_nowspeed;
UiStatic m_skip;
UiStatic m_time;

UiPicture Bar;
UiPicture FullBar;

vector<GKData> all_gk; //ÿ���ؿ�
int gk_no = 1;       //Ŀǰ�Ĺؿ���
GKData now_gk;       //Ŀǰ�Ĺؿ�

DWORD dwCountTimeThreadId;
HANDLE hCountTimeThread; 
DWORD dwCountNowTimeThreadId;
HANDLE hCountNowTimeThread; 

void StopRun(){
	counttime= false;
	isStop=true;
}


//������߳�
DWORD WINAPI CountTime(LPVOID lpParam)  
{
	while (counttime){
		Sleep(1000);
		time++;
		speed = lenght/time/2;
		WCHAR bb[30];
		//wsprintf(bb,L"��ʱ��%02d��:%02d��",time/60,time%60);
		wsprintf(bb,L"%d   %02d",time/60,time%60);
		m_time.SetText(bb);
		m_time.Invalidate();
		//m_time.Update();
		//wsprintf(bb,L"ƽ���ٶȣ�%d��/��",speed*3/10);
		wsprintf(bb,L"%d",speed*3/10);
		m_speed.SetText(bb);
		m_speed.Invalidate();
		//m_speed.Update();

		if (GameMode == 2){
			FullBar.SetPos(90,52,(300/GAME_MAX_TIME)*time,20);
			FullBar.Invalidate();
			//FullBar.Update();
			if (time == GAME_MAX_TIME){
				StopRun();
			}
		}else if (GameMode == 3){
			if (time == now_gk.time){
				StopRun();
			}
		}
	}	
	return true;
}

//������߳�
DWORD WINAPI CountNowTime(LPVOID lpParam)  
{
	while (counttime){
		Sleep(GAME_SPEED_BETWEEN_TIME);
		nowspeed = (lenght-nowlenght)*1000/GAME_SPEED_BETWEEN_TIME/2;
		nowlenght = lenght;
		WCHAR bb[30];
		//wsprintf(bb,L"��ʱ�ٶȣ�%d��/��",nowspeed*3/10);
		wsprintf(bb,L"%d",nowspeed*3/10);
		m_nowspeed.SetText(bb);
		m_nowspeed.Invalidate();
		//m_nowspeed.Update();
	}	
	return true;
}


// ��CMzWnd������������
class CTouchMainWnd: public CMzWndEx
{
	MZ_DECLARE_DYNAMIC(CTouchMainWnd);
public:
	~CTouchMainWnd(void)
	{
		UnRegisterTouchNotify(m_hWnd,MZ_TOUCH);
	}
	// �������е��Ӵ��ڱ�����һ����ť
	UiButton m_btn_reset;
	UiButton m_btn_stop;
	UiButton m_btn_exit;
	UiButton_Image m_btn_select_mode;
	UiButton_Image m_btn_high_score;
	UiPicture BackGround;
	UiPicture AnimoRun;

	UiPicture ModeTitle;


	int old_y;

	//����
	int animo_no;

protected:
	void Reset(){
		counttime= false;
		WaitForSingleObject(hCountTimeThread,INFINITE);
		CloseHandle(hCountTimeThread);
		WaitForSingleObject(hCountNowTimeThread,INFINITE);
		CloseHandle(hCountNowTimeThread);

		skip=0;
		lenght=0;
		speed=0;
		old_y=0;
		time=0;
		isStop=false;
		oldlen=0;
		nowlenght=0;
		nowspeed=0;

		m_skip.SetTextColor(RGB(255,255,255));
		//m_skip.SetText(L"������0��");
		m_skip.SetText(L"0");
		m_skip.Invalidate();
		m_skip.Update();
		m_len.SetTextColor(RGB(255,255,255));
		//m_len.SetText(L"���룺0��");
		m_len.SetText(L"0");
		m_len.Invalidate();
		m_len.Update();
		m_time.SetTextColor(RGB(255,255,255));
		//m_time.SetText(L"��ʱ��0��:00��");
		m_time.SetText(L"0   00");
		m_time.Invalidate();
		m_time.Update();
		m_speed.SetTextColor(RGB(255,255,255));
		//m_speed.SetText(L"ƽ���ٶȣ�0��/��");
		m_speed.SetText(L"0");
		m_speed.Invalidate();
		m_speed.Update();
		m_nowspeed.SetTextColor(RGB(255,255,255));
		//m_nowspeed.SetText(L"��ʱ�ٶȣ�0��/��");
		m_nowspeed.SetText(L"0");
		m_nowspeed.Invalidate();
		m_nowspeed.Update();

		if (GameMode == 2){
			ModeTitle.Invalidate();
			ModeTitle.Update();
			FullBar.SetPos(90,52,0,20);
			FullBar.Invalidate();
			FullBar.Update();
		}
	}

	void ChangeMode(int mode)
	{
		GameMode=mode;
		if (mode==1){
			ModeTitle.LoadImage(AppPath+L"mode1_title.png",true);
			ModeTitle.Invalidate();
			ModeTitle.Update();
			//RemoveUiWin (&Bar);
			//Bar.Invalidate();
			//Bar.Update();
			RemoveUiWin (&FullBar);
			FullBar.Invalidate();
			FullBar.Update();					
		}else if (mode==2){
			ModeTitle.LoadImage(AppPath+L"mode2_title.png",true);
			ModeTitle.Invalidate();
			ModeTitle.Update();
			//Bar.SetPos(90,30,300,20);
			//Bar.LoadImage(AppPath+L"bank_bar.png",true);
			//AddUiWin(&Bar);
			//Bar.Invalidate();
			//Bar.Update();
			FullBar.SetPos(90,52,0,20);
			FullBar.LoadImage(AppPath+L"full_bar.png",true);
			AddUiWin(&FullBar);
			FullBar.Invalidate();
			FullBar.Update();
		}
	}

	// ���ش��ڳ�ʼ������
	virtual BOOL OnInitDialog()
	{
		// �ȵ��û���ĳ�ʼ������
		if (!CMzWndEx::OnInitDialog())
		{
			return FALSE;
		}
		
		// ��ʼ���������еĿؼ�
		RECT rcWork = MzGetWorkArea();
		BackGround.SetPos(0,0,RECT_WIDTH(rcWork),RECT_HEIGHT(rcWork));
		BackGround.LoadImage(AppPath+L"Main_bg.bmp",false);
		//BackGround.LoadImage(MzGetInstanceHandle(),RT_RCDATA,(LPCWSTR)IDB_BITMAP1, 0);
		AddUiWin(&BackGround);

		AnimoRun.SetPos(90,305,298,332);
		animo_no = 1;
		AnimoRun.LoadImage(AppPath+L"Run_1.png",true);
		AddUiWin(&AnimoRun);

		ImagingHelper* mode1 = new ImagingHelper();
		ImagingHelper* mode2 = new ImagingHelper();
		mode1->LoadImageW(AppPath+L"mode_1.png",true,true,true);
		m_btn_select_mode.SetImage_Normal(mode1);
		mode2->LoadImageW(AppPath+L"mode_2.png",true,true,true);
		m_btn_select_mode.SetImage_Pressed(mode2);
		m_btn_select_mode.SetPos(0,0,90,90);
		m_btn_select_mode.SetID(MZ_IDC_BUT_SELECT_MODE);
		AddUiWin(&m_btn_select_mode);

		ImagingHelper* score1 = new ImagingHelper();
		ImagingHelper* score2 = new ImagingHelper();
		score1->LoadImageW(AppPath+L"score_1.png",true,true,true);
		m_btn_high_score.SetImage_Normal(score1);
		score2->LoadImageW(AppPath+L"score_2.png",true,true,true);
		m_btn_high_score.SetImage_Pressed(score2);
		m_btn_high_score.SetPos(390,0,90,90);
		m_btn_high_score.SetID(MZ_IDC_BUT_HIGHT_SCORE);
		AddUiWin(&m_btn_high_score);

		m_btn_reset.SetButtonType(MZC_BUTTON_GRAY);
		m_btn_reset.SetPos(0,80,120,80);
		m_btn_reset.SetID(MZ_IDC_BUT_RESET);
		m_btn_reset.SetText(L"����");
		m_btn_reset.SetTextColor(RGB(0,0,255));
		AddUiWin(&m_btn_reset);

		m_btn_stop.SetButtonType(MZC_BUTTON_GRAY);
		m_btn_stop.SetPos(360,80,120,80);
		m_btn_stop.SetID(MZ_IDC_BUT_STOP);
		m_btn_stop.SetText(L"ֹͣ");
		m_btn_stop.SetTextColor(RGB(0,0,255));
		AddUiWin(&m_btn_stop);

		/*m_btn_exit.SetButtonType(MZC_BUTTON_GRAY);
		m_btn_exit.SetPos(360,80,120,80);
		m_btn_exit.SetID(MZ_IDC_BUT_EXIT);
		m_btn_exit.SetText(L"�˳�");
		m_btn_exit.SetTextColor(RGB(0,0,255));
		AddUiWin(&m_btn_exit);*/

		ModeTitle.SetPos(90,0,300,80);
		AddUiWin(&ModeTitle);

		m_skip.SetPos(180,90,45,20);
		m_skip.SetTextSize(15);
		m_skip.SetDrawTextFormat(DT_RIGHT|DT_VCENTER);
		AddUiWin(&m_skip);
		m_len.SetPos(180,112,45,20);
		m_len.SetTextSize(15);
		m_len.SetDrawTextFormat(DT_RIGHT|DT_VCENTER);
		AddUiWin(&m_len);
		m_time.SetPos(180,134,45,20);
		m_time.SetTextSize(15);
		m_time.SetDrawTextFormat(DT_RIGHT|DT_VCENTER);
		AddUiWin(&m_time);
		m_speed.SetPos(244,101,45,20);
		m_speed.SetTextSize(15);
		m_speed.SetDrawTextFormat(DT_RIGHT|DT_VCENTER);
		AddUiWin(&m_speed);
		m_nowspeed.SetPos(244,135,45,20);
		m_nowspeed.SetTextSize(15);
		m_nowspeed.SetDrawTextFormat(DT_RIGHT|DT_VCENTER);
		AddUiWin(&m_nowspeed);

		ChangeMode(GameMode);
		Reset();		

		if(RegisterTouchNotifyEx(m_hWnd,MZ_TOUCH,TCH_NOTIFY_FLAG_RAWDATA)==FALSE)
			PostQuitMessage(0);
		return TRUE;
	}

	void StartRun(){
		counttime = true;
		hCountTimeThread = CreateThread(NULL,0,CountTime,NULL,0,&dwCountTimeThreadId);
		if (hCountTimeThread == NULL) 
		{
			CloseHandle(hCountTimeThread);
			ExitProcess(0);				
		}
		hCountNowTimeThread = CreateThread(NULL,0,CountNowTime,NULL,0,&dwCountNowTimeThreadId);
		if (hCountTimeThread == NULL) 
		{
			CloseHandle(hCountNowTimeThread);
			ExitProcess(0);				
		}
	}


	LRESULT MzDefWndProc(UINT message, WPARAM wParam, LPARAM lParam)
	{
		TOUCH_RAW_DATA aa;
		switch(message)
		{

		case MZ_TOUCH:
			if (isStop){
				return 0;
			}

			GetTouchRawData(&aa);
			int x = aa.FingerData[0].x;
			int y = aa.FingerData[0].y;


			//��Ч��Χ
			if (y<300 || x<90 || x>390){
				return 0;
			}

			//int z = aa.FingerData[0].z;
			//HDC dc=::GetDC(m_hWnd);
			//Ellipse(dc,x-z,y-z,x+z,y+z);
			//ReleaseDC(m_hWnd,dc);

			WCHAR bb[30];

			//��Ч����
			if ((old_y-y >= GAME_ONE_SKIP_UP_DOWN_MIN_LEG && lenght-oldlen >= GAME_ONE_SKIP_MIN_LEG) || skip == 0){
				if (skip == 0){
					StartRun();
				}
				skip++;
				oldlen = lenght;
				//wsprintf(bb,L"������%d��",skip);
				wsprintf(bb,L"%d",skip);
				m_skip.SetText(bb);
				m_skip.Invalidate();
				m_skip.Update();
			}else{
				//��Ч�߲�
				int sublen = y-old_y;
				if (sublen>0 && sublen<=GAME_TWO_SKIP_BETWEEN_MAX_LEG){					
					lenght += sublen;
					//nowlen += sublen;
					//nowlenght += sublen;
					//wsprintf(bb,L"���룺%d��",lenght/200);
					wsprintf(bb,L"%d",lenght/200);
					m_len.SetText(bb);
					m_len.Invalidate();
					m_len.Update();
					//����
					if (animo_no == 1){
						AnimoRun.LoadImage(AppPath+L"Run_2.png",true);
						animo_no = 2;
					}else{
						AnimoRun.LoadImage(AppPath+L"Run_1.png",true);
						animo_no = 1;
					}
					AnimoRun.Invalidate();
					AnimoRun.Update();

				}
			}
			old_y = y;

			break;

		}

		return CMzWndEx::MzDefWndProc(message,wParam,lParam);
	}


	// ����MZFC��������Ϣ������
	virtual void OnMzCommand(WPARAM wParam, LPARAM lParam)
	{
		UINT_PTR id = LOWORD(wParam);
		switch(id)
		{
		case MZ_IDC_BUT_SELECT_MODE:
			{
				StopRun();
				ModeSelecter dlg;
				dlg.Create(rcWork.left,rcWork.top,RECT_WIDTH(rcWork),RECT_HEIGHT(rcWork), m_hWnd, 0, WS_POPUP);
				dlg.SetAnimateType_Show(MZ_ANIMTYPE_SCROLL_LEFT_TO_RIGHT_2);
				dlg.SetAnimateType_Hide(MZ_ANIMTYPE_SCROLL_RIGHT_TO_LEFT_1);
				int nRet = dlg.DoModal();
				if (nRet!=0)
				{
					//GameMode = nRet;
					ChangeMode(nRet);
					Reset();
				}

			}
			break;
		case MZ_IDC_BUT_RESET:
			Reset();
			break;
		case MZ_IDC_BUT_EXIT:
			PostQuitMessage(0);
			break;
		case MZ_IDC_BUT_STOP:
			StopRun();
			break;
		}
	}
};

MZ_IMPLEMENT_DYNAMIC(CTouchMainWnd)

// ��CMzApp������Ӧ�ó�����
class CTouchApp: public CMzApp
{
public:
	// �����ڱ���
	CTouchMainWnd m_MainWnd;

	// ����Init����
	virtual BOOL Init()
	{
		CoInitializeEx(0, COINIT_MULTITHREADED);
		//������
		SetPowerRequirement(L"BKL1:",D0,POWER_NAME,NULL,0);

		// ��ʼ������
		GKData gk;
		gk.name = L"�ڹ�";
		gk.time = 100;
		gk.leng = 200;
		all_gk.push_back(gk);	
		gk.name = L"С��";
		gk.time = 100;
		gk.leng = 200;
		all_gk.push_back(gk);
		gk.name = L"����";
		gk.time = 100;
		gk.leng = 200;
		all_gk.push_back(gk);
		gk.name = L"С����";
		gk.time = 100;
		gk.leng = 200;
		all_gk.push_back(gk);
		gk.name = L"";
		gk.time = 100;
		gk.leng = 200;
		all_gk.push_back(gk);
		gk.name = L"";
		gk.time = 100;
		gk.leng = 200;
		all_gk.push_back(gk);
		gk.name = L"";
		gk.time = 100;
		gk.leng = 200;
		all_gk.push_back(gk);
		gk.name = L"";
		gk.time = 100;
		gk.leng = 200;
		all_gk.push_back(gk);
		gk.name = L"";
		gk.time = 100;
		gk.leng = 200;
		all_gk.push_back(gk);


		//����������
		RECT rcWork = MzGetWorkArea();
		m_MainWnd.Create(rcWork.left,rcWork.top,RECT_WIDTH(rcWork),RECT_HEIGHT(rcWork), 0, 0, 0);
		m_MainWnd.Show();
		return TRUE;
	}
};

// Ӧ�ó���ȫ�ֱ���
CTouchApp theApp;