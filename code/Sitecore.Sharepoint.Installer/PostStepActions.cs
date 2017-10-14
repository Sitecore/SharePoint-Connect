// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostStepActions.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the PostStepActions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Installer
{
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.IO;
  using System.Linq;
  using System.Threading;
  using Sitecore.Configuration;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Diagnostics;
  using Sitecore.Globalization;
  using Sitecore.Install.Framework;
  using Sitecore.IO;
  using Sitecore.Jobs;
  using Sitecore.Shell.Applications.Globalization.ImportLanguage;

  public class PostStepActions : IPostStep
  {
    public virtual void Run(ITaskOutput output, NameValueCollection metaData)
    {
      this.AddBaseTemplate();

      string translationFolder = FileUtil.MapPath("temp");
      translationFolder = StringUtil.Combine(translationFolder, "SPIF translations", "\\");
      Log.Debug("The following folder is to be used to search translations: " + translationFolder);
      foreach (string file in Directory.GetFiles(translationFolder))
      {
        this.ImportTranslation(file);
      }
    }

    protected void ImportTranslation(string translationFile)
    {
      List<string> selectedLanguageNames = this.GetClientLanguages();
      JobOptions options = new JobOptions("ImportLanguage", "ImportLanguage", "shell", new ImportLanguageForm.Importer("core", translationFile, selectedLanguageNames), "Import")
      {
        ContextUser = Context.User
      };
      Job job = JobManager.Start(options);
      while (!job.IsDone)
      {
        Thread.Sleep(500);
      }
    }

    private void AddBaseTemplate()
    {
      var unversionedFile = ID.Parse("{962B53C4-F93B-4DF9-9821-415C867B8903}");
      var sharepointIntegrationFile = ID.Parse("{591BAF39-C680-4564-A444-134FB7894373}");

      var fileTemplateItem = Database.GetDatabase("master").GetItem(unversionedFile);

      var baseTemplates = ID.ParseArray(fileTemplateItem.Fields[FieldIDs.BaseTemplate].Value).ToList();

      if (!baseTemplates.Contains(sharepointIntegrationFile))
      {
        baseTemplates.Add(sharepointIntegrationFile);
        using (new EditContext(fileTemplateItem))
        {
          fileTemplateItem[FieldIDs.BaseTemplate] = ID.ArrayToString(baseTemplates.ToArray());
        }
      }
    }

    private List<string> GetClientLanguages()
    {
      List<string> languages = new List<string>();
      Database coreDatabase = Factory.GetDatabase("core");
      Assert.IsNotNull(coreDatabase, "Can't find core database");
      foreach (Language lang in LanguageManager.GetLanguages(coreDatabase))
      {
        languages.Add(lang.Name);
      }

      return languages;
    }
  }
}
