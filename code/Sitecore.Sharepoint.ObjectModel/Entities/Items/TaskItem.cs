// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskItem.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the TaskItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Sitecore.Sharepoint.ObjectModel.Entities.Items
{
  using System;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;

  /// <summary>
  /// Represents SharePoint task.
  /// </summary>
  public class TaskItem : ListItem
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TaskItem"/> class.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="list">The list.</param>
    /// <param name="context">The context.</param>
    public TaskItem([NotNull] EntityProperties property, [NotNull] BaseList list, [NotNull] SpContext context)
      : base(property, list, context)
    {
      Assert.ArgumentNotNull(property, "property");
      Assert.ArgumentNotNull(list, "list");
      Assert.ArgumentNotNull(context, "context");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskItem"/> class.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="listName">The list name.</param>
    /// <param name="webUrl">The url of Web.</param>
    /// <param name="context">The context.</param>
    public TaskItem([NotNull] EntityProperties property, [NotNull] string listName, [NotNull] Uri webUrl, [NotNull] SpContext context)
      : base(property, listName, webUrl, context)
    {
      Assert.ArgumentNotNull(property, "property");
      Assert.ArgumentNotNull(listName, "listName");
      Assert.ArgumentNotNull(webUrl, "webUrl");
      Assert.ArgumentNotNull(context, "context");
    }

    /// <summary>
    /// Gets task priority.
    /// </summary>
    [CanBeNull]
    public string Priority
    {
      get
      {
        return this["ows_Priority"];
      }
    }

    /// <summary>
    /// Gets task status.
    /// </summary>
    [CanBeNull]
    public string Status
    {
      get
      {
        return this["ows_Status"];
      }
    }

    /// <summary>
    /// Gets value indicate who assigned on the task.
    /// </summary>
    [CanBeNull]
    public string AssignedTo
    {
      get
      {
        return this["ows_AssignedTo"];
      }
    }

    /// <summary>
    /// Gets task duedate.
    /// </summary>
    public DateTime DueDate
    {
      get
      {
        DateTime dueDate;
        DateTime.TryParse(this["ows_DueDate"], out dueDate);
        return dueDate;
      }
    }

    /// <summary>
    /// Gets the value indicate how much the task is complete.
    /// </summary>
    public double Complete
    {
      get
      {
        double complete;
        double.TryParse(this["ows_PercentComplete"], out complete);
        return complete;
      }
    }
  }
}