namespace Sitecore.Sharepoint.ObjectModel.Utils
{
  using System.Globalization;
  using Sitecore.Diagnostics;
  using System;
  using Sitecore.Globalization;

  public static class DateUtil
  {
    public static string FormatShortDate(DateTime dateTime)
    {
      return FormatDateTime(dateTime, "d");
    }

    private static string FormatDateTime(DateTime dateTime, string format)
    {
      return FormatDateTime(dateTime, format, Context.Culture);
    }

    private static string FormatDateTime(DateTime dateTime, string format, CultureInfo culture)
    {
      if (culture.IsNeutralCulture)
      {
        culture = Language.CreateSpecificCulture(culture.Name);
      }
      return dateTime.ToString(format, culture);
    }
  }
}
