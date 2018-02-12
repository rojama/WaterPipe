#include "CopyRight.h"
#include "main.h"
#include "Common.h"

CopyRight::CopyRight(void)
{
}

CopyRight::~CopyRight(void)
{
}

// Initialization of the window (dialog)
BOOL CopyRight::OnInitDialog()
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
	m_Static.SetLeftMargin(MZM_MARGIN_BIG);
	m_Static.SetMargin(MZM_MARGIN_BIG);
	m_Static.SetTextSize(MZFS_LARGE);
	CMzStringW text(100);
	text = L"程序名称：水管工\n\n程序版本：1.5\n\n程序授权：";
	char szKey[20];
	if (Common::GetMobleKey(szKey)){
		wchar_t* wszString = new wchar_t[20];
		Common::chr2wch(szKey, &wszString); 		
		text = text + L"未注册\n\n机器码：" + wszString;
	}else{
		text = text + L"未知\n\n机器码：无法获取,请确认电话功能处于打开状态。";
	}
	text = text + L"\n\n注册方法:\n　　请使用以上显示的机器码进行注册，机器码为一机一码请勿泄漏。具体方法详见本人魅族论坛注册专贴。\n　　注册后在魅族论坛本人注册专贴中下载“key”文件附件拷贝到游戏安装目录下，即升级为正式版。\n　　你的支持是我最大的动力，谢谢!\n\n作者信息：\n　魅族论坛ID：gchunyan\n　邮箱：gchunyan@sohu.com";
	m_Static.SetText(text);
	m_ScrollWin.AddChild(&m_Static);

	m_Toolbar.SetPos(0,GetHeight()-MZM_HEIGHT_TEXT_TOOLBAR,GetWidth(),MZM_HEIGHT_TEXT_TOOLBAR);
	m_Toolbar.SetID(PIPE_TOOLBAR_COPYRIGHT);
	m_Toolbar.SetButton(1, true, true, L"退出");
	//m_Toolbar.EnableLeftArrow(true);
	AddUiWin(&m_Toolbar);

	return TRUE;
}

// override the MZFC command handler
void CopyRight::OnMzCommand(WPARAM wParam, LPARAM lParam)
{
	UINT_PTR id = LOWORD(wParam);
	switch(id)
	{
	case PIPE_TOOLBAR_COPYRIGHT:
		{
			int nIndex = lParam;
			if (nIndex==1)
			{
				// exit the modal dialog				
				PostQuitMessage(0);
				EndModal(ID_CANCEL);
				return ;
			}			
		}
		break;
	}
}