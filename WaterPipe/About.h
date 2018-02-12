#pragma once
#include <mzfc_inc.h>

class About :
	public CMzWndEx
{
public:
	About(void);
	~About(void);
protected:
	UiScrollWin m_ScrollWin;
	UiStatic m_Static;
	UiToolbar_Text m_Toolbar;
	BOOL OnInitDialog();
	void OnMzCommand(WPARAM wParam, LPARAM lParam);
};
