namespace PokemonSDK.Core.Events;

/// <summary>
/// Event manager for handling game events
/// </summary>
public class EventManager
{
    private readonly Dictionary<Type, List<Delegate>> _eventHandlers = new();
    
    /// <summary>
    /// Subscribe to an event
    /// </summary>
    public void Subscribe<T>(Action<T> handler) where T : GameEvent
    {
        var eventType = typeof(T);
        
        if (!_eventHandlers.ContainsKey(eventType))
        {
            _eventHandlers[eventType] = new List<Delegate>();
        }
        
        _eventHandlers[eventType].Add(handler);
    }
    
    /// <summary>
    /// Unsubscribe from an event
    /// </summary>
    public void Unsubscribe<T>(Action<T> handler) where T : GameEvent
    {
        var eventType = typeof(T);
        
        if (_eventHandlers.ContainsKey(eventType))
        {
            _eventHandlers[eventType].Remove(handler);
        }
    }
    
    /// <summary>
    /// Publish an event to all subscribers
    /// </summary>
    public void Publish<T>(T gameEvent) where T : GameEvent
    {
        var eventType = typeof(T);
        
        if (_eventHandlers.ContainsKey(eventType))
        {
            foreach (var handler in _eventHandlers[eventType])
            {
                ((Action<T>)handler).Invoke(gameEvent);
            }
        }
    }
    
    /// <summary>
    /// Clear all event handlers
    /// </summary>
    public void Clear()
    {
        _eventHandlers.Clear();
    }
}
