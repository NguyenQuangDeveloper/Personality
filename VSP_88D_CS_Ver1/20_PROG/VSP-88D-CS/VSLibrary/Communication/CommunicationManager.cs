using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VSLibrary.Communication.Packet.Protocol.RFGenerator;
using VSLibrary.Communication.Packet.Protocol.Test;
using VSLibrary.Communication.Serial;
using VSLibrary.Communication.Socket;
using VSLibrary.Controller;

namespace VSLibrary.Communication
{
    public class CommunicationManager
    {
        public Dictionary<string, ICommunication> Communication = new Dictionary<string, ICommunication>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// key: 사용자 식별 키, target: 통신 대상 열거형
        /// </summary>
        /// <param name="config">통신 키와 대상 매핑</param>
        public CommunicationManager(IDictionary<string, ICommunicationConfig> configMap)
        {
            if (configMap == null)
                throw new ArgumentNullException(nameof(configMap));

            foreach (var kv in configMap)
            {
                if (kv.Value is CommunicationConfig cfg)
                    cfg.CommunicationName = kv.Key;
            }

            foreach (var kv in configMap)
            {
                var key = kv.Key;
                var cfg = kv.Value;
                ICommunication comm;

                switch (cfg.Target)
                {
                    case CommunicationTarget.TestSerial:
                        comm = new TestSerial(cfg);
                        break;

                    case CommunicationTarget.TestSocket:
                        comm = new TestSocket(cfg);
                        break;

                    case CommunicationTarget.TestSocketServer:
                        comm = new TestSocketServer(cfg);
                        break;

                    case CommunicationTarget.TestModbusASCII:
                        comm = new TestModbusASCII(cfg);
                        break;

                    case CommunicationTarget.TestModbusRTU:
                        comm = new TestModbusRTU(cfg);
                        break;

                    case CommunicationTarget.TestModbusTCP:
                        comm = new TestModbusTCP(cfg);
                        break;

                    case CommunicationTarget.PlasourceRFGenerator:
                        comm = new PlasourceRFGenerator(cfg);                        
                        break;

                    case CommunicationTarget.YoungshinRFGenerator:
                        comm = new YoungshinRFGenerator(cfg);
                        break;

                    default:
                        // 지원하지 않는 Target은 건너뛴다
                        continue;
                }

                comm.MethodList = MakeMethodsList(comm.GetType());

                // 설정 주입
                comm.Config = cfg;
                // 필요시 자동 오픈: comm.OpenAsync().Wait();

                Communication[key] = comm;
            }
        }

        /// <summary>
        /// 생성된 ICommunication 인스턴스를 key로 가져옵니다.
        /// </summary>
        /// <param name="key">생성 시 사용한 키</param>
        /// <returns>ICommunication 인스턴스</returns>
        public ICommunication Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("key는 null 또는 공백이 될 수 없습니다.", nameof(key));

            if (Communication.TryGetValue(key, out var comm))
                return comm;

            throw new KeyNotFoundException($"통신 키 '{key}'에 해당하는 인스턴스가 없습니다.");
        }

        /// <summary>
        /// 모든 채널을 비동기로 엽니다.
        /// </summary>
        public Task OpenAllAsync(CancellationToken cancellationToken = default)
        {
            var openTasks = new List<Task>();
            foreach (var comm in Communication.Values)
            {
                openTasks.Add(comm.OpenAsync(cancellationToken));
            }
            return Task.WhenAll(openTasks);
        }

        /// <summary>
        /// 모든 채널을 비동기로 닫습니다.
        /// </summary>
        public Task CloseAllAsync(CancellationToken cancellationToken = default)
        {
            var closeTasks = new List<Task>();
            foreach (var comm in Communication.Values)
            {
                closeTasks.Add(comm.CloseAsync(cancellationToken));
            }
            return Task.WhenAll(closeTasks);
        }

        private List<string> MakeMethodsList(Type Type)
        {
            var methods = Type
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            //methods.Where(m => m.GetParameters().Length == 0 || m.GetParameters().All(p => p.HasDefaultValue))
            return methods
                .Where(m => typeof(Task).IsAssignableFrom(m.ReturnType)) // Task 또는 Task<T>
                .Select(m => m.Name)
                .Distinct()
                .ToList();
        }

        public async Task CallMethodAsync(string key, string methodName, string[] args)
        {
            if (!Communication.TryGetValue(key, out var comm))
            {
                MessageBox.Show($"'{key}'에 해당하는 통신이 없습니다.", "호출 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var type = comm.GetType();
            var methods = type
                .GetMethods()
                .Where(m => m.Name == methodName && typeof(Task).IsAssignableFrom(m.ReturnType))
                .ToList();

            var triedSignatures = new List<string>();

            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                var sig = $"{method.Name}({string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"))})";
                triedSignatures.Add(sig);

                object?[] invokeArgs;

                if (parameters.Length == 0)
                {
                    invokeArgs = Array.Empty<object>();
                }
                else if (args.Length == parameters.Length)
                {
                    invokeArgs = new object[parameters.Length];
                    bool success = true;
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        try
                        {
                            invokeArgs[i] = Convert.ChangeType(args[i], parameters[i].ParameterType);
                        }
                        catch
                        {
                            success = false;
                            break;
                        }
                    }
                    if (!success) continue;
                }
                else if (parameters.All(p => p.HasDefaultValue))
                {
                    invokeArgs = parameters.Select(p => p.DefaultValue).ToArray();
                }
                else
                {
                    continue;
                }

                var task = (Task)method.Invoke(comm, invokeArgs)!;
                await task;
                return;
            }

            // 실패한 경우, 가능한 시그니처 목록을 안내
            var message = $"호출 가능한 메서드 시그니처를 찾을 수 없습니다.\n\n사용 가능한 시그니처:\n- {string.Join("\n- ", triedSignatures)}";
            MessageBox.Show(message, "메서드 호출 실패", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public List<string> GetMethodList(string key)
        {
            if (!Communication.TryGetValue(key, out var comm))
                return [];

            return comm.MethodList ?? [];
        }

        public bool MethodRequiresParameter(string key, string methodName)
        {
            if (!Communication.TryGetValue(key, out var comm)) return false;

            var methods = comm.GetType()
                .GetMethods()
                .Where(m => m.Name == methodName && typeof(Task).IsAssignableFrom(m.ReturnType));

            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                if (parameters.Length > 0 && parameters.Any(p => !p.HasDefaultValue))
                    return true;
            }

            return false;
        }
    }
}
