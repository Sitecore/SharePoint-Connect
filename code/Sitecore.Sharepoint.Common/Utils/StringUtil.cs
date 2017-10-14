// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringUtil.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the StringUtil type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Common.Utils
{
  using System.Text;
  using Sitecore.Diagnostics;

  public static class StringUtil
  {
    [NotNull]
    public static string ConcatEnsureDelimiter(char delimiter, [NotNull] params string[] values)
    {
      Assert.ArgumentNotNull(values, "values");

      if (values.Length == 0)
      {
        return string.Empty;
      }

      if (values.Length == 1)
      {
        return values[0];
      }

      var resultString = new StringBuilder(Sitecore.StringUtil.EnsurePostfix(delimiter, values[0]));

      for (int i = 1; i < values.Length - 1; i++)
      {
        resultString.Append(Sitecore.StringUtil.RemovePrefix(delimiter, Sitecore.StringUtil.EnsurePostfix(delimiter, values[i])));
      }

      resultString.Append(Sitecore.StringUtil.RemovePrefix(delimiter, values[values.Length - 1]));

      return resultString.ToString();
    }

    [NotNull]
    public static string ConcatEnsureDelimiter(char delimiter, bool isPrefix, bool isPostfix, [NotNull] params string[] values)
    {
      Assert.ArgumentNotNull(values, "values");

      string resultString = ConcatEnsureDelimiter(delimiter, values);

      resultString = isPrefix ? Sitecore.StringUtil.EnsurePrefix(delimiter, resultString) : Sitecore.StringUtil.RemovePrefix(delimiter, resultString);
      resultString = isPostfix ? Sitecore.StringUtil.EnsurePostfix(delimiter, resultString) : Sitecore.StringUtil.RemovePostfix(delimiter, resultString);

      return resultString;
    }

    [NotNull]
    public static string ComposeSharepointPath([NotNull] params string[] pathParts)
    {
      Assert.ArgumentNotNull(pathParts, "pathParts");

      return ConcatEnsureDelimiter('/', true, false, pathParts);
    }
  }
}