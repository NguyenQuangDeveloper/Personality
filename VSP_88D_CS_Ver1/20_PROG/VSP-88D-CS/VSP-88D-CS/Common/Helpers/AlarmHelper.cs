using AlarmConfig.Views.Lefts;
using ChamberControl.Views;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Windows.Media.Media3D;
using VSLibrary.Common.MVVM.Models;
using VSP_88D_CS.Sequence;
namespace VSP_88D_CS.Common.Helpers;

public class AlarmHelper
{
    public static ObservableCollection<ErrorItem> DefaultAlarms()
    {
        var alarms = new ObservableCollection<ErrorItem>();

        alarms.Add(CreateAlarm(
                        eErrCode.Ecode_I_EmgSwFront,
                        "전면 비상정지 스위치",
                        "전면 비상정지 스위치 눌림",
                        "전면 비상 정지 스위치를 해제후 작업해 주십시요. 비상 해제 후 반드시 초기화를 수행해 주십시오.",

                        "FRONT EMERGENCY SWITCH ERROR",
                        "The front-emergency switch is pushed.",
                        "Release the front-emergency switch. Initialize program!!!",

                        "前部紧急停止开关",
                        "前部紧急停止开关被按下",
                        "请释放前部按下的紧急停止开关。 释放后务必执行初始化。",

                        "// TODO: Vietnamese Title",
                        "// TODO: Vietnamese Description",
                        "// TODO: Vietnamese Guide"
                ));

        alarms.Add(CreateAlarm(
                        eErrCode.Ecode_I_EmgSwRear,
                        "뒤쪽 비상정지 스위치",
                        "뒤쪽 비상정지 스위치 눌림",
                        "뒤쪽 비상 정지 스위치를 해제후 작업해 주십시요. 비상 해제 후 반드시 초기화를 수행해 주십시오",

                        "REAR EMERGENCY SWITCH ERROR",
                        "The rear-emergency switch is pushed.",
                        "Release the rear-emergency switch. Initialize program!!!",

                        "后部紧急停止开关",
                        "后部紧急停止开关被按下",
                        "请释放后部按下的紧急停止开关。 释放后务必执行初始化。",

                        "// TODO: Vietnamese Title",
                        "// TODO: Vietnamese Description",
                        "// TODO: Vietnamese Guide"
                ));

        alarms.Add(CreateAlarm(
                        eErrCode.Ecode_I_DoorFrontLeft,
                        "전면 왼쪽 문 열림",
                        "전면 왼쪽 문 닫힘 감지 센서가 동작하지 않습니다.",
                        "전면 왼쪽 문을 닫고 작업해 주십시오.",

                        "FRONT LEFT DOOR OPEN",
                        "The front-left door is open.",
                        "Start after closing door.",

                        "左前门打开",
                        "左前门关闭传感器没有感应到。",
                        "请关闭左前门",

                        "// TODO: Vietnamese Title",
                        "// TODO: Vietnamese Description",
                        "// TODO: Vietnamese Guide"
                ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_DoorFrontCenter,
                                "전면 가운데 문 열림",
                                "전면 가운데 문 닫힘 감지 센서가 동작하지 않습니다.",
                                "전면 가운데 문을 닫고 작업해 주십시오.",

                                "FRONT CENTER DOOR OPEN",
                                "The front-center door is open.",
                                "Start after closing door.",

                                "前部中门打开",
                                "前部中门关闭传感器没有感应到",
                                "请关闭前部中门",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_DoorFrontRight,
                                "전면 오른쪽 문 열림",
                                "전면 오른쪽 문 닫힘 감지 센서가 동작하지 않습니다.",
                                "전면 오른쪽 문을 닫고 작업해 주십시오.",

                                "FRONT RIGHT DOOR OPEN",
                                "The front-right door is open.",
                                "Start after closing door.",

                                "右前门打开",
                                "右前门关闭传感器没有感应到。",
                                "请关闭右前门",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_DoorLeft,
                                "왼쪽 문 열림",
                                "왼쪽 문 닫힘 감지 센서가 동작하지 않습니다.",
                                "왼쪽 문을 닫고 작업해 주십시오.",

                                "LEFT DOOR OPEN",
                                "The left door is open.",
                                "Start after closing door.",

                                "左边门打开",
                                "左边门关闭传感器没有感应到。",
                                "请关闭左边门",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_DoorRight,
                                "오른쪽 문 열림",
                                "오른쪽 문 닫힘 감지 센서가 동작하지 않습니다.",
                                "오른쪽 문을 닫고 작업해 주십시오.",

                                "RIGHT DOOR OPNE",
                                "The right door is open.",
                                "Start after closing door.",

                                "右边门打开",
                                "右边门关闭传感器没有感应到。",
                                "请关闭右边门",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_PumpOvld_Pm1,
                                "펌프 오버 로드",
                                "진공 펌프 오버로드 신호가 감지되었습니다.",
                                "진공펌프를 확인해 주십시오. 모터보호계전기를 확인해 주십시오.",

                                "PUMP OVER-LOAD",
                                "The overload signal detected.",
                                "Check the vacuum pump.",

                                "泵过载",
                                "感应到泵过载信号",
                                "请确认真空泵和电机保护继电器",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_MainAirPressure,
                                "메인 공기압 낮음",
                                "메인 공기압 신호를 감지하지 못했습니다.",
                                "메인 공압이 연결되었는지 확인해 주십시오. 공압 입력 스위치를 확인해 주십시오.",
                                "MAIN AIR PRESSURE",
                                "Main air pressure signal was not  detected.",
                                "Check main air inlet.",
                                "主气压低",
                                "无法感应到主气压信号",
                                "请确认是否连接主气压",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_Gas1Pressure,
                                "가스1 공급압력 낮음",
                                "가스1 압력 신호를 감지하지 못했습니다.",
                                "가스1 공급여부를 확인해 주십시오. 가스1 압력체크 센서를 점검해 주십시오.",
                                "GAS1 PRESSURE LOW",
                                "The pressure of gas1 is low",
                                "Check the inlet of gas1. Check the gas1 pressure sensor.",
                                "气体1供应压力低",
                                "无法感应到气体1的压力信号",
                                "请确认是否连接气体1",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_Gas2Pressure,
                                "가스2 공급압력 낮음",
                                "가스2 압력 신호를 감지하지 못했습니다.",
                                "가스2 공급여부를 확인해 주십시오. 가스2압력체크 센서를 점검해 주십시오.",

                                "GAS2 PRESSURE LOW",
                                "The pressure of gas2 is low",
                                "Check the inlet of gas2. Check the gas2 pressure sensor.",

                                "气体2供应压力低",
                                "无法感应到气体2的压力信号",
                                "请确认是否连接气体2",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_VacValveOpen_Pm1,
                                "진공 밸브 열림 에러",
                                "진공밸브 열림 센서가 감지되지 않았습니다.",
                                "진공밸브 솔레노이드를 확인해 주십시오. 진공밸브 열림 감지센서를 확인해 주십시오.",

                                "VACUUM VALVE",
                                "The vacuum valve open sensor did not detect.",
                                "Check the solenoid of vacuum valve open. Check vacuum valve open sensor.",

                                "真空阀打开报警",
                                "无法感应到真空阀打开信号",
                                "请检查真空阀电磁阀。 检查真空阀打开传感器。",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_ECoolOn_Pm1,
                                "전극 냉각 공압 스위치 에러",
                                "전극 냉각 공압 스위치 신호가 감지되지 않았습니다.",
                                "메인 공압이 공급되는지 확인해 주십시오. 전극 냉각 공압 스위치를 점검해 주십시오.",
                                "ELECTRODE COOLING SWITCH ERROR",
                                "The electrode cooling pressure signal was not detected.",
                                "Check the inlet of main air. Check the electrode cooling switch.",
                                "电极冷却气开关报警",
                                "无法感应到电极冷却气开关信号",
                                "检查是否提供了主气压。 检查电极冷却气开关。",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_MCoolOn_Pm1,
                                "매칭박스 냉각 공압 스위치 에러",
                                "매칭박스 냉각 공압 스위치 신호가 감지되지 않았습니다.",
                                "메인 공압이 공급되는지 확인해 주십시오. 매칭박스 냉각 공압 스위치를 점검해 주십시오.",
                                "MATCHER COOLING SWITCH ERROR",
                                "The matcher cooling pressure signal was not detected.",
                                "Check the inlet of main air. Check the matcher cooling switch.",
                                "匹配箱冷却器开关报警",
                                "无法感应到匹配箱冷却气开关信号",
                                "检查是否提供了主气压。 检查匹配箱冷却气开关。",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_ChamberUp_Pm1,
                                "챔버 업 에러",
                                "챔버 업 센서가 감지되지 않았습니다.",
                                "메인 공압이 공급되는지 확인해 주십시오. 챔버 업 센서를 확인해 주십시오.",
                                "CHAMBER UP ERROR",
                                "The upper sensor of chamber was not detected.",
                                "Check main air inlet. Check the position of the chamber upper sensor.",
                                "腔体打开报警",
                                "腔体打开传感器没有感应到",
                                "检查是否提供了主气压。 检查腔体打开时传感器",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_ChamberDn_Pm1,
                                "챔버 다운 에러",
                                "챔버 다운 센서가 감지되지 않았습니다.",
                                "메인 공압이 공급되는지 확인해 주십시오. 챔버 다운 센서를 확인해 주십시오.",
                                "CHAMBER DOWN ERROR",
                                "The lower sensor of chamber was not detected.",
                                "Check main air inlet. Check the position of the chamber lower sensor.",
                                "腔体关闭报警",
                                "腔体关闭传感器没有感应到",
                                "检查是否提供了主气压。 检查腔体关闭时传感器",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_PumpOilLevel_Pm1,
                                "펌프 오일 에러",
                                "진공 펌프 오일 센서가 감지되지 않았습니다.",
                                "펌프 오일을 점검해 주십시오.",
                                "PUMP OIL LOW",
                                "The pump oil check signal did not detected.",
                                "Check the oil level of the vacuum pump.",
                                "泵油报警",
                                "油位传感器无法感应到泵油",
                                "请检查泵油",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_LoadElevMgzChk_1,
                                "로딩 엘리베이터 메거진 감지 센서 #1 에러",
                                "로딩 엘리베이터 메거진 감지 센서 #1가 감지되지 않았습니다.",
                                "로딩 엘리베이터의 메거진#1을 점검해 주십시오.",
                                "LOADING ELEVATOR MGZ. CHECK #1 ERROR",
                                "The on-elevator 1st lane magazine detect sensor signal was not detected.",
                                "Check the magazine existance at 1st lane of on-elevator.",
                                "上料端料盒感应传感器#1报警",
                                "上料端料盒感应传感器#1没有感应到",
                                "请检查料盒#1",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_LoadElevMgzChk_2,
                                "로딩 엘리베이터 메거진 감지 센서 #2 에러",
                                "로딩 엘리베이터 메거진 감지 센서 #2가 감지되지 않았습니다.",
                                "로딩 엘리베이터의 메거진#2을 점검해 주십시오.",
                                "LOADING ELEVATOR MGZ. CHECK #2 ERROR",
                                "The on-elevator 2nd lane magazine detect sensor signal was not detected.",
                                "Check the magazine existance at 2nd lane of on-elevator.",
                                "上料端料盒感应传感器#2报警",
                                "上料端料盒感应传感器#2没有感应到",
                                "请检查料盒#2",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_LoadElevMgzChk_3,
                                "로딩 엘리베이터 메거진 감지 센서 #3 에러",
                                "로딩 엘리베이터 메거진 감지 센서 #3가 감지되지 않았습니다.",
                                "로딩 엘리베이터의 메거진#3을 점검해 주십시오.",
                                "LOADING ELEVATOR MGZ. CHECK #3 ERROR",
                                "The on-elevator 3rd lane magazine detect sensor signal was not detected.",
                                "Check the magazine existance at 3rd lane of on-elevator.",
                                "上料端料盒感应传感器#3报警",
                                "上料端料盒感应传感器#3没有感应到",
                                "请检查料盒#3",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_LoadElevMgzChk_4,
                                "로딩 엘리베이터 메거진 감지 센서 #4 에러",
                                "로딩 엘리베이터 메거진 감지 센서 #4가 감지되지 않았습니다.",
                                "로딩 엘리베이터의 메거진#4을 점검해 주십시오.",

                                "LOADING ELEVATOR MGZ. CHECK #4 ERROR",
                                "The on-elevator 4th lane magazine detect sensor signal was not detected.",
                                "Check the magazine existance at 4th lane of on-elevator.",

                                "上料端料盒感应传感器#4报警",
                                "上料端料盒感应传感器#4没有感应到",
                                "请检查料盒#4",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_Pusher1Overload,
                                "로딩 푸셔 오버로드",
                                "로딩 푸셔 오버로드 센서가 감지되었습니다.",
                                "로딩 푸셔에 부하가 걸렸는지 확인해 주십시오. 부하의 원인을 제거 후 운전해 주십시오.",

                                "LOADING PUSHER OVERLOAD",
                                "The loading pusher overload sensor signal was detected.",
                                "Check the load of loading pusher. Remove the obstacle if exists.",

                                "上料推杆过载",
                                "上料推杆过载传感器感应到了",
                                "请检查上料推杆是否处于负载状态。 消除负载的原因后再运行。",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_LoadElevStripChk_1,
                                "로딩 엘리베이터 자재 걸림 센서 #1 에러",
                                "로딩 엘리베이터 자재 걸림 센서 #1이 감지되었습니다.",
                                "로딩 엘리베이터 #1 출구를 점검해 주십시오. 출구의 자재를 제거 후 작업해 주십시오.",
                                "LOADING ELEVATOR STRIP CHECK #1 ERROR",
                                "The on-elevator 1st lane out device check sensor signal was detected.",
                                "Check the outlet of on-elevator 1st lane.",
                                "上料升降机传感器#1卡料报警",
                                "上料升降机传感器#1感应到了",
                                "请检查上料升降机#1出口，去除出口的产品后继续作业",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_LoadElevStripChk_2,
                                "로딩 엘리베이터 자재 걸림 센서 #2 에러",
                                "로딩 엘리베이터 자재 걸림 센서 #2이 감지되었습니다.",
                                "로딩 엘리베이터 #2 출구를 점검해 주십시오. 출구의 자재를 제거 후 작업해 주십시오.",
                                "LOADING ELEVATOR STRIP CHECK #2 ERROR",
                                "The on-elevator 2nd lane out device check sensor signal was detected.",
                                "Check the outlet of on-elevator 2nd lane.",
                                "上料升降机传感器#2卡料报警",
                                "上料升降机传感器#2感应到了",
                                "请检查上料升降机#2出口，去除出口的产品后继续作业",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_LoadElevStripChk_3,
                                "로딩 엘리베이터 자재 걸림 센서 #3 에러",
                                "로딩 엘리베이터 자재 걸림 센서 #3이 감지되었습니다.",
                                "로딩 엘리베이터 #3 출구를 점검해 주십시오. 출구의 자재를 제거 후 작업해 주십시오.",
                                "LOADING ELEVATOR STRIP CHECK #3 ERROR",
                                "The on-elevator 3rd lane out device check sensor signal was detected.",
                                "Check the outlet of on-elevator 3rd lane.",
                                "上料升降机传感器#3卡料报警",
                                "上料升降机传感器#3感应到了",
                                "请检查上料升降机#3出口，去除出口的产品后继续作业",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_LoadElevStripChk_4,
                                "로딩 엘리베이터 자재 걸림 센서 #4 에러",
                                "로딩 엘리베이터 자재 걸림 센서 #4이 감지되었습니다.",
                                "로딩 엘리베이터 #4 출구를 점검해 주십시오. 출구의 자재를 제거 후 작업해 주십시오.",
                                "LOADING ELEVATOR STRIP CHECK #4 ERROR",
                                "The on-elevator 4th lane out device check sensor signal was detected.",
                                "Check the outlet of on-elevator 4th lane.",
                                "上料升降机传感器#4卡料报警",
                                "上料升降机传感器#4感应到了",
                                "请检查上料升降机#4出口，去除出口的产品后继续作业",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_InBufStopperUp,
                                "버퍼 스토퍼 업 에러",
                                "버퍼 스토퍼 업 센서가 감지되지 않았습니다.",
                                "메인 공압을 확인해 주십시오. 로딩 버퍼 스토퍼 업 센서의 위치를 확인해 주십시오.",
                                "LOADING STOPPER UP ERROR",
                                "The upper sensor of buffer stopper was not detected.",
                                "Check main air inlet. Check the position of the buffer stopper upper sensor.",
                                "缓冲挡块上升报警",
                                "缓冲挡块上升传感器没有感应到",
                                "请检查主气压，以及传感器的位置",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_InBufStopperDn,
                                "버퍼 스토퍼 다운 에러",
                                "버퍼 스토퍼 다운 센서가 감지되지 않았습니다.",
                                "메인 공압을 확인해 주십시오. 로딩 버퍼 스토퍼 다운 센서의 위치를 확인해 주십시오.",
                                "LOADING STOPPER DOWN ERROR",
                                "The lower sensor of buffer stopper was not detected.",
                                "Check main air inlet. Check the position of the lower stopper upper sensor.",
                                "缓冲挡块下降报警",
                                "缓冲挡块下降传感器没有感应到",
                                "请检查主气压，以及传感器的位置",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_InBufDevArrival_1,
                                "로딩 버퍼 자재 도착 감지 센서 #1 에러",
                                "로딩 버퍼 #1 자재 도착 감지 센서가 동작하지 않습니다.",
                                "로딩 버퍼 자재 도착 감지 센서 #1을 확인해 주십시오.",
                                "LAODING BUFFER DEVICE ARRIVAL CHECK SENSOR #1 ERROR",
                                "The loading buffer 1st lane device arrival sensor was not detected.",
                                "Check the loading buffer 1st lane device arrival check sensor.",
                                "上料轨道产品感应传感器#1报警",
                                "上料轨道#1产品到达传感器没有工作",
                                "请检查上料轨道产品到达传感器#1",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_InBufDevArrival_2,
                                "로딩 버퍼 자재 도착 감지 센서 #2 에러",
                                "로딩 버퍼 #2 자재 도착 감지 센서가 동작하지 않습니다.",
                                "로딩 버퍼 자재 도착 감지 센서 #2을 확인해 주십시오.",
                                "LAODING BUFFER DEVICE ARRIVAL CHECK SENSOR #2 ERROR",
                                "The loading buffer 2nd lane device arrival sensor was not detected.",
                                "Check the loading buffer 2nd lane device arrival check sensor.",
                                "上料轨道产品感应传感器#2报警",
                                "上料轨道#2产品到达传感器没有工作",
                                "请检查上料轨道产品到达传感器#2",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_InBufDevArrival_3,
                                "로딩 버퍼 자재 도착 감지 센서 #3 에러",
                                "로딩 버퍼 #3 자재 도착 감지 센서가 동작하지 않습니다.",
                                "로딩 버퍼 자재 도착 감지 센서 #3을 확인해 주십시오.",
                                "LAODING BUFFER DEVICE ARRIVAL CHECK SENSOR #3 ERROR",
                                "The loading buffer 3rd lane device arrival sensor was not detected.",
                                "Check the loading buffer 3rd lane device arrival check sensor.",
                                "上料轨道产品感应传感器#3报警",
                                "上料轨道#3产品到达传感器没有工作",
                                "请检查上料轨道产品到达传感器#3",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_InBufDevArrival_4,
                                "로딩 버퍼 자재 도착 감지 센서 #4 에러",
                                "로딩 버퍼 #4 자재 도착 감지 센서가 동작하지 않습니다.",
                                "로딩 버퍼 자재 도착 감지 센서 #4을 확인해 주십시오.",
                                "LAODING BUFFER DEVICE ARRIVAL CHECK SENSOR #4 ERROR",
                                "The loading buffer 4th lane device arrival sensor was not detected.",
                                "Check the loading buffer 4th lane device arrival check sensor.",
                                "上料轨道产品感应传感器#4报警",
                                "上料轨道#4产品到达传感器没有工作",
                                "请检查上料轨道产品到达传感器#4",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_InBufCylFwd,
                                "로딩 버퍼 전진 센서 에러",
                                "로딩 버퍼 전진 센서가 동작하지 않습니다.",
                                "메인 공압을 확인해 주십시오. 로딩 버퍼 전진 센서의 위치를 확인해 주십시오.",
                                "LOADING BUFFER FWD. ERROR",
                                "The right-side sensor of buffer was not detected.",
                                "Check main air inlet. Check the position of the buffer right sensor.",
                                "上料轨道前进传感器报警",
                                "上料轨道前进传感器没有动作",
                                "请确认主气压和前进传感器的位置",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_InBufCylBwd,
                                "로딩 버퍼 후진 센서 에러",
                                "로딩 버퍼 후진 센서가 동작하지 않습니다.",
                                "메인 공압을 확인해 주십시오. 로딩 버퍼 후진 센서의 위치를 확인해 주십시오.",
                                "LOADING BUFFER BWD. ERROR",
                                "The left-side sensor of buffer was not detected.",
                                "Check main air inlet. Check the position of the buffer left sensor.",
                                "上料轨道后退传感器报警",
                                "上料轨道后退传感器没有动作",
                                "请确认主气压和后退传感器的位置",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_Pusher2Overload,
                                "인덱스 푸셔 오버로드",
                                "인덱스 푸셔 오버로드 센서가 감지되었습니다.",
                                "인덱스 푸셔에 부하가 걸렸는지 확인해 주십시오. 부하의 원인을 제거 후 운전해 주십시오.",
                                "INDEX PUSHER OVERLOAD",
                                "The index pusher overload sensor signal was detected.",
                                "Check the load of index pusher. Remove the obstacle if exists.",
                                "步进推杆过载",
                                "步进推杆过载传感器被感应到",
                                "请确认步进推杆是否负载，去除负载原因后再运行",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_Pusher2StripChk_1,
                                "챔버 입구 자재 걸림 #1 에러",
                                "챔버 1번째 입구에서 자재가 감지되었습니다.",
                                "자재를 챔버 내로 이동 후 작업해 주십시오.",
                                "CHAMBER IN STRIP CHECK #1 ERROR",
                                "The chamber in device check 1st sensor signal was detected.",
                                "Move the device into chamber.",
                                "腔体入口#1卡料报警",
                                "腔体#1入口产品被感应到",
                                "把产品移到腔体后再进行作业",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_Pusher2StripChk_2,
                                "챔버 입구 자재 걸림 #2 에러",
                                "챔버 2번째 입구에서 자재가 감지되었습니다.",
                                "자재를 챔버 내로 이동 후 작업해 주십시오.",
                                "CHAMBER IN STRIP CHECK #2 ERROR",
                                "The chamber in device check 2nd sensor signal was detected.",
                                "Move the device into chamber.",
                                "腔体入口#2卡料报警",
                                "腔体#2入口产品被感应到",
                                "把产品移到腔体后再进行作业",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_Pusher2StripChk_3,
                                "챔버 입구 자재 걸림 #3 에러",
                                "챔버 3번째 입구에서 자재가 감지되었습니다.",
                                "자재를 챔버 내로 이동 후 작업해 주십시오.",
                                "CHAMBER IN STRIP CHECK #3 ERROR",
                                "The chamber in device check 3rd sensor signal was detected.",
                                "Move the device into chamber.",
                                "腔体入口#3卡料报警",
                                "腔体#3入口产品被感应到",
                                "把产品移到腔体后再进行作业",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_Pusher2StripChk_4,
                                "챔버 입구 자재 걸림 #4 에러",
                                "챔버 4번째 입구에서 자재가 감지되었습니다.",
                                "자재를 챔버 내로 이동 후 작업해 주십시오.",
                                "CHAMBER IN STRIP CHECK #4 ERROR",
                                "The chamber in device check 4th sensor signal was detected.",
                                "Move the device into chamber.",
                                "腔体入口#4卡料报警",
                                "腔体#4入口产品被感应到",
                                "把产品移到腔体后再进行作业",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_Pusher2Up,
                                "인덱스 푸셔 업 에러",
                                "인덱스 푸셔 업 센서가 동작하지 않습니다.",
                                "메인 공압을 확인해 주십시오. 인덱스 푸셔 업 센서의 위치를 확인해 주십시오.",
                                "INDEX PUSHER UP ERROR",
                                "The upper sensor of index pusher was not detected.",
                                "Check main air inlet. Check the position of the index pusher uppper sensor.",
                                "步进推杆上升报警",
                                "步进推杆上升传感器没有动作",
                                "请检查主气压以及传感器的位置",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(eErrCode.Ecode_I_Pusher2Dn,
                                "인덱스 푸셔 다운 에러",
                                "인덱스 푸셔 다운 센서가 동작하지 않습니다.",
                                "메인 공압을 확인해 주십시오. 인덱스 푸셔 다운 센서의 위치를 확인해 주십시오.",
                                "INDEX PUSHER DOWN ERROR",
                                "The lower sensor of index pusher was not detected.",
                                "Check main air inlet. Check the position of the index pusher lower sensor.",
                                "步进推杆下降报警",
                                "步进推杆下降传感器没有动作",
                                "请检查主气压以及传感器的位置",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_OutBufFwd,
                                "언로딩 버퍼 전진 센서 에러",
                                "언로딩 버퍼 전진 센서가 동작하지 않습니다.",
                                "메인 공압을 확인해 주십시오. 언로딩 버퍼 전진 센서의 위치를 확인해 주십시오.",
                                "UNLOADING BUFFER FWD. ERROR",
                                "The right-side sensor of unloading buffer was not detected.",
                                "Check main air inlet. Check the position of the buffer ritght sensor.",
                                "下料轨道前进传感器报警",
                                "下料轨道前进传感器没有动作",
                                "请检查主气压和传感器的位置",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_OutBufBwd,
                                "언로딩 버퍼 후진 센서 에러",
                                "언로딩 버퍼 후진 센서가 동작하지 않습니다.",
                                "메인 공압을 확인해 주십시오. 언로딩 버퍼 후진 센서의 위치를 확인해 주십시오.",
                                "UNLOADING BUFFER BWD. ERROR",
                                "The left-side sensor of unloading buffer was not detected.",
                                "Check main air inlet. Check the position of the buffer left sensor.",
                                "下料轨道后退传感器报警",
                                "下料轨道后退传感器没有动作",
                                "请检查主气压和传感器的位置",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_ChamberOutStripChk_1,
                                "챔버 출구 자재 걸림 센서#1 에러",
                                "챔버 출구 자재 걸림 센서 #1이 감지되었습니다.",
                                "챔버 출구의 자재를 제거 후 작업해 주십시오.",
                                "CHAMBER OUT DEVICE CHECK #1 ERROR",
                                "The chamber out device check #1 sensor is on.",
                                "Remove the device at the 1st chamber out.",
                                "腔体出口#1卡料报警",
                                "腔体#1出口产品被感应到",
                                "把产品移到腔体后再进行作业",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_ChamberOutStripChk_2,
                                "챔버 출구 자재 걸림 센서#2 에러",
                                "챔버 출구 자재 걸림 센서 #2이 감지되었습니다.",
                                "챔버 출구의 자재를 제거 후 작업해 주십시오.",
                                "CHAMBER OUT DEVICE CHECK #2 ERROR",
                                "The chamber out device check #2 sensor is on.",
                                "Remove the device at the 2nd chamber out.",
                                "腔体出口#2卡料报警",
                                "腔体#2出口产品被感应到",
                                "把产品移到腔体后再进行作业",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_ChamberOutStripChk_3,
                                "챔버 출구 자재 걸림 센서#3 에러",
                                "챔버 출구 자재 걸림 센서 #3이 감지되었습니다.",
                                "챔버 출구의 자재를 제거 후 작업해 주십시오.",
                                "CHAMBER OUT DEVICE CHECK #3 ERROR",
                                "The chamber out device check #3 sensor is on.",
                                "Remove the device at the 3rd chamber out.",
                                "腔体出口#3卡料报警",
                                "腔体#3出口产品被感应到",
                                "把产品移到腔体后再进行作业",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_ChamberOutStripChk_4,
                                "챔버 출구 자재 걸림 센서#4 에러",
                                "챔버 출구 자재 걸림 센서 #4이 감지되었습니다.",
                                "챔버 출구의 자재를 제거 후 작업해 주십시오.",
                                "CHAMBER OUT DEVICE CHECK #4 ERROR",
                                "The chamber out device check #4 sensor is on.",
                                "Remove the device at the 4th chamber out.",
                                "腔体出口#4卡料报警",
                                "腔体#4出口产品被感应到",
                                "把产品移到腔体后再进行作业",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_Pusher3Overload,
                                "언로딩 푸셔 오버로드",
                                "언로딩 푸셔 오버로드 센서가 감지되었습니다.",
                                "언로딩 푸셔를 확인해 주십시오.",
                                "UNLOADING PUSHER OVERLOAD",
                                "The unload pusher overload signal was detected.",
                                "Check the unloading pusher.",
                                "下料推杆过载",
                                "下料推杆过载传感器被感应到",
                                "请确认下料推杆",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_Pusher3Up,
                                "언로딩 푸셔 업 에러",
                                "언로딩 푸셔 업 센서가 동작하지 않습니다.",
                                "메인 공압을 확인해 주십시오. 언로딩 푸셔 업 센서의 위치를 확인해 주십시오.",
                                "UNLOADING PUSHER UP ERROR",
                                "The upper sensor of unloading pusher was not detected.",
                                "Check main air inlet. Check the position of the unloading pusher uppper sensor.",
                                "下料推杆上升报警",
                                "下料推杆上升传感器不动作",
                                "请确认主气压和下料推杆上升传感器位置",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_Pusher3Dn,
                                "언로딩 푸셔 다운 에러",
                                "언로딩 푸셔 다운 센서가 동작하지 않습니다.",
                                "메인 공압을 확인해 주십시오. 언로딩 푸셔 다운 센서의 위치를 확인해 주십시오.",
                                "UNLOADING PUSHER DOWN ERROR",
                                "The lower sensor of unloading pusher was not detected.",
                                "Check main air inlet. Check the position of the unloading pusher lower sensor.",
                                "下料推杆下降报警",
                                "下料推杆下降传感器不动作",
                                "请确认主气压和下料推杆下降传感器位置",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_Pusher3Fwd,
                                "언로딩 푸셔 전진 에러",
                                "언로딩 푸셔 전진 센서가 동작하지 않습니다.",
                                "메인 공압을 확인해 주십시오. 언로딩 푸셔 전진 센서의 위치를 확인해 주십시오.",
                                "UNLOADING PUSHER FWD. ERROR",
                                "The right-side sensor of unloading pusher was not detected.",
                                "Check main air inlet. Check the position of the unloading pusher right - side sensor.",
                                "下料推杆前进报警",
                                "下料推杆前进传感器不动作",
                                "请确认主气压和下料推杆前进传感器位置",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_Pusher3Bwd,
                                "언로딩 푸셔 후진 에러",
                                "언로딩 푸셔 후진 센서가 동작하지 않습니다.",
                                "메인 공압을 확인해 주십시오. 언로딩 푸셔 후진 센서의 위치를 확인해 주십시오.",
                                "UNLOADING PUSHER BWD. ERROR",
                                "The left-side sensor of unloading pusher was not detected.",
                                "Check main air inlet. Check the position of the unloading pusher left - side sensor.",
                                "下料推杆后退报警",
                                "下料推杆后退传感器不动作",
                                "请确认主气压和下料推杆后退传感器位置",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_Pusher3DnEnable_1,
                                "언로딩 푸셔 다운 위치 자재 감지 센서 #1 에러",
                                "언로딩 푸셔 다운 위치 자재 감지 센서 #1이 감지되었습니다.",
                                "자재 제거 후 작업해 주십시오.",
                                "UNLOADING PUSHER DOWN POS. DEVICE CHECK #1 ERROR",
                                "The unloading pusher down position device check #1 sensor is on.",
                                "Remove the device at the 1st unloading buffer.",
                                "下料推杆下降位置感应产品#1报警",
                                "下料推杆下降位置传感器#1感应到了产品",
                                "去除产品后再进行作业",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_Pusher3DnEnable_2,
                                "언로딩 푸셔 다운 위치 자재 감지 센서 #2 에러",
                                "언로딩 푸셔 다운 위치 자재 감지 센서 #2이 감지되었습니다.",
                                "자재 제거 후 작업해 주십시오.",
                                "UNLOADING PUSHER DOWN POS. DEVICE CHECK #2 ERROR",
                                "The unloading pusher down position device check #2 sensor is on.",
                                "Remove the device at the 2nd unloading buffer.",
                                "下料推杆下降位置感应产品#2报警",
                                "下料推杆下降位置传感器#2感应到了产品",
                                "去除产品后再进行作业",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_I_Pusher3DnEnable_3,
                                "언로딩 푸셔 다운 위치 자재 감지 센서 #3 에러",
                                "언로딩 푸셔 다운 위치 자재 감지 센서 #3이 감지되었습니다.",
                                "자재 제거 후 작업해 주십시오.",
                                "UNLOADING PUSHER DOWN POS. DEVICE CHECK #3 ERROR",
                                "The unloading pusher down position device check #3 sensor is on.",
                                "Remove the device at the 3rd unloading buffer.",
                                "下料推杆下降位置感应产品#3报警",
                                "下料推杆下降位置传感器#3感应到了产品",
                                "去除产品后再进行作业",
                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4055,
                                "언로딩 푸셔 다운 위치 자재 감지 센서 #4 에러",
                                "언로딩 푸셔 다운 위치 자재 감지 센서 #4이 감지되었습니다.",
                                "자재 제거 후 작업해 주십시오.",

                                "UNLOADING PUSHER DOWN POS. DEVICE CHECK #4 ERROR",
                                "The unloading pusher down position device check #4 sensor is on.",
                                "Remove the device at the 4th unloading buffer.",

                                "下料推杆下降位置感应产品#4报警",
                                "下料推杆下降位置传感器#4感应到了产品",
                                "去除产品后再进行作业",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4056,
                                "언로딩 엘리베이터 자재 걸림 센서 #1 에러",
                                "언로딩 엘리베이터 자재 걸림 센서 #1이 감지되었습니다.",
                                "언로딩 엘리베이터 #1 출구를 점검해 주십시오. 출구의 자재를 제거 후 작업해 주십시오.",

                                "UNLOADING ELEVATOR STRIP CHECK #1 ERROR",
                                "The out-elevator 1st lane in device check sensor signal was detected.",
                                "Check the inlet of out-elevator 1st lane.",

                                "下料升降机传感器#1卡料报警",
                                "下料升降机传感器#1感应到了",
                                "请检查下料升降机#1出口，去除出口的产品后继续作业",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4057,
                                "언로딩 엘리베이터 자재 걸림 센서 #2 에러",
                                "언로딩 엘리베이터 자재 걸림 센서 #2이 감지되었습니다.",
                                "언로딩 엘리베이터 #2 출구를 점검해 주십시오. 출구의 자재를 제거 후 작업해 주십시오.",

                                "UNLOADING ELEVATOR STRIP CHECK #2 ERROR",
                                "The out-elevator 2nd lane in device check sensor signal was detected.",
                                "Check the inlet of out-elevator 2nd lane.",

                                "下料升降机传感器#2卡料报警",
                                "下料升降机传感器#2感应到了",
                                "请检查下料升降机#2出口，去除出口的产品后继续作业",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4058,
                                "언로딩 엘리베이터 자재 걸림 센서 #3 에러",
                                "언로딩 엘리베이터 자재 걸림 센서 #3이 감지되었습니다.",
                                "언로딩 엘리베이터 #3 출구를 점검해 주십시오. 출구의 자재를 제거 후 작업해 주십시오.",

                                "UNLOADING ELEVATOR STRIP CHECK #3 ERROR",
                                "The out-elevator 3rd lane in device check sensor signal was detected.",
                                "Check the inlet of out-elevator 3rd lane.",

                                "下料升降机传感器#3卡料报警",
                                "下料升降机传感器#3感应到了",
                                "请检查下料升降机#3出口，去除出口的产品后继续作业",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4059,
                                "언로딩 엘리베이터 자재 걸림 센서 #4 에러",
                                "언로딩 엘리베이터 자재 걸림 센서 #4이 감지되었습니다.",
                                "언로딩 엘리베이터 #4 출구를 점검해 주십시오. 출구의 자재를 제거 후 작업해 주십시오.",

                                "UNLOADING ELEVATOR STRIP CHECK #4 ERROR",
                                "The out-elevator 4th lane in device check sensor signal was detected.",
                                "Check the inlet of out-elevator 4th lane.",

                                "下料升降机传感器#4卡料报警",
                                "下料升降机传感器#4感应到了",
                                "请检查下料升降机#4出口，去除出口的产品后继续作业",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4060,
                                "언로딩 엘리베이터 메거진 감지 센서 #1 에러",
                                "언로딩 엘리베이터 메거진 감지 센서 #1가 감지되지 않았습니다.",
                                "언로딩 엘리베이터의 메거진#1을 점검해 주십시오.",

                                "UNLOADING ELEVATOR MGZ. CHECK #1 ERROR",
                                "The out-elevator 1st lane magazine detect sensor signal was not detected.",
                                "Check the magazine existance at 1st lane of out-elevator.",

                                "下料端料盒感应传感器#1报警",
                                "下料端料盒感应传感器#1没有感应到",
                                "请检查料盒#1",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4061,
                                "언로딩 엘리베이터 메거진 감지 센서 #2 에러",
                                "언로딩 엘리베이터 메거진 감지 센서 #2가 감지되지 않았습니다.",
                                "언로딩 엘리베이터의 메거진#2을 점검해 주십시오.",

                                "UNLOADING ELEVATOR MGZ. CHECK #2 ERROR",
                                "The out-elevator 2nd lane magazine detect sensor signal was not detected.",
                                "Check the magazine existance at 2nd lane of out-elevator.",

                                "下料端料盒感应传感器#2报警",
                                "下料端料盒感应传感器#2没有感应到",
                                "请检查料盒#2",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4062,
                                "언로딩 엘리베이터 메거진 감지 센서 #3 에러",
                                "언로딩 엘리베이터 메거진 감지 센서 #3가 감지되지 않았습니다.",
                                "언로딩 엘리베이터의 메거진#3을 점검해 주십시오.",

                                "UNLOADING ELEVATOR MGZ. CHECK #3 ERROR",
                                "The out-elevator 3rd lane magazine detect sensor signal was not detected.",
                                "Check the magazine existance at 3rd lane of out-elevator.",

                                "下料端料盒感应传感器#3报警",
                                "下料端料盒感应传感器#3没有感应到",
                                "请检查料盒#3",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4063,
                                "언로딩 엘리베이터 메거진 감지 센서 #4 에러",
                                "언로딩 엘리베이터 메거진 감지 센서 #4가 감지되지 않았습니다.",
                                "언로딩 엘리베이터의 메거진#4을 점검해 주십시오.",

                                "UNLOADING ELEVATOR MGZ. CHECK #4 ERROR",
                                "The out-elevator 4th lane magazine detect sensor signal was not detected.",
                                "Check the magazine existance at 4th lane of out-elevator.",

                                "下料端料盒感应传感器#4报警",
                                "下料端料盒感应传感器#4没有感应到",
                                "请检查料盒#4",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4064,
                                "자재 방향 감지 비전 에러 신호",
                                "비전 에러가 검출되었습니다.",
                                "비전을 확인해 주십시오. 비전 설정을 확인해 주십시오.",

                                "DEVICE DIRECTION CHECK VISION ERROR SIGNAL",
                                "The direction check signal of vision was not ok.",
                                "Check the vision(setting).",

                                "材料方向检测视觉错误信号",
                                "检测到视觉错误。",
                                "请检查你的相机。 请检查您的相机设置。",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4065,
                                "자재 방향 감지 비전 인에블 에러",
                                "비전 인에이블 신호가 동작하지 않습니다.",
                                "비전을 확인해 주십시오.",

                                "DEVICE DIRECTION CHECK VISION ENABLE ERROR",
                                "The enable signal of the vision was not detected.",
                                "Check the vision.",

                                "材料方向检测可视错误",
                                "相机可视信号没有动作",
                                "请检查你的相机。",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4066,
                                "자재 방향 감지 비전 에러",
                                "자재 방향 감지 비전 에러신호가 검출되었습니다.",
                                "비전을 확인해 주십시오.",

                                "DEVICE DIRECTION CHECK VISION ERROR",
                                "The direction check vision error signal is on.",
                                "Check the vision.",

                                "材料方向感知相机报警",
                                "材料方向感知相机检测到错误信号。",
                                "请检查你的相机。",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4067,
                                "가스3 공급압력 낮음",
                                "가스3 압력 신호를 감지하지 못했습니다.",
                                "가스3 공급여부를 확인해 주십시오. 가스3 압력체크 센서를 점검해 주십시오.",

                                "GAS3 PRESSURE LOW",
                                "The pressure of gas3 is low",
                                "Check the inlet of gas3. Check the gas3 pressure sensor.",

                                "气体3供应压力低",
                                "气体3无法检测到压力信号。",
                                "请检查是否供应了气体3。 气体3检查压力检查传感器。",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4068,
                                "가스4 공급압력 낮음",
                                "가스4 압력 신호를 감지하지 못했습니다.",
                                "가스4 공급여부를 확인해 주십시오. 가스4 압력체크 센서를 점검해 주십시오.",

                                "GAS4 PRESSURE LOW",
                                "The pressure of gas4 is low",
                                "Check the inlet of gas4. Check the gas4 pressure sensor.",

                                "气体4供应压力低",
                                "气体4无法检测到压力信号。",
                                "请检查是否供应了气体4。 气体4检查压力检查传感器。",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4069,
                                "인덱스 푸셔 #2 오버로드",
                                "인덱스 푸셔 #2 오버로드 센서가 감지되었습니다.",
                                "인덱스 푸셔 #2에 부하가 걸렸는지 확인해 주십시오. 부하의 원인을 제거 후 운전해 주십시오.",

                                "INDEX PUSHER #2 OVERLOAD",
                                "The index pusher #2 overload sensor signal was detected.",
                                "Check the load of index pusher #2. Remove the obstacle if exists.",

                                "步进推杆#2过载",
                                "步进推杆过载传感器#2被感应到",
                                "请确认步进推杆是否负载，去除负载原因后再运行",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4070,
                                "인덱스 푸셔 #3 오버로드",
                                "인덱스 푸셔 #3 오버로드 센서가 감지되었습니다.",
                                "인덱스 푸셔 #3에 부하가 걸렸는지 확인해 주십시오. 부하의 원인을 제거 후 운전해 주십시오.",

                                "INDEX PUSHER #3 OVERLOAD",
                                "The index pusher #3 overload sensor signal was detected.",
                                "Check the load of index pusher #3. Remove the obstacle if exists.",

                                "步进推杆#3过载",
                                "步进推杆过载传感器#3被感应到",
                                "请确认步进推杆是否负载，去除负载原因后再运行",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4071,
                                "인덱스 푸셔 #4 오버로드",
                                "인덱스 푸셔 #4 오버로드 센서가 감지되었습니다.",
                                "인덱스 푸셔 #4에 부하가 걸렸는지 확인해 주십시오. 부하의 원인을 제거 후 운전해 주십시오.",

                                "INDEX PUSHER #4 OVERLOAD",
                                "The index pusher #4 overload sensor signal was detected.",
                                "Check the load of index pusher #4. Remove the obstacle if exists.",

                                "步进推杆#4过载",
                                "步进推杆过载传感器#4被感应到",
                                "请确认步进推杆是否负载，去除负载原因后再运行",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4072,
                                "로딩 엘리베이터 메거진 감지 센서 #5 에러",
                                "로딩 엘리베이터 메거진 감지 센서 #5가 감지되지 않았습니다.",
                                "로딩 엘리베이터의 메거진#5을 점검해 주십시오.",

                                "LOADING ELEVATOR MGZ. CHECK #5 ERROR",
                                "The on-elevator 5th lane magazine detect sensor signal was not detected.",
                                "Check the magazine existance at 5th lane of on-elevator.",

                                "上料端料盒感应传感器#5报警",
                                "上料端料盒感应传感器#5没有感应到",
                                "请确认上料端料盒#5",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4073,
                                "로딩 엘리베이터 자재 걸림 센서 #5 에러",
                                "로딩 엘리베이터 자재 걸림 센서 #5이 감지되었습니다.",
                                "로딩 엘리베이터 #5 출구를 점검해 주십시오. 출구의 자재를 제거 후 작업해 주십시오.",

                                "LOADING ELEVATOR STRIP CHECK #5 ERROR",
                                "The on-elevator 5th lane out device check sensor signal was detected.",
                                "Check the outlet of on-elevator 5th lane.",

                                "上料端产品卡料传感器#5报警",
                                "上料端产品卡料传感器#5被感应到",
                                "请确认上料端#5出口，把出口的产品去除后再开始作业",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4074,
                                "로딩 버퍼 자재 도착 감지 센서 #5 에러",
                                "로딩 버퍼 #5 자재 도착 감지 센서가 동작하지 않습니다.",
                                "로딩 버퍼 자재 도착 감지 센서 #5를 확인해 주십시오.",

                                "LAODING BUFFER DEVICE ARRIVAL CHECK SENSOR #5 ERROR",
                                "The loading buffer 5th lane device arrival sensor was not detected.",
                                "Check the loading buffer 5th lane device arrival check sensor.",

                                "上料轨道产品到达传感器#5报警",
                                "上料区域#5产品到达传感器没有动作",
                                "请确认上料轨道产品到达传感器#5",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4075,
                                "챔버 입구 자재 걸림 #5 에러",
                                "챔버 5번째 입구에서 자재가 감지되었습니다.",
                                "자재를 챔버 내로 이동 후 작업해 주십시오.",

                                "CHAMBER IN STRIP CHECK #5 ERROR",
                                "The chamber in device check 5th sensor signal was detected.",
                                "Move the device into chamber.",

                                "腔体入口卡料#5报警",
                                "腔体第五入口产品被感应到",
                                "请把产品移入腔体后作业",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4076,
                                "챔버 출구 자재 걸림 센서#5 에러",
                                "챔버 출구 자재 걸림 센서 #5이 감지되었습니다.",
                                "챔버 출구의 자재를 제거 후 작업해 주십시오.",

                                "CHAMBER OUT DEVICE CHECK #5 ERROR",
                                "The chamber out device check #5 sensor is on.",
                                "Remove the device at the 5th chamber out.",

                                "腔体出口卡料传感器#5报警",
                                "腔体出口卡料被传感器#5感应到",
                                "请去除腔体出口产品后再作业",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4077,
                                "언로딩 푸셔 다운 위치 자재 감지 센서 #5 에러",
                                "언로딩 푸셔 다운 위치 자재 감지 센서 #5이 감지되었습니다.",
                                "자재 제거 후 작업해 주십시오.",

                                "UNLOADING PUSHER DOWN POS. DEVICE CHECK #5 ERROR",
                                "The unloading pusher down position device check #5 sensor is on.",
                                "Remove the device at the 5th unloading buffer.",

                                "下料推杆下降位置传感器#5报警",
                                "下料推杆下降位置传感器#5被感应到了",
                                "请去除产品后再作业",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4078,
                                "언로딩 엘리베이터 자재 걸림 센서 #5 에러",
                                "언로딩 엘리베이터 자재 걸림 센서 #5이 감지되었습니다.",
                                "언로딩 엘리베이터 #5 출구를 점검해 주십시오. 출구의 자재를 제거 후 작업해 주십시오.",

                                "UNLOADING ELEVATOR STRIP CHECK #5 ERROR",
                                "The out-elevator 5th lane in device check sensor signal was detected.",
                                "Check the inlet of out-elevator 5th lane.",

                                "下料端产品卡料传感器#5报警",
                                "下料端产品卡料传感器#5被感应到",
                                "请确认下料端#5出口，把出口的产品去除后再开始作业",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4079,
                                "언로딩 엘리베이터 메거진 감지 센서 #5 에러",
                                "언로딩 엘리베이터 메거진 감지 센서 #5가 감지되지 않았습니다.",
                                "언로딩 엘리베이터의 메거진#5을 점검해 주십시오.",

                                "UNLOADING ELEVATOR MGZ. CHECK #5 ERROR",
                                "The out-elevator 5th lane magazine detect sensor signal was not detected.",
                                "Check the magazine existance at 5th lane of out-elevator.",

                                "下料端料盒感应传感器#5报警",
                                "下料端料盒感应传感器#5没有感应到",
                                "请确认下料端料盒#5",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4080,
                                "RF 에러",
                                "지정된 시간동안 RF 진행파의 값이 설정된 값 이내에 들지 못했습니다.",
                                "RF제너레이터를 확인해 주십시오.",

                                "RF ERROR",
                                "The RF forward value is not in setting range during set time.",
                                "Check the RF generator.",

                                "射频报警",
                                "射频波长没有在设定时间内达到",
                                "请确认射频发生器",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4081,
                                "자재 로딩 에러",
                                "로딩 메거진의 자재가 버퍼에 도달하지 못했습니다.",
                                "로딩 버퍼의 자재를 확인해 주십시오.",

                                "DEVICE LOADING ERROR",
                                "The device did not reach to loading buffer.",
                                "Check the device on loading buffer.",

                                "产品上料报警",
                                "上料端料盒里的产品没有传送到轨道上",
                                "请确认上料轨道上产品",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4082,
                                "로딩  엘리베이터 원점 에러",
                                "로딩 엘리베이터 모터 원점 검색하지 않음.",
                                "로딩 엘리베이터 모터 원점 검색을 실행해 주십시오.",

                                "LOADING ELEVATOR NOT HOME",
                                "The loading elevator servo motor did not execute homing process.",
                                "Check the set value of communication.",

                                "上料升降台原点报警",
                                "上料升降台无法检测马达原点",
                                "请实行上料升降台马达原点检测",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4083,
                                "로딩 엘리베이터 드라이버 알람",
                                "로딩 엘리베이터 모터의 드라이버 알람 발생.",
                                "로딩 엘리베이터 모터 드라이버를 점검해 주십시오. 로딩 리니어 모터에 부하가 걸렸는지 확인해 주십시오.",

                                "LOADING ELEVATOR NOT HOME DRIVER ALARM",
                                "The loading elevator servo driver alarm occurred.",
                                "Check the set value of communication.",

                                "上料升降台伺服报警",
                                "上料升降台马达伺服发生报警",
                                "请检查上料升降台马达伺服，确认马达是否过载",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4084,
                                "로딩 엘리베이터 소프트리미트 상한 에러",
                                "로딩 엘리베이터 모터가 소프트리미트 값 이상으로 운전하려고 합니다.",
                                "로딩 엘리베이터의 명령 위치를 소프트 리미트 이내로 변경 후 운전해 주십시오.",

                                "LOADING ELEVATOR + SOFT-LIMIT",
                                "The command position of the loading elevator exceed + soft limit value.",
                                "Check the set value of communication.",

                                "上料升降台软件上限值报警",
                                "上料升降台马达要移动到超出软件上限值",
                                "请把上料升降台移动位置修改为极限位置内",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4085,
                                "로딩 엘리베이터 소프트리미트 하한 에러",
                                "로딩 엘리베이터 모터가 소프트리미트 값 미만으로 운전하려고 합니다.",
                                "로딩 엘리베이터의 명령 위치를 소프트 리미트 이내로 변경 후 운전해 주십시오.",

                                "LOADING ELEVATOR - SOFT-LIMIT",
                                "The command position of the loading elevator exceed - soft limit value.",
                                "Check the set value of communication.",

                                "上料升降台软件下限值报警",
                                "上料升降台马达要移动到超出软件下限值",
                                "请把上料升降台移动位置修改为极限位置内",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4086,
                                "로딩 엘리베이터 운전 중",
                                "로딩 엘리베이터 모터가 운전 중입니다.",
                                "로딩 엘리베이터 모터가 정지 후 명령을 수행할 수 있습니다.",

                                "LOADING ELEVATOR MOVING",
                                "The loading elevator is moving.",
                                "Check the set value of communication.",

                                "上料升降台运行中",
                                "上料升降台马达正在运行",
                                "请在上料升降台马达停止后再给予命令",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4087,
                                "로딩  푸셔 원점 에러",
                                "로딩 푸셔 모터 원점 검색하지 않음.",
                                "로딩 푸셔 모터 원점 검색을 실행해 주십시오.",

                                "LOADING PUSHER NOT HOME",
                                "The loading pusher servo motor did not execute homing process.",
                                "Execute homing process of the loading elevator.",

                                "上料推杆原点报警",
                                "上料推杆马达原点无法检测",
                                "请实行上料推杆马达原点检测",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4088,
                                "로딩 푸셔 드라이버 알람",
                                "로딩 푸셔 모터의 드라이버 알람 발생.",
                                "로딩 푸셔 모터 드라이버를 점검해 주십시오. 로딩 리니어 모터에 부하가 걸렸는지 확인해 주십시오.",

                                "LOADING PUSHER NOT HOME DRIVER ALARM",
                                "The loading pusher servo driver alarm occurred.",
                                "Check the servo driver of loading elevator.",

                                "上料推杆伺服报警",
                                "上料推杆马达伺服发生报警",
                                "请检查上料推杆马达伺服，确认马达是否过载",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4089,
                                "로딩 푸셔 소프트리미트 상한 에러",
                                "로딩 푸셔 모터가 소프트리미트 값 이상으로 운전하려고 합니다.",
                                "로딩 푸셔의 명령 위치를 소프트 리미트 이내로 변경 후 운전해 주십시오.",

                                "LOADING PUSHER + SOFT-LIMIT",
                                "The command position of the loading pusher exceed + soft limit value.",
                                "Check the position of the loading elevator.",

                                "上料推杆软件上限值报警",
                                "上料推杆马达要移动到超出软件上限值",
                                "请把上料推杆移动位置修改为极限位置内",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4090,
                                "로딩 푸셔 소프트리미트 하한 에러",
                                "로딩 푸셔 모터가 소프트리미트 값 미만으로 운전하려고 합니다.",
                                "로딩 푸셔의 명령 위치를 소프트 리미트 이내로 변경 후 운전해 주십시오.",

                                "LOADING PUSHER - SOFT-LIMIT",
                                "The command position of the loading pusher exceed - soft limit value.",
                                "Check the position of the loading elevator.",

                                "上料推杆软件下限值报警",
                                "上料推杆马达要移动到超出软件下限值",
                                "请把上料推杆移动位置修改为极限位置内",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4091,
                                "로딩 푸셔 운전 중",
                                "로딩 푸셔 모터가 운전 중입니다.",
                                "로딩 푸셔 모터가 정지 후 명령을 수행할 수 있습니다.",

                                "LOADING PUSHER MOVING",
                                "The loading pusher is moving.",
                                "You can not command during motor moving.",

                                "上料推杆运行中",
                                "上料推杆马达正在运行",
                                "请在上料推杆马达停止后再给予命令",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4092,
                                "비전 스텝 원점 에러",
                                "비전 스텝 모터 원점 검색하지 않음.",
                                "비전 스텝모터 원점 검색을 실행해 주십시오.",

                                "VISION STEP  NOT HOME",
                                "The vision step motor did not execute homing process.",
                                "Execute homing process of the loading pusher.",

                                "相机步进原点报警",
                                "相机步进马达原点无法检测",
                                "请实行相机步进马达原点检测",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4093,
                                "비전 스텝 드라이버 알람",
                                "비전 스텝 모터의 드라이버 알람 발생.",
                                "비전 스텝 모터 드라이버를 점검해 주십시오. 로딩 리니어 모터에 부하가 걸렸는지 확인해 주십시오.",

                                "VISION STEP NOT HOME DRIVER ALARM",
                                "The vision step driver alarm occurred.",
                                "Check the servo driver of loading pusher.",

                                "相机步进伺服报警",
                                "相机步进马达伺服发生报警",
                                "请检查相机步进马达伺服，确认马达是否过载",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4094,
                                "비전 스텝 소프트리미트 상한 에러",
                                "비전 스텝 모터가 소프트리미트 값 이상으로 운전하려고 합니다.",
                                "비전 스텝의 명령 위치를 소프트 리미트 이내로 변경 후 운전해 주십시오.",

                                "VISION STEP + SOFT-LIMIT",
                                "The command position of the vision step exceed + soft limit value.",
                                "Check the position of the loading pusher.",

                                "相机步进软件上限值报警",
                                "相机步进马达要移动到超出软件上限值",
                                "请把相机步进移动位置修改为极限位置内",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4095,
                                "비전 스텝 소프트리미트 하한 에러",
                                "비전 스텝 모터가 소프트리미트 값 미만으로 운전하려고 합니다.",
                                "비전 스텝의 명령 위치를 소프트 리미트 이내로 변경 후 운전해 주십시오.",

                                "VISION STEP - SOFT-LIMIT",
                                "The command position of the vision step exceed - soft limit value.",
                                "Check the position of the loading pusher.",

                                "相机步进软件下限值报警",
                                "相机步进马达要移动到超出软件下限值",
                                "请把相机步进移动位置修改为极限位置内",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4096,
                                "비전 스텝 운전 중",
                                "비전 스텝 모터가 운전 중입니다.",
                                "비전 스텝 모터가 정지 후 명령을 수행할 수 있습니다.",

                                "VISION STEP MOVING",
                                "The vision step is moving.",
                                "You can not command during motor moving.",

                                "相机步进运行中",
                                "相机步进马达正在运行",
                                "请在相机步进马达停止后再给予命令",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4097,
                                "인덱스 푸셔 원점 에러",
                                "인덱스 푸셔 모터 원점 검색하지 않음.",
                                "인덱스 푸셔 모터 원점 검색을 실행해 주십시오.",

                                "INDEX PUSHER NOT HOME",
                                "The index pusher servo motor did not execute homing process.",
                                "Execute homing process of the vision step.",

                                "步进推杆原点报警",
                                "步进推杆马达原点无法检测",
                                "请实行步进推杆马达原点检测",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4098,
                                "인덱스 푸셔 드라이버 알람",
                                "인덱스 푸셔 모터의 드라이버 알람 발생.",
                                "인덱스 푸셔 모터 드라이버를 점검해 주십시오. 로딩 리니어 모터에 부하가 걸렸는지 확인해 주십시오.",

                                "INDEX PUSHER NOT HOME DRIVER ALARM",
                                "The index pusher servo driver alarm occurred.",
                                "Check the servo driver of vision step.",

                                "步进推杆伺服报警",
                                "步进推杆马达伺服发生报警",
                                "请检查步进推杆马达伺服，确认马达是否过载",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4099,
                                "인덱스 푸셔 소프트리미트 상한 에러",
                                "인덱스 푸셔 모터가 소프트리미트 값 이상으로 운전하려고 합니다.",
                                "인덱스 푸셔의 명령 위치를 소프트 리미트 이내로 변경 후 운전해 주십시오.",

                                "INDEX PUSHER + SOFT-LIMIT",
                                "The command position of the index pusher exceed + soft limit value.",
                                "Check the position of the vision step.",

                                "步进推杆软件上限值报警",
                                "步进推杆马达要移动到超出软件上限值",
                                "请把步进推杆移动位置修改为极限位置内",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4100,
                                "인덱스 푸셔 소프트리미트 하한 에러",
                                "인덱스 푸셔 모터가 소프트리미트 값 미만으로 운전하려고 합니다.",
                                "인덱스 푸셔의 명령 위치를 소프트 리미트 이내로 변경 후 운전해 주십시오.",

                                "INDEX PUSHER - SOFT-LIMIT",
                                "The command position of the index pusher exceed - soft limit value.",
                                "Check the position of the vision step.",

                                "步进推杆软件下限值报警",
                                "步进推杆马达要移动到超出软件下限值",
                                "请把步进推杆移动位置修改为极限位置内",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4101,
                                "인덱스 푸셔 운전 중",
                                "인덱스 푸셔 모터가 운전 중입니다.",
                                "인덱스 푸셔 모터가 정지 후 명령을 수행할 수 있습니다.",

                                "INDEX PUSHER MOVING",
                                "The index pusher is moving.",
                                "You can not command during motor moving.",

                                "步进推杆运行中",
                                "步进推杆马达正在运行",
                                "请在步进推杆马达停止后再给予命令",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4102,
                                "언로딩  엘리베이터 원점 에러",
                                "언로딩 엘리베이터 모터 원점 검색하지 않음.",
                                "언로딩 엘리베이터 모터 원점 검색을 실행해 주십시오.",

                                "UNLOADING ELEVATOR NOT HOME",
                                "The unloading elevator servo motor did not execute homing process.",
                                "Execute homing process of the index pusher.",

                                "下料升降台原点报警",
                                "下料升降台无法检测马达原点",
                                "请实行下料升降台马达原点检测",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4103,
                                "언로딩 엘리베이터 드라이버 알람",
                                "언로딩 엘리베이터 모터의 드라이버 알람 발생.",
                                "언로딩 엘리베이터 모터 드라이버를 점검해 주십시오. 로딩 리니어 모터에 부하가 걸렸는지 확인해 주십시오.",

                                "UNLOADING ELEVATOR NOT HOME DRIVER ALARM",
                                "The unloading elevator servo driver alarm occurred.",
                                "Check the servo driver of index pusher.",

                                "下料升降台伺服报警",
                                "下料升降台马达伺服发生报警",
                                "请检查下料升降台马达伺服，确认马达是否过载",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4104,
                                "언로딩 엘리베이터 소프트리미트 상한 에러",
                                "언로딩 엘리베이터 모터가 소프트리미트 값 이상으로 운전하려고 합니다.",
                                "언로딩 엘리베이터의 명령 위치를 소프트 리미트 이내로 변경 후 운전해 주십시오.",

                                "UNLOADING ELEVATOR + SOFT-LIMIT",
                                "The command position of the unloading elevator exceed + soft limit value.",
                                "Check the position of the index pusher.",

                                "下料升降台软件上限值报警",
                                "下料升降台马达要移动到超出软件上限值",
                                "请把下料升降台移动位置修改为极限位置内",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4105,
                                "언로딩 엘리베이터 소프트리미트 하한 에러",
                                "언로딩 엘리베이터 모터가 소프트리미트 값 미만으로 운전하려고 합니다.",
                                "언로딩 엘리베이터의 명령 위치를 소프트 리미트 이내로 변경 후 운전해 주십시오.",

                                "UNLOADING ELEVATOR - SOFT-LIMIT",
                                "The command position of the unloading elevator exceed - soft limit value.",
                                "Check the position of the index pusher.",

                                "下料升降台软件下限值报警",
                                "下料升降台马达要移动到超出软件下限值",
                                "请把下料升降台移动位置修改为极限位置内",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4106,
                                "언로딩 엘리베이터 운전 중",
                                "언로딩 엘리베이터 모터가 운전 중입니다.",
                                "언로딩 엘리베이터 모터가 정지 후 명령을 수행할 수 있습니다.",

                                "UNLOADING ELEVATOR MOVING",
                                "The unloading elevator is moving.",
                                "You can not command during motor moving.",

                                "下料升降台运行中",
                                "下料升降台马达正在运行",
                                "请在下料升降台马达停止后再给予命令",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4107,
                                "로딩 엘리베이터 상한 리미트 에러",
                                "로딩 엘리베이터 모터 상한 리미트가 감지되었습니다.",
                                "로딩 엘리베이터의 위치를 확인해 주십시오.",

                                "LOADING ELEVATOR + LIMIT",
                                "The + soft limit sensor of the loading elevator is on.",
                                "Check the loading elevator servo.",

                                "上料升降台上限报警",
                                "上料升降台马达上限被感应到",
                                "请确认上料升降台位置",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4108,
                                "로딩 엘리베이터 하한 리미트 에러",
                                "로딩 엘리베이터 모터 하한 리미트가 감지되었습니다.",
                                "로딩 엘리베이터의 위치를 확인해 주십시오.",

                                "LOADING ELEVATOR - LIMIT",
                                "The - soft limit sensor of the loading elevator is on.",
                                "Check the loading elevator servo.",

                                "上料升降台下限报警",
                                "上料升降台马达下限被感应到",
                                "请确认上料升降台位置",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4109,
                                "로딩 푸셔 상한 리미트 에러",
                                "로딩 푸셔 모터 상한 리미트가 감지되었습니다.",
                                "로딩 푸셔의 위치를 확인해 주십시오.",

                                "LOADING PUSHER + LIMIT",
                                "The + soft limit sensor of the loading pusher is on.",
                                "Check the loading pusher servo.",

                                "上料推杆上限报警",
                                "上料推杆马达上限被感应到",
                                "请确认上料推杆位置",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4110,
                                "로딩 푸셔 하한 리미트 에러",
                                "로딩 푸셔 모터 하한 리미트가 감지되었습니다.",
                                "로딩 푸셔의 위치를 확인해 주십시오.",

                                "LOADING PUSHER - LIMIT",
                                "The - soft limit sensor of the loading pusher is on.",
                                "Check the loading pusher servo.",

                                "上料推杆下限报警",
                                "上料推杆马达下限被感应到",
                                "请确认上料推杆位置",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4111,
                                "인스펙션 모터 모터 상한 리미트 에러",
                                "인스펙션 모터 상한 리미트가 감지되었습니다.",
                                "인스펙션 모터의 위치를 확인해 주십시오.",

                                "INSPECTION STEP + LIMIT",
                                "The + soft limit sensor of the inspection step is on.",
                                "Check the inspection step.",

                                "检测马达上限报警",
                                "检测马达上限被感应到",
                                "请确认检测马达位置",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4112,
                                "인스펙션 모터 하한 리미트 에러",
                                "인스펙션 모터 하한 리미트가 감지되었습니다.",
                                "인스펙션 모터의 위치를 확인해 주십시오.",

                                "INSPECTION STEP - LIMIT",
                                "The - soft limit sensor of the inspection step is on.",
                                "Check the inspection step.",

                                "检测马达下限报警",
                                "检测马达下限被感应到",
                                "请确认检测马达位置",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4113,
                                "인덱스 푸셔 상한 리미트 에러",
                                "인덱스 푸셔 모터 상한 리미트가 감지되었습니다.",
                                "인덱스 푸셔의 위치를 확인해 주십시오.",

                                "INDEX PUSHER + LIMIT",
                                "The + soft limit sensor of the index pusher is on.",
                                "Check the index pusher servo.",

                                "步进推杆上限报警",
                                "步进推杆马达上限被感应到",
                                "请确认步进推杆位置",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4114,
                                "인덱스 푸셔 하한 리미트 에러",
                                "인덱스 푸셔 모터 하한 리미트가 감지되었습니다.",
                                "인덱스 푸셔의 위치를 확인해 주십시오.",

                                "INDEX PUSHER - LIMIT",
                                "The - soft limit sensor of the index pusher is on.",
                                "Check the loading index servo.",

                                "步进推杆下限报警",
                                "步进推杆马达下限被感应到",
                                "请确认步进推杆位置",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4115,
                                "언로딩 엘리베이터 상한 리미트 에러",
                                "언로딩 엘리베이터 모터 상한 리미트가 감지되었습니다.",
                                "언로딩 엘리베이터의 위치를 확인해 주십시오.",

                                "UNLOADING ELEVATOR + LIMIT",
                                "The + soft limit sensor of the unloading elevator is on.",
                                "Check the unloading elevator servo.",

                                "下料升降台上限报警",
                                "下料升降台马达上限被感应到",
                                "请确认下料升降台位置",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4116,
                                "언로딩 엘리베이터 하한 리미트 에러",
                                "언로딩 엘리베이터 모터 하한 리미트가 감지되었습니다.",
                                "언로딩 엘리베이터의 위치를 확인해 주십시오.",

                                "UNLOADING ELEVATOR - LIMIT",
                                "The - soft limit sensor of the unloading elevator is on.",
                                "Check the unloading elevator servo.",

                                "下料升降台下限报警",
                                "下料升降台马达下限被感应到",
                                "请确认下料升降台位置",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4117,
                                "가스1 공급 타임아웃",
                                "가스1 주입량이 설정범위내에 들지 못했습니다.",
                                "가스1 공급여부를 확인해 주십시오. MFC1을 확인해 주십시오.",

                                "GAS1 FLOW TIME-OUT",
                                "The injection of gas1 is over the setting range.",
                                "Check the input of the gas1. Check the MFC1.",

                                "气体1供应超时",
                                "气体1注入量不在设定范围内。",
                                "请确认是否供应了气体1，请检查MFC1",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4118,
                                "가스2 공급 타임아웃",
                                "가스2 주입량이 설정범위내에 들지 못했습니다.",
                                "가스2 공급여부를 확인해 주십시오. MFC2을 확인해 주십시오.",

                                "GAS2 FLOW TIME-OUT",
                                "The injection of gas2 is over the setting range.",
                                "Check the input of the gas2. Check the MFC2.",

                                "气体2供应超时",
                                "气体2注入量不在设定范围内。",
                                "请确认是否供应了气体2，请检查MFC2",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4119,
                                "가스3 공급 타임아웃",
                                "가스3 주입량이 설정범위내에 들지 못했습니다.",
                                "가스3 공급여부를 확인해 주십시오. MFC3을 확인해 주십시오.",

                                "GAS3 FLOW TIME-OUT",
                                "The injection of gas2 is over the setting range.",
                                "Check the input of the gas3. Check the MFC3.",

                                "气体3供应超时",
                                "气体3注入量不在设定范围内。",
                                "请确认是否供应了气体3，请检查MFC3",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4120,
                                "인덱스 모터 대기위치 에러",
                                "인덱스 모터가 대기 위치에 있지 않습니다.",
                                "인덱스 모터를 대기위치로 이동 후 작업해 주십시오.",

                                "INDEX MOTOR NOT READY POSITION",
                                "The index motor position is not ready-position.",
                                "Start after moving the index motor to the ready position.",

                                "步进马达待机位置报警",
                                "步进马达没有在待机位置",
                                "请把步进马达移动到待机位置后再作业",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4121,
                                "진공 펌프 타임아웃",
                                "설정된 시간동안 진공값이 범위에 들지 못했습니다.",
                                "진공펌프를 확인해 주십시오. 챔버의 리크를 확인해 주십시오.",

                                "VACUUM TIME-OUT",
                                "The vacuum value did not reach to setting value during set time.",
                                "Check the vacuum pump. Check the leakage of the chamber.",

                                "真空泵超时",
                                "在设定的时间内真空值没有达到范围",
                                "请确认真空泵，请检查腔体有无漏气",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4122,
                                "챔버 업 위치 에러",
                                "챔버가 업 위치에 있지 않습니다.",
                                "챔버 업 후에 작업해 주십시오. 챔버 업 실린더의 센서 위치를 확인해 주십시오.",

                                "CHAMBER IS NOT UP",
                                "The chamber up sensor signal is not on.",
                                "Check the chamber up cylinder. Start after moving the chamber to up position.",

                                "腔体打开位置报警",
                                "腔体买有在打开位置",
                                "请在腔体打开后再作业，请确认腔体打开气缸的感应器位置",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4123,
                                "로딩 푸셔 후진 위치 에러",
                                "로딩 푸셔가 후진 위치에 있지 않습니다.",
                                "로딩 푸셔를 후진위치로 이동후 작업해 주십시오.",

                                "LOADING PUSHER IS NOT RETRACT POSITION",
                                "The loading pusher is not retract position.",
                                "Start after moving the loading pusher to retract position.",

                                "上料推杆后退位置报警",
                                "上料推杆没有在后退位置",
                                "请把上料推杆移动到后退位置后再作业",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4124,
                                "메거진#1 로딩 에러",
                                "로딩 엘리베이터와 언로딩 엘리베이터의 메거진 로딩이 잘못되었습니다. 레인 #1",
                                "로딩 엘리베이터와 언로딩 엘리베이터에 메거진을 확인해 주십시오. 로딩엘리베이터에 메거진이 있(없)으면 언로딩 엘리베이터에도 메거진이 있(없)어야 합니다. 레인 #1",

                                "1ST LANE MAGAZINE MIS-LOADING",
                                "The magazines at loading and unloading elevator are mismatched. The 1st lane.",
                                "Check the magazines at loading and unloading elevator. The existance of the loading and the unloading magazines must be same. The 1st lane.",

                                "料盒#1上料报警",
                                "上料升降台与下料升降台料盒上料出错，#1",
                                "请确认上料升降台与下料升降台料盒状态，#1",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4125,
                                "메거진#2 로딩 에러",
                                "로딩 엘리베이터와 언로딩 엘리베이터의 메거진 로딩이 잘못되었습니다. 레인 #2",
                                "로딩 엘리베이터와 언로딩 엘리베이터에 메거진을 확인해 주십시오. 로딩엘리베이터에 메거진이 있(없)으면 언로딩 엘리베이터에도 메거진이 있(없)어야 합니다. 레인 #2",

                                "2ND LANE MAGAZINE MIS-LOADING",
                                "The magazines at loading and unloading elevator are mismatched. The 2nd lane.",
                                "Check the magazines at loading and unloading elevator. The existance of the loading and the unloading magazines must be same. The 2nd lane.",

                                "料盒#2上料报警",
                                "上料升降台与下料升降台料盒上料出错，#2",
                                "请确认上料升降台与下料升降台料盒状态，#2",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4126,
                                "메거진#3 로딩 에러",
                                "로딩 엘리베이터와 언로딩 엘리베이터의 메거진 로딩이 잘못되었습니다. 레인 #3",
                                "로딩 엘리베이터와 언로딩 엘리베이터에 메거진을 확인해 주십시오. 로딩엘리베이터에 메거진이 있(없)으면 언로딩 엘리베이터에도 메거진이 있(없)어야 합니다. 레인 #3",

                                "3RD LANE MAGAZINE MIS-LOADING",
                                "The magazines at loading and unloading elevator are mismatched. The 3rd lane.",
                                "Check the magazines at loading and unloading elevator. The existance of the loading and the unloading magazines must be same. The 3rd lane.",

                                "料盒#3上料报警",
                                "上料升降台与下料升降台料盒上料出错，#3",
                                "请确认上料升降台与下料升降台料盒状态，#3",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4127,
                                "메거진#4 로딩 에러",
                                "로딩 엘리베이터와 언로딩 엘리베이터의 메거진 로딩이 잘못되었습니다. 레인 #4",
                                "로딩 엘리베이터와 언로딩 엘리베이터에 메거진을 확인해 주십시오. 로딩엘리베이터에 메거진이 있(없)으면 언로딩 엘리베이터에도 메거진이 있(없)어야 합니다. 레인 #4",

                                "4TH LANE MAGAZINE MIS-LOADING",
                                "The magazines at loading and unloading elevator are mismatched. The 4th lane.",
                                "Check the magazines at loading and unloading elevator. The existance of the loading and the unloading magazines must be same. The 4th lane.",

                                "料盒#4上料报警",
                                "上料升降台与下料升降台料盒上料出错，#4",
                                "请确认上料升降台与下料升降台料盒状态，#4",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4128,
                                "메거진#5 로딩 에러",
                                "로딩 엘리베이터와 언로딩 엘리베이터의 메거진 로딩이 잘못되었습니다. 레인 #5",
                                "로딩 엘리베이터와 언로딩 엘리베이터에 메거진을 확인해 주십시오. 로딩엘리베이터에 메거진이 있(없)으면 언로딩 엘리베이터에도 메거진이 있(없)어야 합니다. 레인 #5",

                                "5TH LANE MAGAZINE MIS-LOADING",
                                "The magazines at loading and unloading elevator are mismatched. The 5th lane.",
                                "Check the magazines at loading and unloading elevator. The existance of the loading and the unloading magazines must be same. The 5th lane.",

                                "料盒#5上料报警",
                                "上料升降台与下料升降台料盒上料出错，#5",
                                "请确认上料升降台与下料升降台料盒状态，#5",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4129,
                                "검사기 뱅크 오류",
                                "검사기 뱅크값에 오류가 있습니다.",
                                "검사기 뱅크 값은 1에서 8사이의 값이어야 합니다.",

                                "VISION BANK ERROR",
                                "The vision bank vaule is wrong.",
                                "The bank value of the vision must be within 1~8.",

                                "VISION BANK ERROR",
                                "The vision bank vaule is wrong.",
                                "The bank value of the vision must be within 1~8.",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4130,
                                "로딩 버퍼 #1 자재 걸림",
                                "로딩 버퍼 #1의 자재가 정상적으로 챔버에 투입되지 않았습니다.",
                                "챔버 입구를 확인해 주십시오. 챔버 입구의 자재를 제거 후 작업해 주십시오.",

                                "LOADING BUFFER 1ST LANE JAMMING",
                                "The loading buffer 1st device check sensor time out occur.",
                                "Check the 1st loading buffer. Run after removing the device at 1st loading buffer.",

                                "上料缓冲区＃1卡料",
                                "上料缓冲区#1的产品没有正常的投入到腔体里",
                                "请确认腔体的入口，请把腔体入口的产品去除后再作业",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4131,
                                "로딩 버퍼 #2 자재 걸림",
                                "로딩 버퍼 #2의 자재가 정상적으로 챔버에 투입되지 않았습니다.",
                                "챔버 입구를 확인해 주십시오. 챔버 입구의 자재를 제거 후 작업해 주십시오.",

                                "LOADING BUFFER 2ND LANE JAMMING",
                                "The loading buffer 2nd device check sensor time out occur.",
                                "Check the 2nd loading buffer. Run after removing the device at 2nd loading buffer.",

                                "上料缓冲区＃2卡料",
                                "上料缓冲区#2的产品没有正常的投入到腔体里",
                                "请确认腔体的入口，请把腔体入口的产品去除后再作业",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4132,
                                "로딩 버퍼 #3 자재 걸림",
                                "로딩 버퍼 #3의 자재가 정상적으로 챔버에 투입되지 않았습니다.",
                                "챔버 입구를 확인해 주십시오. 챔버 입구의 자재를 제거 후 작업해 주십시오.",

                                "LOADING BUFFER 3RD LANE JAMMING",
                                "The loading buffer 3rd device check sensor time out occur.",
                                "Check the 3rd loading buffer. Run after removing the device at 3rd loading buffer.",

                                "上料缓冲区＃3卡料",
                                "上料缓冲区#3的产品没有正常的投入到腔体里",
                                "请确认腔体的入口，请把腔体入口的产品去除后再作业",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4133,
                                "로딩 버퍼 #4 자재 걸림",
                                "로딩 버퍼 #4의 자재가 정상적으로 챔버에 투입되지 않았습니다.",
                                "챔버 입구를 확인해 주십시오. 챔버 입구의 자재를 제거 후 작업해 주십시오.",

                                "LOADING BUFFER 4TH LANE JAMMING",
                                "The loading buffer 4th device check sensor time out occur.",
                                "Check the 4th loading buffer. Run after removing the device at 4th loading buffer.",

                                "上料缓冲区＃4卡料",
                                "上料缓冲区#4的产品没有正常的投入到腔体里",
                                "请确认腔体的入口，请把腔体入口的产品去除后再作业",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4134,
                                "로딩 버퍼 #5 자재 걸림",
                                "로딩 버퍼 #5의 자재가 정상적으로 챔버에 투입되지 않았습니다.",
                                "챔버 입구를 확인해 주십시오. 챔버 입구의 자재를 제거 후 작업해 주십시오.",

                                "LOADING BUFFER 5TH LANE JAMMING",
                                "The loading buffer 5th device check sensor time out occur.",
                                "Check the 5th loading buffer. Run after removing the device at 5th loading buffer.",

                                "上料缓冲区＃5卡料",
                                "上料缓冲区#5的产品没有正常的投入到腔体里",
                                "请确认腔体的入口，请把腔体入口的产品去除后再作业",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4135,
                                "메거진 없음",
                                "로딩/언로딩 테이블에 메거진이 없습니다.",
                                "로딩/언로딩 테이블에 자재를 놓고 작업하십시오.",

                                "NO MGZ.",
                                "There is no magazines on the loading and unloading table.",
                                "Put magazine(s) on the loading/unloading table.",

                                "没有料盒",
                                "上料/下料平台没有料盒",
                                "请在上料/下料平台放产品后作业",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4190,
                                "RF 제너레이터 출력 범위 넘음",
                                "RF 제너레이터의 출력이 설정값보다 높습니다.",
                                "RF 제너레이터를 확인해 주십시오. RF 제너레이터를 다시 설정해 주십시오.",

                                "RF GENERATOR OUTPUT OVER RANGE",
                                "The output of the RF-Generator exceeded the range.",
                                "Check the RF-Generator. Tune the Rf-Generator, reading the manual.",

                                "射频发生器超出输出范围",
                                "射频发生器的输出比设定值高出",
                                "请确认射频发生器，请重新设定射频发生器",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4191,
                                "질소 공급압력 낮음",
                                "질소 압력 신호를 감지하지 못했습니다.",
                                "질소 공급여부를 확인해 주십시오. 질소 압력체크 센서를 점검해 주십시오.",

                                "N2 PRESSURE LOW",
                                "The pressure of N2 is low",
                                "Check the inlet of N2. Check the N2 pressure sensor.",

                                "氮气压力供应低",
                                "没有感应到氮气压力信号",
                                "请确认是否有供应氮气，请检查氮气传感器",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4200,
                                "스트립 ID 오류",
                                "호스트로부터 스트립 ID 오류 메시지를 수신했습니다.",
                                "",

                                "WRONG STRIP ID",
                                "Received wrong strip id from the host.",
                                "",

                                "STRIP ID检查错误",
                                "伺服收到了STRIP ID错误的信息",
                                "请去除STRIP后再进行",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4201,
                                "스트립 ID 확인 시간 초과",
                                "호스트로부터 스트립 ID 확인 메시지를 받지 못했습니다.",
                                "",

                                "STRIP ID CONFIRM TIMEOUT",
                                "Cannot received confirm message from the host.",
                                "",

                                "STRIP ID检查超时",
                                "伺服没有收到STRIP ID检查结果",
                                "请去除STRIP后再进行",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4202,
                                "통신 연결 끊김",
                                "통신이 끊겼습니다.",
                                "",

                                "COMMUNICATION DISCONNECTED",
                                "The communication port is disconnected.",
                                "",

                                "CIM通信连接异常",
                                "CIM连接中感应到了线路异常信号",
                                "请确认通信连接",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4203,
                                "인덱스 푸셔 언로딩 시작위치 포지션  알람",
                                "인덱스 푸셔의 언로딩 시작위치 포지션이 로딩 시작위치 보다 작습니다.",
                                "언로딩 시작 위치 포지션을 로딩 시작위치 보다 크게 설정 하십시오.",

                                "Index pusher unloading start position alarm",
                                "The unloading start position of the index pusher is smaller than the loading start position.",
                                "Set the unloading start position to be larger than the loading start position.",

                                "Index pusher unloading start position alarm",
                                "The unloading start position of the index pusher is smaller than the loading start position.",
                                "Set the unloading start position to be larger than the loading start position.",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_4300,
                                "챔버 출구 자재 감지 센서 알람",
                                "챔버 출구 자재 감지 센서에 자재가 감지 되지 않았습니다.",
                                "챔버 아웃 자재 감지 센서를 확인 해 주십시오.",

                                "CHAMBER OUT material Detection Sensor Alarm",
                                "No material detected on chamber out material sensor.",
                                "Check chamber out material detection sensor.",

                                "CHAMBER OUT material Detection Sensor Alarm",
                                "No material detected on chamber out material sensor.",
                                "Check chamber out material detection sensor.",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5000,
                                "로딩 메거진 틀어짐 #1",
                                "로딩 메거진 감지 센서 이상",
                                "로딩 메거진 확인",

                                "LOADING MGZ. TILT ERROR #1",
                                "The loading mgz. Detect sensor signal is abnormal.",
                                "Check the mgz. Loading state of the loading elevator.",

                                "上料料盒倾斜#1",
                                "上料料盒感应器异常",
                                "请确认上料料盒",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5001,
                                "로딩 메거진 틀어짐 #2",
                                "로딩 메거진 감지 센서 이상",
                                "로딩 메거진 확인",

                                "LOADING MGZ. TILT ERROR #2",
                                "The loading mgz. Detect sensor signal is abnormal.",
                                "Check the mgz. Loading state of the loading elevator.",

                                "上料料盒倾斜#2",
                                "上料料盒感应器异常",
                                "请确认上料料盒",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5002,
                                "로딩 메거진 틀어짐 #3",
                                "로딩 메거진 감지 센서 이상",
                                "로딩 메거진 확인",

                                "LOADING MGZ. TILT ERROR #3",
                                "The loading mgz. Detect sensor signal is abnormal.",
                                "Check the mgz. Loading state of the loading elevator.",

                                "上料料盒倾斜#3",
                                "上料料盒感应器异常",
                                "请确认上料料盒",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5003,
                                "로딩 메거진 틀어짐 #4",
                                "로딩 메거진 감지 센서 이상",
                                "로딩 메거진 확인",

                                "LOADING MGZ. TILT ERROR #4",
                                "The loading mgz. Detect sensor signal is abnormal.",
                                "Check the mgz. Loading state of the loading elevator.",

                                "上料料盒倾斜#4",
                                "上料料盒感应器异常",
                                "请确认上料料盒",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5004,
                                "언로딩 메거진 틀어짐 #1",
                                "언로딩 메거진 감지 센서 이상",
                                "언로딩 메거진 확인",

                                "UNLOADING MGZ. TILT ERROR #1",
                                "The unloading mgz. Detect sensor signal is abnormal.",
                                "Check the mgz. Loading state of the unloading elevator.",

                                "下料料盒倾斜#1",
                                "下料料盒感应器异常",
                                "请确认下料料盒",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5005,
                                "언로딩 메거진 틀어짐 #2",
                                "언로딩 메거진 감지 센서 이상",
                                "언로딩 메거진 확인",

                                "UNLOADING MGZ. TILT ERROR #2",
                                "The unloading mgz. Detect sensor signal is abnormal.",
                                "Check the mgz. Loading state of the unloading elevator.",

                                "下料料盒倾斜#2",
                                "下料料盒感应器异常",
                                "请确认下料料盒",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5006,
                                "언로딩 메거진 틀어짐 #3",
                                "언로딩 메거진 감지 센서 이상",
                                "언로딩 메거진 확인",

                                "UNLOADING MGZ. TILT ERROR #3",
                                "The unloading mgz. Detect sensor signal is abnormal.",
                                "Check the mgz. Loading state of the unloading elevator.",

                                "下料料盒倾斜#3",
                                "下料料盒感应器异常",
                                "请确认下料料盒",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5007,
                                "언로딩 메거진 틀어짐 #4",
                                "언로딩 메거진 감지 센서 이상",
                                "언로딩 메거진 확인",

                                "UNLOADING MGZ. TILT ERROR #4",
                                "The unloading mgz. Detect sensor signal is abnormal.",
                                "Check the mgz. Loading state of the unloading elevator.",

                                "下料料盒倾斜#4",
                                "下料料盒感应器异常",
                                "请确认下料料盒",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5008,
                                "이오나이저 알람",
                                "이오나이저 알람 신호 감지",
                                "이오나이저 점검",

                                "IONIZER ALARM",
                                "The ionizer alarm signal is detected.",
                                "Check the ionizer.",

                                "离子风扇报警",
                                "感应到了离子风扇报警信号",
                                "请检查离子风扇",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5009,
                                "챔버 높이 알람",
                                "챔버 높이 감지 신호 에러",
                                "선택된 레시피와 챔버 높이 감지 센서의 신호가 다릅니ㅏ.",

                                "WRONG FLOATING LANE INFO.",
                                "The floating lane detecting sensor signal is different from recipe value.",
                                "Check the kind of chamber lane. Check the recipe value.",

                                "腔体高度报警",
                                "腔体高度感应信号报警",
                                "选定的程序与腔体高度感应器信号不同",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5010,
                                "로딩 푸셔 오버로드 2",
                                "로딩 푸셔 오버로드 센서가 감지되었습니다.",
                                "로딩 푸셔에 부하가 걸렸는지 확인해 주십시오. 부하의 원인을 제거 후 운전해 주십시오.",

                                "LOADING PUSHER OVERLOAD 2",
                                "The loading pusher overload sensor signal was detected.",
                                "Check the load of loading pusher. Remove the obstacle if exists.",

                                "上料推杆过载#2",
                                "上料推杆过载传感器感应到了",
                                "请检查上料推杆是否处于负载状态。 消除负载的原因后再运行。",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5011,
                                "로딩 푸셔 오버로드 3",
                                "로딩 푸셔 오버로드 센서가 감지되었습니다.",
                                "로딩 푸셔에 부하가 걸렸는지 확인해 주십시오. 부하의 원인을 제거 후 운전해 주십시오.",

                                "LOADING PUSHER OVERLOAD 3",
                                "The loading pusher overload sensor signal was detected.",
                                "Check the load of loading pusher. Remove the obstacle if exists.",

                                "上料推杆过载#3",
                                "上料推杆过载传感器感应到了",
                                "请检查上料推杆是否处于负载状态。 消除负载的原因后再运行。",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5012,
                                "로딩 푸셔 오버로드 4",
                                "로딩 푸셔 오버로드 센서가 감지되었습니다.",
                                "로딩 푸셔에 부하가 걸렸는지 확인해 주십시오. 부하의 원인을 제거 후 운전해 주십시오.",

                                "LOADING PUSHER OVERLOAD 4",
                                "The loading pusher overload sensor signal was detected.",
                                "Check the load of loading pusher. Remove the obstacle if exists.",

                                "上料推杆过载#4",
                                "上料推杆过载传感器感应到了",
                                "请检查上料推杆是否处于负载状态。 消除负载的原因后再运行。",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5013,
                                "로딩 푸셔 오버로드 5",
                                "로딩 푸셔 오버로드 센서가 감지되었습니다.",
                                "로딩 푸셔에 부하가 걸렸는지 확인해 주십시오. 부하의 원인을 제거 후 운전해 주십시오.",

                                "LOADING PUSHER OVERLOAD 5",
                                "The loading pusher overload sensor signal was detected.",
                                "Check the load of loading pusher. Remove the obstacle if exists.",

                                "上料推杆过载#5",
                                "上料推杆过载传感器感应到了",
                                "请检查上料推杆是否处于负载状态。 消除负载的原因后再运行。",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5014,
                                "인덱스 푸셔 #5 오버로드",
                                "인덱스 푸셔 #5 오버로드 센서가 감지되었습니다.",
                                "인덱스 푸셔 #5에 부하가 걸렸는지 확인해 주십시오. 부하의 원인을 제거 후 운전해 주십시오.",

                                "INDEX PUSHER #5 OVERLOAD",
                                "The index pusher #5 overload sensor signal was detected.",
                                "Check the load of index pusher #5. Remove the obstacle if exists.",

                                "步进推杆#5过载",
                                "步进推杆#5感应到了过载感应器",
                                "请检查步进推杆#5是否处于负载状态。 消除负载的原因后再运行。",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5015,
                                "챔버 입구 자재 걸림",
                                "챔버 입구에서 자재가 감지되었습니다.",
                                "챔버 입구의 자재를 제거 후 진행해 주십시오.",

                                "CHAMBER IN DEVICE DETECT",
                                "The chamber in device detect sensor is on",
                                "Resume after removing the device at the chamber in.",

                                "腔体入口卡料",
                                "腔体入口感应到了产品",
                                "请去除腔体入口的产品后再进行",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5016,
                                "챔버 출구 자재 걸림",
                                "챔버 출구구에서 자재가 감지되었습니다.",
                                "챔버 출구의 자재를 제거 후 진행해 주십시오.",

                                "CHAMBER OUT DEVICE DETECT",
                                "The chamber out device detect sensor is on",
                                "Resume after removing the device at the chamber out.",

                                "腔体出口卡料",
                                "腔体出口感应到了产品",
                                "请去除腔体出口的产品后再进行",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5017,
                                "언로딩 푸셔 #2 오버로드",
                                "언로딩 푸셔 오버로드 센서가 감지되었습니다.",
                                "언로딩 푸셔를 확인해 주십시오.",

                                "UNLOADING PUSHER #2 OVERLOAD",
                                "The unload pusher overload signal was detected.",
                                "Check the unloading pusher.",

                                "下料推杆过载#2",
                                "下料推杆过载传感器感应到了",
                                "请检查上料推杆是否处于负载状态。 消除负载的原因后再运行。",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5018,
                                "언로딩 푸셔 #3 오버로드",
                                "언로딩 푸셔 오버로드 센서가 감지되었습니다.",
                                "언로딩 푸셔를 확인해 주십시오.",

                                "UNLOADING PUSHER #3 OVERLOAD",
                                "The unload pusher overload signal was detected.",
                                "Check the unloading pusher.",

                                "下料推杆过载#3",
                                "下料推杆过载传感器感应到了",
                                "请检查上料推杆是否处于负载状态。 消除负载的原因后再运行。",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5019,
                                "언로딩 푸셔 #4 오버로드",
                                "언로딩 푸셔 오버로드 센서가 감지되었습니다.",
                                "언로딩 푸셔를 확인해 주십시오.",

                                "UNLOADING PUSHER #4 OVERLOAD",
                                "The unload pusher overload signal was detected.",
                                "Check the unloading pusher.",

                                "下料推杆过载#4",
                                "下料推杆过载传感器感应到了",
                                "请检查上料推杆是否处于负载状态。 消除负载的原因后再运行。",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5020,
                                "언로딩 푸셔 #5 오버로드",
                                "언로딩 푸셔 오버로드 센서가 감지되었습니다.",
                                "언로딩 푸셔를 확인해 주십시오.",

                                "UNLOADING PUSHER #5 OVERLOAD",
                                "The unload pusher overload signal was detected.",
                                "Check the unloading pusher.",

                                "下料推杆过载#5",
                                "下料推杆过载传感器感应到了",
                                "请检查上料推杆是否处于负载状态。 消除负载的原因后再运行。",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5021,
                                "질소 공급압력 낮음",
                                "질소 압력 신호를 감지하지 못했습니다.",
                                "질소 공급여부를 확인해 주십시오. 질소 압력체크 센서를 점검해 주십시오.",

                                "N2 PRESSURE LOW",
                                "The pressure of N2 is low",
                                "Check the inlet of N2. Check the N2 pressure sensor.",

                                "氮气供应压力低",
                                "没有感应到氮气压力信号",
                                "请确认氮气是否有供应，请检查氮气压力检测传感器",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5022,
                                "로딩 메거진 틀어짐",
                                "로딩 메거진 감지 센서 이상",
                                "로딩 메거진 확인",

                                "LOADING MGZ. TILT ERROR",
                                "The loading mgz. Detect sensor signal is abnormal.",
                                "Check the mgz. Loading state of the loading elevator.",

                                "LOADING MGZ. TILT ERROR",
                                "The loading mgz. Detect sensor signal is abnormal.",
                                "Check the mgz. Loading state of the loading elevator.",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5023,
                                "언로딩 메거진 틀어짐",
                                "언로딩 메거진 감지 센서 이상",
                                "언로딩 메거진 확인",

                                "UNLOADING MGZ. TILT ERROR",
                                "The unloading mgz. Detect sensor signal is abnormal.",
                                "Check the mgz. Loading state of the unloading elevator.",

                                "UNLOADING MGZ. TILT ERROR",
                                "The unloading mgz. Detect sensor signal is abnormal.",
                                "Check the mgz. Loading state of the unloading elevator.",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5024,
                                "언로딩 이오나이저 알람",
                                "이오나이저 알람 신호 감지",
                                "이오나이저 점검",

                                "Unloading Ionizer Alarm",
                                "The ionizer alarm signal is detected.",
                                "Check the ionizer.",

                                "离子风扇报警",
                                "感应到了离子风扇报警信号",
                                "请检查离子风扇",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        alarms.Add(CreateAlarm(
                                eErrCode.Ecode_5025,
                                "후면 도어 열림",
                                "후면 안전문이 열렸습니다.",
                                "후면 안전문을 닫고 계속해 주십시오.",

                                "REAR DOOR OPEN",
                                "The rear door is open.",
                                "Start after closing door.",

                                "REAR DOOR OPEN",
                                "The rear door is open.",
                                "Start after closing door.",

                                "// TODO: Vietnamese Title",
                                "// TODO: Vietnamese Description",
                                "// TODO: Vietnamese Guide"
                        ));

        return alarms;
    }

    private static ErrorItem CreateAlarm(eErrCode errCode, string name, string cause, string action, string name_E, string cause_E, string action_E, string name_C, string cause_C, string action_C, string name_V, string cause_V, string action_V)
    {
        ErrorItem result = new();
        result.Code = (int)errCode;
        result.Name = name;
        result.Cause = cause;
        result.Action = action;
        result.Name_E = name_E;
        result.Cause_E = cause_E;
        result.Action_E = action_E;
        result.Name_C = name_C;
        result.Cause_C = cause_C;
        result.Action_C = action_C;
        result.Name_V = name_V;
        result.Cause_V = cause_V;
        result.Action_V = action_V;

        return result;
    }

    public static List<string> GenerateAlarmCodeFromExcel(string filePath)
    {
        var result = new List<string>();

        using (var workbook = new XLWorkbook(filePath))
        {
            var ws = workbook.Worksheet(1);
            int row = 2;

            while (!ws.Cell(row, 1).IsEmpty())
            {
                string code = ws.Cell(row, 1).GetValue<string>().Trim();

                string koTitle = ws.Cell(row, 2).GetValue<string>().Trim();
                string koDesc = ws.Cell(row, 3).GetValue<string>().Trim();
                string koGuide = ws.Cell(row, 4).GetValue<string>().Trim();

                string enTitle = ws.Cell(row, 5).GetValue<string>().Trim();
                string enDesc = ws.Cell(row, 6).GetValue<string>().Trim();
                string enGuide = ws.Cell(row, 7).GetValue<string>().Trim();

                string zhTitle = ws.Cell(row, 8).GetValue<string>().Trim();
                string zhDesc = ws.Cell(row, 9).GetValue<string>().Trim();
                string zhGuide = ws.Cell(row, 10).GetValue<string>().Trim();

                string viTitle = "// TODO: Vietnamese Title";
                string viDesc = "// TODO: Vietnamese Description";
                string viGuide = "// TODO: Vietnamese Guide";

                string codeLine = $@"
                    alarms.Add(CreateAlarm(
                        eErrCode.Ecode_{code},
                        ""{koTitle}"",
                        ""{koDesc}"",
                        ""{koGuide}"",

                        ""{enTitle}"",
                        ""{enDesc}"",
                        ""{enGuide}"",

                        ""{zhTitle}"",
                        ""{zhDesc}"",
                        ""{zhGuide}"",

                        ""{viTitle}"",
                        ""{viDesc}"",
                        ""{viGuide}""
                ));";

                result.Add(codeLine.Trim());
                row++;
            }
        }

        result = result.Where(x => !x.Contains(" \"\",\r\n                        \"\",\r\n                        \"\",\r\n\r\n                        \"\",\r\n                        \"\",\r\n                        \"\",\r\n\r\n                        \"\",\r\n                        \"\",\r\n                        \"\",\r\n\r\n                        \"// TODO: Vietnamese Title\",\r\n                        \"// TODO: Vietnamese Description\",\r\n                        \"// TODO: Vietnamese Guide\"\r\n")).ToList();

        string fullCode = string.Join("\n\n", result);

        return result;
    }
}
