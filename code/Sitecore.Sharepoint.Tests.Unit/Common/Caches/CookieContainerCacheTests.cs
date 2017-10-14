// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CookieContainerCacheTests.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the CookieContainerCacheTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Common.Caches
{
  using System;
  using System.Net;
  using FluentAssertions;
  using Sitecore.Sharepoint.Common.Caches;
  using Xunit;

  public class CookieContainerCacheTests
  {
    private const string Url1 = "1";
    private const string Url2 = "2";

    private static readonly NetworkCredential Credential = new NetworkCredential();

    private readonly CookieContainer cookieContainer = new CookieContainer();

    private readonly CookieContainerCache cookieContainerCache = new CookieContainerCache();

    [Fact]
    public void CookieContainerCache_Should_return_Unexpired_CookieContainer()
    {
      // Act
      this.cookieContainerCache.Add(Url1, Credential, this.cookieContainer, DateTime.Now.AddDays(1));

      // Assert
      this.cookieContainerCache.GetUnexpired(Url1, Credential).Should().Be(this.cookieContainer);
    }

    [Fact]
    public void CookieContainerCache_Should_return_Unexpired_CookieContainer_with_right_key()
    {
      // Act
      this.cookieContainerCache.Add(Url1, Credential, this.cookieContainer, DateTime.Now.AddDays(1));

      // Assert
      this.cookieContainerCache.GetUnexpired(Url2, Credential).Should().Be(null);
    }

    [Fact]
    public void CookieContainerCache_Should_not_return_expired_CookieContainer()
    {
      // Act
      this.cookieContainerCache.Add(Url1, Credential, this.cookieContainer, DateTime.Now);

      // Assert
      this.cookieContainerCache.GetUnexpired(Url1, Credential).Should().Be(null);
    }

    [Fact]
    public void CookieContainerCache_Should_return_latest_Unexpired_CookieContainer()
    {
      // Act
      this.cookieContainerCache.Add(Url1, Credential, new CookieContainer(), DateTime.Now.AddDays(1));
      this.cookieContainerCache.Add(Url1, Credential, this.cookieContainer, DateTime.Now.AddDays(1));

      // Assert
      this.cookieContainerCache.GetUnexpired(Url1, Credential).Should().Be(this.cookieContainer);
    }

    [Fact]
    public void CookieContainerCache_Should_work_with_same_data_despite_of_instance()
    {
      // Act
      this.cookieContainerCache.Add(Url1, Credential, this.cookieContainer, DateTime.Now.AddDays(1));
      
      // Assert
      new CookieContainerCache().GetUnexpired(Url1, Credential).Should().Be(this.cookieContainer);
    }

    [Fact]
    public void CookieContainerCache_Should_not_return_CookieContainer_with_different_password()
    {
      // Act
      this.cookieContainerCache.Add(Url1, Credential, this.cookieContainer, DateTime.Now.AddDays(1));
      Credential.Password += "1";

      // Assert
      this.cookieContainerCache.GetUnexpired(Url1, Credential).Should().Be(null);
    }

    [Fact]
    public void CookieContainerCache_Should_not_return_CookieContainer_with_different_username()
    {
      // Act
      this.cookieContainerCache.Add(Url1, Credential, this.cookieContainer, DateTime.Now.AddDays(1));
      Credential.UserName += "1";

      // Assert
      this.cookieContainerCache.GetUnexpired(Url1, Credential).Should().Be(null);
    }

    [Fact]
    public void CookieContainerCache_Should_not_return_CookieContainer_with_different_domain()
    {
      // Act
      this.cookieContainerCache.Add(Url1, Credential, this.cookieContainer, DateTime.Now.AddDays(1));
      Credential.Domain += "1";

      // Assert
      this.cookieContainerCache.GetUnexpired(Url1, Credential).Should().Be(null);
    }
  }
}
