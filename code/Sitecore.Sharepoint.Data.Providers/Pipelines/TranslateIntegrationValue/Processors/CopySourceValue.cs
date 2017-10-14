// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CopySourceValue.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines CopySourceValue class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.TranslateIntegrationValue
{
  using Sitecore.Diagnostics;

  public class CopySourceValue
  {
    public virtual void Process([NotNull] TranslateIntegrationValueArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      args.TranslatedValue = args.SourceIntegrationItem[args.SourceFieldName];
    }
  }
}