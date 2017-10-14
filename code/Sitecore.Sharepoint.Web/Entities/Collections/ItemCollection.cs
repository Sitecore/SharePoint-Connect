// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemCollection.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines ItemCollection class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Web.Entities.Collections
{
  using System.Collections.Generic;
  using System.IO;
  using System.Runtime.Serialization.Formatters.Binary;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.ObjectModel;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;
  using Sitecore.Sharepoint.ObjectModel.Options;

  public class ItemCollection : ObjectModel.Entities.Collections.ItemCollection
  {
    public const string OptionsSerializationObjectName = "Options";
    public const string PageIndexSerializationObjectName = "PageIndex";
    public const string PagingQueriesSerializationObjectName = "PagingQueries";

    public ItemCollection([NotNull] SpContext context, [NotNull] BaseList list, [NotNull] ItemsRetrievingOptions options)
      : base(context, list, options)
    {
      Assert.ArgumentNotNull(context, "context");
      Assert.ArgumentNotNull(list, "list");
      Assert.ArgumentNotNull(options, "options");
    }

    public ItemCollection([NotNull] SpContext context, [NotNull] BaseList list, [NotNull] string serializationString)
      : this(context, list, new ItemsRetrievingOptions())
    {
      Assert.ArgumentNotNull(context, "context");
      Assert.ArgumentNotNull(list, "list");
      Assert.ArgumentNotNull(serializationString, "serializationString");

      var objects = (Dictionary<string, object>)this.Deserialize(serializationString);

      this.Options = (ItemsRetrievingOptions)objects[OptionsSerializationObjectName];
      this.PageIndex = (int)objects[PageIndexSerializationObjectName];
      this.PagingQueryCollection = (Dictionary<int, string>)objects[PagingQueriesSerializationObjectName];
    }

    [NotNull]
    public virtual string Serialize()
    {
      var objects = new Dictionary<string, object>();
      objects.Add(OptionsSerializationObjectName, this.Options);
      objects.Add(PageIndexSerializationObjectName, this.PageIndex);
      objects.Add(PagingQueriesSerializationObjectName, this.PagingQueryCollection);

      return this.Serialize(objects);
    }

    [NotNull]
    protected string Serialize([NotNull] object targetObject)
    {
      Assert.ArgumentNotNull(targetObject, "targetObject");

      var formatter = new BinaryFormatter();
      var stream = new MemoryStream();
      formatter.Serialize(stream, targetObject);

      byte[] buffer = stream.GetBuffer();

      return Convert.ObjectToBase64(buffer);
    }

    [NotNull]
    protected object Deserialize([NotNull] string serializationString)
    {
      Assert.ArgumentNotNull(serializationString, "serializationString");

      var buffer = (byte[])Convert.Base64ToObject(serializationString);

      var formatter = new BinaryFormatter();
      var stream = new MemoryStream(buffer);

      return formatter.Deserialize(stream);
    }

    public int CurrentPage
    {
      get
      {
        return this.PageIndex;
      }
    }
  }
}