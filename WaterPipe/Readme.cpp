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
	m_Static.SetText(L"��ϷĿ�ģ�\n����������Ĵ���������ȡ����õķ�����\n\n��Ϸ�淨��\n�����Ӷ��˱�ע�ĳ�ˮ�ڿ�ʼ�������Աߴ��õ�ˮ�ܵ�����ӽ��룬������Ҫ����ȱ�ڡ������е�ˮ�ܽ����ˮ���ӳ�ˮ����������ˮ������ȱ�ڽ���������������ȱ�ڽ��õ������֣���ˮ�����Ķ��ٽ��������յķ�����\n\n�Ʒֹ���\n��������ˮ��   6�֡���ֱ��ˮ��   7��\n��������ˮ�� 10�֡���ʮ��ˮ�� 13��\n��������ˮ�� 16�֡�����ȱ���� 20��\n\n\n��������ߵķ�����ս�ɣ�");
	m_ScrollWin.AddChild(&m_Static);

	m_Toolbar.SetPos(0,GetHeight()-MZM_HEIGHT_TEXT_TOOLBAR,GetWidth(),MZM_HEIGHT_TEXT_TOOLBAR);
	m_Toolbar.SetID(PIPE_SETTING_README_TOOLBAR);
	m_Toolbar.SetButton(0, true, true, L"����");
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