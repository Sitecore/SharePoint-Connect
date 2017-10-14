// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassicConnectionConfiguration.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ClassicConnectionConfiguration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.WebServices.ConnectionConfigurations
{
  using System.Web.Services.Protocols;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common;

  public class ClassicConnectionConfiguration : BaseConnectionConfiguration
  {
    public override void Initialize([NotNull] HttpWebClientProtocol service, [NotNull] SpContextBase context)
    {
      Assert.ArgumentNotNull(service, "service");
      Assert.ArgumentNotNull(context, "context");
      
      service.Credentials = context.Credentials;
      service.PreAuthenticate = true;
    }
  }
}