// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobUtil.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the JobUtil class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Common.Utils
{
  using Sitecore.Diagnostics;
  using Sitecore.Jobs;
  using Sitecore.Security.Accounts;

  public class JobUtil
  {
    private static readonly object LockObj = new object();

    public static void StartJob(
                                [NotNull] string jobName, 
                                [NotNull] object obj, 
                                [NotNull] string methodName, 
                                [CanBeNull] object[] parameters)
    {
      Assert.ArgumentNotNullOrEmpty(jobName, "jobName");
      Assert.ArgumentNotNull(obj, "obj");
      Assert.ArgumentNotNullOrEmpty(methodName, "methodName");

      var siteName = Context.Site != null ? Context.Site.Name : "scheduler";
      StartJob(jobName, "SharePoint Integration", siteName, obj, methodName, parameters, Context.User);
    }

    public static void StartJob(
                                [NotNull] string jobName, 
                                [NotNull] string category, 
                                [NotNull] string siteName, 
                                [NotNull] object obj, 
                                [NotNull] string methodName,
                                [CanBeNull] object[] parameters, 
                                [CanBeNull] User contextUser)
    {
      Assert.ArgumentNotNullOrEmpty(jobName, "jobName");
      Assert.ArgumentNotNullOrEmpty(category, "category");
      Assert.ArgumentNotNullOrEmpty(siteName, "siteName");
      Assert.ArgumentNotNull(obj, "obj");
      Assert.ArgumentNotNullOrEmpty(methodName, "methodName");

      var options = new JobOptions(jobName, category, siteName, obj, methodName, parameters)
      {
        ContextUser = contextUser
      };

      lock (LockObj)
      {
        if (JobManager.IsJobRunning(jobName) || JobManager.IsJobQueued(jobName))
        {
          return;
        }

        JobManager.Start(options);
      }
    }
  }
}