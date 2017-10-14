// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CookieContainerCache.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the CookieContainerCache type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Common.Caches
{
  using System;
  using System.Net;
  using Sitecore.Caching;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common.Configuration;

  public class CookieContainerCache : CustomCache, ICookieContainerCache
  {
    public CookieContainerCache()
      : base("Sharepoint.Caching.CookiesCache", Settings.CookiesCacheSize)
    {
    }

    public virtual void Add(string url, NetworkCredential credential, CookieContainer cookieContainer, DateTime expires)
    {
      Assert.ArgumentNotNull(url, "url");
      Assert.ArgumentNotNull(credential, "credential");
      Assert.ArgumentNotNull(cookieContainer, "cookieContainer");

      string key = this.GetKey(url, credential);
      this.SetObject(key, new Tuple<CookieContainer, DateTime>(cookieContainer, expires));
    }

    public virtual CookieContainer GetUnexpired(string url, NetworkCredential credential)
    {
      Assert.ArgumentNotNull(url, "url");
      Assert.ArgumentNotNull(credential, "credential");

      string key = this.GetKey(url, credential);
      var ret = (Tuple<CookieContainer, DateTime>)this.GetObject(key);
      if (ret != null)
      {
        if (ret.Item2 > DateTime.Now)
        {
          return ret.Item1;
        }

        this.Remove(key);
      }

      return null;
    }

    [NotNull]
    protected virtual string GetKey([NotNull] string url, [NotNull] NetworkCredential credential)
    {
      Assert.ArgumentNotNull(url, "url");
      Assert.ArgumentNotNull(credential, "credential");

      return url + credential.Domain + credential.UserName + credential.Password.GetHashCode();
    }
  }
}