#include "About.h"
#include "main.h"
#include "Common.h"

About::About(void)
{
}

About::~About(void)
{
}


// Initialization of the window (dialog)
BOOL About::OnInitDialog()
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
	m_Static.SetTextSize(MZFS_NORMAL);
	CMzStringW text(100);
	text = L"�������ƣ�ˮ�ܹ�\n\n����汾��1.5\n\n������Ȩ��������Ȩ��֤\n\n������Ϣ��\n��������̳ID��gchunyan\n�����䣺gchunyan@sohu.com";
	m_Static.SetText(text);
	m_ScrollWin.AddChild(&m_Static);

	m_Toolbar.SetPos(0,GetHeight()-MZM_HEIGHT_TEXT_TOOLBAR,GetWidth(),MZM_HEIGHT_TEXT_TOOLBAR);
	m_Toolbar.SetID(PIPE_SETTING_ABOUT_TOOLBAR);
	m_Toolbar.SetButton(0, true, true, L"����");
	m_Toolbar.EnableLeftArrow(true);
	AddUiWin(&m_Toolbar);

	return TRUE;
}

// override the MZFC command handler
void About::OnMzCommand(WPARAM wParam, LPARAM lParam)
{
	UINT_PTR id = LOWORD(wParam);
	switch(id)
	{
	case PIPE_SETTING_ABOUT_TOOLBAR:
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