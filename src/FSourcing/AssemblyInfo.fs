namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("FSourcing")>]
[<assembly: AssemblyProductAttribute("FSourcing")>]
[<assembly: AssemblyDescriptionAttribute("Some general concepts for an event sourced application in fsharp")>]
[<assembly: AssemblyVersionAttribute("1.0")>]
[<assembly: AssemblyFileVersionAttribute("1.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.0"
