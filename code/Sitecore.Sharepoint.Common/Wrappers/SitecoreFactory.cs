// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SitecoreFactory.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SitecoreFactoryWrapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Common.Wrappers
{
  using Sitecore.Configuration;

  public class SitecoreFactory
  {
    public virtual object CreateObject(string name, bool assert)
    {
      return Factory.CreateObject(name, assert);
    }
  }
}