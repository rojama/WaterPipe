#pragma once
#include <mzfc_inc.h>

class RankingList:
	public UiList
{
public:
	RankingList(void);
	~RankingList(void);
	int highlight_index;
protected:
	void DrawItem(HDC hdcDst, int nIndex, RECT* prcItem, RECT *prcWin, RECT *prcUpdate);
};

class Ranking :
	public CMzWndEx
{
public:
	Ranking(void);
	~Ranking(void);
	bool showNew;
	UiCaption Listlable;
	RankingList m_List;
	ImagingHelper *img_head; 
	ImagingHelper *img_bg; 
	UiButton_Image but_head;
	UiButton_Image but_bg;
	int highlight;
	int getRankingNo(int score);									//根据分数得到名次(-1 无排名）
	void addRanking(TCHAR * name, int score);				//加在最后一名
	void insertRanking(int no, TCHAR * name, int score);		//插入
	void SaveList(int from,_TCHAR * RankingName,_TCHAR * SettingFileName);
	BOOL SaveBmp(MemoryDC* m_pmemDC, CMzStringW* m_sfilename);
protected:
	UiToolbar_Text m_Toolbar;
	BOOL OnInitDialog();	
	void OnMzCommand(WPARAM wParam, LPARAM lParam);
	LRESULT MzDefWndProc(UINT message, WPARAM wParam, LPARAM lParam);
};


class RankingNameEdit :
	public CMzWndEx
{
public:
	RankingNameEdit(void);
	~RankingNameEdit(void);
	int rankno;
	int score;
	TCHAR * name;
	UiCaption lable;
	UiStatic m_Static;
	UiSingleLineEdit lineedit;	
protected:
	UiToolbar_Text edit_Toolbar;
	BOOL OnInitDialog();
	void OnMzCommand(WPARAM wParam, LPARAM lParam);
};


class RankingBmp :
	public CMzWndEx
{
public:
	RankingBmp(void);
	~RankingBmp(void);
	int rankno;
	ListItem* li;
	ImagingHelper* pimg_bg;
protected:
	UiButton_Image but_bg;	
	UiStatic m_Static;
	UiToolbar_Text bmp_Toolbar;
	BOOL OnInitDialog();
	void OnMzCommand(WPARAM wParam, LPARAM lParam);
};