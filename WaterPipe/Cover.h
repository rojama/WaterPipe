#pragma once
#include <mzfc_inc.h>

class Cover: public CMzWndEx
{
public:
	Cover(void);
	~Cover(void);
	ImagingHelper* pimg_bg;
protected:	
	UiButton_Image but_bg;
	BOOL OnInitDialog();	
};
