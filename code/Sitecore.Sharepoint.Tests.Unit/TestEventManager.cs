namespace Sitecore.Sharepoint.Tests.Unit
{
  using System;
  using System.Collections.Generic;

  using Sitecore.Abstractions;
  using Sitecore.Eventing;

  public class TestEventManager : BaseEventManager
  {
    private readonly Dictionary<Type, List<SubscriptionBase>> subscriptions = new Dictionary<Type, List<SubscriptionBase>>();

    public override void QueueEvent<TEvent>(TEvent @event)
    {
      throw new NotImplementedException();
    }

    public override void QueueEvent<TEvent>(TEvent @event, bool addToGlobalQueue, bool addToLocalQueue)
    {
      throw new NotImplementedException();
    }

    public override void RaiseEvent<TEvent>(TEvent @event)
    {
      List<SubscriptionBase> list;
      if (this.subscriptions.TryGetValue(typeof(TEvent), out list))
      {
        list.ForEach(x => x.Invoke(@event, EventContext.Local));
      }
    }

    public override void RaiseQueuedEvents()
    {
      throw new NotImplementedException();
    }

    public override void RemoveQueuedEvents(EventQueueQuery query)
    {
      throw new NotImplementedException();
    }

    public override void Initialize()
    {
      throw new NotImplementedException();
    }

    public override SubscriptionId Subscribe<TEvent>(Action<TEvent> eventHandler)
    {
      var subscription = new Subscription<TEvent>((e, c) => eventHandler(e), (e, c) => true);
      List<SubscriptionBase> list;
      if (!this.subscriptions.TryGetValue(subscription.Id.Type, out list))
      {
        list = new List<SubscriptionBase>();
        this.subscriptions[subscription.Id.Type] = list;
      }

      list.Add(subscription);
      return subscription.Id;
    }

    public override SubscriptionId Subscribe<TEvent>(Action<TEvent, EventContext> eventHandler)
    {
      throw new NotImplementedException();
    }

    public override SubscriptionId Subscribe<TEvent>(Action<TEvent> eventHandler, Predicate<TEvent> filter)
    {
      throw new NotImplementedException();
    }

    public override SubscriptionId Subscribe<TEvent>(Action<TEvent, EventContext> eventHandler, Func<TEvent, EventContext, bool> filter)
    {
      throw new NotImplementedException();
    }

    public override void Unsubscribe(SubscriptionId subscriptionId)
    {
      throw new NotImplementedException();
    }

    public override bool Enabled
    {
      get
      {
        return true;
      }
    }
  }
}