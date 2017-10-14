// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunIntegrationWizard.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the RunIntegrationWizard command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Web.Shell.Applications.IntegrationWizard.Commands
{
  using System.Collections.Specialized;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Shell.Framework.Commands;
  using Sitecore.Text;
  using Sitecore.Web.UI.Sheer;

  public class RunIntegrationWizard : Command
  {
    public override void Execute([NotNull] CommandContext context)
    {
      Assert.ArgumentNotNull(context, "context");

      Item item = context.Items[0];
      var parameters = new NameValueCollection();
      parameters["pid"] = item.ID.ToString();
      parameters["database"] = item.Database.ToString();
      Context.ClientPage.Start(this, "Run", parameters);
    }


    protected void Run([NotNull] ClientPipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      if (args.IsPostBack)
      {
        if (!string.IsNullOrEmpty(args.Result) && ID.IsID(args.Result))
        {
          Context.ClientPage.SendMessage(this, "item:load(id=" + args.Result + ")");
        }
      }
      else
      {
        string uri = StringUtil.GetString(new[] { args.Parameters["uri"], "control:Sharepoint.IntegrationWizard" });
        var url = new UrlString(UIUtil.GetUri(uri));
        url.Append("pid", args.Parameters["pid"]);
        url.Append("database", args.Parameters["database"]);
        SheerResponse.ShowModalDialog(url.ToString(), true);
        args.WaitForPostBack();
      }
    }
  }
}