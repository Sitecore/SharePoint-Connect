// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowTestsBase.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the WorkflowTestsBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Common.Authentication.Workflows
{
  using System;
  using NSubstitute;
  using Sitecore.Sharepoint.Common.Authentication.Workflows;

  public class WorkflowTestsBase : IDisposable
  {
    private readonly WorkflowsDefaults prevInstance;

    public WorkflowTestsBase()
    {
      this.prevInstance = WorkflowsDefaults.Instance;
      WorkflowsDefaults.Instance = Substitute.For<WorkflowsDefaults>();
    }

    public void Dispose()
    {
      WorkflowsDefaults.Instance = this.prevInstance;
    }
  }
}