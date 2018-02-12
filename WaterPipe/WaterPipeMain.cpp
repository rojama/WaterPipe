#include "Ranking.h"
#include "About.h"
#include "CopyRight.h"
#include "Cover.h"
#include "Readme.h"
#include "resource.h"
#include "main.h"
#include "Common.h"

//全局HDC
HDC hdc;
HWND hWnd;
RECT rcWork = MzGetWorkArea();
int frame_length = 5;  //直线步进长
ImagingHelper* pimg_droplets;
BOOL V_DEBUG = false;

//程序变量
_TCHAR *AppName = _T("WaterPipe");
_TCHAR *RankingName = _T("Ranking");
CMzString AppPath = Common::GetProgramDir();
CMzString SettingFileName(MAX_PATH);
HANDLE hFinalThread;
DWORD dwFinalThreadId;
UiStatic static_level;
UiStatic static_score;
DWORD dwVolume = 0xFFFF;
DWORD dwOrgVolume = 0xFFFF;
bool isInGame = true;
bool isInRepain = false; //重绘
bool isSoundsOn = true;
bool isPreLeft = true;
bool org_isPreLeft = isPreLeft;
int mAction = SHK_RET_APPEXIT_SHELLTOP;  //return value of CMzWnd::OnShellHomeKey()
bool isdebug = false;
bool isPreListUp = true;
bool org_isPreListUp = isPreListUp;
int now_item_no;
vector<ImagingHelper *> pipe_img_all;
vector<UiButton_Image> but_all;
vector<int> now_chack_box;
vector<BoxStatic> box_static;   //at(每个方块的号码)
vector<BoxAnimo> box_animo;		//at(一个步进动画的每个方块)
vector<vector<BoxAnimo>> animo; //at(每个步进动画的号码)
vector<ErrBoxAnimo> box_err_animo;		//at(流水溢出动画的每个方块)
bool isErr;
int Score;  //分数
CMzString debug_str(500);
Ranking ranking;
CMzStringW ranking_last_name(10);
MemoryDC* mdc = new MemoryDC();		//最后的界面保存


//FULL ANIMO PNG
BLENDFUNCTION bf;
ImagingHelper *img_AllWater;  

//设音量
void SetVolume(DWORD volume){
	waveOutSetVolume(0,MAKELPARAM(volume,volume));
}

//包含子串
bool IsContentStr(wchar_t *str, wchar_t *substr){
	wchar_t *p   =   wcsstr(str,substr);   
	if (p == 0){
		return false;
	}else{
		return true;
	}
}

//取BOX的列号  （1开始）
int GetBoxNoInH(int box_no){
	return box_no%PIPE_MAIN_NUM_IMG_H+1;
}

//取BOX的行号 （1开始）
int GetBoxNoInV(int box_no){
	return box_no/PIPE_MAIN_NUM_IMG_H+1;
}

//计分
void AddScore(int box_no){
	//"R", "LR", "UD", "LUR", "LUR", "LRD", "LUD", "LURD", "LURDX", "LU", "RU", "RD", "LD", "LURD\", "LURD//"
	CMzString * BoxDire = &box_static.at(box_no).in_lab;
	CMzString * BoxTag = &box_static.at(box_no).tag;
	if (*BoxTag==L"LR" || *BoxTag==L"UD"){
		Score += 7;
	}else if (BoxTag->Length()==3){
		Score += 10;
	}else if (*BoxTag==L"LURD"){
		Score += 13;
	}else if (*BoxTag==L"LURDX"){
		if ((IsContentStr(BoxDire->C_Str(),L"U")||IsContentStr(BoxDire->C_Str(),L"D"))&&(IsContentStr(BoxDire->C_Str(),L"L")||IsContentStr(BoxDire->C_Str(),L"R"))){
			Score += 16;
		}else{
			Score += 8;
		}
	}else if (BoxTag->Length()==2){
		Score += 6;
	}else if (*BoxTag==L"LURD\\"){
		if ((IsContentStr(BoxDire->C_Str(),L"U")||IsContentStr(BoxDire->C_Str(),L"R"))&&(IsContentStr(BoxDire->C_Str(),L"D")||IsContentStr(BoxDire->C_Str(),L"L"))){
			Score += 12;
		}else{
			Score += 6;
		}
	}else if (*BoxTag==L"LURD/"){
		if ((IsContentStr(BoxDire->C_Str(),L"U")||IsContentStr(BoxDire->C_Str(),L"L"))&&(IsContentStr(BoxDire->C_Str(),L"D")||IsContentStr(BoxDire->C_Str(),L"R"))){
			Score += 12;
		}else{
			Score += 6;
		}
	}
}


//出错动画
void ErrAnimo(){
	for (int i=0; i<box_err_animo.size();i++){		
		//debug
		if (isdebug){
			wsprintf(debug_str.C_Str(), debug_str+L"E(%d-%d)-%s  " ,GetBoxNoInV(box_err_animo.at(i).box), GetBoxNoInH(box_err_animo.at(i).box), box_err_animo.at(i).err_lab.C_Str());
			DrawText(hdc,debug_str.C_Str(),lstrlen (debug_str.C_Str()),&rcWork,0);
		}
		wprintf(L"ErrAnimo (%d-%d), In-%s" ,GetBoxNoInV(box_err_animo.at(i).box), GetBoxNoInH(box_err_animo.at(i).box), box_err_animo.at(i).err_lab.C_Str());
	}

	UiButton_Image* but;
	CMzStringW errlab;
	HDC sdc = pimg_droplets->GetDC();
	MemoryDC* mdc = new MemoryDC();
	mdc->Create(RECT_WIDTH(rcWork),RECT_HEIGHT(rcWork));
	BitBlt(mdc->GetDC(), 0, 0, mdc->GetWidth(),mdc->GetHeight(), hdc, 0, 0, SRCCOPY);
	for (int k=0; k<3; k++){
		for (int i=0; i<17; i++){
			BitBlt(hdc, 0, 0, mdc->GetWidth(),mdc->GetHeight(), mdc->GetDC(), 0, 0, SRCCOPY);
			for (int n=0; n<box_err_animo.size();n++){
				errlab = box_err_animo.at(n).err_lab;
				but = &but_all.at(box_err_animo.at(n).box);
				int desc_x = but->GetPosX();
				int desc_y = but->GetPosY();
				if (errlab==L"L")
				{
					desc_y+=PIPE_MAIN_SIZE_IMG/2;
				}else if (errlab==L"U")
				{
					desc_x+=PIPE_MAIN_SIZE_IMG/2;
				}else if (errlab==L"R")
				{
					desc_x+=PIPE_MAIN_SIZE_IMG;
					desc_y+=PIPE_MAIN_SIZE_IMG/2;
				}else if (errlab==L"D")
				{
					desc_x+=PIPE_MAIN_SIZE_IMG/2;
					desc_y+=PIPE_MAIN_SIZE_IMG;
				}
				desc_x-=45;
				desc_y-=65;
				int now_x1 = desc_x;
				int now_x2 = desc_x+10;
				int now_y = desc_y;
				for (int m=0; m<=i; m++){
					if(m<6){
						now_x1 -= 4;
						now_x2 += 4;
						now_y -= 4;
					}else{
						now_x1 -= 3;
						now_x2 += 3;
						now_y += 10;
					}
				}
				int now_img_top = i*81+1;
				BitBlt(hdc, now_x1, now_y, 80, 80, sdc, 1, now_img_top, SRCAND);
				BitBlt(hdc, now_x1, now_y, 80, 80, sdc, 82, now_img_top, SRCPAINT);
				BitBlt(hdc, now_x2, now_y, 80, 80, sdc, 163, now_img_top, SRCAND);
				BitBlt(hdc, now_x2, now_y, 80, 80, sdc, 244, now_img_top, SRCPAINT);							
			}
			Sleep(50);	
		}
	}
	mdc->Unload();
}

//初始的BOX的检查
void ChackFirst(){
	for (int i=0; i<now_chack_box.size(); i++){
		int now = now_chack_box.at(i);
		CMzStringW now_in = box_static.at(now).in_lab;
		if (IsContentStr(now_in,L"L")){
			if (!IsContentStr(box_static.at(now).tag,L"L")){
				isErr=true;
				ErrBoxAnimo erranimo;
				erranimo.box = now;
				erranimo.err_lab = L"L";
				box_err_animo.push_back(erranimo);
			}
		}
		if (IsContentStr(now_in,L"D")){
			if (!IsContentStr(box_static.at(now).tag,L"D")){
				isErr=true;
				ErrBoxAnimo erranimo;
				erranimo.box = now;
				erranimo.err_lab = L"D";
				box_err_animo.push_back(erranimo);
			}
		}
		if (IsContentStr(now_in,L"U")){
			if (!IsContentStr(box_static.at(now).tag,L"U")){
				isErr=true;
				ErrBoxAnimo erranimo;
				erranimo.box = now;
				erranimo.err_lab = L"U";
				box_err_animo.push_back(erranimo);
			}
		}
		if (IsContentStr(now_in,L"R")){
			if (!IsContentStr(box_static.at(now).tag,L"R")){
				isErr=true;
				ErrBoxAnimo erranimo;
				erranimo.box = now;
				erranimo.err_lab = L"R";
				box_err_animo.push_back(erranimo);
			}
		}
	}
}

//取BOX某方向可流动的BOX号 （-1表示无BOX；-2表示有BOX但是无相接的管道；-3表示有BOX但是相接的管道已填充）
int GetBoxInDirection(int box_no, char direction){
	int now;
	int next;
	switch(direction)
	{
	case 'W': 
		now = GetBoxNoInH(box_no);
		next = box_no-1;
		if (now==1){
			return -1;
		}else if (!IsContentStr(box_static.at(next).tag,L"R")){
			return -2;
		}else if (IsContentStr(box_static.at(next).full_lab,L"R")){
			return -3;
		}else{
			return next;
		}
		break;
	case 'E': 
		now = GetBoxNoInH(box_no);
		next = box_no+1;
		if (now==PIPE_MAIN_NUM_IMG_H){
			return -1;
		}else if (!IsContentStr(box_static.at(next).tag,L"L")){
			return -2;
		}else if (IsContentStr(box_static.at(next).full_lab,L"L")){
			return -3;
		}else{
			return next;
		}
		break;
	case 'N': 
		now = GetBoxNoInV(box_no);
		next = box_no-PIPE_MAIN_NUM_IMG_H;
		if (now==1){
			return -1;
		}else if (!IsContentStr(box_static.at(next).tag,L"D")){
			return -2;
		}else if (IsContentStr(box_static.at(next).full_lab,L"D")){
			return -3;
		}else{
			return next;
		}
		break;
	case 'S': 
		now = GetBoxNoInV(box_no);
		next = box_no+PIPE_MAIN_NUM_IMG_H;
		if (now==PIPE_MAIN_NUM_IMG_V){
			return -1;
		}else if (!IsContentStr(box_static.at(next).tag,L"U")){
			return -2;
		}else if (IsContentStr(box_static.at(next).full_lab,L"U")){
			return -3;
		}else{
			return next;
		}
		break;
	default:
		return -1;
	}
}

void SetRuleForNext(vector<int> *ret, int box_no ,char direction){
	CMzStringW next_in;
	CMzStringW err_lab;
	switch(direction)
	{
	case 'W': 
		next_in = L"R";
		err_lab = L"L";
		break;
	case 'N': 
		next_in = L"D";
		err_lab = L"U";
		break;
	case 'E': 
		next_in = L"L";
		err_lab = L"R";
		break;
	case 'S': 
		next_in = L"U";
		err_lab = L"D";
		break;
	}
	int next = GetBoxInDirection(box_no,direction);
	if (next >= 0){
		ret->push_back(next);
		box_static.at(next).in_lab = box_static.at(next).in_lab + next_in;
	}else if (next >= -2){
		isErr=true;
		ErrBoxAnimo erranimo;
		erranimo.box = box_no;
		erranimo.err_lab = err_lab;
		box_err_animo.push_back(erranimo);
	}
}

