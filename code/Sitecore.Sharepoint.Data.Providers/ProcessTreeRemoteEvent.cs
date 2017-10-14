// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessTreeRemoteEvent.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ProcessTree remote event class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers
{
  using System.Runtime.Serialization;
  using Sitecore.Data;
  using Sitecore.Eventing;

  /// <summary>
  /// Defines the ProcessTree remote event class.
  /// </summary>
  [DataContract]
  public class ProcessTreeRemoteEvent : IHasEventName
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessTreeRemoteEvent"/> class.
    /// </summary>
    /// <param name="id">
    /// The id.
    /// </param>
    public ProcessTreeRemoteEvent(ID id, string databaseName)
    {
      this.Id = id;
      this.DatabaseName = databaseName;
      this.EventName = "spif:processTree";
    }

    /// <summary>
    /// Gets or sets the id of integrationConfigDataSource.
    /// </summary>
    [DataMember]
    public ID Id
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the DatabaseName.
    /// </summary>
    [DataMember]
    public string DatabaseName
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the name of the event.
    /// </summary>
    /// <value>The name of the event.</value>
    [DataMember]
    public string EventName
    {
      get;
      protected set;
    }
  }
}
