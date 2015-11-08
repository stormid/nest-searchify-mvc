using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Nest.Searchify.Mvc")]
[assembly: AssemblyDescription("Provides Mvc specific helpers for the Nest.Searchify library")]
[assembly: AssemblyCompany("Storm ID Ltd")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
    [assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyProduct("Nest.Searchify.Mvc")]

[assembly: ComVisible(false)]
