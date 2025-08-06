using System.Collections.ObjectModel;
using VSLibrary.Common.MVVM.ViewModels;

namespace VSLibrary.UIComponent.Common;

    /// <summary>
    /// 버튼 메뉴를 관리하는 ViewModel 클래스입니다.
    /// </summary>
    public class ButtonMenuViewModel : ViewModelBase
{
	private ObservableCollection<ButtonMenuItem> _allMenuItems;
	/// <summary>
	/// 메뉴 항목 리스트. 외부에서 설정할 수 있습니다.
	/// </summary>
	public ObservableCollection<ButtonMenuItem> ButtonMenus { get; set; }

	/// <summary>
	/// 이전에 선택된 메뉴 항목을 저장합니다.
	/// </summary>
	public ButtonMenuItem PreviousMenuItem { get; set; }

	public int _userAccessLevel;
	private bool _isStartState;

	/// <summary>
	/// Start 상태를 관리합니다.
	/// </summary>
	public bool IsStartState
	{
		get => _isStartState;
		set
		{
			if (SetProperty(ref _isStartState, value))
			{
				UpdateMenuState();
			}
		}
	}

	/// <summary>
	/// 메뉴가 변경될 때 발생하는 이벤트입니다. 변경된 메뉴의 텍스트를 전달합니다.
	/// </summary>
	public event Action<string> MenuChanged;

	/// <summary>
	/// 다국어 지원을 위해 언어를 변경합니다.
	/// </summary>
	/// <typeparam name="T">언어 데이터의 타입</typeparam>
	/// <param name="db">언어 데이터</param>
	/// <param name="language">선택된 언어</param>
	public void ChangeLanguage<T>(IEnumerable<T> db, string language) where T : class
	{
		SetLanguage(db, language);
		foreach (var menuItem in ButtonMenus)
		{
			menuItem.ChangeLanguage(db, language);
		}
	}

	private int _rows = 1;
	/// <summary>
	/// 레이아웃의 행(Row) 개수를 관리합니다.
	/// </summary>
	public int Rows
	{
		get => _rows;
		set => SetProperty(ref _rows, value);
	}


	private int _columns = 10;
	/// <summary>
	/// 레이아웃의 열(Column) 개수를 관리합니다.
	/// </summary>
	public int Columns
	{
		get => _columns;
		set => SetProperty(ref _columns, value);
	}


	private double _buttonWidth = 104;
	/// <summary>
	/// 레이아웃의 행(Row) 개수를 관리합니다.
	/// </summary>
	public double ButtonWidth
	{
		get => _buttonWidth;
		set => SetProperty(ref _buttonWidth, value);
	}


	private double _buttonHeight= 50;
	/// <summary>
	/// 레이아웃의 열(Column) 개수를 관리합니다.
	/// </summary>
	public double ButtonHeight
	{
		get => _buttonHeight;
		set => SetProperty(ref _buttonHeight, value);
	}

	/// <summary>
	/// 기본 생성자입니다.
	/// </summary>
	public ButtonMenuViewModel()
	{
		_allMenuItems = new ObservableCollection<ButtonMenuItem>();
		ButtonMenus = new ObservableCollection<ButtonMenuItem>();
	}
	/// <summary>
	/// 메뉴 항목 데이터를 로드합니다.
	/// </summary>
	/// <param name="items">전체 메뉴 항목 데이터</param>
	public void LoadMenuItems(IEnumerable<ButtonMenuItem> items)
	{
		_allMenuItems.Clear();
		foreach (var item in items)
		{
			_allMenuItems.Add(item);
		}

		// 초기 상태에서 모든 항목 로드
		FilterMenuItems(_ => true);
	}
	/// <summary>
	/// 메뉴 항목을 조건에 따라 필터링합니다.
	/// </summary>
	/// <param name="predicate">필터 조건</param>
	public void FilterMenuItems(Func<ButtonMenuItem, bool> predicate)
	{
		ButtonMenus.Clear();
		foreach (var item in _allMenuItems.Where(predicate))
		{
			ButtonMenus.Add(item);
		}
	}

	/// <summary>
	/// 사용자 레벨에 따른 메뉴 접근 권한을 설정합니다.
	/// </summary>
	/// <typeparam name="T">Enum 타입의 사용자 레벨</typeparam>
	/// <param name="userLevel">사용자 레벨</param>
	public void SetMenuAccessByUserLevel<T>(T userLevel) where T : Enum
	{
		if (ButtonMenus == null) return;

		// Enum 타입의 값을 int로 변환
		int userAccessLevel = Convert.ToInt32(userLevel);
		_userAccessLevel = userAccessLevel;
		UpdateMenuState(); 			
	}
	/// <summary>
	/// 메뉴 상태를 업데이트합니다.
	/// </summary>
	private void UpdateMenuState()
	{
		foreach (var menuItem in ButtonMenus)
		{
			// 기본적으로 메뉴를 비활성화
			menuItem.IsEnabled = false;

			if (IsStartState) // 장비가 Start 상태
			{
				// State가 1인 경우: 강제로 비활성화
				if (menuItem.State == 1)
				{
					menuItem.IsEnabled = false;
				}
				// State가 0인 경우: 사용자 레벨에 따라 활성화 여부 결정
				else
				{
					menuItem.IsEnabled = _userAccessLevel >= menuItem.RequiredAccessLevel;
				}
			}
			else // 장비가 비가동 상태
			{
				// 사용자 레벨에 따라 활성화 여부 결정
				menuItem.IsEnabled = _userAccessLevel >= menuItem.RequiredAccessLevel;
			}
		}
	}
	/// <summary>
	/// 메뉴 선택을 처리하는 메서드입니다.
	/// </summary>
	/// <param name="previousMenu">이전에 선택된 메뉴</param>
	/// <param name="clickedItem">현재 클릭된 메뉴</param>
	/// <param name="select">선택 여부를 나타내는 플래그</param>
	public void UpdateMenuSelection(ButtonMenuItem previousMenu, ButtonMenuItem clickedItem, bool select = false)
	{
		if (previousMenu != null && clickedItem != null)
		{
			previousMenu.IsSelected = false;
			clickedItem.IsSelected = false;
		}

		if (clickedItem != null && previousMenu != null)
		{
			if (select)
			{
				previousMenu.IsSelected = false;
				clickedItem.IsSelected = true;

				// 메뉴 변경 이벤트 트리거
				MenuChanged?.Invoke(clickedItem.MenuText);
			}
			else
			{
				clickedItem.IsSelected = false;
				previousMenu.IsSelected = true;

				PreviousMenuItem = previousMenu;

				// 메뉴 변경 이벤트 트리거
				MenuChanged?.Invoke(previousMenu.MenuText);
			}
		}
	}
}
