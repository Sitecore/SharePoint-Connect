// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventWatcher.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the EventWatcher type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using FluentAssertions;
  using Sitecore.Events;
  using Xunit;

  public class EventWatcher : IDisposable
  {
    private readonly Dictionary<string, EventExecInfo> dictionary = new Dictionary<string, EventExecInfo>();

    public void Expect(string eventName, Func<object, EventArgs, bool> checkCall = null)
    {
      this.dictionary.Add(eventName, new EventExecInfo(checkCall));
      Event.Subscribe(eventName, this.Handle);
    }

    public void EnsureExecuted()
    {
      foreach (var pair in this.dictionary)
      {
        pair.Value.IsExecuted.Should().BeTrue("Event '{0}' is not executed", pair.Key);
        Assert.True(pair.Value.IsCorrect, string.Format("Event '{0}' is executed but with wrong arguments", pair.Key));
      }
    }

    public void Dispose()
    {
      foreach (var pair in this.dictionary)
      {
        Event.Unsubscribe(pair.Key, this.Handle);
      }
    }

    private void Handle(object sender, EventArgs args)
    {
      var sitecoreEventArgs = args as SitecoreEventArgs;
      this.dictionary[sitecoreEventArgs.EventName].Args = args;
      this.dictionary[sitecoreEventArgs.EventName].Sender = sender;
      this.dictionary[sitecoreEventArgs.EventName].IsExecuted = true;
    }

    private class EventExecInfo
    {
      public EventExecInfo(Func<object, EventArgs, bool> checkCall)
      {
        this.CheckCall = checkCall;
      }

      public bool IsExecuted { get; set; }

      public EventArgs Args { get; set; }

      public object Sender { get; set; }

      public Func<object, EventArgs, bool> CheckCall { get; set; }

      public bool IsCorrect
      {
        get
        {
          return this.CheckCall == null || this.CheckCall(this.Sender, this.Args);
        }
      }
    }
  }
}