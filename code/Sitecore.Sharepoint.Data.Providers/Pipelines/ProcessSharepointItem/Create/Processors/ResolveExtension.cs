// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolveExtension.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ResolveName type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.CreateSharepointItem
{
  using Sitecore.Configuration;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Pipelines.ProcessSharepointItem;

  public class ResolveExtension
  {
    public void Process(ProcessSharepointItemArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.IsNotNullOrEmpty(args.SourceIntegrationItemName, "args.SourceIntegrationItemName");

      var item = args.SourceIntegrationItem;
      if (item == null)
      {
        return;
      }

      var ext = item["Extension"];
      if (!string.IsNullOrEmpty(ext))
      {
        var sitecoreExtension = Settings.Media.WhitespaceReplacement + ext;
        if (Settings.Media.IncludeExtensionsInItemNames)
        {
          args.SourceIntegrationItemName = StringUtil.RemovePostfix(sitecoreExtension, args.SourceIntegrationItemName);
        }

        args.SourceIntegrationItemName += '.' + ext;
      }
    }
  }
}