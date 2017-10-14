// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentItem.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the DocumentItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Entities.Items
{
  using System;
  using System.IO;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common.Texts;
  using Sitecore.Sharepoint.ObjectModel.Actions;
  using Sitecore.Sharepoint.ObjectModel.Connectors;
  using Sitecore.Sharepoint.ObjectModel.Entities.Collections;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;
  using Sitecore.Sharepoint.ObjectModel.Options;
  using Sitecore.Web;

  /// <summary>
  /// Represents SharePoint document
  /// </summary>
  public class DocumentItem : LibraryItem
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentItem"/> class.
    /// </summary>
    /// <param name="title">The title of item.</param>
    /// <param name="listName">The list name.</param>
    /// <param name="webUrl">The url of Web.</param>
    /// <param name="context">The context.</param>
    public DocumentItem([NotNull] string title, String parentFolder, [NotNull] string listName, [NotNull] Uri webUrl, [NotNull] SpContext context)
      : base(listName, webUrl, context)
    {
      // Add possibility to create DocumentItem object that does not exist in SharePoint source.

      Assert.ArgumentNotNull(title, "title");
      Assert.ArgumentNotNull(listName, "listName");
      Assert.ArgumentNotNull(webUrl, "webUrl");
      Assert.ArgumentNotNull(context, "context");

      this["ows_FileDirRef"] = StringUtil.RemovePrefix('/', StringUtil.RemovePostfix('/', StringUtil.RemovePostfix('/', this.RootFolder) + StringUtil.EnsurePrefix('/', parentFolder)));
      this["ows_FileLeafRef"] = title;
      this["ows_FileRef"] = StringUtil.EnsurePostfix('/', this["ows_FileDirRef"]) + title;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentItem"/> class.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="list">The list.</param>
    /// <param name="context">The context.</param>
    public DocumentItem([NotNull] EntityProperties property, [NotNull] BaseList list, [NotNull] SpContext context)
      : base(property, list, context)
    {
      Assert.ArgumentNotNull(property, "property");
      Assert.ArgumentNotNull(list, "list");
      Assert.ArgumentNotNull(context, "context");
    }

    /// <summary>                                                               
    /// Initializes a new instance of the <see cref="DocumentItem"/> class.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="listName">The list name.</param>
    /// <param name="webUrl">The url of Web.</param>
    /// <param name="context">The context.</param>
    public DocumentItem([NotNull] EntityProperties property, [NotNull] string listName, [NotNull] Uri webUrl, [NotNull] SpContext context)
      : base(property, listName, webUrl, context)
    {
      Assert.ArgumentNotNull(property, "property");
      Assert.ArgumentNotNull(listName, "listName");
      Assert.ArgumentNotNull(webUrl, "webUrl");
      Assert.ArgumentNotNull(context, "context");
    }

    /// <summary>  
    /// Gets FileRef.
    /// </summary>
    [CanBeNull]
    public string FileRef
    {
      get
      {
        return StringUtil.EnsurePrefix('/', this["ows_FileRef"]);
      }
    }

    /// <summary>
    /// Gets SharePoint url to Item.
    /// </summary>
    [CanBeNull]
    public virtual string OpenItemUrl
    {
      get
      {
        return string.Format(
                             "{0}{1}",
                             StringUtil.EnsurePostfix('/', WebUtil.GetServerUrl(WebUrl, false)),
                             this["ows_FileRef"]);
      }
    }

    /// <summary>
    /// Gets user who has checked out this document.
    /// </summary>
    [CanBeNull]
    public string CheckOutUser
    {
      get
      {
        return this.ContainsKey("ows_CheckoutUser") ? this["ows_CheckoutUser"] : string.Empty;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this document is checked out.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is checked out; otherwise, <c>false</c>.
    /// </value>
    public bool IsCheckedOut
    {
      get
      {
        return this.ContainsKey("ows_CheckoutUser");
      }
    }

    private Stream NewStream { get; set; }

    /// <summary>
    /// CheckIn document
    /// </summary>
    /// <param name="comment">
    /// Comment to pass to Sharepoint
    /// </param>
    /// <returns>
    /// whether check in finished successfully
    /// </returns>
    public bool CheckIn([NotNull] string comment)
    {
      Assert.ArgumentNotNull(comment, "comment");

      return new DocumentItemConnector(this.Context, this).CheckIn(comment, "1");
    }

    /// <summary>
    /// Checkout document
    /// </summary>
    /// <param name="localCheckout">
    /// Whether document should be checked out locally or on the server side
    /// </param>
    /// <returns>
    /// Whether check out process has completed successfully
    /// </returns>
    public bool CheckOut(bool localCheckout)
    {
      return new DocumentItemConnector(this.Context, this).CheckOut(localCheckout);
    }

    /// <summary>
    /// Upload stream to the server.
    /// </summary>
    /// <param name="data">The stream.</param>
    public void SetStream([NotNull] Stream streamData)
    {
      Assert.ArgumentNotNull(streamData, "streamData");

      this.NewStream = streamData;
    }

    /// <summary>
    /// Gets the binary data of the document
    /// </summary>
    /// <returns>The binary data of the document</returns>
    [CanBeNull]
    public Stream GetStream()
    {
      return new MemoryStream(new DocumentItemConnector(this.Context, this).GetStream());
    }

    public override EntityProperties Update()
    {
      EntityProperties returnProperties = null;

      if (this.NewStream != null)
      {
        new DocumentItemConnector(this.Context, this).UploadStream(this.NewStream);

        this.NewStream.Close();
        this.NewStream = null;
      }

      if (IsNew)
      {
        var itemsRetrievingOptions = new ItemsRetrievingOptions()
        {
          Folder = this.Folder,
          WherePart = String.Format("<Eq><FieldRef Name=\"{0}\"/><Value Type=\"Text\">{1}</Value></Eq>", "FileLeafRef", this["ows_FileLeafRef"])
        };
        ItemCollection item = this.List.GetItems(itemsRetrievingOptions);

        if (item.Count == 1)
        {
          this.Properties["ows_ID"] = ((DocumentItem)item[0]).Properties["ows_ID"];
          this.Properties["ows_FileLeafRef"] = ((DocumentItem)item[0]).Properties["ows_FileLeafRef"];
          this.Properties["ows_FileRef"] = ((DocumentItem)item[0]).Properties["ows_FileRef"];
          this.Properties["ows_FileDirRef"] = ((DocumentItem)item[0]).Properties["ows_FileDirRef"];

          foreach (var propertie in ((DocumentItem)item[0]).Properties)
          {
            if (this.Properties[propertie.Key] == null)
            {
              this.Properties[propertie.Key] = propertie.Value;
            }
          }
        }
        else
        {
          Log.Error("Can't reload item from Sharepoint.", this);
          throw new NullReferenceException("Can't reload item from Sharepoint.");
        }
      }

      if (!IsNew)
      {
        returnProperties = base.Update();
      }

      return returnProperties;
    }

    /// <summary>
    /// Gets the actions.
    /// </summary>
    /// <returns>List item actions.</returns>
    public override ActionCollection GetActions()
    {
      var actionsList = base.GetActions();
      var action = new ClientAction("open", UIMessages.OpenFile, "window.open('{0}');return false;", this, this.OpenItemUrl)
      {
        IsDefault = true
      };

      actionsList.Insert(0, action);

      if (this.IsCheckedOut)
      {
        actionsList.Insert(1, new ServerAction("checkin", UIMessages.CheckIn, this.DocumentCheckIn, this));
      }
      else
      {
        actionsList.Insert(1, new ServerAction("checkout", UIMessages.CheckOut, this.DocumentCheckOut, this));
      }

      actionsList.Add(new LinkAction("properties", UIMessages.Properties, this.PropertiesUrl, "_blank", this, null));
      return actionsList;
    }

    /// <summary>
    /// Executes the check in event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void DocumentCheckIn([CanBeNull] object sender, [CanBeNull] EventArgs e)
    {
      this.CheckIn("checkin");
    }

    /// <summary>
    /// Executes the check out event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void DocumentCheckOut([CanBeNull] object sender, [CanBeNull] EventArgs e)
    {
      this.CheckOut(false);
    }
  }
}