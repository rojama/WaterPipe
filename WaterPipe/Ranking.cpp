#include "Ranking.h"
#include "resource.h"
#include "main.h"
#include "Common.h"

CMzString RankAppPath = Common::GetProgramDir();


RankingList::RankingList(void)
{
}

RankingList::~RankingList(void)
{
}

void RankingList::DrawItem(HDC hdcDst, int nIndex, RECT* prcItem, RECT *prcWin, RECT *prcUpdate)
{
	// draw the high-light background for the now item
	if (nIndex == highlight_index)
	{
		MzDrawDeleteItemBg(hdcDst, prcItem);
	}

	 // draw the high-light background for the selected item
    if (nIndex == GetSelectedIndex())
    {      
		MzDrawSelectedBg(hdcDst, prcItem);
    }

	// draw the text
	CMzStringW str(30);
	ListItem* pItem = GetItem(nIndex);
	wchar_t *buf = L" 第%d名： %d分　%s";
	if (pItem)
	{		
		wsprintf(str.C_Str(), buf, nIndex+1, *(DWORD*)pItem->Data, pItem->Text.C_Str());
		RECT* textRect = new RECT(*prcItem);
		textRect->right = 430;
		MzDrawText(hdcDst, str.C_Str(), textRect, DT_LEFT|DT_VCENTER|DT_SINGLELINE|DT_END_ELLIPSIS);
		RECT* arrowRect = new RECT(*prcItem);
		arrowRect->left = 430;
		ImagingHelper *imgArrow = ImagingHelper::GetImageObject(GetMzResModuleHandle(),MZRES_IDR_PNG_ARROW_RIGHT, true);
		imgArrow->Draw (hdcDst, arrowRect);
	}
}

Ranking::Ranking(void)
{	
}

Ranking::~Ranking(void)
{
}

int Ranking::getRankingNo(int score){
	DWORD* nowscore;
	int itemcount = m_List.GetItemCount();
	if (score == 0){
		return -1;
	}else if (itemcount == 0){
		return 1;
	}else{
		for (int i=0;i<itemcount;i++){
			nowscore = (DWORD*)m_List.GetItem(i)->Data;
			if (score>*nowscore){
				return i+1;
			}
		}
		if (itemcount<PIPE_RANKING_MAX){
			return itemcount+1;
		}
	}
	return -1;
}

void Ranking::addRanking(TCHAR * name, int score)
{
	ListItem li;
	DWORD* data = new DWORD(score);
	li.Data = data;
	li.Text = name;
	m_List.AddItem(li);
}

void Ranking::insertRanking(int no, TCHAR * name, int score)
{
	ListItem li;
	DWORD* data = new DWORD(score);
	li.Data = data;
	li.Text = name;
	if (m_List.GetItemCount() < no){
		m_List.AddItem(li);
	}else{
		m_List.InsertItem(li,no-1);
	}
	if (m_List.GetItemCount() > PIPE_RANKING_MAX){
		m_List.RemoveItem(PIPE_RANKING_MAX);
	}	
}

