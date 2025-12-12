using UnityEngine;
using System.Collections.Generic;

#region Non Generic Event
public abstract class GameEvent : ScriptableObject
{
    private readonly List<IGameEventListener> listeners = new List<IGameEventListener>();

    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised();
        }
    }

    public void RegisterListener(IGameEventListener listener)
    {
        if (!listeners.Contains(listener)) listeners.Add(listener);
    }
    public void UnregisterListener(IGameEventListener listener)
    {
        if (listeners.Contains(listener)) listeners.Remove(listener);
    }
}
#endregion

#region Generic Event
public abstract class GameEvent<T> : ScriptableObject
{
    private readonly List<IGameEventListener<T>> listeners = new List<IGameEventListener<T>>();

    public void Raise(T item)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised(item);
        }
    }

    public void RegisterListener(IGameEventListener<T> listener)
    {
        if (!listeners.Contains(listener)) listeners.Add(listener);
    }
    public void UnregisterListener(IGameEventListener<T> listener)
    {
        if (listeners.Contains(listener)) listeners.Remove(listener);
    }
}
#endregion