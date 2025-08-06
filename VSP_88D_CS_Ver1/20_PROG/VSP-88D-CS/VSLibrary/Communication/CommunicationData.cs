using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using VSLibrary.Common.MVVM.ViewModels;

namespace VSLibrary.Communication
{
    /// <summary>
    /// 개별 통신 설정 정보
    /// </summary>
    [ObservableObject]
    public partial class CommunicationConfig : ICommunicationConfig
    {
        // 통신 장비 식별 이름
        [ObservableProperty]
        private string _communicationName;

        // 통신 방식
        [ObservableProperty]
        private CommunicationTarget _target;

        // Serial 전용 설정
        [ObservableProperty]
        private string _portName;

        [ObservableProperty]
        private int _baudRate;

        [ObservableProperty]
        private Parity _parity;

        [ObservableProperty]
        private int _dataBits;

        [ObservableProperty]
        private StopBits _stopBits;

        // TCP 전용 설정
        [ObservableProperty]
        private string _host;

        [ObservableProperty]
        private int _port;

        [ObservableProperty]
        private bool _backgroundPacket = true;      

    }
}
