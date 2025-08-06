using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSP_88D_CS.Sequence
{
    public enum eErrCode
    {
        // DI Error *********************************************************	
        Ecode_I_EmgSwFront = 4000,// 0
        Ecode_I_EmgSwRear = 4001,// 1
        Ecode_I_DoorFrontLeft = 4002,// 2
        Ecode_I_DoorFrontCenter = 4003,// 3
        Ecode_I_DoorFrontRight = 4004,// 4
        Ecode_I_DoorLeft = 4005,// 5
        Ecode_I_DoorRight = 4006,// 6
        Ecode_I_PumpOvld_Pm1 = 4007,// 7
        Ecode_I_MainAirPressure = 4008,// 8
        Ecode_I_Gas1Pressure = 4009,// 9
        Ecode_I_Gas2Pressure = 4010,// 10
        Ecode_I_VacValveOpen_Pm1,// 11
        Ecode_I_ECoolOn_Pm1,// 12
        Ecode_I_MCoolOn_Pm1,// 13
        Ecode_I_ChamberUp_Pm1,// 14
        Ecode_I_ChamberDn_Pm1,// 15
        Ecode_I_PumpOilLevel_Pm1,// 16
        Ecode_I_LoadElevMgzChk_1,// 17
        Ecode_I_LoadElevMgzChk_2,// 18
        Ecode_I_LoadElevMgzChk_3,// 19
        Ecode_I_LoadElevMgzChk_4,// 20
        Ecode_I_Pusher1Overload,// 21
        Ecode_I_LoadElevStripChk_1,// 22
        Ecode_I_LoadElevStripChk_2,// 23
        Ecode_I_LoadElevStripChk_3,// 24
        Ecode_I_LoadElevStripChk_4,// 25
        Ecode_I_InBufStopperUp,// 26
        Ecode_I_InBufStopperDn,// 27
        Ecode_I_InBufDevArrival_1,// 28
        Ecode_I_InBufDevArrival_2,// 29
        Ecode_I_InBufDevArrival_3,// 30
        Ecode_I_InBufDevArrival_4,// 31
        Ecode_I_InBufCylFwd,// 32
        Ecode_I_InBufCylBwd,// 33
        Ecode_I_Pusher2Overload,// 34
        Ecode_I_Pusher2StripChk_1,// 35
        Ecode_I_Pusher2StripChk_2,// 36
        Ecode_I_Pusher2StripChk_3,// 37
        Ecode_I_Pusher2StripChk_4,// 38
        Ecode_I_Pusher2Up,// 39
        Ecode_I_Pusher2Dn,// 40
        Ecode_I_OutBufFwd,// 41
        Ecode_I_OutBufBwd,// 42
        Ecode_I_ChamberOutStripChk_1,// 43
        Ecode_I_ChamberOutStripChk_2,// 44
        Ecode_I_ChamberOutStripChk_3,// 45
        Ecode_I_ChamberOutStripChk_4,// 46
        Ecode_I_Pusher3Overload,// 47
        Ecode_I_Pusher3Up,// 48
        Ecode_I_Pusher3Dn,// 49
        Ecode_I_Pusher3Fwd,// 50
        Ecode_I_Pusher3Bwd,// 51
        Ecode_I_Pusher3DnEnable_1,// 52
        Ecode_I_Pusher3DnEnable_2,// 53
        Ecode_I_Pusher3DnEnable_3,// 54
        Ecode_I_Pusher3DnEnable_4,// 55
        Ecode_I_UnLoadElevStripChk_1,// 56
        Ecode_I_UnLoadElevStripChk_2,// 57
        Ecode_I_UnLoadElevStripChk_3,// 58
        Ecode_I_UnLoadElevStripChk_4,// 59
        Ecode_I_UnLoadElevMgzChk_1,// 60
        Ecode_I_UnLoadElevMgzChk_2,// 61
        Ecode_I_UnLoadElevMgzChk_3,// 62
        Ecode_I_UnLoadElevMgzChk_4,// 63
        Ecode_I_VisionOut,// 64
        Ecode_I_VisionEnable,// 65
        Ecode_I_VisionError,// 66
        Ecode_I_Gas3Pressure,// 67
        Ecode_I_Gas4Pressure,// 68
        Ecode_I_Pusher2Overload2,// 69
        Ecode_I_Pusher2Overload3,// 70
        Ecode_I_Pusher2Overload4,// 71
        Ecode_I_LoadElevMgzChk_5,// 72
        Ecode_I_LoadElevStripChk_5,// 73
        Ecode_I_InBufDevArrival_5,// 74
        Ecode_I_Pusher2StripChk_5,// 75
        Ecode_I_ChamberOutStripChk_5,// 76
        Ecode_I_Pusher3DnEnable_5,// 77
        Ecode_I_UnLoadElevStripChk_5,// 78
        Ecode_I_UnLoadElevMgzChk_5,// 79
        Ecode_I_LoadMgzTiltChk_1,// 80
        Ecode_I_LoadMgzTiltChk_2,// 81
        Ecode_I_LoadMgzTiltChk_3,// 82
        Ecode_I_LoadMgzTiltChk_4,// 83
        Ecode_I_UnldMgzTiltChk_1,// 84
        Ecode_I_UnldMgzTiltChk_2,// 85
        Ecode_I_UnldMgzTiltChk_3,// 86
        Ecode_I_UnldMgzTiltChk_4,// 87
        Ecode_I_IonizerAlarm,// 88
        Ecode_I_LaneFloatDetect,// 89
        Ecode_I_Pusher1Overload2,// 90
        Ecode_I_Pusher1Overload3,// 91
        Ecode_I_Pusher1Overload4,// 92
        Ecode_I_Pusher1Overload5,// 93
        Ecode_I_Pusher2Overload5,// 94
        Ecode_I_ChamberInEdge,// 95
        Ecode_I_ChamberOutEdge,// 96
        Ecode_I_Pusher3Overload2,// 97
        Ecode_I_Pusher3Overload3,// 98
        Ecode_I_Pusher3Overload4,// 99
        Ecode_I_Pusher3Overload5,// 100
        Ecode_I_N2Pressure_CJ,// 101
        Ecode_I_LoadElevMgzTiltCheck,// 102
        Ecode_I_UnldElevMgzTiltCheck,// 103
        Ecode_I_IonizerAlarm2,// 104
        Ecode_I_X069,// 105
        Ecode_I_X06A,// 106
        Ecode_I_X06B,// 107
        Ecode_I_X06C,// 108
        Ecode_I_X06D,// 109
        Ecode_I_X06E,// 110
        Ecode_I_X06F,// 111

        // Unlisted Error ****************************************************	
        Ecode_LdPushNotRetPos = 200,// IO에는 없음
        Ecode_ElevMoving,// 201
        Ecode_GetUnldIdxRngOver,// 202
        Ecode_GetUnldIdxErr,// 203
        Ecode_ElevEmpty,// 204
        Ecode_TopMgzEmpty,// 205
        Ecode_BtmMgzEmpty,// 206
        Ecode_BridgeNotUpPos,// 207
        Ecode_BridgeNotDownPos,// 208
        Ecode_IdxPushNotRetPos,// 209
        Ecode_IdxPusherNotUpPos,// 210
        Ecode_IndexPosNotBridgeUpAllow,// 211
        Ecode_IdxPusherNotDownPos,// 212
        Ecode_IndexPosInterlockWithElev,// 213
        Ecode_GripDetectInCorrect_1,// 214
        Ecode_GripDetectInCorrect_2,// 215
        Ecode_GripDetectInCorrect_3,// 216
        Ecode_GripDetectInCorrect_4,// 217
        Ecode_GripDetectInCorrect_5,// 218
        Ecode_ChamberNotUpPos,// 219
        Ecode_NeedOilChange,// 220
        Ecode_BcrReadTimeOut,// 221
        Ecode_IdxPushUnldPosErr,// 222
        Ecode_BcrNotUpPos,// 223

        // AIO Error *********************************************************	
        Ecode_Vacuum_TimeOver_Pm1 = 400,// 400 nVacuumTimeOut A
        Ecode_RfOn_TimeOver_Pm1,// 401 nRfOnTimeOut
        Ecode_RfOn_Error_Pm1,// 402
        Ecode_Gas1_TimeOver_Pm1,// 403 nGas1TimeOut A
        Ecode_Gas2_TimeOver_Pm1,// 404 nGas2TimeOut A
        Ecode_Gas3_TimeOver_Pm1,// 405 nGas3TimeOut A
        Ecode_Gas4_TimeOver_Pm1,// 406 nGas4TimeOut A
        Ecode_OverPressure_Pm1,// 407 Over Pressure during cleaning process
        Ecode_RfGenCommError_Pm1,// 408
        Ecode_VacGaugeFault_Pm1,// 409 진공 게이지 값이 설정값 미만
        Ecode_VacGaugeOpen_Pm1,// 410 진공 게이지 밸브 열림 신호 미감지

        // RF Generator Error *********************************************************	
        Ecode_OverTemp = 450,//
        Ecode_OverHeat,// 451
        Ecode_CurrentHigh,// 452
        Ecode_ForwardHigh,// 453
        Ecode_ReflectHigh,// 454
        Ecode_ExternalInterlock,// 455
        Ecode_InterlockSw,// 456
        Ecode_FanOffFaultInterlock,// 457
        Ecode_OverHeat_ExternalInterlock,// 458
        Ecode_CurrentHigh_SwInterlock,// 459

        // MOTION Error ******************************************************	
        Ecode_LoadElevZ_NotHome = 800,// 800=======================
        Ecode_LoadElevZ_AmpAlarm,// 801
        Ecode_LoadElevZ_SoftMinLimit,// 802
        Ecode_LoadElevZ_SoftMaxLimit,// 803
        Ecode_LoadElevZ_MinLimit,// 804
        Ecode_LoadElevZ_MaxLimit,// 805
        Ecode_LoadElevZ_HomeStartFail,// 806
        Ecode_LoadElevZ_HomeFail,// 807
        Ecode_LoadElevZ_TimeOut,// 808
        Ecode_LoadElevZ_StartFail,// 809

        Ecode_LoadPush_X_NotHome = 820,// 820=======================
        Ecode_LoadPush_X_AmpAlarm,// 821
        Ecode_LoadPush_X_SoftMinLimit,// 822
        Ecode_LoadPush_X_SoftMaxLimit,// 823
        Ecode_LoadPush_X_MinLimit,// 824
        Ecode_LoadPush_X_MaxLimit,// 825
        Ecode_LoadPush_X_HomeStartFail,// 826
        Ecode_LoadPush_X_HomeFail,// 827
        Ecode_LoadPush_X_TimeOut,// 828
        Ecode_LoadPush_X_StartFail,// 829

        Ecode_IndexPush_X_NotHome = 840,// 840 =======================
        Ecode_IndexPush_X_AmpAlarm,// 841
        Ecode_IndexPush_X_SoftMinLimit,// 842
        Ecode_IndexPush_X_SoftMaxLimit,// 843
        Ecode_IndexPush_X_MinLimit,// 844
        Ecode_IndexPush_X_MaxLimit,// 845
        Ecode_IndexPush_X_HomeStartFail,// 846
        Ecode_IndexPush_X_HomeFail,// 847
        Ecode_IndexPush_X_TimeOut,// 848
        Ecode_IndexPush_X_StartFail,// 849

        Ecode_UnldElevZ_Z_NotHome = 860,// 860 =======================
        Ecode_UnldElevZ_Z_AmpAlarm,// 861
        Ecode_UnldElevZ_Z_SoftMinLimit,// 862
        Ecode_UnldElevZ_Z_SoftMaxLimit,// 863
        Ecode_UnldElevZ_Z_MinLimit,// 864
        Ecode_UnldElevZ_Z_MaxLimit,// 865
        Ecode_UnldElevZ_Z_HomeStartFail,// 866
        Ecode_UnldElevZ_Z_HomeFail,// 867
        Ecode_UnldElevZ_Z_TimeOut,// 868
        Ecode_UnldElevZ_Z_StartFail,// 869

        Ecode_Inspect_Y_NotHome = 880,// 880 =======================
        Ecode_Inspect_Y_AmpAlarm,// 881
        Ecode_Inspect_Y_SoftMinLimit,// 882
        Ecode_Inspect_Y_SoftMaxLimit,// 883
        Ecode_Inspect_Y_MinLimit,// 884
        Ecode_Inspect_Y_MaxLimit,// 885
        Ecode_Inspect_Y_HomeStartFail,// 886
        Ecode_Inspect_Y_HomeFail,// 887
        Ecode_Inspect_Y_TimeOut,// 888
        Ecode_Inspect_Y_StartFail,// 889

    }
}
