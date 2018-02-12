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
	text = L"�������ƣ�ˮ�ܹ�\n\n����汾��1.5\n\n������Ȩ��";
	char szKey[20];
	if (Common::GetMobleKey(szKey)){
		wchar_t* wszString = new wchar_t[20];
		Common::chr2wch(szKey, &wszString); 		
		text = text + L"δע��\n\n�����룺" + wszString;
	}else{
		text = text + L"δ֪\n\n�����룺�޷���ȡ,��ȷ�ϵ绰���ܴ��ڴ�״̬��";
	}
	text = text + L"\n\nע�᷽��:\n������ʹ��������ʾ�Ļ��������ע�ᣬ������Ϊһ��һ������й©�����巽���������������̳ע��ר����\n����ע�����������̳����ע��ר�������ء�key���ļ�������������Ϸ��װĿ¼�£�������Ϊ��ʽ�档\n�������֧���������Ķ�����лл!\n\n������Ϣ��\n��������̳ID��gchunyan\n�����䣺gchunyan@sohu.com";
	m_Static.SetText(text);
	m_ScrollWin.AddChild(&m_Static);

	m_Toolbar.SetPos(0,GetHeight()-MZM_HEIGHT_TEXT_TOOLBAR,GetWidth(),MZM_HEIGHT_TEXT_TOOLBAR);
	m_Toolbar.SetID(PIPE_TOOLBAR_COPYRIGHT);
	m_Toolbar.SetButton(1, true, true, L"�˳�");
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