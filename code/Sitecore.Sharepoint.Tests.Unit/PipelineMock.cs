namespace Sitecore.Sharepoint.Tests.Unit
{
  using System;
  using System.Threading;

  using NSubstitute;

  using Sitecore.Abstractions;
  using Sitecore.Pipelines;
  using Sitecore.Sharepoint.Data.Providers.Pipelines.Runners;

  public class PipelinesHandler : IDisposable
  {
    private static readonly object SyncObject = new object();
    private readonly BaseCorePipelineManager pipelineManager;

    public PipelinesHandler()
    {
      Monitor.Enter(SyncObject);
      this.pipelineManager = Substitute.For<BaseCorePipelineManager>();
      PipelineRunner.PipelineManager = this.pipelineManager;
    }

    public void Dispose()
    {
      PipelineRunner.PipelineManager = null;
      Monitor.Exit(SyncObject);
    }

    public PipelinesHandler ShouldReceiveCall<T>(string pipelineName, Predicate<T> argsPredicate) where T : PipelineArgs
    {
      this.pipelineManager.Received().Run(pipelineName, Arg.Is<T>(x => argsPredicate(x)), true);
      return this;
    }

    public PipelinesHandler ThrowInPipeline(string pipelineName, Exception exc = null)
    {
      this.pipelineManager.When(x => x.Run(pipelineName, Arg.Any<PipelineArgs>(), true))
        .Do(x => { throw exc ?? new Exception(); });
      return this;
    }
  }
}