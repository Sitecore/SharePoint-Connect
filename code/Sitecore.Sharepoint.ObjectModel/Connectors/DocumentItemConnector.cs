// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentItemConnector.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the DocumentItemConnector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Connectors
{
  using System;
  using System.IO;
  using System.Net;
  using Diagnostics;
  using Sharepoint.Data.WebServices.SharepointLists;
  using Sitecore.Sharepoint.Data.WebServices;
  using Sitecore.Sharepoint.Data.WebServices.SharepointCopy;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;
  using Sitecore.Sharepoint.ObjectModel.Utils;

  /// <summary>
  /// Object to work with web services
  /// </summary>
  public class DocumentItemConnector
  {
    /// <summary>
    /// Sharepoint security context 
    /// </summary>
    protected readonly SpContext context;

    /// <summary>
    /// Document that uses this connector
    /// </summary>
    protected readonly DocumentItem document;

    /// <summary>
    /// Web service to work with
    /// </summary>
    protected readonly Lists ListsWebService;

    /// <summary>
    /// Web service for copying files within a SharePoint site and between SharePoint sites
    /// </summary>
    protected readonly Copy CopyWebService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentItemConnector"/> class.
    /// </summary>
    /// <param name="context">
    /// Sharepoint context to be used to connect to web services
    /// </param>
    /// <param name="document">
    /// Document that will use this connector
    /// </param>
    public DocumentItemConnector([NotNull] SpContext context, [NotNull] DocumentItem document)
    {
      Assert.ArgumentNotNull(context, "context");
      Assert.ArgumentNotNull(document, "document");
      
      this.ListsWebService = new Lists();
      this.ListsWebService.SetServer(document.WebUrl, context);
      
      this.CopyWebService = new Copy();
      this.CopyWebService.SetServer(document.WebUrl, context);

      this.context = context;
      this.document = document;
    }

    /// <summary>
    /// Check in document
    /// </summary>
    /// <param name="comment">The comment for SharePoint</param>
    /// <param name="checkInType">The check in type. Possible values
    /// A string representation of the values 0, 1 or 2, where 0 = MinorCheckIn, 1 = MajorCheckIn, and 2 = OverwriteCheckIn.</param>
    /// <returns>
    /// Whether check in process has completed successfully
    /// </returns>
    public bool CheckIn([NotNull] string comment, [NotNull] string checkInType)
    {
      Assert.ArgumentNotNull(comment, "comment");
      Assert.ArgumentNotNull(checkInType, "checkInType");
      
      string documentUrl = StringUtil.EnsurePostfix('/', this.context.Url) + StringUtil.RemovePrefix('/', this.document.FileRef);
      SharepointUtils.LogDebugInfo(this.context, "Checking in file: [" + documentUrl + "]");

      return this.ListsWebService.CheckInFile(documentUrl, comment, checkInType);
    }

    /// <summary>
    /// Checkout document
    /// </summary>
    /// <param name="localCheckout">Whether document should be checked out locally or on the server side</param>
    /// <returns>
    /// Whether check out process has completed successfully
    /// </returns>
    public bool CheckOut(bool localCheckout)
    {
      string documentUrl = StringUtil.EnsurePostfix('/', this.context.Url) + StringUtil.RemovePrefix('/', this.document.FileRef);
      SharepointUtils.LogDebugInfo(this.context, "Checking out file: [" + documentUrl + "]");
      return this.ListsWebService.CheckOutFile(documentUrl, localCheckout ? "true" : "false", string.Empty);
    }

    /// <summary>
    /// Gets the binary data of the document
    /// </summary>
    /// <returns>The binary data of the document</returns>
    public byte[] GetStream()
    {
      string documentUrl = StringUtil.EnsurePostfix('/', this.context.Url) + StringUtil.RemovePrefix('/', this.document.FileRef);
      FieldInformation[] documentFieldList;
      byte[] documentStream;

      this.CopyWebService.GetItem(documentUrl, out documentFieldList, out documentStream);

      return documentStream;
    }

    public void UploadStream(Stream stream)
    {
      string sourceUrl = " ";
      string[] destinationUrls = { Uri.EscapeUriString(this.document.OpenItemUrl) };
      FieldInformation[] fields = { new FieldInformation() };
      CopyResult[] results;

      var buffer = new byte[stream.Length];
      stream.Read(buffer, 0, buffer.Length);

      this.CopyWebService.CopyIntoItems(sourceUrl, destinationUrls, fields, buffer, out results);
    }
  }
}