#pragma once

#include <mzfc_inc.h>
#include <TouchNotifyApi.h>
#include <ShellNotifyMsg.h>
#include <pm.h>
#include "ModeSelecter.h"

#define MZ_IDC_BUT_RESET  101
#define MZ_IDC_BUT_EXIT  102
#define MZ_IDC_BUT_STOP  103
#define MZ_IDC_BUT_SELECT_MODE  104
#define MZ_IDC_BUT_HIGHT_SCORE  105
#define MZ_TOUCH         110

#define GAME_ONE_SKIP_UP_DOWN_MIN_LEG  130     //前后脚步伐至少差距多少，算一步；
#define GAME_ONE_SKIP_MIN_LEG  60     //步伐至少走多少，算一步；
#define GAME_TWO_SKIP_BETWEEN_MAX_LEG  50     //两次记点时至多差距多少，算在连续的一步；
#define GAME_SPEED_BETWEEN_TIME  800     //即时速度的计算时间间隔

#define GAME_MAX_TIME  30     //计时

#define GK_MAX 10      //总关卡数
#define GK_TIME_1 150  //关卡1的总时间
#define GK_LENG_1 150  //关卡1的总距离
#define GK_TIME_2 150  //关卡2的总时间
#define GK_LENG_2 150  //关卡2的总距离
#define GK_TIME_3 150  //关卡3的总时间
#define GK_LENG_3 150  //关卡3的总距离
#define GK_TIME_4 150  //关卡4的总时间
#define GK_LENG_4 150  //关卡4的总距离
#define GK_TIME_5 150  //关卡5的总时间
#define GK_LENG_5 150  //关卡5的总距离
#define GK_TIME_6 150  //关卡6的总时间
#define GK_LENG_6 150  //关卡6的总距离
#define GK_TIME_7 150  //关卡7的总时间
#define GK_LENG_7 150  //关卡7的总距离
#define GK_TIME_8 150  //关卡8的总时间
#define GK_LENG_8 150  //关卡8的总距离
#define GK_TIME_9 150  //关卡9的总时间
#define GK_LENG_9 150  //关卡9的总距离
#define GK_TIME_10 150  //关卡10的总时间
#define GK_LENG_10 150  //关卡10的总距离

typedef struct _GKData 
{
	CMzStringW name;				//关卡的名字
    int time;						//关卡的总时间
    int leng;						//关卡的总距离
} GKData, *PGKData;


class FingerMill
{
public:

	FingerMill(void);
	~FingerMill(void);
};
