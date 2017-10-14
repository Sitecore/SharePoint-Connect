// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationItem.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the IntegrationItem class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.Items
{
  using Sitecore.Data;
  using Sitecore.Data.Fields;
  using Sitecore.Data.Items;

  public class IntegrationItem : CustomItemBase
  {
    public IntegrationItem(Item item)
      : base(item)
    {
    }

    public Field IsActiveField
    {
      get
      {
        return this.InnerItem.Fields[Common.FieldIDs.IsIntegrationItem];
      }
    }

    public bool IsActive
    {
      get
      {
        return new CheckboxField(this.IsActiveField).Checked;
      }

      set
      {
        using (new EditContext(this.InnerItem))
        {
          new CheckboxField(this.IsActiveField).Checked = value;
        }
      }
    }

    public string GUID
    {
      get
      {
        return this.InnerItem.Fields[Common.FieldNames.GUID].Value;
      }

      set
      {
        using (new EditContext(this.InnerItem))
        {
          this.InnerItem.Fields[Common.FieldNames.GUID].Value = value;
        }
      }
    }

    public bool IsNew
    {
      get
      {
        return string.IsNullOrWhiteSpace(this.GUID);
      }
    }

    public ID TemplateID
    {
      get
      {
        return this.InnerItem.TemplateID;
      }
    }
  }
}
