#load ".cake/Configuration.cake"

/**********************************************************/
Setup<Configuration>(ctx => Configuration.Create(ctx, c => {
    c.Solution.WebProjectResolvers.Clear();
    //c.Solution.IncludeAsWebProject(p => false);
}));
/**********************************************************/

#load ".cake/CI.cake"

// -- DotNetCore
#load ".cake/Restore-DotNetCore.cake"
#load ".cake/Build-DotNetCore.cake"
#load ".cake/Test-DotNetCore.cake"
#load ".cake/Publish-Pack-DotNetCore.cake"
// -------------

RunTarget(Argument("target", Argument("Target", "Default")));