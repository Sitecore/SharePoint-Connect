// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICookieContainerCache.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ICookieContainerCache type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Common.Caches
{
  using System;
  using System.Net;

  public interface ICookieContainerCache
  {
    void Add([NotNull] string url, [NotNull] NetworkCredential credential, [NotNull] CookieContainer cookieContainer, DateTime expires);

    [CanBeNull]
    CookieContainer GetUnexpired([NotNull] string url, [NotNull] NetworkCredential credential);
  }
}