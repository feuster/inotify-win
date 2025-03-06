using Cake.Common;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Publish;
using Cake.Core;
using Cake.Frosting;
using System.IO;

///=========================================================================================================
/// This Is the Cake build code. Do not modify after this line!
///=========================================================================================================

///---------------------------------------------------------------------------------------------------------

//Disable package related warnings since the cake builder will be disposed after build and never be released
#pragma warning disable NU1903, NU1904

/// <summary>
/// Init cake builder
/// </summary>
public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .UseContext<BuildContext>()
            .Run(args);
    }
}

/// <summary>
/// Cake builder configuration
/// </summary>
public class BuildContext : FrostingContext
{
    public string DotNetPublishConfiguration { get; set; }
    public string Framework { get; set; }
    public string Runtime { get; set; }
    public string PublishDir { get; set; }
    public string PublishExeName { get; set; }
    public string PublishVersion { get; set; }
    public BuildContext(ICakeContext context)
        : base(context)
    {
        DotNetPublishConfiguration = context.Argument("configuration", "Release");
        Framework = context.Argument("framework", "net9.0");
        Runtime = context.Argument("runtime", "win-x64");
        PublishDir = context.Argument("publishdir", "../publish");
        PublishExeName = context.Argument("publishexename", "InotifyWait");
        PublishVersion = context.Argument("publishversion", "1.10.0.0"); //default version from original port
    }
}

/// <summary>
/// Clean release folder from previous builds
/// </summary>
[TaskName("Clean")]
public sealed class CleanTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.CleanDirectory(context.PublishDir);
    }
}

/// <summary>
/// Cake builder dotnet publish
/// </summary>
[TaskName("Publish")]
[IsDependentOn(typeof(CleanTask))]
public sealed class PublishTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetPublish($"../InotifyWait.sln", new DotNetPublishSettings
        {
            Configuration = context.DotNetPublishConfiguration,
            WorkingDirectory = $"./",
            Framework = context.Framework,
            Runtime = context.Runtime,
            SelfContained = true,
            PublishSingleFile = true,
            PublishTrimmed = true,
            PublishReadyToRun = true,
            EnableCompressionInSingleFile = true,
            IncludeNativeLibrariesForSelfExtract = true,
            IncludeAllContentForSelfExtract = true,
            ArgumentCustomization = args => args.Append("/p:PublishDir=\"" + context.PublishDir + "\"")
                                                .Append("/p:DebugType=none")
                                                .Append("/p:DebugSymbols=false")
                                                .Append("/p:PublishAoT=true")
                                                .Append("/p:TrimMode=full")
                                                .Append("/p:Platform=\"Any CPU\"")
                                                .Append("/p:AssemblyName=\"" + context.PublishExeName + "\"")
        });
    }
}

/// <summary>
/// Create a release Zip archive with license and readme
/// </summary>
[TaskName("Pack")]
[IsDependentOn(typeof(PublishTask))]
public sealed class PackTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        if (File.Exists("../LICENSE.md")) File.Copy("../LICENSE.md", context.PublishDir + "/LICENSE.txt");
        if (File.Exists("../README.md")) File.Copy("../README.md", context.PublishDir + "/README.md");
        if (File.Exists(context.PublishDir + "/" + context.PublishExeName + ".pdb")) File.Delete(context.PublishDir + "/" + context.PublishExeName + ".pdb");
        if (File.Exists(context.PublishDir + "/" + context.PublishExeName + ".exe")) File.Move(context.PublishDir + "/" + context.PublishExeName + ".exe", context.PublishDir + "/" + context.PublishExeName + "_" + context.PublishVersion + ".exe");
        context.Zip(context.PublishDir + "/", context.PublishDir + "/" + context.PublishExeName + "_" + context.PublishVersion + ".zip");
    }
}

/// <summary>
/// Default cake builder task which starts the building process
/// </summary>
[TaskName("Default")]
[IsDependentOn(typeof(PackTask))]
public class DefaultTask : FrostingTask
{
}