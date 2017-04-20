#tool "xunit.runner.console"
#tool "GitVersion.CommandLine"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target                  = Argument("target", "Default");
var configuration           = Argument("configuration", "Release");
var solutionPath            = MakeAbsolute(File(Argument("solutionPath", "./Nest.Searchify.Mvc.sln")));
var nugetProjects           = Argument("nugetProjects", "Nest.Searchify.Mvc");


//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var testAssemblies          = "./tests/**/bin/" +configuration +"/*.Tests.dll";

var artifacts               = MakeAbsolute(Directory(Argument("artifactPath", "./artifacts")));
var buildOutput             = MakeAbsolute(Directory(artifacts +"/build/"));
var testResultsPath         = MakeAbsolute(Directory(artifacts + "./test-results"));
var versionAssemblyInfo     = MakeAbsolute(File(Argument("versionAssemblyInfo", "VersionAssemblyInfo.cs")));

IEnumerable<FilePath> nugetProjectPaths     = null;
SolutionParserResult solution               = null;
GitVersion versionInfo                      = null;

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Setup(() => {
    if(!FileExists(solutionPath)) throw new Exception(string.Format("Solution file not found - {0}", solutionPath.ToString()));
    solution = ParseSolution(solutionPath.ToString());

    var projects = solution.Projects.Where(x => nugetProjects.Contains(x.Name));
    if(projects == null || !projects.Any()) throw new Exception(string.Format("Unable to find projects '{0}' in solution '{1}'", nugetProjects, solutionPath.GetFilenameWithoutExtension()));
    nugetProjectPaths = projects.Select(p => p.Path);
    
    // if(!FileExists(nugetProjectPath)) throw new Exception("project path not found");
    Information("[Setup] Using Solution '{0}'", solutionPath.ToString());
});

Task("Clean")
    .Does(() =>
{
    CleanDirectories(artifacts.ToString());
    CreateDirectory(artifacts);
    CreateDirectory(buildOutput);
    
    var binDirs = GetDirectories(solutionPath.GetDirectory() +@"\src\**\bin");
    var objDirs = GetDirectories(solutionPath.GetDirectory() +@"\src\**\obj");
    CleanDirectories(binDirs);
    CleanDirectories(objDirs);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore(solutionPath, new NuGetRestoreSettings());
});

Task("Update-Version-Info")
    .IsDependentOn("CreateVersionAssemblyInfo")
    .Does(() => 
{
        versionInfo = GitVersion(new GitVersionSettings {
            UpdateAssemblyInfo = true,
            UpdateAssemblyInfoFilePath = versionAssemblyInfo
        });

    if(versionInfo != null) {
        Information("Version: {0}", versionInfo.FullSemVer);
    } else {
        throw new Exception("Unable to determine version");
    }
});

Task("CreateVersionAssemblyInfo")
    .WithCriteria(() => !FileExists(versionAssemblyInfo))
    .Does(() =>
{
    Information("Creating version assembly info");
    CreateAssemblyInfo(versionAssemblyInfo, new AssemblyInfoSettings {
        Version = "0.0.0.0",
        FileVersion = "0.0.0.0",
        InformationalVersion = "",
    });
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Update-Version-Info")
    .Does(() =>
{
    MSBuild(solutionPath, settings => settings
        .WithProperty("TreatWarningsAsErrors","true")
        .WithProperty("UseSharedCompilation", "false")
        .WithProperty("AutoParameterizationWebConfigConnectionStrings", "false")
        .SetVerbosity(Verbosity.Quiet)
        .SetConfiguration(configuration)
        .WithTarget("Rebuild")
    );
});

Task("Copy-Files")
    .IsDependentOn("Build")
    .Does(() => 
{
    EnsureDirectoryExists(buildOutput);
    EnsureDirectoryExists(buildOutput +"/content");
    CopyFile("./src/Nest.Searchify.Mvc/bin/" +configuration +"/Nest.Searchify.Mvc.dll", buildOutput +"/Nest.Searchify.Mvc.dll");
    CopyFile("./src/Nest.Searchify.Mvc/bin/" +configuration +"/Nest.Searchify.Mvc.pdb", buildOutput +"/Nest.Searchify.Mvc.pdb");
    CopyDirectory("./src/Nest.Searchify.Mvc/Templates", buildOutput +"/content");
});


Task("Build-NuGet-Package")
    .IsDependentOn("Build")
    .IsDependentOn("Copy-Files")
    .Does(() => 
{
        var settings = new NuGetPackSettings {
            BasePath = buildOutput,
            Id = "Nest.Searchify.Mvc",
            Authors = new [] { "Phil Oyston", "Storm ID" },
            Owners = new [] {"philo", "StormId" },
            Description = "Provides Mvc specific helpers for the Nest.Searchify library",
            LicenseUrl = new Uri("https://raw.githubusercontent.com/stormid/nest-searchify-mvc/master/LICENSE"),
            ProjectUrl = new Uri("https://github.com/stormid/nest-searchify-mvc"),
            IconUrl = new Uri("http://stormid.com/_/images/icons/apple-touch-icon.png"),
            RequireLicenseAcceptance = false,
            Properties = new Dictionary<string, string> { { "Configuration", configuration }},
            Symbols = false,
            NoPackageAnalysis = true,
            Version = versionInfo.NuGetVersionV2,
            OutputDirectory = artifacts,
            Tags = new[] { "Elasticsearch", "Nest", "Storm" },
            Files = new[] {
                new NuSpecContent { Source = "Nest.Searchify.Mvc.dll", Target = "lib/net45" },
                new NuSpecContent { Source = "Nest.Searchify.Mvc.pdb", Target = "lib/net45" },

                new NuSpecContent { Source = "content/**/*", Target = "" }
            },
            Dependencies = new [] {
                new NuSpecDependency { Id = "Nest.Queryify", Version = "0.6.1" },
                new NuSpecDependency { Id = "Nest.Searchify", Version = "0.9.3" },
                new NuSpecDependency { Id = "Nest", Version = "[1.6,6]" },
            }
        };
        NuGetPack("./src/Nest.Searchify.Mvc/Nest.Searchify.Mvc.nuspec", settings);                     
});

Task("Package")
    .IsDependentOn("Build")
    .IsDependentOn("Build-NuGet-Package")
    .Does(() => { });

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    CreateDirectory(testResultsPath);

    var settings = new XUnit2Settings {
        XmlReportV1 = true,
        NoAppDomain = true,
        OutputDirectory = testResultsPath,
    };
    
    XUnit2(testAssemblies, settings);
});

Task("Update-AppVeyor-Build-Number")
    .IsDependentOn("Update-Version-Info")
    .WithCriteria(() => AppVeyor.IsRunningOnAppVeyor)
    .Does(() =>
{
    AppVeyor.UpdateBuildVersion(versionInfo.FullSemVer +"|" +AppVeyor.Environment.Build.Number);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Update-Version-Info")
    .IsDependentOn("Update-AppVeyor-Build-Number")
    .IsDependentOn("Build")
    // .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Package")
    ;

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
