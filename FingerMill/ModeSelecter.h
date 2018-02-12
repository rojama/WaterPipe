#pragma once
#include <mzfc_inc.h>

#define MZ_IDC_BUT_NO_MODE       201
#define MZ_IDC_BUT_FREE_MODE     202
#define MZ_IDC_BUT_WALK_MODE     203
#define MZ_IDC_BUT_RUN_MODE      204

class ModeSelecter:
	public CMzWndEx
{
protected:
	UiButton_Image m_btn_no;
	UiButton_Image m_btn_free;
	UiButton_Image m_btn_walk;
	UiButton_Image m_btn_run;
	UiPicture BackGround;
public:
	ModeSelecter(void);
	~ModeSelecter(void);
	BOOL OnInitDialog();
	void OnMzCommand(WPARAM wParam, LPARAM lParam);
};
