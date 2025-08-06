using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Input;
using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.Common.MVVM.ViewModels;
using VSLibrary.Database;
using VSLibrary.Enums;
using VSLibrary.UIComponent.Common.Services;

namespace VSLibrary.UIComponent.Common
{
    /// <summary>
    /// 버튼 메뉴 항목을 나타내는 모델 클래스입니다.
    /// WPF와 Blazor에서 모두 사용할 수 있도록 기능을 분리했습니다.
    /// </summary>
    public class ButtonMenuItem : ViewModelBase
    {
        private readonly IVSLogger _logger;

        /// <summary>
        /// 기본 생성자 (DI 없이도 인스턴스 생성 가능)
        /// </summary>
        private readonly IServiceProvider _serviceProvider;
        public ButtonMenuItem() { }
        public ButtonMenuItem(IVSLogger logger, IServiceProvider serviceProvider = null)
        {
            _logger = logger;
            _serviceProvider ??= serviceProvider;
        }

        /// <summary>
        /// 다국어 지원을 위해 언어를 변경합니다.
        /// </summary>
        public void ChangeLanguage<T>(IEnumerable<T> db, string language) where T : class
        {
            SetLanguage(db, language);
        }

        private string _imagePath;

        /// <summary>
        /// 메뉴 항목의 이미지 경로를 가져오거나 설정합니다.
        /// </summary>
        public string ImagePath
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath, value);
        }

        private Visibility _visibility;

        /// <summary>
        /// 메뉴 항목의 가시성을 가져오거나 설정합니다. (WPF 전용)
        /// </summary>
        public Visibility Visibility
        {
            get => _visibility;
            set
            {
                SetProperty(ref _visibility, value);
            }
           
        }

        private bool _isVisible = true;

        /// <summary>
        /// Blazor에서 사용되는 가시성 속성 (Visibility 대체)
        /// </summary>
        [IgnoreColumn]
        public bool IsVisible
        {
            get => _isVisible;
            set
            {                
                SetProperty(ref _isVisible, value);
                Visibility = value ? Visibility.Visible : Visibility.Hidden;                
            }
        }


        private bool _isEnabled;

        /// <summary>
        /// 메뉴 항목의 활성화 상태를 가져오거나 설정합니다.
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        private bool _isSelected;

        /// <summary>
        /// 메뉴 항목의 선택 상태를 가져오거나 설정합니다.
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }


        private ICommand _selectMenuItemCommand;

        /// <summary>
        /// WPF에서 메뉴 항목 선택 시 실행되는 명령입니다.
        /// </summary>
        [IgnoreColumn]
        public ICommand SelectMenuItemCommand
        {
            get => _selectMenuItemCommand;
            set => SetProperty(ref _selectMenuItemCommand, value);
        }

        /// <summary>
        /// 레시피 레코드의 고유 ID입니다.
        /// </summary>
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// 메뉴 항목의 텍스트를 가져오거나 설정합니다.
        /// </summary>
        [UniqueGroup("MenuItemUniqueKey")]
        public string MenuText { get; set; }

        /// <summary>
        /// 메뉴 항목과 연결된 ViewModel 이름을 가져오거나 설정합니다.
        /// </summary>
        [UniqueGroup("MenuItemUniqueKey")]
        public string ViewModel { get; set; }

        /// <summary>
        /// 메뉴 항목과 연결된 RegionName 이름을 가져오거나 설정합니다.
        /// </summary>
        [UniqueGroup("MenuItemUniqueKey")]
        public string RegionName { get; set; }

        /// <summary>
        /// 메뉴 항목의 접근 권한 레벨을 가져오거나 설정합니다.
        /// </summary>
        public int RequiredAccessLevel { get; set; }

        /// <summary>
        /// 메뉴 항목의 동작 상태를 나타냅니다. (0: 레벨 기준, 1: 상태 기반)
        /// </summary>
        public int State { get; set; }

        private Action? _onClickAction;

        /// <summary>
        /// WPF에서 사용할 수 있도록 메뉴 항목 클릭 시 실행될 동작을 설정합니다.
        /// Blazor에서는 EventCallback을 사용합니다.
        /// </summary>
        [IgnoreColumn]
        public Action? OnClickAction
        {
            get => _onClickAction;
            set
            {
                _onClickAction = value == null ? null : () =>
                {
                    LogClickAction();
                    value.Invoke();
                };
            }
        }

        /// <summary>
        /// 로그를 남기는 메서드입니다.
        /// </summary>
        private void LogClickAction()
        {
            var logMessage = $"[{ViewModel}:{RegionName}:{MenuText} Click]";

            if (_serviceProvider != null)
            {
                using var scope = _serviceProvider.CreateScope();  // ✅ 새로운 DI 스코프 생성
                var clientIpService = scope.ServiceProvider.GetRequiredService<ClientIpService>();  // ✅ 최신 서비스 가져오기

                string? clientIp = clientIpService.GetClientIp();
                logMessage += $" - IP: {clientIp ?? "Unknown"}";
            }

            _logger?.WriteLine(logMessage, LogType.UI);
        }
    }
}
