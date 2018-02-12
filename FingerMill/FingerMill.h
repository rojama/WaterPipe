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

#define GAME_ONE_SKIP_UP_DOWN_MIN_LEG  130     //ǰ��Ų������ٲ����٣���һ����
#define GAME_ONE_SKIP_MIN_LEG  60     //���������߶��٣���һ����
#define GAME_TWO_SKIP_BETWEEN_MAX_LEG  50     //���μǵ�ʱ��������٣�����������һ����
#define GAME_SPEED_BETWEEN_TIME  800     //��ʱ�ٶȵļ���ʱ����

#define GAME_MAX_TIME  30     //��ʱ

#define GK_MAX 10      //�ܹؿ���
#define GK_TIME_1 150  //�ؿ�1����ʱ��
#define GK_LENG_1 150  //�ؿ�1���ܾ���
#define GK_TIME_2 150  //�ؿ�2����ʱ��
#define GK_LENG_2 150  //�ؿ�2���ܾ���
#define GK_TIME_3 150  //�ؿ�3����ʱ��
#define GK_LENG_3 150  //�ؿ�3���ܾ���
#define GK_TIME_4 150  //�ؿ�4����ʱ��
#define GK_LENG_4 150  //�ؿ�4���ܾ���
#define GK_TIME_5 150  //�ؿ�5����ʱ��
#define GK_LENG_5 150  //�ؿ�5���ܾ���
#define GK_TIME_6 150  //�ؿ�6����ʱ��
#define GK_LENG_6 150  //�ؿ�6���ܾ���
#define GK_TIME_7 150  //�ؿ�7����ʱ��
#define GK_LENG_7 150  //�ؿ�7���ܾ���
#define GK_TIME_8 150  //�ؿ�8����ʱ��
#define GK_LENG_8 150  //�ؿ�8���ܾ���
#define GK_TIME_9 150  //�ؿ�9����ʱ��
#define GK_LENG_9 150  //�ؿ�9���ܾ���
#define GK_TIME_10 150  //�ؿ�10����ʱ��
#define GK_LENG_10 150  //�ؿ�10���ܾ���

typedef struct _GKData 
{
	CMzStringW name;				//�ؿ�������
    int time;						//�ؿ�����ʱ��
    int leng;						//�ؿ����ܾ���
} GKData, *PGKData;


class FingerMill
{
public:

	FingerMill(void);
	~FingerMill(void);
};
