// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TranslateDateToIsoDate.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines TranslateDateToIsoDate class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.TranslateSharepointValue
{
  using System;
  using Sitecore.Data.Fields;
  using Sitecore.Diagnostics;

  public class TranslateDateToIsoDate
  {
    public virtual void Process([NotNull] TranslateSharepointValueArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      if (string.IsNullOrEmpty(args.TranslatedValue))
      {
        return;
      }

      Field targetField = args.TargetIntegrationItem.Fields[args.TargetFieldName];
      if (targetField == null || (targetField.TypeKey != "datetime" && targetField.TypeKey != "date"))
      {
        return;
      }

      DateTime sourceFieldValue;
      if (DateTime.TryParse(args.TranslatedValue, out sourceFieldValue))
      {
        args.TranslatedValue = DateUtil.ToIsoDate(sourceFieldValue);
      }
    }
  }
}