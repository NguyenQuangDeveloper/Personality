using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using VSLibrary.Communication;

namespace VSLibrary.Communication.Socket
{
    public abstract class SocketServerBase : CommunicationBase
    {
        private readonly TcpListener _listener;
        private TcpClient? _client;
        private NetworkStream? _stream;

        private CancellationTokenSource? _acceptLoopCts;
        private bool _accepting = false;

        protected bool IsClientConnected => _client?.Connected == true;

        protected SocketServerBase(ICommunicationConfig config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            _listener = new TcpListener(IPAddress.Loopback, config.Port);
        }

        protected override async Task WriteCoreAsync(byte[] data, CancellationToken cancellationToken)
        {
            if (_stream == null || !IsClientConnected)
            {
                EventMessage(Config.CommunicationName, CommunicationEventType.TxError, "클라이언트가 연결되어 있지 않습니다.");
                return;
            }

            try
            {
                await _stream.WriteAsync(data, 0, data.Length, cancellationToken);
            }
            catch (Exception ex)
            {
                EventMessage(Config.CommunicationName, CommunicationEventType.TxError, $"전송 오류: {ex.Message}");
            }
        }

        public override Task OpenAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                if (IsOpen)
                {
                    return;
                }

                try
                {
                    _listener.Start();
                    _acceptLoopCts = new CancellationTokenSource();
                    _ = AcceptLoopAsync(_acceptLoopCts.Token);
                    _ = OnDoworkAsync();
                    IsOpen = true;

                    EventMessage(Config.CommunicationName, CommunicationEventType.Connected, "서버 시작됨.");
                }
                catch (Exception ex)
                {
                    EventMessage(Config.CommunicationName, CommunicationEventType.ConnectionError, $"서버 시작 실패: {ex.Message}");
                    IsOpen = false;
                }
            }, cancellationToken);
        }

        public override Task CloseAsync(CancellationToken cancellationToken = default)
        {
            OffDoworkAsync();

            try
            {
                _acceptLoopCts?.Cancel();
                _acceptLoopCts?.Dispose();
                _acceptLoopCts = null;

                _stream?.Close();
                _client?.Close();
                _stream = null;
                _client = null;

                _listener.Stop();
            }
            catch (Exception ex)
            {
                EventMessage(Config.CommunicationName, CommunicationEventType.ConnectionError, $"닫는 중 예외: {ex.Message}");
            }

            IsOpen = false;
            EventMessage(Config.CommunicationName, CommunicationEventType.Disconnected, "서버 중지됨.");
            return Task.CompletedTask;
        }

        private async Task AcceptLoopAsync(CancellationToken token)
        {
            if (_accepting)
                return;

            _accepting = true;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (_client == null || !_client.Connected)
                    {
                        _client = await _listener.AcceptTcpClientAsync(token);
                        _stream = _client.GetStream();
                        EventMessage(Config.CommunicationName, CommunicationEventType.Connected, "클라이언트 연결됨.");
                    }

                    var socket = _client.Client;
                    if (socket.Poll(0, SelectMode.SelectRead) && socket.Available == 0)
                    {
                        _stream?.Close();
                        _client?.Close();
                        _stream = null;
                        _client = null;

                        EventMessage(Config.CommunicationName, CommunicationEventType.Disconnected, "클라이언트 연결 끊김.");
                    }

                    if (_stream != null && _stream.DataAvailable)
                    {
                        var buffer = new byte[_client.Available];
                        var bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, token);
                        if (bytesRead > 0)
                            ProcessReceivedBytes(buffer);
                    }

                    await Task.Delay(10, token);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                EventMessage(Config.CommunicationName, CommunicationEventType.RxError, $"수신 오류: {ex.Message}");
            }
            finally
            {
                _accepting = false;
            }
        }

        protected virtual async Task DoWorkAsync() { }

        public override Task OnDoworkAsync(CancellationToken cancellationToken = default)
        {
            if (_isWorking)
                return Task.CompletedTask;

            _doWorkCts = new CancellationTokenSource();
            _isWorking = true;

            _ = DoWorkAsync().ContinueWith(t =>
            {
                _isWorking = false;
            }, TaskScheduler.Default);

            return Task.CompletedTask;
        }

        public override Task OffDoworkAsync(CancellationToken cancellationToken = default)
        {
            if (!_isWorking || _doWorkCts?.IsCancellationRequested == true)
                return Task.CompletedTask;

            try { _doWorkCts?.Cancel(); } catch { }

            return Task.CompletedTask;
        }
    }
}
