// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectModelUtil.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ObjectModelUtil class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Intranet.Sharepoint.Web.Utils
{
  using System.Collections.Generic;
  using System.Net;
  using Sitecore.Sharepoint.ObjectModel.Entities.Collections;
  using WebEntity = Sitecore.Sharepoint.ObjectModel.Entities.Web;

  public class ObjectModelUtil
  {
    // TODO: [110516 dan] Add possibility to get only accessible Webs using Sitecore.Sharepoint.ObjectModel.Entities.Collections.WebCollection class.
    public static List<WebEntity> GetAccessibleWebs(WebCollection webs)
    {
      var accessibleWebs = new List<WebEntity>();

      foreach (WebEntity web in webs)
      {
        WebEntity accessibleWeb = null;

        try
        {
          accessibleWeb = web.GetWeb(web.Path);
        }
        catch (WebException webException)
        {
          var httpWebResponse = webException.Response as HttpWebResponse;
          if (httpWebResponse == null || httpWebResponse.StatusCode != HttpStatusCode.Unauthorized)
          {
            throw webException;
          }
        }

        if (accessibleWeb != null)
        {
          accessibleWebs.Add(web);
        }
      }

      return accessibleWebs;
    }
  }
}