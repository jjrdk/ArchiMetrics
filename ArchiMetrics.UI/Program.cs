// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Concurrent;

namespace ArchiMetrics.UI
{
	using System;
	using System.Globalization;
	using System.Reflection;

	public sealed class Program
	{
		private static readonly ConcurrentDictionary<string, Assembly> KnownAssemblies = new ConcurrentDictionary<string, Assembly>();
		private static Assembly _executingAssembly;

		private Program()
		{
		}

		[STAThread]
		public static void Main()
		{
			AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
			var app = new App();
			app.InitializeComponent();
			app.Run();
		}

		private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
		{
			return GetAssembly(args.Name);
		}

		private static Assembly GetAssembly(string x)
		{
			return KnownAssemblies.GetOrAdd(
				x,
				name =>
				{
					var assemblyName = new AssemblyName(name);

					var executingAssembly = GetExecutingAssembly();
					var path = assemblyName.Name + ".dll";
					if (assemblyName.CultureInfo != null && assemblyName.CultureInfo.Equals(CultureInfo.InvariantCulture) == false)
					{
						path = string.Format(@"{0}\{1}", assemblyName.CultureInfo, path);
					}

					using (var stream = executingAssembly.GetManifestResourceStream(path))
					{
						if (stream == null)
						{
							return null;
						}

						var assemblyRawBytes = new byte[stream.Length];
						stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
						return Assembly.Load(assemblyRawBytes);
					}
				});
		}

		private static Assembly GetExecutingAssembly()
		{
			return _executingAssembly ?? (_executingAssembly = Assembly.GetExecutingAssembly());
		}
	}
}