#pragma once
#include <mzfc_inc.h>

class CopyRight :
	public CMzWndEx
{
public:
	CopyRight(void);
	~CopyRight(void);
protected:
	UiScrollWin m_ScrollWin;
	UiStatic m_Static;
	UiToolbar_Text m_Toolbar;
	BOOL OnInitDialog();
	void OnMzCommand(WPARAM wParam, LPARAM lParam);
};
