// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   AssemblyInfo.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("ArchiMetrics.Analysis")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Reimers.dk")]
[assembly: AssemblyProduct("ArchiMetrics.Analysis")]
[assembly: AssemblyCopyright("Copyright © Reimers.dk 2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("6cf79dfb-9fc0-4e15-96eb-d92d74b6e6a0")]

// Version information for an assembly consists of the following four values:
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyInformationalVersion("0.12.0.0-beta")]
[assembly: AssemblyVersion("0.12.0.0")]
[assembly: AssemblyFileVersion("0.12.0.0")]
[assembly: InternalsVisibleTo("ArchiMetrics.Analysis.Tests")]
[assembly: InternalsVisibleTo("ArchiMetrics.CodeReview.Rules")]
[assembly: InternalsVisibleTo("ArchiMetrics.Raven")]
[assembly: InternalsVisibleTo("ArchiMetrics.Raven.Tests")]
[assembly: InternalsVisibleTo("ArchiMetrics.Tfs")]
[assembly: InternalsVisibleTo("ArchiMetrics.DataLoader")]
[assembly: InternalsVisibleTo("ArchiMetrics.DataLoader.Tests")]
[assembly: InternalsVisibleTo("ArchiMetrics.DataLoader.Console")]
[assembly: InternalsVisibleTo("ArchiMetrics.ExcelWriter")]
[assembly: InternalsVisibleTo("ArchiMetrics.Service.IntegrationTests")]
[assembly: InternalsVisibleTo("ArchiMetrics")]
[assembly: InternalsVisibleTo("ScriptCs.Metrics")]