// Initialization of the window (dialog)
BOOL Ranking::OnInitDialog()
{
	// Must all the Init of parent class first!
	if (!CMzWndEx::OnInitDialog())
	{
		return FALSE;
	}

	img_head = ImagingHelper::GetImageObject(MzGetInstanceHandle(), IDB_PNG_Ranking_head, true);	

	img_bg = new ImagingHelper();
	if (img_bg->LoadImage(RankAppPath+L"Ranking_bg.bmp",true,true,false)){
		but_bg.SetImage_Normal(img_bg);
		but_bg.SetEnable(false);
		but_bg.SetPos(0,0,GetWidth(),GetHeight()-MZM_HEIGHT_TEXT_TOOLBAR);
		AddUiWin(&but_bg);
	}

	int y = 0;
	but_head.SetImage_Normal(img_head);
	but_head.SetEnable(false);
	but_head.SetPos(0,0,GetWidth(),img_head->GetImageHeight());
	AddUiWin(&but_head);

	y+=PIPE_HEIGHT_CAPTION;

	// Then init the controls & other things in the window
	m_List.SetPos(0,y,GetWidth(),GetHeight()-MZM_HEIGHT_TEXT_TOOLBAR-y);
	m_List.SetID(PIPE_MAIN_RANKING_LIST);
	m_List.EnableScrollBarV(true);
	m_List.EnableNotifyMessage(true);
	m_List.SetItemHeight(PIPE_RANKING_HIGHT);
	m_List.highlight_index = highlight-1;
	m_List.EnableGridlines (false);
	if (highlight>10){
		m_List.ScrollTo(UI_SCROLLTO_BOTTOM , 0, false);
	}else{
		m_List.ScrollTo(UI_SCROLLTO_TOP , 0, false);
	}
	AddUiWin(&m_List);

	m_Toolbar.SetPos(0,GetHeight()-MZM_HEIGHT_TEXT_TOOLBAR,GetWidth(),MZM_HEIGHT_TEXT_TOOLBAR);
	m_Toolbar.EnableLeftArrow(true);
	m_Toolbar.SetButton(0, true, true, L"返回");
	if (showNew){
		m_Toolbar.SetButton(2, true, true, L"新游戏");
	}	
	m_Toolbar.SetID(PIPE_RANKING_LIST_TOOLBAR);
	AddUiWin(&m_Toolbar);


	return TRUE;
}

  // override the MZFC window messages handler
LRESULT Ranking::MzDefWndProc(UINT message, WPARAM wParam, LPARAM lParam)
{
	switch(message)
	{
	// process the mouse notify message
	case MZ_WM_MOUSE_NOTIFY:
	  {
		int nID = LOWORD(wParam);
		int nNotify = HIWORD(wParam);
		int x = LOWORD(lParam);
		int y = HIWORD(lParam);

		// process the mouse left button down notification
		if (nID==PIPE_MAIN_RANKING_LIST && nNotify==MZ_MN_LBUTTONDOWN)
		{
		  if (!m_List.IsMouseDownAtScrolling() && !m_List.IsMouseMoved())
		  {
			int nIndex = m_List.CalcIndexOfPos(x, y);
			m_List.SetSelectedIndex(nIndex);
			m_List.Invalidate();
			m_List.Update();
		  }
		  return 0;
		}
		else
		// process the mouse left button down notification
		if (nID==PIPE_MAIN_RANKING_LIST && nNotify==MZ_MN_LBUTTONUP)
		{
		  if (!m_List.IsMouseDownAtScrolling() && !m_List.IsMouseMoved())
		  {
				int nIndex = m_List.CalcIndexOfPos(x, y);
				if (nIndex == -1) return 0;
				RankingBmp dlg;
				dlg.rankno = nIndex+1;				
				dlg.li = m_List.GetItem(nIndex);
				RECT rcWork = MzGetWorkArea();
				dlg.Create(rcWork.left,rcWork.top,RECT_WIDTH(rcWork),RECT_HEIGHT(rcWork), m_hWnd, 0, WS_POPUP);
				// set the animation of the window
				dlg.SetAnimateType_Show(MZ_ANIMTYPE_SCROLL_RIGHT_TO_LEFT_PUSH);
				dlg.SetAnimateType_Hide(MZ_ANIMTYPE_SCROLL_LEFT_TO_RIGHT_PUSH);
				int nRet = dlg.DoModal();
		  }
		  return 0;
		}
		else
		// process the mouse move notification
		if (nID==PIPE_MAIN_RANKING_LIST && nNotify==MZ_MN_MOUSEMOVE)
		{
		  m_List.SetSelectedIndex(-1);
		  m_List.Invalidate();
		  m_List.Update();
		  return 0;
		}
	  }
	  return 0;
	}
	return CMzWndEx::MzDefWndProc(message,wParam,lParam);
}