//取下一次流水BOX的所有编号
vector<int> GetNext(int box_no){
	CMzStringW box_tag = box_static.at(box_no).tag;
	wchar_t * box_in = box_static.at(box_no).in_lab.C_Str();
	vector<int> ret;
	if (box_tag==L"LR"){
		if (IsContentStr(box_in,L"L") && IsContentStr(box_in,L"R")){

		}else if (IsContentStr(box_in,L"L")){
			SetRuleForNext(&ret, box_no, 'E');
		}else if (IsContentStr(box_in,L"R")){
			SetRuleForNext(&ret, box_no, 'W');
		}
	}else if (box_tag==L"UD"){
		if (IsContentStr(box_in,L"U") && IsContentStr(box_in,L"D")){

		}else if (IsContentStr(box_in,L"U")){
			SetRuleForNext(&ret, box_no, 'S');
		}else if (IsContentStr(box_in,L"D")){
			SetRuleForNext(&ret, box_no, 'N');
		}
	}else if (box_tag==L"LUR"){
		if (IsContentStr(box_in,L"L") && IsContentStr(box_in,L"U") && IsContentStr(box_in,L"R")){

		}else if (IsContentStr(box_in,L"L") && IsContentStr(box_in,L"U")){
			SetRuleForNext(&ret, box_no, 'E');
		}else if (IsContentStr(box_in,L"L") && IsContentStr(box_in,L"R")){
			SetRuleForNext(&ret, box_no, 'N');
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"U")){
			SetRuleForNext(&ret, box_no, 'W');
		}else if (IsContentStr(box_in,L"L")){
			SetRuleForNext(&ret, box_no, 'N');
			SetRuleForNext(&ret, box_no, 'E');
		}else if (IsContentStr(box_in,L"U")){
			SetRuleForNext(&ret, box_no, 'W');
			SetRuleForNext(&ret, box_no, 'E');
		}else if (IsContentStr(box_in,L"R")){
			SetRuleForNext(&ret, box_no, 'W');
			SetRuleForNext(&ret, box_no, 'N');
		}
	}else if (box_tag==L"URD"){
		if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"U") && IsContentStr(box_in,L"R")){

		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"U")){
			SetRuleForNext(&ret, box_no, 'E');
		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"R")){
			SetRuleForNext(&ret, box_no, 'N');
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"U")){
			SetRuleForNext(&ret, box_no, 'S');
		}else if (IsContentStr(box_in,L"D")){
			SetRuleForNext(&ret, box_no, 'N');
			SetRuleForNext(&ret, box_no, 'E');
		}else if (IsContentStr(box_in,L"U")){
			SetRuleForNext(&ret, box_no, 'S');
			SetRuleForNext(&ret, box_no, 'E');
		}else if (IsContentStr(box_in,L"R")){
			SetRuleForNext(&ret, box_no, 'S');
			SetRuleForNext(&ret, box_no, 'N');
		}
	}else if (box_tag==L"LRD"){
		if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"L") && IsContentStr(box_in,L"R")){

		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"L")){
			SetRuleForNext(&ret, box_no, 'E');
		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"R")){
			SetRuleForNext(&ret, box_no, 'W');
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"L")){
			SetRuleForNext(&ret, box_no, 'S');
		}else if (IsContentStr(box_in,L"D")){
			SetRuleForNext(&ret, box_no, 'W');
			SetRuleForNext(&ret, box_no, 'E');
		}else if (IsContentStr(box_in,L"L")){
			SetRuleForNext(&ret, box_no, 'S');
			SetRuleForNext(&ret, box_no, 'E');
		}else if (IsContentStr(box_in,L"R")){
			SetRuleForNext(&ret, box_no, 'S');
			SetRuleForNext(&ret, box_no, 'W');
		}
	}else if (box_tag==L"LUD"){
		if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"L") && IsContentStr(box_in,L"U")){

		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"L")){
			SetRuleForNext(&ret, box_no, 'N');
		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"U")){
			SetRuleForNext(&ret, box_no, 'W');
		}else if (IsContentStr(box_in,L"U") && IsContentStr(box_in,L"L")){
			SetRuleForNext(&ret, box_no, 'S');
		}else if (IsContentStr(box_in,L"D")){
			SetRuleForNext(&ret, box_no, 'W');
			SetRuleForNext(&ret, box_no, 'N');
		}else if (IsContentStr(box_in,L"L")){
			SetRuleForNext(&ret, box_no, 'S');
			SetRuleForNext(&ret, box_no, 'N');
		}else if (IsContentStr(box_in,L"U")){
			SetRuleForNext(&ret, box_no, 'S');
			SetRuleForNext(&ret, box_no, 'W');
		}
	}else if (box_tag==L"LURD"){
		if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"D") && IsContentStr(box_in,L"L") && IsContentStr(box_in,L"U")){

		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"D") && IsContentStr(box_in,L"L")){
			SetRuleForNext(&ret, box_no, 'N');
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"D") && IsContentStr(box_in,L"U")){
			SetRuleForNext(&ret, box_no, 'W');
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"U") && IsContentStr(box_in,L"L")){
			SetRuleForNext(&ret, box_no, 'S');
		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"U") && IsContentStr(box_in,L"L")){
			SetRuleForNext(&ret, box_no, 'E');
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"L")){
			SetRuleForNext(&ret, box_no, 'S');
			SetRuleForNext(&ret, box_no, 'N');
		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"U")){
			SetRuleForNext(&ret, box_no, 'E');
			SetRuleForNext(&ret, box_no, 'W');
		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"L")){
			SetRuleForNext(&ret, box_no, 'E');
			SetRuleForNext(&ret, box_no, 'N');
		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"R")){
			SetRuleForNext(&ret, box_no, 'N');
			SetRuleForNext(&ret, box_no, 'W');
		}else if (IsContentStr(box_in,L"L") && IsContentStr(box_in,L"U")){
			SetRuleForNext(&ret, box_no, 'E');
			SetRuleForNext(&ret, box_no, 'S');
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"U")){
			SetRuleForNext(&ret, box_no, 'S');
			SetRuleForNext(&ret, box_no, 'W');
		}else if (IsContentStr(box_in,L"D")){
			SetRuleForNext(&ret, box_no, 'N');
			SetRuleForNext(&ret, box_no, 'E');
			SetRuleForNext(&ret, box_no, 'W');
		}else if (IsContentStr(box_in,L"U")){
			SetRuleForNext(&ret, box_no, 'S');
			SetRuleForNext(&ret, box_no, 'E');
			SetRuleForNext(&ret, box_no, 'W');
		}else if (IsContentStr(box_in,L"L")){
			SetRuleForNext(&ret, box_no, 'N');
			SetRuleForNext(&ret, box_no, 'E');
			SetRuleForNext(&ret, box_no, 'S');
		}else if (IsContentStr(box_in,L"R")){
			SetRuleForNext(&ret, box_no, 'N');
			SetRuleForNext(&ret, box_no, 'S');
			SetRuleForNext(&ret, box_no, 'W');
		}
	}else if (box_tag==L"LURDX"){
		if (IsContentStr(box_in,L"L") && IsContentStr(box_in,L"R")){

		}else if (IsContentStr(box_in,L"L")){
			SetRuleForNext(&ret, box_no, 'E');
		}else if (IsContentStr(box_in,L"R")){
			SetRuleForNext(&ret, box_no, 'W');
		}
		if (IsContentStr(box_in,L"U") && IsContentStr(box_in,L"D")){

		}else if (IsContentStr(box_in,L"U")){
			SetRuleForNext(&ret, box_no, 'S');
		}else if (IsContentStr(box_in,L"D")){
			SetRuleForNext(&ret, box_no, 'N');
		}
		box_static.at(box_no).in_lab = L"";
	}else if (box_tag==L"LU"){
		if (IsContentStr(box_in,L"L") && IsContentStr(box_in,L"U")){

		}else if (IsContentStr(box_in,L"L")){
			SetRuleForNext(&ret, box_no, 'N');
		}else if (IsContentStr(box_in,L"U")){
			SetRuleForNext(&ret, box_no, 'W');
		}
	}else if (box_tag==L"RU"){
		if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"U")){

		}else if (IsContentStr(box_in,L"R")){
			SetRuleForNext(&ret, box_no, 'N');
		}else if (IsContentStr(box_in,L"U")){
			SetRuleForNext(&ret, box_no, 'E');
		}
	}else if (box_tag==L"RD"){
		if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"D")){

		}else if (IsContentStr(box_in,L"R")){
			SetRuleForNext(&ret, box_no, 'S');
		}else if (IsContentStr(box_in,L"D")){
			SetRuleForNext(&ret, box_no, 'E');
		}
	}else if (box_tag==L"LD"){
		if (IsContentStr(box_in,L"L") && IsContentStr(box_in,L"D")){

		}else if (IsContentStr(box_in,L"L")){
			SetRuleForNext(&ret, box_no, 'S');
		}else if (IsContentStr(box_in,L"D")){
			SetRuleForNext(&ret, box_no, 'W');
		}
	}else if (box_tag==L"LURD\\"){
		if (IsContentStr(box_in,L"L") && IsContentStr(box_in,L"D")){

		}else if (IsContentStr(box_in,L"L")){
			SetRuleForNext(&ret, box_no, 'S');
		}else if (IsContentStr(box_in,L"D")){
			SetRuleForNext(&ret, box_no, 'W');
		}
		if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"U")){

		}else if (IsContentStr(box_in,L"R")){
			SetRuleForNext(&ret, box_no, 'N');
		}else if (IsContentStr(box_in,L"U")){
			SetRuleForNext(&ret, box_no, 'E');
		}
		box_static.at(box_no).in_lab = L"";
	}else if (box_tag==L"LURD/"){
		if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"D")){

		}else if (IsContentStr(box_in,L"R")){
			SetRuleForNext(&ret, box_no, 'S');
		}else if (IsContentStr(box_in,L"D")){
			SetRuleForNext(&ret, box_no, 'E');
		}
		if (IsContentStr(box_in,L"L") && IsContentStr(box_in,L"U")){

		}else if (IsContentStr(box_in,L"L")){
			SetRuleForNext(&ret, box_no, 'N');
		}else if (IsContentStr(box_in,L"U")){
			SetRuleForNext(&ret, box_no, 'W');
		}
		box_static.at(box_no).in_lab = L"";
	}
	return ret;
}


void DrawLine(HDC hdc,int xBeg,int yBeg,int xEnd,int yEnd,COLORREF clr){
	MoveToEx (hdc, xBeg, yBeg, NULL) ;	        
	LineTo (hdc, xEnd, yEnd) ;
}


//直线动画
void SubLineAnimo(int box_x, int box_y, char direction, int frame){
	switch(direction)
	{
	case 'W': 
		AlphaBlend(hdc,box_x+PIPE_MAIN_SIZE_IMG-frame_length*frame,box_y,frame_length,PIPE_MAIN_SIZE_IMG,img_AllWater->GetDC(),PIPE_MAIN_SIZE_IMG-frame_length*frame,PIPE_MAIN_SIZE_IMG*4,frame_length,PIPE_MAIN_SIZE_IMG,bf);
		break;
	case 'N': 
		AlphaBlend(hdc,box_x,box_y+PIPE_MAIN_SIZE_IMG-frame_length*frame,PIPE_MAIN_SIZE_IMG,frame_length,img_AllWater->GetDC(),PIPE_MAIN_SIZE_IMG,PIPE_MAIN_SIZE_IMG*5-frame_length*frame,PIPE_MAIN_SIZE_IMG,frame_length,bf);
		break;
	case 'E': 
		AlphaBlend(hdc,box_x+frame_length*(frame-1),box_y,frame_length,PIPE_MAIN_SIZE_IMG,img_AllWater->GetDC(),frame_length*(frame-1),PIPE_MAIN_SIZE_IMG*4,frame_length,PIPE_MAIN_SIZE_IMG,bf);
		break;
	case 'S': 
		AlphaBlend(hdc,box_x,box_y+frame_length*(frame-1),PIPE_MAIN_SIZE_IMG,frame_length,img_AllWater->GetDC(),PIPE_MAIN_SIZE_IMG,PIPE_MAIN_SIZE_IMG*4+frame_length*(frame-1),PIPE_MAIN_SIZE_IMG,frame_length,bf);
		break;
	}
}

//曲线动画
void SubArcAnimo(int box_x, int box_y, wchar_t *  from ,wchar_t *  to, int frame){
	CMzStringW f = CMzStringW(from);
	CMzStringW t = CMzStringW(to);
	if (f==L"L")
	{
		if (t==L"D")
		{
			AlphaBlend(hdc,box_x,box_y+PIPE_MAIN_SIZE_ACR_DESC,PIPE_MAIN_SIZE_ACR_IMG,PIPE_MAIN_SIZE_ACR_IMG,img_AllWater->GetDC(),PIPE_MAIN_SIZE_IMG*(frame-1),PIPE_MAIN_SIZE_ACR_DESC,PIPE_MAIN_SIZE_ACR_IMG,PIPE_MAIN_SIZE_ACR_IMG,bf);
		}else if (t==L"U")
		{
			AlphaBlend(hdc,box_x,box_y,PIPE_MAIN_SIZE_ACR_IMG,PIPE_MAIN_SIZE_ACR_IMG,img_AllWater->GetDC(),PIPE_MAIN_SIZE_IMG*(PIPE_ANIMO_FRAME-frame),2*PIPE_MAIN_SIZE_IMG,PIPE_MAIN_SIZE_ACR_IMG,PIPE_MAIN_SIZE_ACR_IMG,bf);
		}
	}else if (f==L"U")
	{
		if (t==L"L")
		{
			AlphaBlend(hdc,box_x,box_y,PIPE_MAIN_SIZE_ACR_IMG,PIPE_MAIN_SIZE_ACR_IMG,img_AllWater->GetDC(),PIPE_MAIN_SIZE_IMG*(frame-1),2*PIPE_MAIN_SIZE_IMG,PIPE_MAIN_SIZE_ACR_IMG,PIPE_MAIN_SIZE_ACR_IMG,bf);
		}else if (t==L"R")
		{
			AlphaBlend(hdc,box_x+PIPE_MAIN_SIZE_ACR_DESC,box_y,PIPE_MAIN_SIZE_ACR_IMG,PIPE_MAIN_SIZE_ACR_IMG,img_AllWater->GetDC(),PIPE_MAIN_SIZE_IMG*(PIPE_ANIMO_FRAME-frame)+PIPE_MAIN_SIZE_ACR_DESC,PIPE_MAIN_SIZE_IMG,PIPE_MAIN_SIZE_ACR_IMG,PIPE_MAIN_SIZE_ACR_IMG,bf);
		}
	}else if (f==L"R")
	{
		if (t==L"U")
		{
			AlphaBlend(hdc,box_x+PIPE_MAIN_SIZE_ACR_DESC,box_y,PIPE_MAIN_SIZE_ACR_IMG,PIPE_MAIN_SIZE_ACR_IMG,img_AllWater->GetDC(),PIPE_MAIN_SIZE_IMG*(frame-1)+PIPE_MAIN_SIZE_ACR_DESC,PIPE_MAIN_SIZE_IMG,PIPE_MAIN_SIZE_ACR_IMG,PIPE_MAIN_SIZE_ACR_IMG,bf);
		}else if (t==L"D")
		{
			AlphaBlend(hdc,box_x+PIPE_MAIN_SIZE_ACR_DESC,box_y+PIPE_MAIN_SIZE_ACR_DESC,PIPE_MAIN_SIZE_ACR_IMG,PIPE_MAIN_SIZE_ACR_IMG,img_AllWater->GetDC(),PIPE_MAIN_SIZE_IMG*(PIPE_ANIMO_FRAME-frame)+PIPE_MAIN_SIZE_ACR_DESC,3*PIPE_MAIN_SIZE_IMG+PIPE_MAIN_SIZE_ACR_DESC,PIPE_MAIN_SIZE_ACR_IMG,PIPE_MAIN_SIZE_ACR_IMG,bf);
		}
	}else if (f==L"D")
	{
		if (t==L"R")
		{
			AlphaBlend(hdc,box_x+PIPE_MAIN_SIZE_ACR_DESC,box_y+PIPE_MAIN_SIZE_ACR_DESC,PIPE_MAIN_SIZE_ACR_IMG,PIPE_MAIN_SIZE_ACR_IMG,img_AllWater->GetDC(),PIPE_MAIN_SIZE_IMG*(frame-1)+PIPE_MAIN_SIZE_ACR_DESC,3*PIPE_MAIN_SIZE_IMG+PIPE_MAIN_SIZE_ACR_DESC,PIPE_MAIN_SIZE_ACR_IMG,PIPE_MAIN_SIZE_ACR_IMG,bf);
		}else if (t==L"L")
		{
			AlphaBlend(hdc,box_x,box_y+PIPE_MAIN_SIZE_ACR_DESC,PIPE_MAIN_SIZE_ACR_IMG,PIPE_MAIN_SIZE_ACR_IMG,img_AllWater->GetDC(),PIPE_MAIN_SIZE_IMG*(PIPE_ANIMO_FRAME-frame),PIPE_MAIN_SIZE_ACR_DESC,PIPE_MAIN_SIZE_ACR_IMG,PIPE_MAIN_SIZE_ACR_IMG,bf);
		}
	}
}


//取相反方向
char GetOpDire(char direction){
	switch(direction)
	{
	case 'W': 
		return 'E';
		break;
	case 'N': 
		return 'S';
		break;
	case 'E': 
		return 'W';
		break;
	case 'S': 
		return 'N';
		break;
	default:
		return ' ';
	}
}

//取水方向
char GetWaterDire(wchar_t * in){
	if (CMzStringW(in)==L"R")
	{
		return 'W';
	}else if (CMzStringW(in)==L"L")
	{
		return 'E';
	}else if (CMzStringW(in)==L"U")
	{
		return 'S';
	}else if (CMzStringW(in)==L"D")
	{
		return 'N';
	}else{
		return ' ';
	}
}

//L型管道公用
void SubLAnimo(int box_x, int box_y, wchar_t * box_in, wchar_t * Dire[2]){
	int frame = 1;
	if (IsContentStr(box_in,Dire[0]) && IsContentStr(box_in,Dire[1])){
		for (; frame<=PIPE_ANIMO_HAFE_FRAME;frame++){
			SubArcAnimo(box_x,box_y,Dire[0],Dire[1],frame);
			if (frame<PIPE_ANIMO_HAFE_FRAME) {
				SubArcAnimo(box_x,box_y,Dire[1],Dire[0],frame);
			}
			Sleep(PIPE_ANIMO_FRAME_BETWEEN);
		}
	}else if (IsContentStr(box_in,Dire[0])){
		for (; frame<=PIPE_ANIMO_FRAME;frame++){
			SubArcAnimo(box_x,box_y,Dire[0],Dire[1],frame);
			Sleep(PIPE_ANIMO_FRAME_BETWEEN);
		}
	}else if (IsContentStr(box_in,Dire[1])){
		for (; frame<=PIPE_ANIMO_FRAME;frame++){
			SubArcAnimo(box_x,box_y,Dire[1],Dire[0],frame);				
			Sleep(PIPE_ANIMO_FRAME_BETWEEN);
		}
	}
}

