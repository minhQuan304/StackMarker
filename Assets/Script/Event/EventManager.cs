using System;
using GenericEventBus;

public static class EventManager
{
    private static GenericEventBus<IEvent> s_bus = new GenericEventBus<IEvent>();

    public static GenericEventBus<IEvent> Bus => s_bus;

    public static bool Raise<TEvent>(in TEvent @event) where TEvent : IEvent
    {
        return Bus.Raise(@event);
    }

    public static void SubscribeTo<TEvent>(GenericEventBus<IEvent>.EventHandler<TEvent> handler, float priority = 0)
        where TEvent : IEvent
    {
        //Logger.Log("SUBSCRIBE "+handler.GetType());
        Bus.SubscribeTo(handler, priority);
    }
    public static void UnsubscribeFrom<TEvent>(GenericEventBus<IEvent>.EventHandler<TEvent> handler)
        where TEvent : IEvent
    {
        //Logger.Log("UnsubscribeFrom " + handler.GetType());
        Bus.UnsubscribeFrom(handler);
    }
}