void Ranking::SaveList(int from,_TCHAR * RankingName,_TCHAR * SettingFileName){
	_TCHAR buff[20];
	for (int i=from; i<=m_List.GetItemCount(); i++){	
		ListItem* item = m_List.GetItem(i-1);
		wsprintf(buff,_T("name_%d"),i);
		IniWriteString (RankingName, buff, (TCHAR *)item->Text, SettingFileName);
		wsprintf(buff,_T("score_%d"),i);
		IniWriteInt (RankingName, buff, *(DWORD*)item->Data, SettingFileName);		
	}
	//rename bmp
	CMzStringW oldfilename(50);
	CMzStringW newfilename(50);
	for (int i=m_List.GetItemCount()-1; i>=from; i--){
		wsprintf(oldfilename.C_Str(), RankAppPath+L"ranking\\rank_%d.bmp", i);
		wsprintf(newfilename.C_Str(), RankAppPath+L"ranking\\rank_%d.bmp", i+1);
		if (i==PIPE_RANKING_MAX-1){
			DeleteFile(newfilename);
		}
		MoveFile(oldfilename, newfilename); 
	}
}


// override the MZFC command handler
void Ranking::OnMzCommand(WPARAM wParam, LPARAM lParam)
{
	UINT_PTR id = LOWORD(wParam);
	switch(id)
	{
	case PIPE_RANKING_LIST_TOOLBAR:
		{
			img_bg->UnloadImage();
			int nIndex = lParam;
			if (nIndex==0)
			{
				// exit the modal dialog
				EndModal(ID_CANCEL);
				return ;
			}
			if (nIndex==2)
			{
				// exit the modal dialog
				EndModal(ID_OK);
				return ;
			}
		}
		break;
	}
}


////////////////////////////////////////////////
//名字编辑部分
////////////////////////////////////////////

RankingNameEdit::RankingNameEdit(void)
{
}

RankingNameEdit::~RankingNameEdit(void)
{
}


// Initialization of the window (dialog)
BOOL RankingNameEdit::OnInitDialog()
{
	// Must all the Init of parent class first!
	if (!CMzWndEx::OnInitDialog())
	{
		return FALSE;
	}

	// Then init the controls & other things in the window
	int y = 0;
	lable.SetPos(0,y,GetWidth(),PIPE_HEIGHT_CAPTION);
	CMzString str(20);
	wsprintf(str.C_Str(), L"恭喜！你获得了第%d名，得分为%d分。" ,rankno, score);
	lable.SetTextSize(25);
	lable.SetTextColor(RGB(0,0,255));
	lable.SetText(str.C_Str());
	AddUiWin(&lable);

	y+=PIPE_HEIGHT_CAPTION;

	m_Static.SetPos(0,y,GetWidth(),PIPE_HEIGHT_CAPTION);
	m_Static.SetText(L"请输入你的姓名");
	AddUiWin(&m_Static);

	y+=PIPE_HEIGHT_CAPTION;

	lineedit.SetPos(80,y,GetWidth()-160,MZM_HEIGHT_SINGLELINE_EDIT);
	lineedit.SetMaxChars(7);
	lineedit.EnableZoomIn(true);
	lineedit.SetSipMode(IM_SIP_MODE_GEL_PY);
	lineedit.SetText(name);
	lineedit.SetTip2(L"姓名");
	lineedit.SetFocus(true);
	lineedit.SetLeftInvalid(100);
	lineedit.SetID(PIPE_RANKING_NAMEEDIT);
	AddUiWin(&lineedit);

	edit_Toolbar.SetPos(0,GetHeight()-MZM_HEIGHT_TEXT_TOOLBAR,GetWidth(),MZM_HEIGHT_TEXT_TOOLBAR);
	edit_Toolbar.SetButton(0, true, true, L"不记录");
	edit_Toolbar.SetButton(2, true, true, L"记录");
	edit_Toolbar.SetID(PIPE_RANKING_NAMEEDIT_TOOLBAR);
	AddUiWin(&edit_Toolbar);

	return TRUE;
}

