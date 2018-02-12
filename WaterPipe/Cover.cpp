#include "Cover.h"
#include "Common.h"

Cover::Cover(void)
{
}

Cover::~Cover(void)
{
}


// Initialization of the window (dialog)
BOOL Cover::OnInitDialog()
{
	// Must all the Init of parent class first!
	if (!CMzWndEx::OnInitDialog())
	{
		return FALSE;
	}

	// Then init the controls & other things in the window
	but_bg.SetImage_Normal(pimg_bg);
	but_bg.SetEnable(false);
	but_bg.SetPos(0,0,GetWidth(),GetHeight());
	AddUiWin(&but_bg);

	return TRUE;
}