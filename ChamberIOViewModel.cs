using MaterialDesignThemes.Wpf;
using System.Windows.Input;
using VSPO_CS.App.Models.Chamber;
using VSPO_CS.App.Models.IO;
using VSPO_CS.Shared.Base;
using VSPO_CS.Shared.Commands;

namespace VSPO_CS.App.ViewModels.IOTest;

public class ChamberIOViewModel : ViewModelBase
{
    private Action<int> _updatePaginationSetting;
    public bool _lockEvent = false;

    private int _currentPage;
    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            SetProperty(ref _currentPage, value);
        }
    }

    private int _pageSize = 14;
    public int PageSize
    {
        get => _pageSize;
        set
        {
            SetProperty(ref _pageSize, value);
            UpdatePropertyChanged();
            OnEntriesPerPageChanged(value);
        }
    }

    private void OnEntriesPerPageChanged(int value)
    {
        if (_lockEvent)
            return;

        if (value > 0)
            _updatePaginationSetting?.Invoke(value);
    }

    public Action<IOAddressModel>? OnOutputClick { get; set; }

    private ChamberDTO? _chamber;
    public ChamberDTO? Chamber
    {
        get => _chamber;
        set => SetProperty(ref _chamber, value);
    }

    public int TotalPages => (Chamber!.InputAddresses!.Count + PageSize - 1) / PageSize;
    public bool CanNavigatePrevious => CurrentPage > 1;
    public bool CanNavigateNext => CurrentPage < TotalPages;

    public ICommand PageNavigateCommand { get; }
    public ICommand OnSignalControlClickCommand { get; }

    public ChamberIOViewModel(ChamberDTO chamber, Action<int> updatePaginationSetting)
    {
        this.Chamber = chamber;
        CurrentPage = 1;

        LoadPagedItems();
        _updatePaginationSetting = updatePaginationSetting;

        OnSignalControlClickCommand = new RelayCommand<object>(OnSignalControlClick);
        PageNavigateCommand = new RelayCommand<string>(PageNavigate);
    }

    public void LoadPagedItems()
    {
        if (Chamber == null)
            return;

        var items = Chamber.InputAddresses!.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

        Chamber.PagedInputAddresses = new(items);
        OnPropertyChanged(nameof(Chamber.PagedInputAddresses));
    }

    private void PageNavigate(string param)
    {
        if (param == "Next")
        {
            CurrentPage++;
            LoadPagedItems();
        }
        else
        {
            CurrentPage--;
            LoadPagedItems();
        }

        UpdatePropertyChanged();
    }

    private void UpdatePropertyChanged()
    {
        OnPropertyChanged(nameof(TotalPages));
        OnPropertyChanged(nameof(CanNavigatePrevious));
        OnPropertyChanged(nameof(CanNavigateNext));
    }

    private void OnSignalControlClick(object obj)
    {
        if (obj is not IOAddressModel registerAddress)
            return;

        if (registerAddress.Address <= 16) //Output address
        {
            OnOutputClick?.Invoke(registerAddress);
        }
    }
}
