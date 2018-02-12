#include "ModeSelecter.h"
#include "Common.h"

ModeSelecter::ModeSelecter(void)
{
}

ModeSelecter::~ModeSelecter(void)
{
}

// Initialization of the window (dialog)
BOOL ModeSelecter::OnInitDialog()
{
	// Must all the Init of parent class first!
	if (!CMzWndEx::OnInitDialog())
	{
		return FALSE;
	}

	RECT rcWork = MzGetWorkArea();
	BackGround.SetPos(0,0,RECT_WIDTH(rcWork),RECT_HEIGHT(rcWork));
	BackGround.LoadImage(Common::GetProgramDir()+L"select_mode.bmp",false);
	AddUiWin(&BackGround);

	// Then init the controls & other things in the window
		m_btn_no.SetButtonType(MZC_BUTTON_LINE_NONE);
		m_btn_no.SetPos(33,611,120,45);
		m_btn_no.SetID(MZ_IDC_BUT_NO_MODE);
		//m_btn_no.SetText(L"返回游戏");
		//m_btn_no.SetTextColor(RGB(0,0,255));
		AddUiWin(&m_btn_no);

		m_btn_free.SetButtonType(MZC_BUTTON_LINE_NONE);
		m_btn_free.SetPos(336,142,120,45);
		m_btn_free.SetID(MZ_IDC_BUT_FREE_MODE);
		//m_btn_free.SetText(L"自由练习模式");
		//m_btn_free.SetTextColor(RGB(0,0,255));
		AddUiWin(&m_btn_free);

		m_btn_walk.SetButtonType(MZC_BUTTON_LINE_NONE);
		m_btn_walk.SetPos(336,268,120,45);
		m_btn_walk.SetID(MZ_IDC_BUT_WALK_MODE);
		//m_btn_walk.SetText(L"30秒竞走模式");
		//m_btn_walk.SetTextColor(RGB(0,0,255));
		AddUiWin(&m_btn_walk);
	
		m_btn_run.SetButtonType(MZC_BUTTON_LINE_NONE);
		m_btn_run.SetPos(336,395,120,45);
		m_btn_run.SetID(MZ_IDC_BUT_RUN_MODE);
		//m_btn_run.SetText(L"龟兔赛跑模式");
		//m_btn_run.SetTextColor(RGB(0,0,255));
		AddUiWin(&m_btn_run);
	

	return TRUE;
}

// override the MZFC command handler
void ModeSelecter::OnMzCommand(WPARAM wParam, LPARAM lParam)
{
	UINT_PTR id = LOWORD(wParam);
	switch(id)
	{
	case MZ_IDC_BUT_NO_MODE:
		EndModal(0);
		break;
	case MZ_IDC_BUT_FREE_MODE:
		EndModal(1);
		break;
	case MZ_IDC_BUT_WALK_MODE:
		EndModal(2);
		break;
	case MZ_IDC_BUT_RUN_MODE:
		EndModal(3);
		break;
	}
}