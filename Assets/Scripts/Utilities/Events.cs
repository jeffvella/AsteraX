using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Events
{
    public interface IEventListener<in T, in TArg>
    {
        void OnRaised(T eventInfo, TArg value);
    }

    public interface IEventObserver { }

    public class ActionEventDelegator<T, TArg> : IEventListener<T, TArg>, IEventObserver
    {
        public ActionEventDelegator(Action<T, TArg> action)
        {
            _action = action;
        }

        private readonly Action<T, TArg> _action;

        public void OnRaised(T eventInfo, TArg value)
        {
            _action.Invoke(eventInfo, value);
        }
    }

    public class GameEventBase<TInfo, TArg> : ScriptableObject
    {
        public TInfo EventInfo;

        private readonly List<IEventListener<TInfo, TArg>> _eventListeners = new List<IEventListener<TInfo, TArg>>();

        public void Register(Action<TInfo, TArg> action)
        {
            Register(new ActionEventDelegator<TInfo, TArg>(action));
        }

        public void Register(IEventListener<TInfo, TArg> observer)
        {
            if (!_eventListeners.Contains(observer))
            {
                _eventListeners.Add(observer);
            }
        }

        public void Deregister(IEventListener<TInfo, TArg> observer)
        {
            if (_eventListeners.Contains(observer))
            {
                _eventListeners.Remove(observer);
            }
        }

        public void Raise(TArg value)
        {
            for (int i = _eventListeners.Count - 1; i >= 0; i--)
            {
                // todo automatically remove null/destroyed observers
                _eventListeners[i]?.OnRaised(EventInfo, value);
            }
        }
    }

}

//public class TestEventListener : MonoBehaviour
//{
//    public TestGameEvent EventToListenFor;

//    void Awake()
//    {
//        EventToListenFor.Register(MyEventHandler);
//    }

//    public void MyEventHandler(TestEventInfo info, TestEventArgument details)
//    {
//        Debug.Log($"Received Event {info.Name} ({info.Type}) with argument {details.Message}");
//    }
//}

//public class TestEventRaiser : MonoBehaviour
//{
//    public TestGameEvent Event;

//    public void RaiseEvent()
//    {
//        Event?.Raise(new TestEventArgument
//        {
//            Message = "A Raised Event!"
//        });
//    }
//}

//public struct TestEventArgument
//{
//    public string Message;
//}

//[Serializable]
//public struct TestEventInfo
//{
//    public int Id;
//    public string Name;
//    public EventType Type;

//    public enum EventType
//    {
//        None = 0,
//        Minor,
//        Serious,
//    }
//}

//[CreateAssetMenu]
//public class TestGameEvent : GameEventBase<TestEventInfo, TestEventArgument>
//{

//}

