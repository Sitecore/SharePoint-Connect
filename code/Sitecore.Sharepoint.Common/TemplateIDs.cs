// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplateIDs.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines TemplateIDs class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Common
{
  using Sitecore.Data;

  public static class TemplateIDs
  {
    public static readonly ID IntegrationFolder = new ID("{F28EDE93-DB84-472E-B45B-595968A9C97C}");

    public static readonly ID IntegrationConfig = new ID("{E3E23DCC-CA55-4049-BBF0-B503023517C6}");

    public static readonly ID IntegrationBase = new ID("{0F463681-B941-421A-A69E-73AE74145736}");

    /// <summary>
    /// Item ID for /sitecore/templates/Sharepoint/Item Level Integration/Sharepoint Integration Items
    /// This is a parent for all auto generated data templates 
    /// </summary>
    public static readonly ID IntegrationItemTemplatesRoot = new ID("{2CAF221E-8238-494E-BA53-7D2CD34E8A0B}");

    public static readonly ID SharepointWeb = new ID("{32D3E6C7-5C52-4E0E-9901-4234AB1C9347}");
  }
}