// override the MZFC command handler
void RankingNameEdit::OnMzCommand(WPARAM wParam, LPARAM lParam)
{
	UINT_PTR id = LOWORD(wParam);
	switch(id)
	{
	case PIPE_RANKING_NAMEEDIT_TOOLBAR:
		{
			int nIndex = lParam;
			if (nIndex==0)
			{
				// exit the modal dialog				
				if (MzIsSipOpen()){
					MzCloseSip();
				}
				EndModal(ID_CANCEL);
				return ;
			}
			else if (nIndex==2)
			{
				// exit the modal dialog
				name = lineedit.GetText();
				if (MzIsSipOpen()){
					MzCloseSip();
				}
				EndModal(ID_OK);				
				return ;
			}
		}
		break;
	}
}




////////////////////////////////////////////////
//排名结果图部分
////////////////////////////////////////////

RankingBmp::RankingBmp(void)
{
}

RankingBmp::~RankingBmp(void)
{
}


// Initialization of the window (dialog)
BOOL RankingBmp::OnInitDialog()
{
	// Must all the Init of parent class first!
	if (!CMzWndEx::OnInitDialog())
	{
		return FALSE;
	}

	// Then init the controls & other things in the window
	pimg_bg = new ImagingHelper();
	CMzStringW filename(50);	
	wsprintf(filename.C_Str(), RankAppPath+L"ranking\\rank_%d.bmp", rankno);
		
	if (pimg_bg->LoadImage(filename,false,false,false)){
		but_bg.SetImage_Normal(pimg_bg);
		but_bg.SetEnable(false);
		but_bg.SetPos(0,0,GetWidth(),GetHeight()-MZM_HEIGHT_TEXT_TOOLBAR);		
	}else{
		but_bg.SetText(L"记录图像不存在");
		but_bg.SetTextColor(MZCLR_FONT_GRAY);
		but_bg.SetTextSize(MZFS_MAX);
		but_bg.SetEnable(false);
		but_bg.SetPos(0,250,GetWidth(),100);		
	}
	AddUiWin(&but_bg);

	bmp_Toolbar.SetPos(0,GetHeight()-MZM_HEIGHT_TEXT_TOOLBAR,GetWidth(),MZM_HEIGHT_TEXT_TOOLBAR);
	bmp_Toolbar.SetButton(0, true, true, L"排名榜");
	bmp_Toolbar.EnableLeftArrow(true);	
	bmp_Toolbar.SetID(PIPE_RANKING_NAMEEDIT_TOOLBAR);
	AddUiWin(&bmp_Toolbar);

	CMzStringW Text(30);	
	wsprintf(Text.C_Str(), L" 第%d名：%s　%d分", rankno, li->Text.C_Str(), *(DWORD*)li->Data);
	m_Static.SetPos(150,GetHeight()-MZM_HEIGHT_TEXT_TOOLBAR,GetWidth()-150,MZM_HEIGHT_TEXT_TOOLBAR);
	m_Static.SetText(Text);
	m_Static.SetTextColor(MZCLR_TOOLBAR_TEXT_DISABLED);
	m_Static.SetTextSize(MZFS_MID);
	AddUiWin(&m_Static);

	return TRUE;
}

// override the MZFC command handler
void RankingBmp::OnMzCommand(WPARAM wParam, LPARAM lParam)
{
	UINT_PTR id = LOWORD(wParam);
	switch(id)
	{
	case PIPE_RANKING_NAMEEDIT_TOOLBAR:
		{
			int nIndex = lParam;
			if (nIndex==0)
			{
				pimg_bg->UnloadImage();
				EndModal(ID_CANCEL);
				return ;
			}			
		}
		break;
	}
}