using VSLibrary.Common.MVVM.ViewModels;

namespace ChamberControl;

public class Gauge : ViewModelBase
{
    float _CurrentValue = 0;
    public float CurrentValue
    {
        get => _CurrentValue;
        set { SetProperty(ref _CurrentValue, value); CalculateRectangleValue(); }
    }
   
    float _MaxValue = 10;
    public float MaxValue
    {
        get => _MaxValue;
        set { SetProperty(ref _MaxValue, value); CalculateRectangleValue(); }
    }
    GaugeUnit _Unit = GaugeUnit.Torr;
    public GaugeUnit Unit
    {
        get => _Unit;
        set => SetProperty(ref _Unit, value);
    }
    /// <summary>
    /// No initialization required
    /// </summary>
    float _RectangleHeight = 200;
    public float RectangleHeight
    {
        get => _RectangleHeight;
        set { SetProperty(ref _RectangleHeight, value); CalculateRectangleValue(); }
    }
    /// <summary>
    /// No initialization required
    /// </summary>
    float _RectangleValue = 0;
    public float RectangleValue
    {
        get => _RectangleValue;
        set => SetProperty(ref _RectangleValue, value);
    }
    private void CalculateRectangleValue()
    {
        if (CurrentValue > MaxValue) { RectangleValue = RectangleHeight;return; }
        if (CurrentValue < 0) { RectangleValue = 0;return; }
        RectangleValue = RectangleHeight * CurrentValue / MaxValue;
    }
}
public enum GaugeUnit { Torr, mTorr, Pa }