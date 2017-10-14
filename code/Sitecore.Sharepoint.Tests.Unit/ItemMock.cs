namespace Sitecore.Sharepoint.Tests.Unit
{
  using System;
  using System.Collections;

  using NSubstitute;

  using Sitecore.Collections;
  using Sitecore.Data;
  using Sitecore.Data.Engines;
  using Sitecore.Data.Fields;
  using Sitecore.Data.Items;
  using Sitecore.Data.Templates;

  public class ItemMock : IEnumerable
  {
    private readonly ItemList childList = new ItemList();

    public ItemMock(ID id = null, Database database = null)
    {
      this.Item = Substitute.For<Item>(id ?? ID.NewID, ItemData.Empty, database ?? Substitute.For<Database>());
      this.Item.Fields.Returns(Substitute.For<FieldCollection>(this.Item));
      this.Item.Editing.Returns(Substitute.For<ItemEditing>(this.Item));

      var templateItem = Substitute.For<TemplateItem>(this.Item);
      this.Item.Template.Returns(templateItem);

      this.Item.Children.Returns(new ChildList(this.Item, this.childList));
      this.Item.Database.GetItem(this.Item.ID).Returns(this.Item);
      this.Item.Database.GetItem(this.Item.ID.ToString()).Returns(this.Item);
    }

    public ID ID
    {
      get
      {
        return this.Item.ID;
      }
    }

    private Item Item { get; set; }

    public static implicit operator Item(ItemMock itemMock)
    {
      return itemMock.Item;
    }

    public IEnumerator GetEnumerator()
    {
      throw new NotImplementedException();
    }

    public void Add(string name, ID id, string value)
    {
      this.WithField(name, id, value);
    }

    public void Add(string name, string value)
    {
      this.WithField(name, value);
    }

    public void Add(ID id, string value)
    {
      this.WithField(id, value);
    }

    public ItemMock WithTemplate(ID templateId)
    {
      this.Item.Template.ID.Returns(templateId);

      this.Item.TemplateID.Returns(templateId);

      var runtimeSettings = Substitute.For<ItemRuntimeSettings>(this.Item);
      runtimeSettings.TemplateDatabase.Returns(this.Item.Database);
      this.Item.RuntimeSettings.Returns(runtimeSettings);


      var engines = Substitute.For<DatabaseEngines>(this.Item.Database);
      var templateEngine = Substitute.For<TemplateEngine>(this.Item.Database);
      var template = new Template.Builder(templateId.ToString(), templateId, new TemplateCollection());

      templateEngine.GetTemplate(templateId).Returns(template.Template);

      engines.TemplateEngine.Returns(templateEngine);
      this.Item.Database.Engines.Returns(engines);
      this.Item.Database.GetTemplate(templateId).Returns(this.Item.Template);

      return this;
    }

    public ItemMock WithName(string name)
    {
      this.Item.Name.Returns(name);
      return this;
    }

    public ItemMock WithChild(ItemMock child)
    {
      this.childList.Add(child);

      return this;
    }

    public ItemMock WithParent(ItemMock parent)
    {
      parent.WithChild(this);
      this.Item.Parent.Returns(parent);
      this.Item.ParentID.Returns(parent.ID);
      return this;
    }

    public ItemMock WithField(string name, string value)
    {
      return this.WithField(name, ID.NewID, value);
    }

    public ItemMock WithField(ID id, string value)
    {
      return this.WithField(string.Empty, id, value);
    }

    public ItemMock WithField(string name, ID id, string value)
    {
      var field = Substitute.For<Field>(id, this.Item);
      field.Name.Returns(name);
      field.Value.Returns(value);

      this.Item.Fields[name].Returns(field);
      this.Item.Fields[id].Returns(field);

      this.Item[id].Returns(value);
      this.Item[name].Returns(value);

      var sectionItem = Substitute.For<TemplateSectionItem>(this.Item, this.Item.Template);
      var templateField = Substitute.For<TemplateFieldItem>(this.Item, sectionItem);

      this.Item.Template.GetField(name).Returns(templateField);
      this.Item.Template.GetField(id).Returns(templateField);

      return this;
    }

    public void Add(ItemMock child)
    {
      this.WithChild(child);
    }
  }
}