//T型管道公用
void SubTAnimo(int box_x, int box_y, wchar_t * box_in, wchar_t * Dire[3]){
	int frame = 1;
	if (IsContentStr(box_in,Dire[0]) && IsContentStr(box_in,Dire[1]) && IsContentStr(box_in,Dire[2])){
		for (; frame<=PIPE_ANIMO_HAFE_FRAME;frame++){
			SubLineAnimo(box_x,box_y,GetWaterDire(Dire[0]),frame);
			if (frame<PIPE_ANIMO_HAFE_FRAME) {
				SubLineAnimo(box_x,box_y,GetWaterDire(Dire[1]),frame);
				SubLineAnimo(box_x,box_y,GetWaterDire(Dire[2]),frame);
			}
			Sleep(PIPE_ANIMO_FRAME_BETWEEN);
		}
	}else if (IsContentStr(box_in,Dire[0]) && IsContentStr(box_in,Dire[1])){
		for (; frame<=PIPE_ANIMO_FRAME;frame++){
			if (frame<PIPE_ANIMO_HAFE_FRAME){
				SubLineAnimo(box_x,box_y,GetWaterDire(Dire[1]),frame);
			}
			SubLineAnimo(box_x,box_y,GetWaterDire(Dire[0]),frame);
			Sleep(PIPE_ANIMO_FRAME_BETWEEN);
		}				
	}else if (IsContentStr(box_in,Dire[0]) && IsContentStr(box_in,Dire[2])){
		for (; frame<=PIPE_ANIMO_FRAME;frame++){
			if (frame<=PIPE_ANIMO_HAFE_FRAME){
				SubLineAnimo(box_x,box_y,GetWaterDire(Dire[0]),frame);
				if (frame<PIPE_ANIMO_HAFE_FRAME) {
					SubLineAnimo(box_x,box_y,GetWaterDire(Dire[2]),frame);
				}
			}else{
				SubLineAnimo(box_x,box_y,GetOpDire(GetWaterDire(Dire[1])),frame);
			}					
			Sleep(PIPE_ANIMO_FRAME_BETWEEN);
		}
	}else if (IsContentStr(box_in,Dire[1]) && IsContentStr(box_in,Dire[2])){
		for (; frame<=PIPE_ANIMO_FRAME;frame++){
			if (frame<PIPE_ANIMO_HAFE_FRAME){
				SubLineAnimo(box_x,box_y,GetWaterDire(Dire[1]),frame);
			}
			SubLineAnimo(box_x,box_y,GetWaterDire(Dire[2]),frame);
			Sleep(PIPE_ANIMO_FRAME_BETWEEN);
		}
	}else if (IsContentStr(box_in,Dire[0])){
		for (; frame<=PIPE_ANIMO_FRAME;frame++){
			if (frame>PIPE_ANIMO_HAFE_FRAME){
				SubLineAnimo(box_x,box_y,GetOpDire(GetWaterDire(Dire[1])),frame);
			}
			SubLineAnimo(box_x,box_y,GetWaterDire(Dire[0]),frame);
			Sleep(PIPE_ANIMO_FRAME_BETWEEN);
		}
	}else if (IsContentStr(box_in,Dire[1])){
		for (; frame<=PIPE_ANIMO_FRAME;frame++){
			if (frame<PIPE_ANIMO_HAFE_FRAME){
				SubLineAnimo(box_x,box_y,GetWaterDire(Dire[1]),frame);
			}else{
				SubLineAnimo(box_x,box_y,GetWaterDire(Dire[0]),frame);
				if (frame>PIPE_ANIMO_HAFE_FRAME) {
					SubLineAnimo(box_x,box_y,GetWaterDire(Dire[2]),frame);
				}
			}					
			Sleep(PIPE_ANIMO_FRAME_BETWEEN);
		}
	}else if (IsContentStr(box_in,Dire[2])){
		for (; frame<=PIPE_ANIMO_FRAME;frame++){
			if (frame>PIPE_ANIMO_HAFE_FRAME){
				SubLineAnimo(box_x,box_y,GetOpDire(GetWaterDire(Dire[1])),frame);
			}
			SubLineAnimo(box_x,box_y,GetWaterDire(Dire[2]),frame);
			Sleep(PIPE_ANIMO_FRAME_BETWEEN);
		}
	}
}


