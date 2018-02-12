#include "Readme.h"
#include "main.h"

Readme::Readme(void)
{
}

Readme::~Readme(void)
{
}

// Initialization of the window (dialog)
BOOL Readme::OnInitDialog()
{
	// Must all the Init of parent class first!
	if (!CMzWndEx::OnInitDialog())
	{
		return FALSE;
	}

	// Then init the controls & other things in the window
	m_ScrollWin.SetPos(0,0,GetWidth(),GetHeight()-MZM_HEIGHT_TEXT_TOOLBAR);
	m_ScrollWin.EnableScrollBarV(true);
	AddUiWin(&m_ScrollWin);

	m_Static.SetPos(0,0,GetWidth(),GetHeight()-MZM_HEIGHT_TEXT_TOOLBAR);
	m_Static.SetDrawTextFormat(DT_WORDBREAK);
	m_Static.SetLeftMargin(MZM_MARGIN_MID);
	m_Static.SetMargin(MZM_MARGIN_MID);
	m_Static.SetTextSize(MZFS_MID);
	m_Static.SetText(L"游戏目的：\n　　运用你的聪明才智来取得最好的分数。\n\n游戏玩法：\n　　从顶端标注的出水口开始，根据旁边待用的水管点击格子接入，尽量不要留有缺口。在所有的水管接入后，水将从出水口中流出，水流遇见缺口将不再流动（如无缺口将得到奖励分），水流出的多少将决定最终的分数。\n\n计分规则：\n　　弧线水管   6分、　直线水管   7分\n　　丁形水管 10分、　十字水管 13分\n　　立交水管 16分、　无缺奖励 20分\n\n\n　　向更高的分数挑战吧！");
	m_ScrollWin.AddChild(&m_Static);

	m_Toolbar.SetPos(0,GetHeight()-MZM_HEIGHT_TEXT_TOOLBAR,GetWidth(),MZM_HEIGHT_TEXT_TOOLBAR);
	m_Toolbar.SetID(PIPE_SETTING_README_TOOLBAR);
	m_Toolbar.SetButton(0, true, true, L"设置");
	m_Toolbar.EnableLeftArrow(true);
	AddUiWin(&m_Toolbar);

	return TRUE;
}

// override the MZFC command handler
void Readme::OnMzCommand(WPARAM wParam, LPARAM lParam)
{
	UINT_PTR id = LOWORD(wParam);
	switch(id)
	{
	case PIPE_SETTING_README_TOOLBAR:
		{
			int nIndex = lParam;
			if (nIndex==0)
			{
				// exit the modal dialog				
				EndModal(ID_CANCEL);
				return ;
			}			
		}
		break;
	}
}