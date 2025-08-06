using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SequenceEngine.Bases;

public class UnitStep
{
    public Func<bool> CheckCondition { get; set; } = () => true;
    public Func<bool> ExecuteAction { get; set; } = () => true;
    public Action ActionTimeout { get; set; }
    public Func<bool> IsStateOK { get; set; } = () => true;
    public Func<int> NextStep { get; set; }

    private int _timeout { get; set; } = 0;
    private bool _coditionFalse = true;
    private Stopwatch _time = new();
    private bool _hasExecuted = false;
    private bool _isExcuteCompleted = false;

    public UnitStep(int timeout = 0)
    {
        _timeout = timeout;
        _time = new Stopwatch();
    }

    public void Start()
    {
        if (!CheckCondition())
        {
            _coditionFalse = true;
            return;
        }

        _coditionFalse = false;

        if (!_hasExecuted)
        {
            SetTimer();
            _hasExecuted = true;
        }

        if(ExecuteAction() && _hasExecuted)
        {
            _isExcuteCompleted = true;
            IsTimeout();
        }    
    }

    public bool IsComplete()
    {
        if (_coditionFalse) return false;
        if (!_isExcuteCompleted) return false;
        var result = IsStateOK();
        if (result)
        {
            _hasExecuted = false;
            _isExcuteCompleted = false;
        }    
        return result;
    }

    public bool IsTimeout()
    {
        if (_coditionFalse) return false;
        if (!_isExcuteCompleted) return false;
        if (_timeout <= 0) return false;

        if (_time.ElapsedMilliseconds > _timeout)
        {
            _hasExecuted = false;
            _isExcuteCompleted = false;

            if (ActionTimeout != null)
                ActionTimeout.Invoke();

            return true;
        }

        return false;
    }

    private void SetTimer()
    {
        _time.Restart();
    }
}
