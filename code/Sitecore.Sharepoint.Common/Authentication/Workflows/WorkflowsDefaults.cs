// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowsDefaults.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the WorkflowsDefaults type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Common.Authentication.Workflows
{
  using System.ServiceModel.Security;
  using Sitecore.Sharepoint.Common.Authentication.Workflows.Helpers;
  
  internal class WorkflowsDefaults
  {
    private static WorkflowsDefaults instance;

    public static WorkflowsDefaults Instance
    {
      get
      {
        return instance ?? (instance = new WorkflowsDefaults());
      }

      set
      {
        instance = value;
      }
    }

    public virtual TokenGetter GetTokenGetter()
    {
      return new TokenGetter(new WSTrustChannelFactoryWrapper());
    }

    public virtual CookieGetter GetCookieHelper()
    {
      return new CookieGetter();
    }

    public virtual TrustVersion GetTrustVersion()
    {
      return TrustVersion.WSTrust13;
    }

    public virtual SharePointOnlineCredentialsWrapper GetSharePointOnlineCredentialsWrapper()
    {
      return new SharePointOnlineCredentialsWrapper();
    }
  }
}