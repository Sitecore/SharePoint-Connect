// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogWatcher.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the LogWatcher type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit
{
  using System;
  using FluentAssertions;
  using Sitecore.Diagnostics;

  public class LogWatcher
  {
    private readonly Action action;

    public LogWatcher(Action action)
    {
      this.action = action;
    }

    public LogNotificationLevel Level { get; private set; }
    
    public string Message { get; private set; }
    
    public Exception Exception { get; private set; }

    public LogWatcher Ensure()
    {
      LogNotification.Notification += this.OnLogNotification;
      this.action();
      LogNotification.Notification -= this.OnLogNotification;
      return this;
    }

    public LogWatcher LevelIs(LogNotificationLevel level)
    {
      this.Level.Should().Be(level, "Level should be correct");
      return this;
    }

    public LogWatcher MessageIs(string message)
    {
      this.Message.Should().Be(message, "Message should be correct");
      return this;
    }

    public LogWatcher ExceptionIs(Exception exc)
    {
      this.Exception.Should().Be(exc, "Exception should be correct");
      return this;
    }

    private void OnLogNotification(LogNotificationLevel level, string message, Exception exception)
    {
      this.Level = level;
      this.Message = message;
      this.Exception = exception;
    }
  }
}