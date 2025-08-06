using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSP_88D_CS.Sequence;
using VSP_COMMON;

namespace VSP_88D_CS.Common
{

    public enum eMsgKind
    {
        MODE_CHANGE = 0,
        STEP_DONE,
        CYCLE_DONE,
        STEP_ADVANCE,
        CLEAN_TIME_UP,
        PRODUCT_UP,
        CALC_CYCLE,
        ONELEV_EMPTY,
        LOG_OCCUR,
        START_CLEAN,// For Cleaning Log
        END_CLEAN,  // For Cleaning Log
        JOB_END,    // LOT_END
        JOB_CANCEL,
        AGING_END,
        SLEEP_END,
        MANUAL_CLEAN_END,
        FLUSH_END,
        SWITCH_ON,  // START, STOP, RESET, LOCK/UNLOCK
        LEAK_TEST_ON,
        CHAMBER_COOL_ON,
        STRIP_POS_CHANGE,
        MGZ_SCAN_START,
        MGZ_SCAN_UPDATE,
        MGZ_SCAN_END,
        LAST_STRIP_DONE,
        RESET_GLOBAL_FLAGS,
        END_SLEEP_AGING, // SLEEP -> NORMAL, AGING -> NORMAL
        TURN_TBL_COUNT,
        LOAD_SYRINGE,
        UNLD_SYRINGE,
        LOT_HOST_START,
        JOB_START,
        NEED_OP_CONFIRM,
        TEST_ATK,
    }
    
    public interface IEventAggregator
    {
        void Subscribe<TEvent>(Action<TEvent> action);
        void Publish<TEvent>(TEvent eventMessage);
    }

    public class EventAggregator : IEventAggregator
    {
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new();

        public void Subscribe<TEvent>(Action<TEvent> action)
        {
            if (!_subscribers.ContainsKey(typeof(TEvent)))
            {
                _subscribers[typeof(TEvent)] = new List<Delegate>();
            }
            _subscribers[typeof(TEvent)].Add(action);
        }

        public void Publish<TEvent>(TEvent eventMessage)
        {
            if (_subscribers.ContainsKey(typeof(TEvent)))
            {
                foreach (var subscriber in _subscribers[typeof(TEvent)])
                {
                    ((Action<TEvent>)subscriber)?.Invoke(eventMessage);
                }
            }
        }
    }

    public class EventProvider
    {
        private static readonly Lazy<EventAggregator> _instance = new(() => new EventAggregator());

        public static EventAggregator Instance => _instance.Value;
    }

    public class VS_PROC_MSG
    {
        public eMsgKind Kind;
        public eProcessStep Step;
    }

    public class VS_ALARM_MSG
    {
        public int ErrCode;
        public ALARM Kind;
    }

    public class VS_CIM_SEND_EVENT_MSG
    {
        public int CEID;
        public object param;
    }
}
