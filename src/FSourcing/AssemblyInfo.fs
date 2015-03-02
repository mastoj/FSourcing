namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("FSourcing")>]
[<assembly: AssemblyProductAttribute("FSourcing")>]
[<assembly: AssemblyDescriptionAttribute("Some general concepts for an event sourced application in fsharp")>]
[<assembly: AssemblyVersionAttribute("0.0.2")>]
[<assembly: AssemblyFileVersionAttribute("0.0.2")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "0.0.2"
