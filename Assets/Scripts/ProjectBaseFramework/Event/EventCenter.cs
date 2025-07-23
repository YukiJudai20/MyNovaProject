using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public interface IEventInfo
{

}

public class EventInfo<T> : IEventInfo
{
    public System.Action<T> actions;

    public EventInfo( System.Action<T> action)
    {
        actions += action;
    }
}

public class EventInfo : IEventInfo
{
    public System.Action actions;

    public EventInfo(System.Action action)
    {
        actions += action;
    }
}

public class EventCenter : Singleton<EventCenter>
{

    private Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();
    
    public void AddEventListener<T>(string name, System.Action<T> action)
    {
        if( eventDic.ContainsKey(name) )
        {
            (eventDic[name] as EventInfo<T>).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo<T>( action ));
        }
    }
    
    public void AddEventListener(string name, System.Action action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo(action));
        }
    }
    
    public void RemoveEventListener<T>(string name, System.Action<T> action)
    {
        if (eventDic.ContainsKey(name))
            (eventDic[name] as EventInfo<T>).actions -= action;
    }


    public void RemoveEventListener(string name, System.Action action)
    {
        if (eventDic.ContainsKey(name))
            (eventDic[name] as EventInfo).actions -= action;
    }
    
    public void EventTrigger<T>(string name, T info)
    {
        //有没有对应的事件监听
        //有的情况
        if (eventDic.ContainsKey(name))
        {
            //eventDic[name]();
            if((eventDic[name] as EventInfo<T>).actions != null)
                (eventDic[name] as EventInfo<T>).actions.Invoke(info);
            //eventDic[name].Invoke(info);
        }
    }

    public void EventTrigger(string name)
    {
        //有没有对应的事件监听
        //有的情况
        if (eventDic.ContainsKey(name))
        {
            //eventDic[name]();
            if ((eventDic[name] as EventInfo).actions != null)
                (eventDic[name] as EventInfo).actions.Invoke();
            //eventDic[name].Invoke(info);
        }
    }
    
    public void Clear()
    {
        eventDic.Clear();
    }
}
