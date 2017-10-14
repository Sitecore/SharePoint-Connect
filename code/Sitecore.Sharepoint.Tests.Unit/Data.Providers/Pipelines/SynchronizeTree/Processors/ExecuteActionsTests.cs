// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExecuteActionsTests.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ExecuteActionsTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.Pipelines.SynchronizeTree.Processors
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Sharepoint.Data.Providers.SyncActions;
  using Sitecore.Sharepoint.Pipelines.SynchronizeTree;
  using Xunit;

  public class ExecuteActionsTests
  {
    private readonly ExecuteActions processor = new ExecuteActions();
    private readonly SynchronizeTreeArgs args = new SynchronizeTreeArgs();

    [Fact]
    public void should_throw_if_args_is_not_set()
    {
      // Act
      Action action = () => this.processor.Process(null);

      // Assert
      action.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: args");
    }

    [Fact]
    public void should_throw_if_action_list_is_not_set()
    {
      // Act
      Action action = () => this.processor.Process(this.args);

      // Assert
      action.ShouldThrow<InvalidOperationException>().WithMessage("Value can't be null: args.ActionList");
    }

    [Fact]
    public void should_execute_actions()
    {
      // Arrange
      this.args.ActionList = new List<SyncActionBase>();
      Enumerable.Range(1, 10).ToList().ForEach(x => this.args.ActionList.Add(Substitute.For<SyncActionBase>()));

      // Act
      this.processor.Process(this.args);

      // Assert
      this.args.ActionList.ForEach(x => x.Received().Execute());
    }
  }
}