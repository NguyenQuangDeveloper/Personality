using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using VSLibrary.Communication;

namespace VSLibrary.Communication.Serial
{
    /// <summary>
    /// 시리얼 통신 전용 추상 클래스
    /// (CommunicationBase 공통 로직 활용)
    /// </summary>
    public abstract class SerialBase : CommunicationBase
    {
        protected readonly SerialPort _serialPort;

        protected SerialBase(ICommunicationConfig config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            _serialPort = new SerialPort(config.PortName, config.BaudRate, config.Parity, config.DataBits, config.StopBits);
            
            _serialPort.DataReceived += DataReceivedHandler;
        }

        /// <summary>
        /// 실제 쓰기: SerialPort에 데이터 작성
        /// </summary>
        protected override Task WriteCoreAsync(byte[] data, CancellationToken cancellationToken)
        {
            if (!_serialPort.IsOpen)
            {
                EventMessage(Config.CommunicationName, CommunicationEventType.TxError, "Serial port is not open.");
                //Console.WriteLine("Serial port is not open.");
                return Task.CompletedTask;
            }
            _serialPort.Write(data, 0, data.Length);
            return Task.CompletedTask;
        }        

        /// <summary>
        /// 포트 열기 및 주기 작업 시작
        /// </summary>
        public override Task OpenAsync(CancellationToken cancellationToken = default)
            => Task.Run(async () =>
            {
                try
                {
                    if (!_serialPort.IsOpen)
                    {
                        _serialPort.Open();
                        IsOpen = true;
                        EventMessage(Config.CommunicationName, CommunicationEventType.Connected, $"Serial Open {Config.PortName}");
                        
                        if (ShouldAutoInitialize)
                        {
                            await InitializeAsync();
                            OnDoworkAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine($"[SerialBase] OpenAsync 실패 ({Config.PortName}): {ex.Message}");
                    EventMessage(Config.CommunicationName, CommunicationEventType.ConnectionError, $"DataReceived handler threw exception : {ex.Message}");
                    //OnError($"[SerialBase] OpenAsync 실패 ({Config.PortName}): {ex.Message}");
                    IsOpen = false;
                }
            }, cancellationToken);

        /// <summary>
        /// 포트 닫기 및 리소스 해제
        /// </summary>
        public override Task CloseAsync(CancellationToken cancellationToken = default)
        {
            OffDoworkAsync();
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
                IsOpen = false;
                EventMessage(Config.CommunicationName, CommunicationEventType.Disconnected, $"Serial close {Config.PortName}");
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 수신된 바이트를 CommunicationBase.ProcessReceivedBytes로 전달
        /// </summary>
        protected void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int count = _serialPort.BytesToRead;
                var chunk = new byte[count];
                _serialPort.Read(chunk, 0, count);
                ProcessReceivedBytes(chunk);
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"[SerialBase] DataReceived error ({Config.PortName}): {ex}");
                EventMessage(Config.CommunicationName, CommunicationEventType.RxError, $"DataReceived error ({Config.PortName}): {ex}");
                //OnError($"[SerialBase] DataReceived error ({Config.PortName}): {ex}");
            }
        }

        /// <summary>
        /// 필요 시 오버라이드
        /// </summary>
        protected virtual async Task DoWorkAsync() { }        

        public override Task OnDoworkAsync(CancellationToken cancellationToken = default)
        {
            if (_isWorking)
                return Task.CompletedTask;

            _doWorkCts = new CancellationTokenSource();
            _isWorking = true;
            _ = DoWorkAsync().ContinueWith(t =>
            {
                // 작업이 끝나거나 중단되면 상태 초기화
                _isWorking = false;
            }, TaskScheduler.Default);

            return Task.CompletedTask;
        }

        public override Task OffDoworkAsync(CancellationToken cancellationToken = default)
        {
            if (!_isWorking || _doWorkCts == null || _doWorkCts.IsCancellationRequested)
                return Task.CompletedTask;

            try { _doWorkCts.Cancel(); } catch { }

            return Task.CompletedTask;
        }
    }
}