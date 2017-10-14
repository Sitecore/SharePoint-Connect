// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CopySourceValue.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines CopySourceValue class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.TranslateSharepointValue
{
  using Sitecore.Diagnostics;

  public class CopySourceValue
  {
    public virtual void Process([NotNull] TranslateSharepointValueArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      args.TranslatedValue = args.SourceSharepointItem[args.SourceFieldName];
    }
  }
}