//填充动画
DWORD WINAPI FullAnimoThread(LPVOID lpParam)  
{   
	PThreadData data = (PThreadData) lpParam;
	wprintf(L"FullAnimo (%d-%d), In-%s  " ,GetBoxNoInV(data->box_no), GetBoxNoInH(data->box_no), data->box_static->in_lab.C_Str());
	//debug
	if (isdebug){
		wsprintf(debug_str.C_Str(), debug_str+L"F(%d-%d)-%s  " ,GetBoxNoInV(data->box_no), GetBoxNoInH(data->box_no), data->box_static->in_lab.C_Str());
		DrawText(hdc,debug_str.C_Str(),lstrlen (debug_str.C_Str()),&rcWork,0);
	}
	UiButton_Image* but = (UiButton_Image *) data->box;
	int box_x = but->GetPosX();
	int box_y = but->GetPosY();
	CMzStringW box_tag = data->box_static->tag;
	wchar_t * box_in = data->box_static->in_lab.C_Str();
	int frame = 1;		
	if (box_tag==L"LR"){
		if (IsContentStr(box_in,L"L") && IsContentStr(box_in,L"R")){
			for (; frame<=PIPE_ANIMO_HAFE_FRAME;frame++){
				SubLineAnimo(box_x,box_y,'W',frame);
				if (frame<PIPE_ANIMO_HAFE_FRAME) {
					SubLineAnimo(box_x,box_y,'E',frame);
				}
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
		}else if (IsContentStr(box_in,L"L")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				SubLineAnimo(box_x,box_y,'E',frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
		}else if (IsContentStr(box_in,L"R")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				SubLineAnimo(box_x,box_y,'W',frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
		}
		data->box_static->full_lab = box_tag;
	}else if (box_tag==L"UD"){
		if (IsContentStr(box_in,L"U") && IsContentStr(box_in,L"D")){
			for (; frame<=PIPE_ANIMO_HAFE_FRAME;frame++){
				SubLineAnimo(box_x,box_y,'N',frame);
				if (frame<PIPE_ANIMO_HAFE_FRAME) {
					SubLineAnimo(box_x,box_y,'S',frame);
				}
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
		}else if (IsContentStr(box_in,L"U")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				SubLineAnimo(box_x,box_y,'S',frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
		}else if (IsContentStr(box_in,L"D")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				SubLineAnimo(box_x,box_y,'N',frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
		}
		data->box_static->full_lab = box_tag;
	}else if (box_tag==L"LUR"){
		wchar_t * Dire[3] = {L"L",L"U",L"R"};
		SubTAnimo(box_x, box_y, box_in, Dire);		
		data->box_static->full_lab = box_tag;
	}else if (box_tag==L"URD"){
		wchar_t * Dire[3] = {L"U",L"R",L"D"};
		SubTAnimo(box_x, box_y, box_in, Dire);
		data->box_static->full_lab = box_tag;
	}else if (box_tag==L"LRD"){
		wchar_t * Dire[3] = {L"L",L"D",L"R"};
		SubTAnimo(box_x, box_y, box_in, Dire);
		data->box_static->full_lab = box_tag;
	}else if (box_tag==L"LUD"){
		wchar_t * Dire[3] = {L"U",L"L",L"D"};
		SubTAnimo(box_x, box_y, box_in, Dire);
		data->box_static->full_lab = box_tag;
	}else if (box_tag==L"LURD"){
		if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"D") && IsContentStr(box_in,L"L") && IsContentStr(box_in,L"U")){
			for (; frame<=PIPE_ANIMO_HAFE_FRAME;frame++){
				SubLineAnimo(box_x,box_y,'N',frame);				
				SubLineAnimo(box_x,box_y,'W',frame);
				if (frame<PIPE_ANIMO_HAFE_FRAME) {
					SubLineAnimo(box_x,box_y,'S',frame);
					SubLineAnimo(box_x,box_y,'E',frame);
				}
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"D") && IsContentStr(box_in,L"L")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				if (frame<=PIPE_ANIMO_HAFE_FRAME){
					SubLineAnimo(box_x,box_y,'W',frame);	
					if (frame<PIPE_ANIMO_HAFE_FRAME) {
						SubLineAnimo(box_x,box_y,'E',frame);
					}
				}
				SubLineAnimo(box_x,box_y,'N',frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"D") && IsContentStr(box_in,L"U")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				if (frame<=PIPE_ANIMO_HAFE_FRAME){
					SubLineAnimo(box_x,box_y,'N',frame);
					if (frame<PIPE_ANIMO_HAFE_FRAME) {
						SubLineAnimo(box_x,box_y,'S',frame);
					}
				}
				SubLineAnimo(box_x,box_y,'W',frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"U") && IsContentStr(box_in,L"L")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				if (frame<=PIPE_ANIMO_HAFE_FRAME){
					SubLineAnimo(box_x,box_y,'W',frame);	
					if (frame<PIPE_ANIMO_HAFE_FRAME) {
						SubLineAnimo(box_x,box_y,'E',frame);
					}
				}
				SubLineAnimo(box_x,box_y,'S',frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"U") && IsContentStr(box_in,L"L")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				if (frame<=PIPE_ANIMO_HAFE_FRAME){
					SubLineAnimo(box_x,box_y,'N',frame);	
					if (frame<PIPE_ANIMO_HAFE_FRAME) {
						SubLineAnimo(box_x,box_y,'S',frame);
					}
				}
				SubLineAnimo(box_x,box_y,'E',frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"L")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				if (frame<=PIPE_ANIMO_HAFE_FRAME){
					SubLineAnimo(box_x,box_y,'E',frame);
					if (frame<PIPE_ANIMO_HAFE_FRAME) {
						SubLineAnimo(box_x,box_y,'W',frame);					
					}else{
						SubLineAnimo(box_x,box_y,'N',frame);
					}
				}else{
					SubLineAnimo(box_x,box_y,'N',frame);
					SubLineAnimo(box_x,box_y,'S',frame);
				}
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"U")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				if (frame<=PIPE_ANIMO_HAFE_FRAME){
					SubLineAnimo(box_x,box_y,'N',frame);
					if (frame<PIPE_ANIMO_HAFE_FRAME) {
						SubLineAnimo(box_x,box_y,'S',frame);	
					}else{
						SubLineAnimo(box_x,box_y,'E',frame);
					}
				}else{
					SubLineAnimo(box_x,box_y,'E',frame);				
					SubLineAnimo(box_x,box_y,'W',frame);	
				}
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"L")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){				
				SubLineAnimo(box_x,box_y,'E',frame);				
				SubLineAnimo(box_x,box_y,'N',frame);				
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"R")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){				
				SubLineAnimo(box_x,box_y,'W',frame);				
				SubLineAnimo(box_x,box_y,'N',frame);				
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
		}else if (IsContentStr(box_in,L"L") && IsContentStr(box_in,L"U")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){				
				SubLineAnimo(box_x,box_y,'E',frame);				
				SubLineAnimo(box_x,box_y,'S',frame);				
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"U")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){				
				SubLineAnimo(box_x,box_y,'W',frame);				
				SubLineAnimo(box_x,box_y,'S',frame);				
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
		}else if (IsContentStr(box_in,L"D")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				if (frame>=PIPE_ANIMO_HAFE_FRAME){
					SubLineAnimo(box_x,box_y,'W',frame);
					if (frame>PIPE_ANIMO_HAFE_FRAME) {
						SubLineAnimo(box_x,box_y,'E',frame);
					}
				}
				SubLineAnimo(box_x,box_y,'N',frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
		}else if (IsContentStr(box_in,L"U")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				if (frame>=PIPE_ANIMO_HAFE_FRAME){
					SubLineAnimo(box_x,box_y,'W',frame);
					if (frame>PIPE_ANIMO_HAFE_FRAME) {
						SubLineAnimo(box_x,box_y,'E',frame);
					}
				}
				SubLineAnimo(box_x,box_y,'S',frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
		}else if (IsContentStr(box_in,L"L")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				if (frame>=PIPE_ANIMO_HAFE_FRAME){
					SubLineAnimo(box_x,box_y,'N',frame);	
					if (frame>PIPE_ANIMO_HAFE_FRAME) {
						SubLineAnimo(box_x,box_y,'S',frame);
					}
				}
				SubLineAnimo(box_x,box_y,'E',frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
		}else if (IsContentStr(box_in,L"R")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				if (frame>=PIPE_ANIMO_HAFE_FRAME){
					SubLineAnimo(box_x,box_y,'N',frame);	
					if (frame>PIPE_ANIMO_HAFE_FRAME) {
						SubLineAnimo(box_x,box_y,'S',frame);
					}
				}
				SubLineAnimo(box_x,box_y,'W',frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
		}
		data->box_static->full_lab = box_tag;
	}else if (box_tag==L"LURDX"){		
		if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"D") && IsContentStr(box_in,L"L") && IsContentStr(box_in,L"U")){
			for (; frame<=PIPE_ANIMO_HAFE_FRAME;frame++){
				SubLineAnimo(box_x,box_y,'W',frame);
				if (frame<PIPE_ANIMO_HAFE_FRAME) {
					if (frame<PIPE_ANIMO_HAFE_FRAME-2){
						SubLineAnimo(box_x,box_y,'S',frame);
						SubLineAnimo(box_x,box_y,'N',frame);
					}
					SubLineAnimo(box_x,box_y,'E',frame);
				}
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"D") && IsContentStr(box_in,L"L")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				if (frame<=PIPE_ANIMO_HAFE_FRAME){
					SubLineAnimo(box_x,box_y,'W',frame);	
					if (frame<PIPE_ANIMO_HAFE_FRAME) {
						SubLineAnimo(box_x,box_y,'E',frame);
					}
				}
				if (frame<PIPE_ANIMO_HAFE_FRAME-2||frame>PIPE_ANIMO_HAFE_FRAME+2){
					SubLineAnimo(box_x,box_y,'N',frame);
				}
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"D") && IsContentStr(box_in,L"U")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				if (frame<PIPE_ANIMO_HAFE_FRAME-2){
					SubLineAnimo(box_x,box_y,'N',frame);
					//if (frame<PIPE_ANIMO_HAFE_FRAME) {
					SubLineAnimo(box_x,box_y,'S',frame);
					//}
				}
				SubLineAnimo(box_x,box_y,'W',frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"U") && IsContentStr(box_in,L"L")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				if (frame<=PIPE_ANIMO_HAFE_FRAME){
					SubLineAnimo(box_x,box_y,'W',frame);	
					if (frame<PIPE_ANIMO_HAFE_FRAME) {
						SubLineAnimo(box_x,box_y,'E',frame);
					}
				}
				if (frame<PIPE_ANIMO_HAFE_FRAME-2||frame>PIPE_ANIMO_HAFE_FRAME+2){
					SubLineAnimo(box_x,box_y,'S',frame);
				}
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"U") && IsContentStr(box_in,L"L")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				if (frame<PIPE_ANIMO_HAFE_FRAME-2){
					SubLineAnimo(box_x,box_y,'N',frame);	
					//if (frame<PIPE_ANIMO_HAFE_FRAME) {
					SubLineAnimo(box_x,box_y,'S',frame);
					//}
				}
				SubLineAnimo(box_x,box_y,'E',frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"L")){
			for (; frame<=PIPE_ANIMO_HAFE_FRAME;frame++){
				SubLineAnimo(box_x,box_y,'E',frame);
				if (frame<PIPE_ANIMO_HAFE_FRAME) {
					SubLineAnimo(box_x,box_y,'W',frame);
				}
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = data->box_static->full_lab + L"RL";
		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"U")){
			for (; frame<PIPE_ANIMO_HAFE_FRAME-2;frame++){
				SubLineAnimo(box_x,box_y,'N',frame);
				//if (frame<PIPE_ANIMO_HAFE_FRAME) {
				SubLineAnimo(box_x,box_y,'S',frame);				
				//}
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = data->box_static->full_lab + L"DU";
		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"L")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){				
				SubLineAnimo(box_x,box_y,'E',frame);
				if (frame<PIPE_ANIMO_HAFE_FRAME-2||frame>PIPE_ANIMO_HAFE_FRAME+2){
					SubLineAnimo(box_x,box_y,'N',frame);	
				}
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"R")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){				
				SubLineAnimo(box_x,box_y,'W',frame);
				if (frame<PIPE_ANIMO_HAFE_FRAME-2||frame>PIPE_ANIMO_HAFE_FRAME+2){
					SubLineAnimo(box_x,box_y,'N',frame);	
				}
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"L") && IsContentStr(box_in,L"U")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){				
				SubLineAnimo(box_x,box_y,'E',frame);	
				if (frame<PIPE_ANIMO_HAFE_FRAME-2||frame>PIPE_ANIMO_HAFE_FRAME+2){
					SubLineAnimo(box_x,box_y,'S',frame);				
				}
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"U")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){				
				SubLineAnimo(box_x,box_y,'W',frame);
				if (frame<PIPE_ANIMO_HAFE_FRAME-2||frame>PIPE_ANIMO_HAFE_FRAME+2){
					SubLineAnimo(box_x,box_y,'S',frame);				
				}
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"D")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				if (frame<PIPE_ANIMO_HAFE_FRAME-2||frame>PIPE_ANIMO_HAFE_FRAME+2){
					SubLineAnimo(box_x,box_y,'N',frame);
				}
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = data->box_static->full_lab + L"DU";
		}else if (IsContentStr(box_in,L"U")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				if (frame<PIPE_ANIMO_HAFE_FRAME-2||frame>PIPE_ANIMO_HAFE_FRAME+2){
					SubLineAnimo(box_x,box_y,'S',frame);
				}
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = data->box_static->full_lab + L"DU";
		}else if (IsContentStr(box_in,L"L")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				SubLineAnimo(box_x,box_y,'E',frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = data->box_static->full_lab + L"RL";
		}else if (IsContentStr(box_in,L"R")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				SubLineAnimo(box_x,box_y,'W',frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = data->box_static->full_lab + L"RL";
		}		
	}else if (box_tag==L"LU"){
		wchar_t * Dire[2] = {L"U",L"L"};
		SubLAnimo(box_x, box_y, box_in, Dire);
		data->box_static->full_lab = box_tag;
	}else if (box_tag==L"RU"){
		wchar_t * Dire[2] = {L"U",L"R"};
		SubLAnimo(box_x, box_y, box_in, Dire);
		data->box_static->full_lab = box_tag;
	}else if (box_tag==L"RD"){
		wchar_t * Dire[2] = {L"R",L"D"};
		SubLAnimo(box_x, box_y, box_in, Dire);
		data->box_static->full_lab = box_tag;
	}else if (box_tag==L"LD"){
		wchar_t * Dire[2] = {L"D",L"L"};
		SubLAnimo(box_x, box_y, box_in, Dire);
		data->box_static->full_lab = box_tag;
	}else if (box_tag==L"LURD\\"){
		if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"D") && IsContentStr(box_in,L"L") && IsContentStr(box_in,L"U")){
			for (; frame<=PIPE_ANIMO_HAFE_FRAME;frame++){
				SubArcAnimo(box_x,box_y,L"L",L"D",frame);
				if (frame<PIPE_ANIMO_HAFE_FRAME) {
					SubArcAnimo(box_x,box_y,L"D",L"L",frame);
				}				
				SubArcAnimo(box_x,box_y,L"U",L"R",frame);
				if (frame<PIPE_ANIMO_HAFE_FRAME) {
					SubArcAnimo(box_x,box_y,L"R",L"U",frame);
				}
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"D") && IsContentStr(box_in,L"L")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				if (frame<=PIPE_ANIMO_HAFE_FRAME){
					SubArcAnimo(box_x,box_y,L"L",L"D",frame);
					if (frame<PIPE_ANIMO_HAFE_FRAME) {
						SubArcAnimo(box_x,box_y,L"D",L"L",frame);
					}
				}
				SubArcAnimo(box_x,box_y,L"R",L"U",frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"D") && IsContentStr(box_in,L"U")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				if (frame<=PIPE_ANIMO_HAFE_FRAME){
					SubArcAnimo(box_x,box_y,L"U",L"R",frame);
					if (frame<PIPE_ANIMO_HAFE_FRAME) {
						SubArcAnimo(box_x,box_y,L"R",L"U",frame);
					}
				}
				SubArcAnimo(box_x,box_y,L"D",L"L",frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"U") && IsContentStr(box_in,L"L")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				if (frame<=PIPE_ANIMO_HAFE_FRAME){
					SubArcAnimo(box_x,box_y,L"U",L"R",frame);
					if (frame<PIPE_ANIMO_HAFE_FRAME) {
						SubArcAnimo(box_x,box_y,L"R",L"U",frame);
					}
				}
				SubArcAnimo(box_x,box_y,L"L",L"D",frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"U") && IsContentStr(box_in,L"L")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				if (frame<=PIPE_ANIMO_HAFE_FRAME){
					SubArcAnimo(box_x,box_y,L"L",L"D",frame);
					if (frame<PIPE_ANIMO_HAFE_FRAME) {
						SubArcAnimo(box_x,box_y,L"D",L"L",frame);
					}
				}
				SubArcAnimo(box_x,box_y,L"U",L"R",frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"L")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				SubArcAnimo(box_x,box_y,L"L",L"D",frame);		
				SubArcAnimo(box_x,box_y,L"R",L"U",frame);			
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"U")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				SubArcAnimo(box_x,box_y,L"D",L"L",frame);
				SubArcAnimo(box_x,box_y,L"U",L"R",frame);	
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"L")){
			for (; frame<=PIPE_ANIMO_HAFE_FRAME;frame++){				
				SubArcAnimo(box_x,box_y,L"D",L"L",frame);	
				if (frame<PIPE_ANIMO_HAFE_FRAME) {
					SubArcAnimo(box_x,box_y,L"L",L"D",frame);				
				}
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = data->box_static->full_lab + L"DL";
		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"R")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){				
				SubArcAnimo(box_x,box_y,L"D",L"L",frame);			
				SubArcAnimo(box_x,box_y,L"R",L"U",frame);			
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"L") && IsContentStr(box_in,L"U")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){				
				SubArcAnimo(box_x,box_y,L"L",L"D",frame);		
				SubArcAnimo(box_x,box_y,L"U",L"R",frame);	
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"U")){
			for (; frame<=PIPE_ANIMO_HAFE_FRAME;frame++){				
				SubArcAnimo(box_x,box_y,L"U",L"R",frame);
				if (frame<PIPE_ANIMO_HAFE_FRAME) {
					SubArcAnimo(box_x,box_y,L"R",L"U",frame);
				}
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = data->box_static->full_lab + L"RU";
		}else if (IsContentStr(box_in,L"D")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				SubArcAnimo(box_x,box_y,L"D",L"L",frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = data->box_static->full_lab + L"DL";
		}else if (IsContentStr(box_in,L"U")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				SubArcAnimo(box_x,box_y,L"U",L"R",frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = data->box_static->full_lab + L"RU";
		}else if (IsContentStr(box_in,L"L")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				SubArcAnimo(box_x,box_y,L"L",L"D",frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = data->box_static->full_lab + L"DL";
		}else if (IsContentStr(box_in,L"R")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				SubArcAnimo(box_x,box_y,L"R",L"U",frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = data->box_static->full_lab + L"RU";
		}	

	}else if (box_tag==L"LURD/"){
		if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"D") && IsContentStr(box_in,L"L") && IsContentStr(box_in,L"U")){
			for (; frame<=PIPE_ANIMO_HAFE_FRAME;frame++){
				SubArcAnimo(box_x,box_y,L"L",L"D",frame);
				if (frame<PIPE_ANIMO_HAFE_FRAME) {
					SubArcAnimo(box_x,box_y,L"D",L"L",frame);
				}
				SubArcAnimo(box_x,box_y,L"U",L"R",frame);
				if (frame<PIPE_ANIMO_HAFE_FRAME) {
					SubArcAnimo(box_x,box_y,L"R",L"U",frame);
				}
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"D") && IsContentStr(box_in,L"L")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				if (frame<=PIPE_ANIMO_HAFE_FRAME){
					SubArcAnimo(box_x,box_y,L"R",L"D",frame);
					if (frame<PIPE_ANIMO_HAFE_FRAME) {
						SubArcAnimo(box_x,box_y,L"D",L"R",frame);
					}
				}
				SubArcAnimo(box_x,box_y,L"L",L"U",frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"D") && IsContentStr(box_in,L"U")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				if (frame<=PIPE_ANIMO_HAFE_FRAME){
					SubArcAnimo(box_x,box_y,L"R",L"D",frame);
					if (frame<PIPE_ANIMO_HAFE_FRAME) {
						SubArcAnimo(box_x,box_y,L"D",L"R",frame);
					}
				}
				SubArcAnimo(box_x,box_y,L"U",L"L",frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"U") && IsContentStr(box_in,L"L")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				if (frame<=PIPE_ANIMO_HAFE_FRAME){
					SubArcAnimo(box_x,box_y,L"U",L"L",frame);
					if (frame<PIPE_ANIMO_HAFE_FRAME) {
						SubArcAnimo(box_x,box_y,L"L",L"U",frame);
					}
				}
				SubArcAnimo(box_x,box_y,L"R",L"D",frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"U") && IsContentStr(box_in,L"L")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				if (frame<=PIPE_ANIMO_HAFE_FRAME){
					SubArcAnimo(box_x,box_y,L"U",L"L",frame);
					if (frame<PIPE_ANIMO_HAFE_FRAME) {
						SubArcAnimo(box_x,box_y,L"L",L"U",frame);
					}
				}
				SubArcAnimo(box_x,box_y,L"D",L"R",frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"L")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				SubArcAnimo(box_x,box_y,L"L",L"U",frame);		
				SubArcAnimo(box_x,box_y,L"R",L"D",frame);			
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"U")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				SubArcAnimo(box_x,box_y,L"D",L"R",frame);
				SubArcAnimo(box_x,box_y,L"U",L"L",frame);	
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"L")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){				
				SubArcAnimo(box_x,box_y,L"D",L"R",frame);		
				SubArcAnimo(box_x,box_y,L"L",L"U",frame);				
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"D") && IsContentStr(box_in,L"R")){
			for (; frame<=PIPE_ANIMO_HAFE_FRAME;frame++){				
				SubArcAnimo(box_x,box_y,L"D",L"R",frame);	
				if (frame<PIPE_ANIMO_HAFE_FRAME) {
					SubArcAnimo(box_x,box_y,L"R",L"D",frame);			
				}
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = data->box_static->full_lab + L"RD";
		}else if (IsContentStr(box_in,L"L") && IsContentStr(box_in,L"U")){
			for (; frame<=PIPE_ANIMO_HAFE_FRAME;frame++){				
				SubArcAnimo(box_x,box_y,L"L",L"U",frame);
				if (frame<PIPE_ANIMO_HAFE_FRAME) {
					SubArcAnimo(box_x,box_y,L"U",L"L",frame);	
				}
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = data->box_static->full_lab + L"LU";
		}else if (IsContentStr(box_in,L"R") && IsContentStr(box_in,L"U")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){				
				SubArcAnimo(box_x,box_y,L"U",L"L",frame);
				SubArcAnimo(box_x,box_y,L"R",L"D",frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = box_tag;
		}else if (IsContentStr(box_in,L"D")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				SubArcAnimo(box_x,box_y,L"D",L"R",frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = data->box_static->full_lab + L"DR";
		}else if (IsContentStr(box_in,L"U")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				SubArcAnimo(box_x,box_y,L"U",L"L",frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = data->box_static->full_lab + L"LU";
		}else if (IsContentStr(box_in,L"L")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				SubArcAnimo(box_x,box_y,L"L",L"U",frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = data->box_static->full_lab + L"UL";
		}else if (IsContentStr(box_in,L"R")){
			for (; frame<=PIPE_ANIMO_FRAME;frame++){
				SubArcAnimo(box_x,box_y,L"R",L"D",frame);
				Sleep(PIPE_ANIMO_FRAME_BETWEEN);
			}
			data->box_static->full_lab = data->box_static->full_lab + L"RD";
		}	

	}
	return 0;
} 

class Scorbar: public CUiStatusBar
{
public:
	UiButtonEx* sta;
protected:
	int  OnMouseMove (UINT fwKeys, int xPos, int yPos) {
		sta->SetText2(Int2String(GetPadpos()*100/0xFFFF,2)+L"%");
		sta->Invalidate();
		sta->Update();
		return CUiStatusBar::OnMouseMove(fwKeys,xPos,yPos);
	}
};

// 设置面版定义窗口
class SettingWnd: public CMzWndEx
{
	MZ_DECLARE_DYNAMIC(SettingWnd);
public:	
	UiScrollWin m_ScrollWin;
	UiToolbar_Text m_Toolbar;
	UiCaption m_Caption1;
	UiCaption m_Caption2;
	UiCaption m_Caption3;

	UiButtonEx m_BtnSetting_PipeDire;
	UiButtonEx m_BtnSetting_PreWhere;
	UiButtonEx m_BtnSetting_MKeyAction;

	UiStatic m_Static_SysVolume;
	Scorbar m_StatusBar_SysVolume;
	UiStatic m_Static_Volume;
	Scorbar m_StatusBar_Volume;
	UiButtonEx m_BtnSetting_SoundSwitch;
	UiButtonEx m_BtnSetting_DebugSwitch;
	UiButtonEx m_BtnSetting_readme;
	UiButtonEx m_BtnSetting_about;
	UiButtonEx m_BtnSetting_SoundLable;
	UiButtonEx m_BtnSetting_DebugLable;
	UiButtonEx m_BtnSetting_VolumeLable;
	UiButtonEx m_BtnSetting_SysVolumeLable;

	static void ReadSetting (){
		if (!Common::FileExists(SettingFileName)){
			IniCreateFile(SettingFileName);
		}

		DWORD* readint = new DWORD();
		if (IsExistApp(AppName,SettingFileName)){	
			IniReadInt (AppName, _T("Volume"), &dwVolume, SettingFileName);		
			//SetVolume(dwVolume);
			
			if (IniReadInt (AppName, _T("isSoundsOn"), readint, SettingFileName)){
				if (*readint == 1)
					isSoundsOn = true;
				else
					isSoundsOn = false;
			}
			if (IniReadInt (AppName, _T("isdebug"), readint, SettingFileName)){
				if (*readint == 1)
					isdebug = true;
				else
					isdebug = false;
			}
			if (IniReadInt (AppName, _T("isPreLeft"), readint, SettingFileName)){
				if (*readint == 1)
					isPreLeft = true;
				else
					isPreLeft = false;
				org_isPreLeft = isPreLeft;
			}
			if (IniReadInt (AppName, _T("isPreListUp"), readint, SettingFileName)){
				if (*readint == 1)
					isPreListUp = true;
				else
					isPreListUp = false;
				org_isPreListUp = isPreListUp;
			}
			if (IniReadInt (AppName, _T("MKeyAction"), readint, SettingFileName)){
				mAction = *readint;
				if (mAction == SHK_RET_APPNOEXIT_SHELLNOTOP){
					HoldShellUsingSomeKeyFunction(hWnd, MZ_HARDKEY_HOME);
				}else{
					UnHoldShellUsingSomeKeyFunction (hWnd, MZ_HARDKEY_HOME);
				}
			}
		}

		if (IsExistApp(RankingName,SettingFileName)){	
			_TCHAR buff[20];
			TCHAR* readstring = new TCHAR();
			for (int i=1; i<=PIPE_RANKING_MAX; i++){					
				wsprintf(buff,_T("score_%d"),i);
				if (IniReadInt (RankingName, buff, readint, SettingFileName)){
					wsprintf(buff,_T("name_%d"),i);	
					if (IniReadString (RankingName, buff, &readstring, SettingFileName)){
						ranking.addRanking(readstring,*readint);
					}
				}else{
					break;
				}
			}
			if (ranking.m_List.GetItemCount() > 0){
				ranking_last_name = readstring;
			}else{
				ranking_last_name = L"新玩家";
			}
		}else{
			ranking_last_name = L"新玩家";
		}
		
	}

protected:


	// Initialization of the window (dialog)
	virtual BOOL OnInitDialog()
	{
		// Must call the Init of parent class first!
		if (!CMzWndEx::OnInitDialog())
		{
			return FALSE;
		}

		// Then init the controls & other things in the window
		ImagingHelper *imgArrow = ImagingHelper::GetImageObject(GetMzResModuleHandle(),MZRES_IDR_PNG_ARROW_RIGHT, true);

		m_ScrollWin.SetPos(0,0,GetWidth(),GetHeight()-MZM_HEIGHT_TEXT_TOOLBAR);
		m_ScrollWin.SetID(PIPE_TOOLBAR_SET_SCROLLWIN);
		m_ScrollWin.EnableScrollBarV(true);
		AddUiWin(&m_ScrollWin);

		int y = 0;
		m_Caption1.SetPos(0,y,GetWidth(),MZM_HEIGHT_CAPTION);
		m_Caption1.SetText(L"界面");
		m_ScrollWin.AddChild(&m_Caption1);

		y+=MZM_HEIGHT_CAPTION;
		m_BtnSetting_PreWhere.SetPos(0,y,GetWidth(),MZM_HEIGHT_BUTTONEX);
		m_BtnSetting_PreWhere.SetText(L"信息面版位置(重启程序生效)");
		m_BtnSetting_PreWhere.SetTextMaxLen(0);
		if (isPreLeft){
			m_BtnSetting_PreWhere.SetText2(L"左边");
		}else{
			m_BtnSetting_PreWhere.SetText2(L"右边");
		}
		m_BtnSetting_PreWhere.SetButtonType(MZC_BUTTON_LINE_BOTTOM);
		m_BtnSetting_PreWhere.SetID(PIPE_SETTING_PIPEWHERE);
		m_ScrollWin.AddChild(&m_BtnSetting_PreWhere);

		y+=MZM_HEIGHT_BUTTONEX;
		m_BtnSetting_PipeDire.SetPos(0,y,GetWidth(),MZM_HEIGHT_BUTTONEX);
		m_BtnSetting_PipeDire.SetText(L"管道移动方向(新游戏生效)");
		m_BtnSetting_PipeDire.SetTextMaxLen(0);
		if (isPreListUp){
			m_BtnSetting_PipeDire.SetText2(L"向上");
		}else{
			m_BtnSetting_PipeDire.SetText2(L"向下");
		}
		m_BtnSetting_PipeDire.SetButtonType(MZC_BUTTON_LINE_BOTTOM);
		m_BtnSetting_PipeDire.SetID(PIPE_SETTING_PIPEDIRE);
		m_ScrollWin.AddChild(&m_BtnSetting_PipeDire);

		if(V_DEBUG){
			y+=MZM_HEIGHT_BUTTONEX;
			m_BtnSetting_DebugLable.SetPos(0,y,GetWidth(),MZM_HEIGHT_BUTTONEX);
			m_BtnSetting_DebugLable.SetText(L"调试信息");
			m_BtnSetting_DebugLable.SetButtonType(MZC_BUTTON_LINE_BOTTOM);
			m_BtnSetting_DebugLable.SetEnable(false);
			m_ScrollWin.AddChild(&m_BtnSetting_DebugLable);

			m_BtnSetting_DebugSwitch.SetPos(350,y,GetWidth()-350,MZM_HEIGHT_BUTTONEX);
			m_BtnSetting_DebugSwitch.SetButtonType(MZC_BUTTON_SWITCH);
			m_BtnSetting_DebugSwitch.SetButtonMode(MZCS_BUTTON_HOVER);
			m_BtnSetting_DebugSwitch.SetID(PIPE_SETTING_DEBUG);
			if (isdebug == true){
				m_BtnSetting_DebugSwitch.SetState(1);
			}else{
				m_BtnSetting_DebugSwitch.SetState(0);
			}
			m_ScrollWin.AddChild(&m_BtnSetting_DebugSwitch);
		}

		y+=MZM_HEIGHT_BUTTONEX;
		m_Caption2.SetPos(0,y,GetWidth(),MZM_HEIGHT_CAPTION);
		m_Caption2.SetText(L"声音");
		m_ScrollWin.AddChild(&m_Caption2);

		y+=MZM_HEIGHT_CAPTION;
		m_BtnSetting_SysVolumeLable.SetPos(0,y,GetWidth(),MZM_HEIGHT_BUTTONEX);
		m_BtnSetting_SysVolumeLable.SetText(L"系统音量");
		m_BtnSetting_SysVolumeLable.SetButtonType(MZC_BUTTON_LINE_BOTTOM);
		m_BtnSetting_SysVolumeLable.SetEnable(false);
		m_ScrollWin.AddChild(&m_BtnSetting_SysVolumeLable);

		m_StatusBar_SysVolume.SetPos(150,y,240,MZM_HEIGHT_BUTTONEX);		
		m_StatusBar_SysVolume.SetRange (0x0000, 0xFFFF);
		m_StatusBar_SysVolume.SetPadpos(dwOrgVolume);
		m_BtnSetting_SysVolumeLable.SetText2(Int2String(dwOrgVolume*100/0xFFFF,2)+L"%");
		m_StatusBar_SysVolume.sta = &m_BtnSetting_SysVolumeLable;
		m_StatusBar_SysVolume.SetID(PIPE_SETTING_SYSVOLUME);
		m_ScrollWin.AddChild(&m_StatusBar_SysVolume);

		y+=MZM_HEIGHT_BUTTONEX;
		m_BtnSetting_VolumeLable.SetPos(0,y,GetWidth(),MZM_HEIGHT_BUTTONEX);
		m_BtnSetting_VolumeLable.SetText(L"游戏音量");
		m_BtnSetting_VolumeLable.SetButtonType(MZC_BUTTON_LINE_BOTTOM);
		m_BtnSetting_VolumeLable.SetEnable(false);
		m_ScrollWin.AddChild(&m_BtnSetting_VolumeLable);

		m_StatusBar_Volume.SetPos(150,y,240,MZM_HEIGHT_BUTTONEX);		
		m_StatusBar_Volume.SetRange (0x0000, 0xFFFF);
		//waveOutGetVolume(0,&dwVolume);
		m_StatusBar_Volume.SetPadpos(dwVolume);
		m_BtnSetting_VolumeLable.SetText2(Int2String(dwVolume*100/0xFFFF,2)+L"%");
		m_StatusBar_Volume.sta = &m_BtnSetting_VolumeLable;
		m_StatusBar_Volume.SetID(PIPE_SETTING_VOLUME);
		m_ScrollWin.AddChild(&m_StatusBar_Volume);

		y+=MZM_HEIGHT_BUTTONEX;
		m_BtnSetting_SoundLable.SetPos(0,y,GetWidth(),MZM_HEIGHT_BUTTONEX);
		m_BtnSetting_SoundLable.SetText(L"音效");
		m_BtnSetting_SoundLable.SetButtonType(MZC_BUTTON_LINE_BOTTOM);
		m_BtnSetting_SoundLable.SetEnable(false);
		m_ScrollWin.AddChild(&m_BtnSetting_SoundLable);

		m_BtnSetting_SoundSwitch.SetPos(350,y,GetWidth()-350,MZM_HEIGHT_BUTTONEX);
		m_BtnSetting_SoundSwitch.SetButtonType(MZC_BUTTON_SWITCH);
		m_BtnSetting_SoundSwitch.SetButtonMode(MZCS_BUTTON_HOVER);
		m_BtnSetting_SoundSwitch.SetID(PIPE_SETTING_SOUND);
		if (isSoundsOn == true){
			m_BtnSetting_SoundSwitch.SetState(1);
		}else{
			m_BtnSetting_SoundSwitch.SetState(0);
		}
		m_ScrollWin.AddChild(&m_BtnSetting_SoundSwitch);

		y+=MZM_HEIGHT_BUTTONEX;
		m_Caption3.SetPos(0,y,GetWidth(),MZM_HEIGHT_CAPTION);
		m_Caption3.SetText(L"其它");
		m_ScrollWin.AddChild(&m_Caption3);

		y+=MZM_HEIGHT_CAPTION;
		m_BtnSetting_MKeyAction.SetPos(0,y,GetWidth(),MZM_HEIGHT_BUTTONEX);
		m_BtnSetting_MKeyAction.SetText(L"M键动作");
		m_BtnSetting_MKeyAction.SetTextMaxLen(0);
		if (mAction == SHK_RET_APPNOEXIT_SHELLNOTOP){
			m_BtnSetting_MKeyAction.SetText2(L"无效");
		}else if (mAction == SHK_RET_APPNOEXIT_SHELLTOP){
			m_BtnSetting_MKeyAction.SetText2(L"最小化");
		}else if (mAction == SHK_RET_APPEXIT_SHELLTOP){
			m_BtnSetting_MKeyAction.SetText2(L"关闭");
		}
		m_BtnSetting_MKeyAction.SetButtonType(MZC_BUTTON_LINE_BOTTOM);
		m_BtnSetting_MKeyAction.SetID(PIPE_SETTING_MKEYACT);
		m_ScrollWin.AddChild(&m_BtnSetting_MKeyAction);

		y+=MZM_HEIGHT_BUTTONEX;
		m_BtnSetting_readme.SetPos(0,y,GetWidth(),MZM_HEIGHT_BUTTONEX);
		m_BtnSetting_readme.SetText(L"游戏说明");
		m_BtnSetting_readme.SetButtonType(MZC_BUTTON_LINE_BOTTOM);		
		m_BtnSetting_readme.SetID(PIPE_SETTING_README);
		m_BtnSetting_readme.SetImage2(imgArrow);
		m_BtnSetting_readme.SetImageWidth2(imgArrow->GetImageWidth());
		m_BtnSetting_readme.SetShowImage2(true);
		m_ScrollWin.AddChild(&m_BtnSetting_readme);

		y+=MZM_HEIGHT_BUTTONEX;
		m_BtnSetting_about.SetPos(0,y,GetWidth(),MZM_HEIGHT_BUTTONEX);
		m_BtnSetting_about.SetText(L"关于游戏");
		m_BtnSetting_about.SetButtonType(MZC_BUTTON_LINE_BOTTOM);		
		m_BtnSetting_about.SetID(PIPE_SETTING_ABOUT);
		m_BtnSetting_about.SetImage2(imgArrow);
		m_BtnSetting_about.SetImageWidth2(imgArrow->GetImageWidth());
		m_BtnSetting_about.SetShowImage2(true);
		m_ScrollWin.AddChild(&m_BtnSetting_about);

		m_Toolbar.SetPos(0,GetHeight()-MZM_HEIGHT_TEXT_TOOLBAR,GetWidth(),MZM_HEIGHT_TEXT_TOOLBAR);
		m_Toolbar.SetButton(0, true, true, L"取消");
		m_Toolbar.EnableLeftArrow(true);
		m_Toolbar.SetButton(2, true, true, L"保存");
		m_Toolbar.SetID(PIPE_TOOLBAR_SET_TOOLBAR);
		AddUiWin(&m_Toolbar);

		return TRUE;
	}

	// override the MZFC command handler
	virtual void OnMzCommand(WPARAM wParam, LPARAM lParam)
	{
		UINT_PTR id = LOWORD(wParam);
		switch(id)
		{
		case PIPE_TOOLBAR_SET_TOOLBAR:
			{
				int nIndex = lParam;
				if (nIndex==0)
				{
					// exit the modal dialog
					EndModal(ID_CANCEL);
					return ;
				}

				if (nIndex==2)
				{
					//saving settings...
					SaveSetting();
					//then exit the dialog
					EndModal(ID_OK);
					return;
				}
			}
			break;
		case PIPE_SETTING_README:
			{
				Readme dlg;
				dlg.Create(rcWork.left,rcWork.top,RECT_WIDTH(rcWork),RECT_HEIGHT(rcWork), m_hWnd, 0, WS_POPUP);
				// set the animation of the window
				dlg.SetAnimateType_Show(MZ_ANIMTYPE_SCROLL_RIGHT_TO_LEFT_PUSH);
				dlg.SetAnimateType_Hide(MZ_ANIMTYPE_SCROLL_LEFT_TO_RIGHT_PUSH);
				dlg.DoModal();
				return;
			}
			break;
		case PIPE_SETTING_ABOUT:
			{
				About dlg;
				dlg.Create(rcWork.left,rcWork.top,RECT_WIDTH(rcWork),RECT_HEIGHT(rcWork), m_hWnd, 0, WS_POPUP);
				// set the animation of the window
				dlg.SetAnimateType_Show(MZ_ANIMTYPE_SCROLL_RIGHT_TO_LEFT_PUSH);
				dlg.SetAnimateType_Hide(MZ_ANIMTYPE_SCROLL_LEFT_TO_RIGHT_PUSH);
				dlg.DoModal();
				return;
			}
			break;
		case PIPE_SETTING_VOLUME:
			{
				return;
			}
			break;

		case PIPE_SETTING_SOUND: 
		case PIPE_SETTING_DEBUG:
			{
				MzPlaySound_Switch ();			
			}
			break;

		case PIPE_SETTING_PIPEWHERE:
			{
				MzPlaySound_Key ();
				if (m_BtnSetting_PreWhere.GetText2() == L"左边"){
					m_BtnSetting_PreWhere.SetText2(L"右边");					
				}else{
					m_BtnSetting_PreWhere.SetText2(L"左边");
				}
				m_BtnSetting_PreWhere.Invalidate();
				m_BtnSetting_PreWhere.Update();
			}
			break;

		case PIPE_SETTING_PIPEDIRE:
			{
				MzPlaySound_Key ();
				if (m_BtnSetting_PipeDire.GetText2() == L"向上"){
					m_BtnSetting_PipeDire.SetText2(L"向下");					
				}else{
					m_BtnSetting_PipeDire.SetText2(L"向上");
				}
				m_BtnSetting_PipeDire.Invalidate();
				m_BtnSetting_PipeDire.Update();
			}
			break;

		case PIPE_SETTING_MKEYACT:
			{
				MzPlaySound_Key ();
				if (m_BtnSetting_MKeyAction.GetText2() == L"无效"){
					m_BtnSetting_MKeyAction.SetText2(L"最小化");					
				}else if (m_BtnSetting_MKeyAction.GetText2() == L"最小化"){
					m_BtnSetting_MKeyAction.SetText2(L"关闭");
				}else if (m_BtnSetting_MKeyAction.GetText2() == L"关闭"){
					m_BtnSetting_MKeyAction.SetText2(L"无效");
				}
				m_BtnSetting_MKeyAction.Invalidate();
				m_BtnSetting_MKeyAction.Update();
			}
			break;
		}
	}

	void SaveSetting (){
		if (!IsExistApp(AppName,SettingFileName)){
			IniCreateFile(SettingFileName);
		}

		//保存系统音量
		dwOrgVolume = m_StatusBar_SysVolume.GetPadpos();
		SetVolume(dwOrgVolume);
		dwVolume = m_StatusBar_Volume.GetPadpos();
		
		IniWriteInt (AppName, _T("Volume"), dwVolume, SettingFileName);					

		isSoundsOn = (m_BtnSetting_SoundSwitch.GetState() == 1);
		if (isSoundsOn){
			IniWriteInt (AppName, _T("isSoundsOn"), 1, SettingFileName);
		}else{
			IniWriteInt (AppName, _T("isSoundsOn"), 0, SettingFileName);
		}

		isdebug = (m_BtnSetting_DebugSwitch.GetState() == 1);
		if (isdebug){
			IniWriteInt (AppName, _T("isdebug"), 1, SettingFileName);
		}else{
			IniWriteInt (AppName, _T("isdebug"), 0, SettingFileName);
		}

		isPreListUp = (m_BtnSetting_PipeDire.GetText2() == L"向上");
		if (isPreListUp){
			IniWriteInt (AppName, _T("isPreListUp"), 1, SettingFileName);
		}else{
			IniWriteInt (AppName, _T("isPreListUp"), 0, SettingFileName);
		}

		isPreLeft = (m_BtnSetting_PreWhere.GetText2() == L"左边");
		if (isPreLeft){
			IniWriteInt (AppName, _T("isPreLeft"), 1, SettingFileName);
		}else{
			IniWriteInt (AppName, _T("isPreLeft"), 0, SettingFileName);
		}

		if (m_BtnSetting_MKeyAction.GetText2() == L"无效"){
			mAction = SHK_RET_APPNOEXIT_SHELLNOTOP;					
		}else if (m_BtnSetting_MKeyAction.GetText2() == L"最小化"){
			mAction = SHK_RET_APPNOEXIT_SHELLTOP;
		}else if (m_BtnSetting_MKeyAction.GetText2() == L"关闭"){
			mAction = SHK_RET_APPEXIT_SHELLTOP;
		}
		if (mAction == SHK_RET_APPNOEXIT_SHELLNOTOP){
			HoldShellUsingSomeKeyFunction(hWnd, MZ_HARDKEY_HOME);
		}else{
			UnHoldShellUsingSomeKeyFunction (hWnd, MZ_HARDKEY_HOME);
		}
		IniWriteInt (AppName, _T("MKeyAction"), mAction, SettingFileName);
	}
private:

};
MZ_IMPLEMENT_DYNAMIC(SettingWnd)

// A new list control derived from UiList
class PreviewList:
	public UiList
{
public:
	// override the DrawItem member function to do your own drawing of the list
	void DrawItem(HDC hdcDst, int nIndex, RECT* prcItem, RECT *prcWin, RECT *prcUpdate)
	{
		// draw the high-light background for the selected item
		if (nIndex == GetSelectedIndex())
		{
			MzDrawSelectedBg(hdcDst, prcItem);
		}

		// draw an image on the left
		ListItem* pItem = GetItem(nIndex);
		if (pItem->Data != NULL){
			ImagingHelper * pimg = (ImagingHelper *)pItem->Data;
			if (pimg)
			{
				pimg->Draw(hdcDst, prcItem, false, false);
			}
		}
	}
protected:
private:
};

// 主窗口 
class MainWnd: public CMzWndEx
{
	MZ_DECLARE_DYNAMIC(MainWnd);
public:
	// 主窗口中的子窗口变量
	PreviewList p_List;	
	UiToolbar_Text m_Toolbar;
	UiButton_Image but_bg;	
	UiButton_Image but_WangGe;	
	UiButton_Image but_YuLang;	
	UiButton but_skip;

	UiButton_Image but_1_1;
	UiButton_Image but_1_2;
	UiButton_Image but_1_3;
	UiButton_Image but_1_4;
	UiButton_Image but_1_5;
	UiButton_Image but_2_1;
	UiButton_Image but_2_2;
	UiButton_Image but_2_3;
	UiButton_Image but_2_4;
	UiButton_Image but_2_5;
	UiButton_Image but_3_1;
	UiButton_Image but_3_2;
	UiButton_Image but_3_3;
	UiButton_Image but_3_4;
	UiButton_Image but_3_5;
	UiButton_Image but_4_1;
	UiButton_Image but_4_2;
	UiButton_Image but_4_3;
	UiButton_Image but_4_4;
	UiButton_Image but_4_5;
	UiButton_Image but_5_1;
	UiButton_Image but_5_2;
	UiButton_Image but_5_3;
	UiButton_Image but_5_4;
	UiButton_Image but_5_5;
	UiButton_Image but_6_1;
	UiButton_Image but_6_2;
	UiButton_Image but_6_3;
	UiButton_Image but_6_4;
	UiButton_Image but_6_5;
	UiButton_Image but_7_1;
	UiButton_Image but_7_2;
	UiButton_Image but_7_3;
	UiButton_Image but_7_4;
	UiButton_Image but_7_5;
	UiButton_Image but_8_1;
	UiButton_Image but_8_2;
	UiButton_Image but_8_3;
	UiButton_Image but_8_4;
	UiButton_Image but_8_5;	

	ImagingHelper *pimg_bg;
	ImagingHelper *pimg_WangGe;
	ImagingHelper *pimg_YuLang;
	ImagingHelper *pimg_none;
	ImagingHelper *pimg_pipe0;
	ImagingHelper *pimg_pipe1;
	ImagingHelper *pimg_pipe2;
	ImagingHelper *pimg_pipe3;
	ImagingHelper *pimg_pipe4;
	ImagingHelper *pimg_pipe5;
	ImagingHelper *pimg_pipe6;
	ImagingHelper *pimg_pipe7;
	ImagingHelper *pimg_pipe8;
	ImagingHelper *pimg_pipe9;
	ImagingHelper *pimg_pipe10;
	ImagingHelper *pimg_pipe11;
	ImagingHelper *pimg_pipe12;
	ImagingHelper *pimg_pipe13;
	ImagingHelper *pimg_pipe14;

	// 重载窗口初始化函数
	virtual BOOL OnInitDialog()
	{
		// 先调用基类的初始化函数
		if (!CMzWndEx::OnInitDialog())
		{
			return FALSE;
		}

		//初始化变量
		pimg_none = ImagingHelper::GetImageObject(MzGetInstanceHandle(), IDB_PNG_PIPE_NONE, true);	
		pimg_pipe0 = ImagingHelper::GetImageObject(MzGetInstanceHandle(), IDB_PNG_PIPE0, true);	
		pipe_img_all.push_back(pimg_pipe0);
		pimg_pipe1 = ImagingHelper::GetImageObject(MzGetInstanceHandle(), IDB_PNG_PIPE1, true);	
		pipe_img_all.push_back(pimg_pipe1);
		pimg_pipe2 = ImagingHelper::GetImageObject(MzGetInstanceHandle(), IDB_PNG_PIPE2, true);	
		pipe_img_all.push_back(pimg_pipe2);
		pimg_pipe3 = ImagingHelper::GetImageObject(MzGetInstanceHandle(), IDB_PNG_PIPE3, true);
		pipe_img_all.push_back(pimg_pipe3);
		pimg_pipe4 = ImagingHelper::GetImageObject(MzGetInstanceHandle(), IDB_PNG_PIPE4, true);	
		pipe_img_all.push_back(pimg_pipe4);
		pimg_pipe5 = ImagingHelper::GetImageObject(MzGetInstanceHandle(), IDB_PNG_PIPE5, true);	
		pipe_img_all.push_back(pimg_pipe5);
		pimg_pipe6 = ImagingHelper::GetImageObject(MzGetInstanceHandle(), IDB_PNG_PIPE6, true);	
		pipe_img_all.push_back(pimg_pipe6);
		pimg_pipe7 = ImagingHelper::GetImageObject(MzGetInstanceHandle(), IDB_PNG_PIPE7, true);	
		pipe_img_all.push_back(pimg_pipe7);
		pimg_pipe8 = ImagingHelper::GetImageObject(MzGetInstanceHandle(), IDB_PNG_PIPE8, true);	
		pipe_img_all.push_back(pimg_pipe8);
		pimg_pipe9 = ImagingHelper::GetImageObject(MzGetInstanceHandle(), IDB_PNG_PIPE9, true);	
		pipe_img_all.push_back(pimg_pipe9);
		pimg_pipe10 = ImagingHelper::GetImageObject(MzGetInstanceHandle(), IDB_PNG_PIPE10, true);	
		pipe_img_all.push_back(pimg_pipe10);
		pimg_pipe11 = ImagingHelper::GetImageObject(MzGetInstanceHandle(), IDB_PNG_PIPE11, true);	
		pipe_img_all.push_back(pimg_pipe11);
		pimg_pipe12 = ImagingHelper::GetImageObject(MzGetInstanceHandle(), IDB_PNG_PIPE12, true);
		pipe_img_all.push_back(pimg_pipe12);	
		pimg_pipe13 = ImagingHelper::GetImageObject(MzGetInstanceHandle(), IDB_PNG_PIPE13, true);
		pipe_img_all.push_back(pimg_pipe13);	
		pimg_pipe14 = ImagingHelper::GetImageObject(MzGetInstanceHandle(), IDB_PNG_PIPE14, true);	
		pipe_img_all.push_back(pimg_pipe14);

		hdc = GetWindowDC(m_hWnd);
		hWnd = m_hWnd;

		// 注册按键消息
		HoldShellBatteryLowWarningBox(hWnd);
		HoldShellUsingSomeKeyFunction(hWnd, MZ_HARDKEY_PLAY|MZ_HARDKEY_POWER);

		pimg_droplets = ImagingHelper::GetImageObject(MzGetInstanceHandle(), IDB_BITMAP_droplets, false);
		HDC sdc = pimg_droplets->GetDC();
		BitBlt(sdc,1,1,80,1380,sdc,1,1,DSTINVERT);
		BitBlt(sdc,163,1,80,1380,sdc,163,1,DSTINVERT);

		ranking.Create(rcWork.left,rcWork.top,RECT_WIDTH(rcWork),RECT_HEIGHT(rcWork), m_hWnd, 0, WS_POPUP);
		ranking.SetAnimateType_Show(MZ_ANIMTYPE_FADE);
		ranking.SetAnimateType_Hide(MZ_ANIMTYPE_FADE);

		//设置
		SettingWnd::ReadSetting();		

		// 设置主窗口

		// 初始化主窗口中的控件
		//主窗口背景
		int YuLang_L = 0;
		int Lab_L = 0;
		int WangGe_L = 0;
		if (isPreLeft){
			YuLang_L = 0;
			Lab_L = 0;
			WangGe_L = 90;
		}else{
			YuLang_L = 395;
			Lab_L = 390;
			WangGe_L = 0;
		}

		ImagingHelper* pimg_bg = new ImagingHelper();
		if (pimg_bg->LoadImageW(AppPath+L"Main_bg.bmp",true,true,false)){
			but_bg.SetImage_Normal(pimg_bg);
			but_bg.SetEnable(false);
			but_bg.SetPos(0,0,GetWidth(),GetHeight()-MZM_HEIGHT_TEXT_TOOLBAR);
			AddUiWin(&but_bg);
		}	

		pimg_YuLang = ImagingHelper::GetImageObject(MzGetInstanceHandle(), IDB_PNG_YuLang, true);
		but_YuLang.SetImage_Normal(pimg_YuLang);
		but_YuLang.SetEnable(false);
		but_YuLang.SetPos(YuLang_L,0,pimg_YuLang->GetImageWidth(),pimg_YuLang->GetImageHeight());		
		AddUiWin(&but_YuLang);

		pimg_WangGe = ImagingHelper::GetImageObject(MzGetInstanceHandle(), IDB_PNG_WangGe, true);
		but_WangGe.SetImage_Normal(pimg_WangGe);
		but_WangGe.SetEnable(false);
		but_WangGe.SetPos(WangGe_L,0,pimg_WangGe->GetImageWidth(),pimg_WangGe->GetImageHeight());		
		AddUiWin(&but_WangGe);

		//信息部分
		int up_down = 0;

		//计分部分	
		static_score.SetPos(Lab_L+PIPE_MAIN_SPAN_OUT, up_down+PIPE_MAIN_SPAN_OUT, PIPE_MAIN_SIZE_IMG+PIPE_MAIN_SPAN_IN, PIPE_MAIN_LABLE_H);
		static_score.SetDrawTextFormat(DT_LEFT);
		static_score.SetTextSize(21);
		AddUiWin(&static_score);
		up_down = static_score.GetPosY()+static_score.GetHeight();

		static_level.SetPos(Lab_L+PIPE_MAIN_SPAN_OUT, up_down+PIPE_MAIN_SPAN_OUT, PIPE_MAIN_SIZE_IMG+PIPE_MAIN_SPAN_IN, PIPE_MAIN_LABLE_H);
		static_level.SetDrawTextFormat(DT_LEFT);
		static_level.SetTextSize(21);
		AddUiWin(&static_level);
		up_down = static_level.GetPosY()+static_level.GetHeight();


		//preview list
		p_List.SetPos(YuLang_L+PIPE_MAIN_SPAN_OUT, up_down+PIPE_MAIN_SPAN_OUT, PIPE_MAIN_SIZE_IMG, PIPE_MAIN_NUM_RRELIST_IMG*(PIPE_MAIN_SIZE_IMG+PIPE_MAIN_SPAN_IN));
		p_List.SetID(PIPE_MAIN_PRELIST);
		p_List.SetEnable(false);
		//p_List.EnableGridlines (false);
		p_List.SetGridlineColor_GradientBottomRight(RGB(200,200,255));
		p_List.SetGridlineColor_GradientBottomLeft(RGB(50,255,50));
		p_List.SetGridlineColor_GradientTopRight(RGB(200,200,255));
		p_List.SetGridlineColor_GradientTopLeft(RGB(50,255,50));
		p_List.SetItemHeight(PIPE_MAIN_SIZE_IMG+PIPE_MAIN_SPAN_IN);
		AddUiWin(&p_List);
		up_down = p_List.GetPosY()+p_List.GetHeight();

		//skip button
		if (isPreLeft){
			but_skip.SetPos(YuLang_L, up_down+PIPE_MAIN_SPAN_OUT, PIPE_MAIN_SIZE_IMG+PIPE_MAIN_SPAN_OUT+PIPE_MAIN_SPAN_IN, 100);
		}else{
			but_skip.SetPos(YuLang_L-PIPE_MAIN_SPAN_OUT, up_down+PIPE_MAIN_SPAN_OUT, PIPE_MAIN_SIZE_IMG+PIPE_MAIN_SPAN_OUT+PIPE_MAIN_SPAN_IN, 100);
		}
		but_skip.SetID(PIPE_MAIN_BTN_SKIP);
		but_skip.SetButtonType(MZC_BUTTON_ORANGE);
		but_skip.SetText(L"跳过");
		AddUiWin(&but_skip);
		up_down = p_List.GetPosY()+p_List.GetHeight();

		//main button
		but_all.push_back(but_1_1);
		but_all.push_back(but_1_2);
		but_all.push_back(but_1_3);
		but_all.push_back(but_1_4);
		but_all.push_back(but_1_5);
		but_all.push_back(but_2_1);
		but_all.push_back(but_2_2);
		but_all.push_back(but_2_3);
		but_all.push_back(but_2_4);
		but_all.push_back(but_2_5);
		but_all.push_back(but_3_1);
		but_all.push_back(but_3_2);
		but_all.push_back(but_3_3);
		but_all.push_back(but_3_4);
		but_all.push_back(but_3_5);
		but_all.push_back(but_4_1);
		but_all.push_back(but_4_2);
		but_all.push_back(but_4_3);
		but_all.push_back(but_4_4);
		but_all.push_back(but_4_5);
		but_all.push_back(but_5_1);
		but_all.push_back(but_5_2);
		but_all.push_back(but_5_3);
		but_all.push_back(but_5_4);
		but_all.push_back(but_5_5);
		but_all.push_back(but_6_1);
		but_all.push_back(but_6_2);
		but_all.push_back(but_6_3);
		but_all.push_back(but_6_4);
		but_all.push_back(but_6_5);
		but_all.push_back(but_7_1);
		but_all.push_back(but_7_2);
		but_all.push_back(but_7_3);
		but_all.push_back(but_7_4);
		but_all.push_back(but_7_5);
		but_all.push_back(but_8_1);
		but_all.push_back(but_8_2);
		but_all.push_back(but_8_3);
		but_all.push_back(but_8_4);
		but_all.push_back(but_8_5);

		int img_l = WangGe_L+PIPE_MAIN_SPAN_OUT;
		for (int i=0;i<PIPE_MAIN_NUM_IMG_V;i++)
		{
			for (int k=0;k<PIPE_MAIN_NUM_IMG_H;k++)
			{
				BoxStatic bs ={L"",L"",L""};  
				box_static.push_back(bs);
				but_all.at(PIPE_MAIN_NUM_IMG_H*i+k).SetButtonType(MZC_BUTTON_LINE_NONE);
				but_all.at(PIPE_MAIN_NUM_IMG_H*i+k).SetPos(img_l+k*(PIPE_MAIN_SIZE_IMG+PIPE_MAIN_SPAN_IMG),PIPE_MAIN_SPAN_OUT+i*(PIPE_MAIN_SIZE_IMG+PIPE_MAIN_SPAN_IMG),PIPE_MAIN_SIZE_IMG,PIPE_MAIN_SIZE_IMG);
				but_all.at(PIPE_MAIN_NUM_IMG_H*i+k).SetID(PIPE_MAIN_BTN_1_1+k+i*10);
				AddUiWin(&but_all.at(PIPE_MAIN_NUM_IMG_H*i+k));
			}
		}

		// Init the toolbar and add it into the window
		m_Toolbar.SetPos(0,GetHeight()-MZM_HEIGHT_TEXT_TOOLBAR,GetWidth(),MZM_HEIGHT_TEXT_TOOLBAR);
		m_Toolbar.SetButton(0, true, true, L"游戏");
		m_Toolbar.SetButton(1, true, true, L"排名榜");
		m_Toolbar.SetButton(2, true, true, L"设置");
		m_Toolbar.SetID(PIPE_TOOLBAR);
		AddUiWin(&m_Toolbar);


		//new game
		NewGame();

		return TRUE;
	}

	virtual void  PaintWin (HDC hdc, RECT *prcUpdate) {
		// 先调用基类的初始化函数
		CMzWndEx::PaintWin(hdc, prcUpdate);
		//恢复最后的样子
		if (isInRepain){
			if (org_isPreLeft){
				BitBlt(hdc, 90, 0, mdc->GetWidth(),mdc->GetHeight(), mdc->GetDC(), 0, 0, SRCCOPY);
			}else{
				BitBlt(hdc, 0, 0, mdc->GetWidth(),mdc->GetHeight(), mdc->GetDC(), 0, 0, SRCCOPY);
			}
		}
	}

	virtual int OnShellHomeKey (UINT message, WPARAM wParam, LPARAM lParam) {
		SetVolume(dwOrgVolume);
		return mAction;
		//return SHK_RET_APPNOEXIT_SHELLNOTOP;
	}


	// 重载MZFC的命令消息处理函数
	virtual void OnMzCommand(WPARAM wParam, LPARAM lParam)
	{
		UINT_PTR id = LOWORD(wParam);
		switch(id)
		{
		case PIPE_TOOLBAR:
			{
				int nIndex = lParam;
				// 游戏菜单
				if (nIndex==0)
				{
					// pop out a PopupMenu:
					CPopupMenu ppm;
					struct PopupMenuItemProp pmip;      

					pmip.itemCr = MZC_BUTTON_ORANGE;
					pmip.itemRetID = PIPE_TOOLBAR_GAME_EXIT;
					pmip.str = L"退出";
					ppm.AddItem(pmip);  

					pmip.itemCr = MZC_BUTTON_PELLUCID;
					pmip.itemRetID = PIPE_TOOLBAR_GAME_CONTIUNE;
					pmip.str = L"返回";
					ppm.AddItem(pmip);

					if (isInGame){
						pmip.itemCr = MZC_BUTTON_PELLUCID;
						pmip.itemRetID = PIPE_TOOLBAR_GAME_GO;
						pmip.str = L"流水";
						ppm.AddItem(pmip); 
					}

					pmip.itemCr = MZC_BUTTON_PELLUCID;
					pmip.itemRetID = PIPE_TOOLBAR_GAME_NEW;
					pmip.str = L"新游戏";
					ppm.AddItem(pmip);

					ppm.Create(rcWork.left,rcWork.bottom - ppm.GetHeight(),RECT_WIDTH(rcWork),RECT_HEIGHT(rcWork),m_hWnd,0,WS_POPUP); 
					ppm.SetAnimateType_Show(MZ_ANIMTYPE_FADE);
					ppm.SetAnimateType_Hide(MZ_ANIMTYPE_FADE);

					int nID = ppm.DoModal();
					if (nID==PIPE_TOOLBAR_GAME_NEW)
					{
						NewGame();
					}
					if (nID==PIPE_TOOLBAR_GAME_GO)
					{
						Final();
					}
					if (nID==PIPE_TOOLBAR_GAME_EXIT)
					{
						if(1 == MzMessageBoxEx(m_hWnd, L"确定退出吗！", L"退出", MB_YESNO, false))
						{
							SetVolume(dwOrgVolume);
							PostQuitMessage(0);
						}
					}
					return;
				}

				// 排名榜面版
				if (nIndex==1)
				{
					ranking.showNew=false;
					ranking.highlight = 0;
					int nRet = ranking.DoModal();
					return;
				}

				// 设置面版
				if (nIndex==2)
				{
					SettingWnd dlg;
					dlg.Create(rcWork.left,rcWork.top,RECT_WIDTH(rcWork),RECT_HEIGHT(rcWork), m_hWnd, 0, WS_POPUP);
					// set the animation of the window
					dlg.SetAnimateType_Show(MZ_ANIMTYPE_FADE);
					dlg.SetAnimateType_Hide(MZ_ANIMTYPE_FADE);
					int nRet = dlg.DoModal();
					if (nRet==ID_OK)
					{

					}
					return;
				}

			}
			break;
		case PIPE_MAIN_BTN_SKIP:
			{
				if (isInGame){
					PutImage(0,0,true);
				}else{
					NewGame();
				}
			}
			break;
		default:
			{
				if (isInGame){
					int h = (id-PIPE_MAIN_BTN_1_1)%10;
					int v = (id-PIPE_MAIN_BTN_1_1)/10;
					PutImage(h,v,false);
				}				
			}
			break;
		}
	}

	// 点网格按钮 从0开始，
	void PutImage(int h,int v,bool skip){
		int but_no;
		if (now_item_no < PIPE_MAIN_NUM_IMG){
			now_item_no++;
			int item_in_list = 0;
			if (org_isPreListUp){
				item_in_list = now_item_no-1;
			}else{
				item_in_list = PIPE_MAIN_NUM_IMG+PIPE_MAIN_NUM_RRELIST_IMG-now_item_no;				
			}

			if (isSoundsOn) SetVolume(dwVolume);

			if (skip){
				if (isSoundsOn) PlaySound((LPCWSTR)IDR_WAVE_moving, MzGetInstanceHandle(), SND_RESOURCE | SND_ASYNC);	
			}else{
				//update button image
				ListItem* pItem = p_List.GetItem(item_in_list);
				ImagingHelper * pimg;
				but_no=PIPE_MAIN_NUM_IMG_H*v+h;

				CMzStringW orgstr = box_static.at(but_no).tag;
				CMzStringW newstr = CMzStringW(pItem->Text);
				if ((orgstr==L"LU" && newstr==L"RD") || (orgstr==L"RD" && newstr==L"LU")){
					pimg = pimg_pipe14;
					newstr = L"LURD/";
					//play boink sound
					if (isSoundsOn) PlaySound((LPCWSTR)IDR_WAVE_boink, MzGetInstanceHandle(), SND_RESOURCE | SND_ASYNC);	
				}else if ((orgstr==L"LD" && newstr==L"RU") || (orgstr==L"RU" && newstr==L"LD")){
					pimg = pimg_pipe13;
					newstr = L"LURD\\";
					//play boink sound
					if (isSoundsOn) PlaySound((LPCWSTR)IDR_WAVE_boink, MzGetInstanceHandle(), SND_RESOURCE | SND_ASYNC);	
				}else if (orgstr==L"LURD/" && (newstr==L"RD" || newstr==L"LU")){
					pimg = pimg_pipe14;
					newstr = L"LURD/";
					//play breaking sound
					if (isSoundsOn) {
						PlaySound((LPCWSTR)IDR_WAVE_glass_breaking, MzGetInstanceHandle(), SND_RESOURCE | SND_ASYNC);	
					}
				}else if (orgstr==L"LURD\\" && (newstr==L"RU" || newstr==L"LD")){
					pimg = pimg_pipe13;
					newstr = L"LURD\\";
					//play breaking sound
					if (isSoundsOn) {
						PlaySound((LPCWSTR)IDR_WAVE_glass_breaking, MzGetInstanceHandle(), SND_RESOURCE | SND_ASYNC);	
					}
				}else{	
					pimg = (ImagingHelper *)pItem->Data;
					if (orgstr.IsEmpty()){
						//play boink sound
						if (isSoundsOn) PlaySound((LPCWSTR)IDR_WAVE_boink, MzGetInstanceHandle(), SND_RESOURCE | SND_ASYNC);	
					}else{
						//play breaking sound
						if (isSoundsOn) {
							PlaySound((LPCWSTR)IDR_WAVE_glass_breaking, MzGetInstanceHandle(), SND_RESOURCE | SND_ASYNC);	
						}
					}
				}


				but_all.at(but_no).SetImage_Normal(pimg);
				but_all.at(but_no).Invalidate();
				but_all.at(but_no).Update();
				box_static.at(but_no).tag = newstr;
			}

			//set levelnum 
			CMzString str(10);
			wsprintf(str.C_Str(), L"剩余:%d", PIPE_MAIN_NUM_IMG-now_item_no);
			static_level.SetText(str.C_Str());
			static_level.Invalidate();
			static_level.Update();

			//scroll list
			int pos = 0;
			if (org_isPreListUp){
				pos = -(PIPE_MAIN_SIZE_IMG+PIPE_MAIN_SPAN_IN)*now_item_no;				
			}else{
				pos = -(PIPE_MAIN_SIZE_IMG+PIPE_MAIN_SPAN_IN)*(PIPE_MAIN_NUM_IMG-now_item_no);
			}
			p_List.ScrollTo(UI_SCROLLTO_POS,pos,true);

			//remove this item in list
			//p_List.RemoveItem(item_in_list);
			//p_List.InvalidateItem(item_in_list);
			//p_List.Update();

			Sleep(100);
			if (isSoundsOn) PlaySound(NULL, NULL, NULL);
			if (isSoundsOn) SetVolume(dwOrgVolume);

		}
		//all out in box
		if (now_item_no == PIPE_MAIN_NUM_IMG){
			Final();
			now_item_no++;  //防止下次进入
		}
	}

	void Final(){
		hFinalThread = CreateThread(NULL,0,FinalThread,this,0,&dwFinalThreadId);
		// Check the return value for success.
		// If failure, close existing thread handles, 
		// free memory allocation, and exit. 
		if (hFinalThread == NULL) 
		{
			CloseHandle(hFinalThread);
			ExitProcess(0);				
		}
	}

	// 新游戏
	void NewGame(){
		//clear 
		isInRepain = false;
		isInGame = true;
		debug_str = L" ";
		isErr = false;
		now_item_no = 0;
		Score = 0;
		box_err_animo.clear();
		now_chack_box.clear();
		animo.clear();
		Invalidate();
		but_skip.SetText(L"跳过");
		but_skip.Invalidate();

		//set 
		org_isPreListUp = isPreListUp;

		// Close all thread handles and free memory allocation.
		CloseHandle(hFinalThread);			

		//add rand items to list
		ListItem li;
		p_List.RemoveAll();

		if (!isPreListUp){
			for (int i=0;i<PIPE_MAIN_NUM_RRELIST_IMG;i++)
			{
				li.Data = pimg_none;
				p_List.AddItem(li);
			}
		}		

		SYSTEMTIME time ;
		GetLocalTime(&time);
		int nYear =time.wYear % 100;
		int nMonth  = time.wMonth;
		int nDay = time.wDay;
		int nHour = time.wHour;
		int nMinute = time.wMinute;
		int nSecond = time.wSecond;

		srand(nYear+nMonth+nDay+nHour+nMinute+nSecond); 
		for (int i=0;i<PIPE_MAIN_NUM_IMG;i++)
		{
			int a = rand()%12;
			li.Data = pipe_img_all.at(a+1);
			switch(a+IDB_PNG_PIPE1)
			{
			case IDB_PNG_PIPE1: li.Text = L"LR"; break;
			case IDB_PNG_PIPE2: li.Text = L"UD"; break;
			case IDB_PNG_PIPE3: li.Text = L"LUR"; break;
			case IDB_PNG_PIPE4: li.Text = L"URD"; break;
			case IDB_PNG_PIPE5: li.Text = L"LRD"; break;
			case IDB_PNG_PIPE6: li.Text = L"LUD"; break;
			case IDB_PNG_PIPE7: li.Text = L"LURD"; break;
			case IDB_PNG_PIPE8: li.Text = L"LURDX"; break;
			case IDB_PNG_PIPE9: li.Text = L"LU"; break;
			case IDB_PNG_PIPE10: li.Text = L"RU"; break;
			case IDB_PNG_PIPE11: li.Text = L"RD"; break;
			case IDB_PNG_PIPE12: li.Text = L"LD"; break;
			case IDB_PNG_PIPE13: li.Text = L"LURD\\"; break;
			case IDB_PNG_PIPE14: li.Text = L"LURD/"; break;
			}
			p_List.AddItem(li);
		}

		if (isPreListUp){
			for (int i=0;i<PIPE_MAIN_NUM_RRELIST_IMG;i++)
			{
				li.Data = NULL;
				p_List.AddItem(li);
			}
		}	

		if (isPreListUp)
			p_List.ScrollTo(UI_SCROLLTO_TOP,0,false);
		else
			p_List.ScrollTo(UI_SCROLLTO_BOTTOM,0,false);
		p_List.Invalidate();

		//clear image		
		int all_img_num = PIPE_MAIN_NUM_IMG_H*PIPE_MAIN_NUM_IMG_V;
		for (int but_no=0;but_no<all_img_num;but_no++)
		{
			but_all.at(but_no).SetEnable(true);
			but_all.at(but_no).SetImage_Normal(NULL);
			but_all.at(but_no).Invalidate();
			box_static.at(but_no).tag = L"";
			box_static.at(but_no).in_lab = L"";
			box_static.at(but_no).full_lab = L"";
			//起点设置
			if (but_no==2){
				box_static.at(but_no).in_lab = L"U";
				now_chack_box.push_back(but_no);
			}			
		}

		//set level lable
		CMzString str(10);
		wsprintf(str.C_Str(), L"剩余:%d", PIPE_MAIN_NUM_IMG);
		static_level.SetText(str.C_Str());
		static_level.Invalidate();

		//set score lable
		wsprintf(str.C_Str(), L"分数:%d", Score);
		static_score.SetText(str.C_Str());
		static_score.Invalidate();

	}
};
MZ_IMPLEMENT_DYNAMIC(MainWnd)


// 从CMzApp派生主应用程序类
class MainApp: public CMzApp
{
public:
	// 主窗口变量
	MainWnd m_MainWnd;

	// 重载Init函数
	virtual BOOL Init()
	{
		CoInitializeEx(0, COINIT_MULTITHREADED);

		//不锁屏
		SetPowerRequirement(L"BKL1:",D0,POWER_NAME,NULL,0);

		ImagingHelper* pimg_bg = new ImagingHelper();
		pimg_bg->LoadImage(AppPath+L"Cover_bg.bmp",true,true,false);

		Cover cov;	
		cov.pimg_bg = pimg_bg;
		//cov.Create(rcWork.left,rcWork.top,RECT_WIDTH(rcWork),RECT_HEIGHT(rcWork), NULL, NULL, NULL);
		cov.Create((RECT_WIDTH(rcWork)-pimg_bg->GetImageWidth())/2,(RECT_HEIGHT(rcWork)-pimg_bg->GetImageHeight())/2+40 ,pimg_bg->GetImageWidth(),pimg_bg->GetImageHeight(), NULL, NULL, NULL);
		cov.SetBgColor(NULL);
		cov.AnimateWindow(MZ_ANIMTYPE_FADE,TRUE);
		cov.Show();
//
//BOOL go;
//		// enable microphone and speaker for phone device.
//		LONG result;
//		HPHONEAPP phoneApp = 0;
//		DWORD dwNumPhones = 0;
//		DWORD dwApiVersion = TAPI_CURRENT_VERSION;
//		PHONEINITIALIZEEXPARAMS phoneParms;
//
//		memset (&phoneParms, 0, sizeof(phoneParms));
//		phoneParms.dwTotalSize = sizeof(phoneParms);
//		phoneParms.dwOptions = PHONEINITIALIZEEXOPTION_USEEVENT;
//
//		result = phoneInitializeEx ( &phoneApp, NULL, NULL, NULL, &dwNumPhones, &dwApiVersion, &phoneParms );
//
//		if ( 0 == result )
//		{
//			HPHONE phone = 0;
//
//			result = phoneOpen ( phoneApp, 0, &phone, dwApiVersion, 0, 0, PHONEPRIVILEGE_OWNER );
//
//			if ( 0 == result )
//			{
//				HGLOBAL PhoneStatusHandle;   
//				  LPPHONESTATUS PhoneStatusPtr;   
//				  DWORD lResult;   
//				  DWORD dwNeededSize;   
//				  DWORD ButtonLampMode;   
//				  DWORD ButtonLampIndex;   
//				//   Allocate   memory   for   the   PHONESTATUS   structure   
//
//				PhoneStatusHandle   =   GlobalAlloc(GHND,   sizeof(PHONESTATUS)   +   1024);   
//
//				PhoneStatusPtr   =   (LPPHONESTATUS)GlobalLock(PhoneStatusHandle);    
//
//				//   Get   the   phone   status   
//
//				PhoneStatusPtr->dwTotalSize   =   GlobalSize(PhoneStatusHandle);   
//				result = phoneGetStatus(phone, PhoneStatusPtr);
//
//				WINAPI::phone
//go = Common::IsValidRegKey();
//
//				phoneClose ( phone );
//			}
//
//			phoneShutdown ( phoneApp );
//		}

		//bool isFly = GetFlyModeState();

		//if (isFly){
		//	SetFlyModeReg(false);
		//	FlyModeSendMsgToShell(false);
		//}

		//key_chack
		BOOL go = Common::IsValidRegKey();

		/*if (isFly){
			SetFlyModeReg(true);
			FlyModeSendMsgToShell(true);
		}*/

		if (go == TRUE){
			//原音量
			waveOutGetVolume(0,&dwOrgVolume);
			dwOrgVolume = LOWORD(dwOrgVolume);

			//混合选择
			bf.BlendOp = AC_SRC_OVER;
			bf.BlendFlags =0;		
			bf.SourceConstantAlpha =255;
			bf.AlphaFormat =AC_SRC_ALPHA;
			//FULL PNG LOAD
			img_AllWater = ImagingHelper::GetImageObject(MzGetInstanceHandle(), IDB_PNG_AllWater, true);	
			//path
			SettingFileName = AppPath+L"app.ini";

			//创建主窗口
			m_MainWnd.Create(rcWork.left,rcWork.top,RECT_WIDTH(rcWork),RECT_HEIGHT(rcWork), NULL, NULL, NULL);
			m_MainWnd.AnimateWindow(MZ_ANIMTYPE_FADE,TRUE);
			m_MainWnd.Show();	
		}else{
			CopyRight dlg;
			dlg.Create(rcWork.left,rcWork.top,RECT_WIDTH(rcWork),RECT_HEIGHT(rcWork), NULL, NULL, WS_POPUP);
			// set the animation of the window
			dlg.SetAnimateType_Show(MZ_ANIMTYPE_FADE);
			dlg.SetAnimateType_Hide(MZ_ANIMTYPE_FADE);			
			dlg.DoModal();			
		}
		return TRUE;
	}
};

// 应用程序全局变量
MainApp theApp;


//最后处理线程
DWORD WINAPI FinalThread(LPVOID lpParam)  
{
	//禁止点击
	theApp.m_MainWnd.m_Toolbar.SetEnable(false);
	theApp.m_MainWnd.m_Toolbar.SetTextColor(MZCLR_TOOLBAR_TEXT_DISABLED);
	theApp.m_MainWnd.m_Toolbar.Invalidate();
	theApp.m_MainWnd.m_Toolbar.Update();
	theApp.m_MainWnd.but_skip.SetEnable(false);
	theApp.m_MainWnd.but_skip.SetTextColor(MZCLR_TOOLBAR_TEXT_DISABLED);
	theApp.m_MainWnd.but_skip.Invalidate();
	theApp.m_MainWnd.but_skip.Update();
	for (int i=0; i<but_all.size(); i++){
		but_all.at(i).SetEnable(false);
	}

	Sleep(1000);

	isInGame = false;	
		
	ChackFirst();
	vector<int> temp_chack_box;
	vector<int> sub_temp_chack_box;		
	DWORD dwThreadId[PIPE_ANIMO_MAX_THREADS];
	HANDLE hThread[PIPE_ANIMO_MAX_THREADS]; 

	//play water sound 
	if (isSoundsOn) SetVolume(dwVolume);
	if (isSoundsOn) PlaySound((LPCWSTR)IDR_WAVE_water_flow, MzGetInstanceHandle(), SND_LOOP | SND_RESOURCE | SND_ASYNC);

	PThreadData pData;
	while (!isErr && !now_chack_box.empty()){
		//填充动画线程
		int threads_count = now_chack_box.size();

		// Create PIPE_ANIMO_MAX_THREADS worker threads.
		for (int i=0; i<threads_count; i++)
		{
			int now = now_chack_box.at(i);
			AddScore(now);

			// Allocate memory for thread data.
			pData = (PThreadData) HeapAlloc(GetProcessHeap(), HEAP_ZERO_MEMORY,
				sizeof(ThreadData));
			if( pData == NULL )
				ExitProcess(2);

			// Generate unique data for each thread.
			pData->box_no = now;
			pData->box = &but_all.at(now);
			pData->box_static = &box_static.at(now);
			//wprintf(L"chack Score-%d,In-$s\t" ,pData->box_no, pData->box_static->in_lab.C_Str());
			hThread[i] = CreateThread(NULL,0,FullAnimoThread,pData,0,&dwThreadId[i]);

			// Check the return value for success.
			// If failure, close existing thread handles, 
			// free memory allocation, and exit. 
			if (hThread[i] == NULL) 
			{
				for(i=0; i<threads_count; i++)
				{
					if (hThread[i] != NULL)
					{
						CloseHandle(hThread[i]);
					}
				}
				HeapFree(GetProcessHeap(), 0, pData);
				ExitProcess(i);
			}
		}

		for(int i=0; i<threads_count; i++)
		{
			// Wait until all threads have terminated.
			WaitForSingleObject(hThread[i],INFINITE);
			// Close all thread handles and free memory allocation.
			CloseHandle(hThread[i]);
		}


		//set score lable
		CMzString str(10);
		wsprintf(str.C_Str(), L"分数:%d", Score);
		static_score.SetText(str.C_Str());
		static_score.Invalidate();
		static_score.Update();

		printf("\n------------NEXT-------------\n"); 
		//debug
		if (isdebug){
			wsprintf(debug_str.C_Str(), debug_str+L"\n---NEXT---\n");
			DrawText(hdc,debug_str.C_Str(),lstrlen(debug_str),&rcWork,0);
		}

		//查找下一步
		for (int i=0; i<now_chack_box.size(); i++){
			int now = now_chack_box.at(i);
			sub_temp_chack_box.clear();
			sub_temp_chack_box = GetNext(now);				
			if (!sub_temp_chack_box.empty()){
				temp_chack_box.insert(temp_chack_box.end(),sub_temp_chack_box.begin(),sub_temp_chack_box.end());
			}
		}
		//去重
		now_chack_box.clear();
		for (int l=0; l<temp_chack_box.size(); l++){
			bool insert = true;
			int temp = temp_chack_box.at(l);
			for (int m=0; m<now_chack_box.size(); m++){
				if (now_chack_box.at(m)==temp){
					insert = false;
					break;
				}
			}
			if (insert) now_chack_box.push_back(temp);
		}
		temp_chack_box.clear();
	}
	HeapFree(GetProcessHeap(), 0, pData);

	//stop water sound 
	if (isSoundsOn) PlaySound(NULL, NULL, NULL);
	if (isSoundsOn) SetVolume(dwOrgVolume);

	if (isErr){
		//play err sound
		if (isSoundsOn) SetVolume(dwVolume);
		if (isSoundsOn) PlaySound((LPCWSTR)IDR_WAVE_warning, MzGetInstanceHandle(), SND_LOOP | SND_RESOURCE | SND_ASYNC);

		ErrAnimo();

		//stop err sound 
		if (isSoundsOn) PlaySound(NULL, NULL, NULL);
		if (isSoundsOn) SetVolume(dwOrgVolume);
	}else{
		//加数奖励
		Score += 20;
		CMzString str(10);
		wsprintf(str.C_Str(), L"分数:%d", Score);
		static_score.SetText(str.C_Str());
		static_score.Invalidate();
		static_score.Update();
	}

	//允许点击
	theApp.m_MainWnd.m_Toolbar.SetEnable(true);
	theApp.m_MainWnd.m_Toolbar.SetTextColor(MZCLR_FONT_WHITE);
	theApp.m_MainWnd.m_Toolbar.Invalidate();
	theApp.m_MainWnd.m_Toolbar.Update();
	theApp.m_MainWnd.but_skip.SetEnable(true);
	theApp.m_MainWnd.but_skip.SetTextColor(MZCLR_FONT_WHITE);
	theApp.m_MainWnd.but_skip.SetText(L"开始");
	theApp.m_MainWnd.but_skip.Invalidate();
	theApp.m_MainWnd.but_skip.Update();

	//保存最后的样子
	mdc->Create(390,RECT_HEIGHT(rcWork)-MZM_HEIGHT_TEXT_TOOLBAR);
	if (org_isPreLeft){
		BitBlt(mdc->GetDC(), 0, 0, mdc->GetWidth(),mdc->GetHeight(), hdc, 90, 0, SRCCOPY);
	}else{
		BitBlt(mdc->GetDC(), 0, 0, mdc->GetWidth(),mdc->GetHeight(), hdc, 0, 0, SRCCOPY);
	}


	isInRepain = true;

	//进入排行中
	int rankno = ranking.getRankingNo(Score);
	if (rankno != -1){
		printf("\nIn Ranking\n"); 
		//保存记录样子
		MemoryDC* save_mdc = new MemoryDC();
		save_mdc->Create(RECT_WIDTH(rcWork),RECT_HEIGHT(rcWork)-MZM_HEIGHT_TEXT_TOOLBAR);
		BitBlt(save_mdc->GetDC(), 0, 0, save_mdc->GetWidth(),save_mdc->GetHeight(), hdc, 0, 0, SRCCOPY);

		RankingNameEdit rne_dlg;		
		rne_dlg.name = ranking_last_name;
		rne_dlg.score = Score;
		rne_dlg.rankno = rankno;
		rne_dlg.Create(rcWork.left,rcWork.top,RECT_WIDTH(rcWork),RECT_HEIGHT(rcWork), hWnd, 0, WS_POPUP);
		rne_dlg.SetAnimateType_Show(MZ_ANIMTYPE_FADE);
		rne_dlg.SetAnimateType_Hide(MZ_ANIMTYPE_NONE);
		int nRet = rne_dlg.DoModal();
		if (nRet==ID_OK){
			MzBeginWaitDlg(hWnd,NULL,TRUE);

			ranking_last_name = rne_dlg.name;
			ranking.insertRanking(rankno,ranking_last_name,Score);
			ranking.m_List.SetSelectedIndex(-1);
			//save ranking			
			ranking.SaveList(rankno, RankingName, SettingFileName);
			//save bmp
			CMzString filename(128);
			wchar_t *buf = AppPath+L"ranking\\rank_%d.bmp";
			wsprintf(filename.C_Str(), buf, rankno);
			Common::SaveBmp(save_mdc,&filename);			

			MzEndWaitDlg();
			Ranking rank_dlg;
			rank_dlg.m_List=ranking.m_List;
			rank_dlg.highlight = rankno;
			rank_dlg.showNew = true;
			rank_dlg.Create(rcWork.left,rcWork.top,RECT_WIDTH(rcWork),RECT_HEIGHT(rcWork), hWnd, 0, WS_POPUP);
			rank_dlg.SetAnimateType_Show(MZ_ANIMTYPE_NONE);
			rank_dlg.SetAnimateType_Hide(MZ_ANIMTYPE_FADE);			
			nRet = rank_dlg.DoModal();
			if (nRet==ID_OK){
				theApp.m_MainWnd.NewGame();
				return 0;
			}
		}
	}
	return 0;
}









