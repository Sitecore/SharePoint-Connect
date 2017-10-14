// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CookieGetter.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the CookieGetter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Common.Authentication.Workflows.Helpers
{
  using System;
  using System.IdentityModel.Protocols.WSTrust;
  using System.IdentityModel.Services;
  using System.Net;
  using System.Text;
  using System.Web;
  using Sitecore.Diagnostics;

  public class CookieGetter
  {
    [NotNull]
    public virtual CookieContainer GetCookieOnPremises([NotNull] string url, [NotNull] RequestSecurityTokenResponse requestSecurityToken)
    {
      Assert.ArgumentNotNull(url, "url");
      Assert.ArgumentNotNull(requestSecurityToken, "requestSecurityToken");
      
      var serializer = new WSFederationSerializer();
      var responseAsString = serializer.GetResponseAsString(requestSecurityToken, new WSTrustSerializationContext());
      return this.GetCookieOnPremises(new Uri(url), responseAsString);
    }

    [NotNull]
    protected virtual CookieContainer GetCookieOnPremises([NotNull] Uri url, [NotNull] string adfsTokenXml)
    {
      Assert.ArgumentNotNull(url, "url");
      Assert.ArgumentNotNull(adfsTokenXml, "adfsTokenXml");

      var sharepointRequest = this.CreateWebRequest(url);

      this.SetRequestBody(sharepointRequest, url, adfsTokenXml);

      using (var webResponse = (HttpWebResponse)sharepointRequest.GetResponse())
      {
        return this.GetCookies(webResponse);
      }
    }

    [NotNull]
    protected virtual CookieContainer GetCookies([NotNull] HttpWebResponse webResponse)
    {
      Assert.ArgumentNotNull(webResponse, "webResponse");
      
      var ret = new CookieContainer();
      ret.Add(webResponse.Cookies);
      return ret;
    }

    protected virtual void SetRequestBody([NotNull] HttpWebRequest sharepointRequest, [NotNull] Uri url, [NotNull] string adfsTokenXml)
    {
      Assert.ArgumentNotNull(sharepointRequest, "sharepointRequest");
      Assert.ArgumentNotNull(url, "url");
      Assert.ArgumentNotNull(adfsTokenXml, "adfsTokenXml");
      
      using (var newStream = sharepointRequest.GetRequestStream())
      {
        var loginInformation = string.Format(
          "wa=wsignin1.0&wctx={0}&wresult={1}",
          HttpUtility.UrlEncode(url.GetLeftPart(UriPartial.Authority) + "/_layouts/Authenticate.aspx?Source=%2F"),
          HttpUtility.UrlEncode(adfsTokenXml));

        var loginInformationBytes = Encoding.UTF8.GetBytes(loginInformation);
        newStream.Write(loginInformationBytes, 0, loginInformationBytes.Length);
      }
    }

    protected virtual HttpWebRequest CreateWebRequest(Uri url)
    {
      Assert.ArgumentNotNull(url, "url");
      var sharepointRequest = (HttpWebRequest)WebRequest.Create(url);

      sharepointRequest.Method = "POST";
      sharepointRequest.ContentType = "application/x-www-form-urlencoded";
      sharepointRequest.CookieContainer = new CookieContainer();
      sharepointRequest.AllowAutoRedirect = false; // This is important

      return sharepointRequest;
    }
  }
}