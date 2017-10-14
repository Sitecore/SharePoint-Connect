// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SitecoreEventManager.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SitecoreEventManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Common.Wrappers
{
  using Sitecore.Eventing;

  public class SitecoreEventManager
  {
    public virtual void QueueEvent<TEvent>(TEvent @event)
    {
      EventManager.QueueEvent(@event);
    }
  }
}
