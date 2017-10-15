// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HistoryProviderIDTableTests.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the HistoryProviderIDTableTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.Pipelines.SynchronizeTree.Helpers
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using FluentAssertions;

  using Microsoft.Extensions.DependencyInjection;

  using NSubstitute;

  using Ploeh.AutoFixture.Xunit2;

  using Sitecore.Abstractions;
  using Sitecore.Collections;
  using Sitecore.Configuration;
  using Sitecore.Data;
  using Sitecore.Data.IDTables;
  using Sitecore.Data.Items;
  using Sitecore.DependencyInjection;
  using Sitecore.Events;
  using Sitecore.Sharepoint.Common;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Cache;
  using Sitecore.Sharepoint.Data.Providers.IntegrationConfig;
  using Sitecore.Sharepoint.Data.Providers.Items;
  using Sitecore.Sharepoint.Data.Providers.SyncActions;
  using Sitecore.Sharepoint.Pipelines.SynchronizeTree;
  using Sitecore.Sharepoint.Pipelines.SynchronizeTree.Helpers;
  using Sitecore.Web.UI.Sheer;
  using Xunit;
  using Xunit.Extensions;

  using Actions = GenericKeyValurPairCollection<bool, string>;
  using TestItems = GenericKeyValurPairCollection<string, string>;

  public class HistoryProviderIDTableTests
  {
    private const string PrefixSave = HistoryProviderIDTable.PrefixSave;

    private const string PrefixDelete = HistoryProviderIDTable.PrefixDelete;

    private readonly IDTableProvider tableProvider = Substitute.For<IDTableProvider>();

    private readonly BaseFactory factory = Substitute.For<BaseFactory>();

    private readonly HistoryProviderIDTable provider;

    public HistoryProviderIDTableTests()
    {
      var defaultFactory = ServiceLocator.ServiceProvider.GetRequiredService<BaseFactory>();
      var cache = (Hashtable)typeof(DefaultFactory).GetField("cache", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(defaultFactory);
      cache["IDTable"] = this.tableProvider;
      this.provider = new HistoryProviderIDTable(this.factory);
    }

    [Fact]
    public void IsItemChanged_should_return_true_if_item_is_in_id_table()
    {
      // Act
      var isItemChanged = this.provider.IsItemChanged(this.CreateArgsWithDefaultEntries(), "ChangedGUID");

      // Assert
      isItemChanged.Should().BeTrue();
    }

    [Fact]
    public void IsItemChanged_should_return_false_if_item_is_not_in_id_table()
    {
      // Act
      var isItemChanged = this.provider.IsItemChanged(this.CreateArgsWithDefaultEntries(), "NotChangedGUID");

      // Assert
      isItemChanged.Should().BeFalse();
    }

    [Fact]
    public void IsItemDeleted_should_return_true_if_item_is_in_id_table()
    {
      // Act
      var isItemChanged = this.provider.IsItemDeleted(this.CreateArgsWithDefaultEntries(), "DeletedGUID");

      // Assert
      isItemChanged.Should().BeTrue();
    }

    [Fact]
    public void IsItemDeleted_should_return_false_if_item_is_not_in_id_table()
    {
      // Act
      var isItemChanged = this.provider.IsItemChanged(this.CreateArgsWithDefaultEntries(), "NotDeletedGUID");

      // Assert
      isItemChanged.Should().BeFalse();
    }

    [Fact]
    public void Refresh_should_throw_if_args_is_not_set()
    {
      // Act
      Action action = () => this.provider.Refresh(null);

      // Assert
      action.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: args");
    }

    [Fact]
    public void Refresh_should_throw_if_action_list_is_not_set()
    {
      // Act
      Action action = () => this.provider.Refresh(new SynchronizeTreeArgs());

      // Assert
      action.ShouldThrow<InvalidOperationException>().WithMessage("Value can't be null: args.ActionList");
    }

    [Fact]
    public void Refresh_should_delete_all_success_entries_form_the_id_table()
    {
      // Arrange
      var args = this.CreateArgsWithDefaultEntries(new Actions { { true, "ChangedGUID" }, { true, "DeletedGUID" } });
      var dictionary = ((Dictionary<string, List<IDTableEntry>>)args.CustomData["IDTableEntries"]);

      // Act
      this.provider.Refresh(args);

      // Assert
      this.tableProvider.Received(1).Remove(PrefixSave, dictionary[PrefixSave][0].Key);
      this.tableProvider.Received(1).Remove(PrefixDelete, dictionary[PrefixDelete][0].Key);
    }

    [Fact]
    public void Refresh_should_not_delete_any_failed_entries_form_the_id_table()
    {
      // Arrange
      var args = this.CreateArgsWithDefaultEntries(new Actions { { false, "DeletedGUID" }, { false, "ChangedGUID" } });

      // Act
      this.provider.Refresh(args);

      // Assert
      this.tableProvider.DidNotReceiveWithAnyArgs().Remove(default(string), default(string));
    }

    [Fact]
    public void Refresh_should_remove_all_entries_without_action()
    {
      // Arrange
      var args = this.CreateArgsWithDefaultEntries(new Actions { { true, "SomeGuid" } });
      var dictionary = ((Dictionary<string, List<IDTableEntry>>)args.CustomData["IDTableEntries"]);

      // Act
      this.provider.Refresh(args);

      // Assert
      this.tableProvider.Received(1).Remove(PrefixDelete, dictionary[PrefixDelete][0].Key);
      this.tableProvider.Received(1).Remove(PrefixSave, dictionary[PrefixSave][0].Key);
    }

    [Fact]
    public void Refresh_should_not_throw_if_there_is_no_entries()
    {
      // Arrange
      var args = this.CreateArgsWithDefaultEntries(new Actions { true });
      
      this.tableProvider.GetKeys(null).ReturnsForAnyArgs(new IDTableEntry[0]);

      // Act
      Action action = () => this.provider.Refresh(args);

      // Assert
      action.ShouldNotThrow();
    }

    [Fact]
    public void Init_should_init_save_dictionary()
    {
      // Arrange
      var synchronizeTreeArgs = new SynchronizeTreeArgs();
      synchronizeTreeArgs.Context = this.CreateSynchContext(new IntegrationConfigData("server", "list", "templateID"));
      var idTableEntries = new[] { new IDTableEntry(PrefixSave, "key", ID.NewID, synchronizeTreeArgs.Context.ParentID, "customData") };
      this.tableProvider.GetKeys(PrefixSave).Returns(idTableEntries);

      // Act
      this.provider.Init(synchronizeTreeArgs);
      
      // Assert
      var dictionary = (Dictionary<string, List<IDTableEntry>>)synchronizeTreeArgs.CustomData["IDTableEntries"];
      dictionary[PrefixSave].Should().Contain(idTableEntries);
    }

    [Fact]
    public void Init_should_init_delete_dictionary()
    {
      var synchronizeTreeArgs = new SynchronizeTreeArgs();
      synchronizeTreeArgs.Context = this.CreateSynchContext(new IntegrationConfigData("server", "list", "templateID"));
      var idTableEntries = new[] { new IDTableEntry(PrefixDelete, "key", ID.NewID, synchronizeTreeArgs.Context.ParentID, "customData") };
      this.tableProvider.GetKeys(PrefixDelete).Returns(idTableEntries);

      // Act
      this.provider.Init(synchronizeTreeArgs);

      // Assert
      var dictionary = (Dictionary<string, List<IDTableEntry>>)synchronizeTreeArgs.CustomData["IDTableEntries"];
      dictionary[PrefixDelete].Should().Contain(idTableEntries);
    }

    [Fact]
    public void Init_should_return_entries_for_parentid_only()
    {
      var synchronizeTreeArgs = new SynchronizeTreeArgs();
      synchronizeTreeArgs.Context = this.CreateSynchContext(new IntegrationConfigData("server", "list", "templateID"));
      var idTableEntries = new[]
      {
        new IDTableEntry(PrefixDelete, "key", ID.NewID, synchronizeTreeArgs.Context.ParentID, "customData"),
        new IDTableEntry(PrefixDelete, "key", ID.NewID, ID.NewID, "customData")
      };
      this.tableProvider.GetKeys(PrefixDelete).Returns(idTableEntries);

      // Act
      this.provider.Init(synchronizeTreeArgs);

      // Assert
      var dictionary = (Dictionary<string, List<IDTableEntry>>)synchronizeTreeArgs.CustomData["IDTableEntries"];
      dictionary[PrefixDelete].Should().Contain(idTableEntries[0]);
      dictionary[PrefixDelete].Should().NotContain(idTableEntries[1]);
    }

    [Fact]
    public void PrepareToDelete_should_throw_if_args_is_not_set()
    {
      // Act
      Action action = () => this.provider.PrepareToDelete(null);

      // Assert
      action.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: args");
    }

    [Fact]
    public void PrepareToDelete_should_add_all_active_integration_items_to_the_custom_data()
    {
      // Arrange
      var items = this.CreateItems(new TestItems { { "1", "sharepointGUID1" }, { "2", "sharepointGUID2" } });

      var args = new ClientPipelineArgs();
      args.Parameters["items"] = string.Join("|", items.Select(x => x.ID));
      args.Parameters["database"] = items.First().Database.Name;

      // Act
      this.provider.PrepareToDelete(args);

      // Assert
      args.CustomData.Should().Contain(new SafeDictionary<string, object>
      {
        { items[0].ID.ToString(), "sharepointGUID1" },
        { items[0].ID + "parentID", items[0].ParentID },
        { items[1].ID.ToString(), "sharepointGUID2" },
        { items[1].ID + "parentID", items[1].ParentID }
      });
    }

    [Theory, AutoData]
    public void PrepareToDelete_should_not_add_new_active_integration_items_to_the_custom_data(string databaseName)
    {
      // Arrange
      var items = this.CreateItems(new TestItems { { "1", "sharepointGUID1" }, { "2", "sharepointGUID2" } });
      var args = new ClientPipelineArgs();
      args.Parameters["items"] = string.Join("|", items.Select(x => x.ID));
      args.Parameters["database"] = items.First().Database.Name;
      new IntegrationItem(items[0]).GUID = string.Empty;

      // Act
      this.provider.PrepareToDelete(args);

      // Assert
      args.CustomData.Should().NotContain(x => x.Key == items[0].ID.ToString() || x.Key == items[0].ID + "parentID");
    }

    [Fact]
    public void PrepareToDelete_should_not_add_any_not_integration_items_to_the_custom_data()
    {
      // Arrange
      var items = this.CreateItems(new TestItems { { "1", "1" }, { "2", "2" } }, itemName => false);
      var args = new ClientPipelineArgs();
      args.Parameters["items"] = string.Join("|", items.Select(x => x.ID));
      args.Parameters["database"] = items.First().Database.Name;

      // Act
      this.provider.PrepareToDelete(args);

      // Assert
      args.CustomData.Should().NotContain(new KeyValuePair<string, object>(items[0].ID.ToString(), items[0]));
      args.CustomData.Should().NotContain(new KeyValuePair<string, object>(items[1].ID.ToString(), items[1]));
    }

    [Fact]
    public void PrepareToDelete_should_mark_delete_as_canceled_id_configuration_item_appear()
    {
      // Arrange
      var items = this.CreateItems(
      new TestItems { { "1", "1" }, { "2", "2" } }, 
      itemName => true, 
      itemName => itemName == "2" ? TemplateIDs.IntegrationConfig : TemplateIDs.IntegrationBase);

      var args = new ClientPipelineArgs();
      args.Parameters["items"] = string.Join("|", items.Select(x => x.ID));
      args.Parameters["database"] = items.First().Database.Name;

      // Act
      this.provider.PrepareToDelete(args);

      // Assert
      args.Parameters["SharepointUpdateCancelation"].Should().Be("1");
    }

    [Fact]
    public void ProcessDelete_should_throw_if_args_is_not_set()
    {
      // Act
      Action action = () => this.provider.ProcessDelete(null);

      // Assert
      action.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: args");
    }

    [Fact]
    public void ProcessDelete_should_add_entries_for_all_items_from_custom_data()
    {
      // Arrange
      var nameGuids = new TestItems { { "1", "GUID1" }, { "2", "GUID2" } };
      var items = this.CreateItems(nameGuids);

      var args = new ClientPipelineArgs();
      args.Parameters["items"] = string.Join("|", items.Select(x => x.ID));
      foreach (var item in items)
      {
        args.CustomData.Add(item.ID.ToString(), item[FieldNames.GUID]);
        args.CustomData.Add(item.ID + "parentID", item.ParentID);
      }

      // Act
      this.provider.ProcessDelete(args);

      // Assert
      foreach (var item in items)
      {
        this.tableProvider.Received().Add(Arg.Is<IDTableEntry>(x => x.Prefix == PrefixDelete && x.CustomData == item.Fields[FieldNames.GUID].Value && x.ParentID == item.ParentID));
      }
    }

    [Fact]
    public void ProcessDelete_should_ignore_items_not_present_in_args()
    {
      // Arrange
      var nameGuids = new TestItems { { "1", "GUID1" }, { "2", "GUID2" }, { "3", "GUID3" }, { "4", "GUID4" } };
      var items = this.CreateItems(nameGuids);
      
      var args = new ClientPipelineArgs();
      args.Parameters["items"] = string.Join("|", items.Select(x => x.ID));
      
      // Add full item info
      args.CustomData.Add(items[0].ID.ToString(), items[0][FieldNames.GUID]);
      args.CustomData.Add(items[0].ID + "parentID", items[0].ParentID);
      
      // Add only item id for 2 item
      args.CustomData.Add(items[1].ID.ToString(), items[1][FieldNames.GUID]);

      // Add only parent id for 3 item
      args.CustomData.Add(items[2].ID.ToString(), string.Empty);
      args.CustomData.Add(items[2].ID + "parentID", items[2].ParentID);

      // Add nothing parent id for 4 item
      
      // Act
      this.provider.ProcessDelete(args);

      // Assert
      this.tableProvider.Received(1).Add(Arg.Is<IDTableEntry>(x => x.Prefix == PrefixDelete && x.CustomData == items[0].Fields[FieldNames.GUID].Value && x.ParentID == items[0].ParentID));
      this.tableProvider.DidNotReceive().Add(Arg.Is<IDTableEntry>(x => x.Prefix == PrefixDelete && x.CustomData == items[1].Fields[FieldNames.GUID].Value && x.ParentID == items[1].ParentID));
      this.tableProvider.DidNotReceive().Add(Arg.Is<IDTableEntry>(x => x.Prefix == PrefixDelete && x.CustomData == items[2].Fields[FieldNames.GUID].Value && x.ParentID == items[2].ParentID));
      this.tableProvider.DidNotReceive().Add(Arg.Is<IDTableEntry>(x => x.Prefix == PrefixDelete && x.CustomData == items[3].Fields[FieldNames.GUID].Value && x.ParentID == items[3].ParentID));
    }

    [Fact]
    public void ProcessDelete_should_do_nothing_if_cancel_flag_is_set()
    {
      // Arrange
      var items = this.CreateItems(new TestItems { { "1", "GUID1" } });

      var args = new ClientPipelineArgs();
      args.Parameters["SharepointUpdateCancelation"] = "1";
      args.Parameters["items"] = string.Join("|", items.Select(x => x.ID));
      foreach (var item in items)
      {
        args.CustomData.Add(item.ID.ToString(), item);
      }
      
      // Act
      this.provider.ProcessDelete(args);

      // Assert
      this.tableProvider.DidNotReceiveWithAnyArgs().Add(null);
    }

    [Fact]
    public void OnItemSaved_should_add_entry_in_table_for_active_integration_item()
    {
      // Arrange
      var item = this.CreateItems(new TestItems { { "1", "GUID1" } })[0];
      using (new ItemIsContentItem(item))
      {
        var eventArgs = new SitecoreEventArgs("testEvent", new object[] { item }, new EventResult());

        // Act
        this.provider.OnItemSaved(this, eventArgs);

        // Assert
        this.tableProvider.Received()
          .Add(
            Arg.Is<IDTableEntry>(x => x.Prefix == PrefixSave && x.CustomData == "GUID1" && x.ParentID == item.ParentID));
      }
    }

    [Fact]
    public void OnItemSaved_should_not_add_entry_in_table_for_not_integration_item()
    {
      // Arrange
      var item = this.CreateItems(new TestItems { { "1", "GUID1" } }, itemName => false)[0];
      var eventArgs = new SitecoreEventArgs("testEvent", new object[] { item }, new EventResult());

      // Act
      this.provider.OnItemSaved(this, eventArgs);

      // Assert
      this.tableProvider.DidNotReceiveWithAnyArgs().Add(default(IDTableEntry));
    }

    [Fact]
    public void OnItemSaved_should_not_add_entry_in_table_for_new_integration_item()
    {
      // Arrange
      var item = this.CreateItems(new TestItems { { "1", "GUID1" } })[0];
      var eventArgs = new SitecoreEventArgs("testEvent", new object[] { item }, new EventResult());
      new IntegrationItem(item).GUID = string.Empty;

      // Act
      this.provider.OnItemSaved(this, eventArgs);

      // Assert
      this.tableProvider.DidNotReceiveWithAnyArgs().Add(default(IDTableEntry));
    }

    [Fact]
    public void OnItemSaved_should_do_nothing_if_IntegrationDisabler_is_enabled()
    {
      // Arrange
      var item = this.CreateItems(new TestItems { { "1", "GUID1" } })[0];
      var eventArgs = new SitecoreEventArgs("testEvent", new object[] { item }, new EventResult());

      // Act
      using (new IntegrationDisabler())
      {
        this.provider.OnItemSaved(this, eventArgs);
      }

      // Assert
      this.tableProvider.DidNotReceiveWithAnyArgs().Add(default(IDTableEntry));
    }

    [Fact]
    public void should_throw_if_item_is_not_set()
    {
      // Act
      Action action = () => this.provider.OnItemSaved(this, new SitecoreEventArgs("testEvent", new[] { new object() }, new EventResult()));

      // Assert
      action.ShouldThrow<InvalidOperationException>().WithMessage("No item in parameters");
    }

    private List<SyncActionBase> CreateActions(Actions actionDedinitions)
    {
      var actionList = new List<SyncActionBase>();
      foreach (KeyValuePair<bool, string> actionDefinition in actionDedinitions)
      {
        var action = Substitute.For<SyncActionBase>();
        action.IsSuccessful.Returns(actionDefinition.Key);
        action.SharepotinGUID.Returns(actionDefinition.Value);
        actionList.Add(action);
      }

      return actionList;
    }

    private List<Item> CreateItems(TestItems testItems, Func<string, bool> isActiveIntegrationItemFunc = null, Func<string, ID> chooseTemplateFunc = null)
    {
      var result = new List<Item>();

      var database = Substitute.For<Database>();
      database.Name.Returns(ID.NewID.ToString());

      this.factory.GetDatabase(database.Name).Returns(database);

      var root = new ItemMock(ID.NewID, database);
      foreach (KeyValuePair<string, string> testItem in testItems)
      {
        var id = ID.NewID;
        var item =
          new ItemMock(id, database)
            {
              { FieldNames.GUID, testItem.Value },
              {
                "IsIntegration", FieldIDs.IsIntegrationItem,
                isActiveIntegrationItemFunc != null
                  ? (isActiveIntegrationItemFunc(testItem.Key) ? "1" : "0")
                  : "1"
              }
            }.WithTemplate(
              chooseTemplateFunc != null
                ? chooseTemplateFunc(testItem.Key)
                : TemplateIDs.IntegrationBase).WithParent(root);

        result.Add(item);
      }

      return result;
    }

    private SynchContext CreateSynchContext(IntegrationConfigData integrationConfigData)
    {
      var id = ID.NewID;
      IntegrationCache.AddIntegrationConfigData(id, integrationConfigData, 0);
      var synchContext = new SynchContext(id, Substitute.For<Database>());
      return synchContext;
    }

    private SynchronizeTreeArgs CreateArgsWithDefaultEntries(Actions actions = null, bool stubsTableProvider = true)
    {
      var args = new SynchronizeTreeArgs();
      if (actions != null)
      {
        args.ActionList = this.CreateActions(actions);
      }

      var context = this.CreateSynchContext(new IntegrationConfigData("server", "list", "templateID"));
      args.Context = context;
      var saveEntries = new List<IDTableEntry> { new IDTableEntry(PrefixSave, ID.NewID.ToString(), ID.NewID, context.ParentID, "ChangedGUID") };
      var deleteEntries = new List<IDTableEntry> { new IDTableEntry(PrefixDelete, ID.NewID.ToString(), ID.NewID, context.ParentID, "DeletedGUID") };
      args.CustomData["IDTableEntries"] = new Dictionary<string, List<IDTableEntry>>
      {
        { PrefixSave, saveEntries },
        { PrefixDelete, deleteEntries }
      };

      if (stubsTableProvider)
      {
        this.tableProvider.GetKeys(PrefixSave, Arg.Any<ID>()).Returns(saveEntries.ToArray());
        this.tableProvider.GetKeys(PrefixDelete, Arg.Any<ID>()).Returns(deleteEntries.ToArray());
      }

      return args;
    }
  }
}
