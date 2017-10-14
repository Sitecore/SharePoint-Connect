// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisableSharepointProvider.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the DisableSharepointProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.Pipelines
{
  using System.Linq;
  using Sitecore.Data.Fields;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Publishing.Pipelines.PublishItem;

  using SharepointFieldIDs = Sitecore.Sharepoint.Common.FieldIDs;

  /// <summary>
  /// Class uses for publishing Sharepoint items.
  /// </summary>
  public class DisableSharepointProvider
  {
    /// <summary>
    /// This method disable SharepointProvider.
    /// </summary>
    /// <param name="args">The args  .</param>
    public void Disable([NotNull] PublishItemContext args)
    {
      Assert.ArgumentNotNull(args, "args");

      args.CustomData["SharepointDisabler"] = new IntegrationDisabler();
    }

    /// <summary>
    /// Clear IsSharepointItem field for published items and enable SharepointProvider.
    /// </summary>
    /// <param name="args">The args   .</param>
    public void Clear([NotNull] PublishItemContext args)
    {
      Assert.ArgumentNotNull(args, "args");

      if (args.VersionToPublish != null)
      {
        var item = args.PublishHelper.GetTargetItem(args.VersionToPublish.ID);
        if (item != null && item.Fields.FirstOrDefault(field => field.ID == SharepointFieldIDs.IsIntegrationItem) != null)
        {
          using (new EditContext(item))
          {
            new CheckboxField(item.Fields[SharepointFieldIDs.IsIntegrationItem]).Checked = false;
          }
        }
      }

      if (args.CustomData["SharepointDisabler"] != null)
      {
        var disabler = args.CustomData["SharepointDisabler"] as IntegrationDisabler;
        if (disabler != null)
        {
          args.CustomData["SharepointDisabler"] = null;
          disabler.Dispose();
        }
      }
    }
  }
}
