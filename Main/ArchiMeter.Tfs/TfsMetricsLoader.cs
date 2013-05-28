namespace ArchiMeter.Tfs
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Globalization;
	using System.Linq;
	using System.Threading.Tasks;
	using Common;
	using Common.Documents;
	using Common.Metrics;

	public class TfsMetricsLoader : IDataLoader
	{
		private readonly IFactory<SqlConnection> _connectionFactory;
		private readonly IFactory<IDataSession<TfsMetricsDocument>> _sessionProvider;

		public TfsMetricsLoader(IFactory<SqlConnection> connectionFactory, IFactory<IDataSession<TfsMetricsDocument>> sessionProvider)
		{
			_connectionFactory = connectionFactory;
			_sessionProvider = sessionProvider;
		}

		public async Task Load(ProjectSettings settings)
		{
			if (settings.TfsDefinitions == null || settings.Date == DateTime.MinValue || settings.TfsDefinitions.All(d => string.IsNullOrWhiteSpace(d.Definition)))
			{
				return;
			}

			try
			{
				var tfsMetrics = GetTfsMetrics(settings).ToArray();
				using (var session = _sessionProvider.Create())
				{
					foreach (var document in tfsMetrics)
					{
						await session.Store(document);
						Console.WriteLine("Stored TFS metrics for " + document.ProjectName);
					}
					await session.Flush();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~TfsMetricsLoader()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
			}
		}

		private IEnumerable<TfsMetricsDocument> GetTfsMetrics(ProjectSettings settings)
		{
			return settings
				.TfsDefinitions
				.Select(d => d.Definition)
				.SelectMany(buildDef => GetBuilds(buildDef, settings.Date))
				.SelectMany(build =>
					{
						var metrics = GetMetrics(build.BuildId).ToArray();
						var assemblyMetrics = metrics.Where(m => !m.ParentId.HasValue);

						return assemblyMetrics
							.SelectMany(m => metrics.Where(x => x.ParentId.HasValue && x.ParentId.Value == m.Id))
							.Select(m => new NamespaceMetric(
											 m.MaintainabilityIndex,
											 m.CyclomaticComplexity,
											 m.LinesOfCode,
											 new TypeCoupling[0],
											 m.DepthOfInheritance.HasValue ? m.DepthOfInheritance.Value : 0,
											 m.Name,
											 metrics.Where(x => x.ParentId.HasValue && x.ParentId.Value == m.Id)
													.Select(x => new TypeMetric(
																	 TypeMetricKind.Unknown,
																	 metrics.Where(y => y.ParentId.HasValue && y.ParentId.Value == x.Id)
																			.Select(y => new MemberMetric(
																							 y.Name,
																							 null,
																							 MemberMetricKind.Unknown,
																							 0,
																							 y.LinesOfCode,
																							 y.MaintainabilityIndex,
																							 y.CyclomaticComplexity,
																							 y.Name,
																							 0,
																							 new TypeCoupling[0],
																							 0,
																							 0))
																			.ToArray(),
																	 x.LinesOfCode,
																	 x.CyclomaticComplexity,
																	 x.MaintainabilityIndex,
																	 x.DepthOfInheritance.HasValue ? x.DepthOfInheritance.Value : 0,
																	 new TypeCoupling[0],
																	 x.Name))
													.ToArray()))
							.Select(m => new
											 {
												 BuildId = build.BuildId,
												 Project = settings.Name,
												 Revision = settings.Revision,
												 Time = settings.Date,
												 Metrics = m
											 });

					})
				.GroupBy(a => a.Project)
				.Select(g =>
					{
						var first = g.First();
						return new TfsMetricsDocument
								   {
									   BuildNumber = first.BuildId.ToString(CultureInfo.InvariantCulture),
									   Id = TfsMetricsDocument.GetId(first.Project, first.Revision, first.BuildId.ToString(CultureInfo.InvariantCulture)),
									   MetricsDate = first.Time,
									   Metrics = g.Select(x => x.Metrics).ToArray(),
									   ProjectName = first.Project,
									   ProjectVersion = first.Revision
								   };
					});
		}

		private IEnumerable<TfsMetric> GetMetrics(int buildNumber)
		{
			const string Query = @"SELECT * FROM [TFS_Metrics].[dbo].[Metric] WHERE BuildID = @BuildNo";

			using (var databaseConnection = _connectionFactory.Create())
			{
				using (var cmd = new SqlCommand(Query, databaseConnection))
				{
					cmd.Parameters.AddWithValue("@BuildNo", buildNumber);

					databaseConnection.Open();
					using (var reader = cmd.ExecuteReader())
					{
						var metrics = new List<TfsMetric>();
						while (reader.Read())
						{
							try
							{
								var doi = reader[8];
								var parent = reader[2];
								metrics.Add(
									new TfsMetric
										{
											Id = reader.GetInt32(0),
											BuildNumber = reader.GetInt32(1),
											ParentId = parent == DBNull.Value ? (int?)null : (int)parent,
											MetricType = reader.GetInt32(3),
											Name = reader.GetString(4),
											MaintainabilityIndex = reader.GetInt32(5),
											CyclomaticComplexity = reader.GetInt32(6),
											ClassCoupling = reader.GetInt32(7),
											DepthOfInheritance = doi == DBNull.Value ? (int?)null : (int)doi,
											LinesOfCode = reader.GetInt32(9)
										});
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
								Console.WriteLine(ex.StackTrace);
								return null;
							}
						}

						return metrics;
					}
				}
			}
		}

		private IEnumerable<TfsBuild> GetBuilds(string teamProject, DateTime revisionTime)
		{
			const string Query = @"SELECT TOP 1 max([ID]) as ID
      ,BuildName
      ,TeamProject
      ,min(DATEDIFF(SECOND, BuildTime, @DateParam)) as ClosestBuild
  FROM [TFS_Metrics].[dbo].[Build]
  WHERE BuildName LIKE @Project + '%' AND DATEDIFF(SECOND, BuildTime, @DateParam) >= 0
  GROUP BY BuildName, TeamProject
  ORDER BY ClosestBuild";
			using (var databaseConnection = _connectionFactory.Create())
			{
				using (var cmd = new SqlCommand(Query, databaseConnection))
				{
					cmd.Parameters.AddWithValue("@DateParam", revisionTime);
					cmd.Parameters.AddWithValue("@Project", teamProject);

					databaseConnection.Open();
					using (var reader = cmd.ExecuteReader())
					{
						var builds = new List<TfsBuild>();
						while (reader.Read())
						{
							builds.Add(
								new TfsBuild
									{
										BuildId = reader.GetInt32(0),
										BuildName = reader.GetString(1),
										TeamProject = reader.GetString(2),
										BuildTime = DateTime.UtcNow.AddSeconds(-reader.GetInt32(3))
									});
						}

						return builds;
					}
				}
			}
		}

		private class TfsBuild
		{
			public int BuildId { get; set; }

			public string BuildName { get; set; }

			public string TeamProject { get; set; }

			public DateTime BuildTime { get; set; }
		}

		private class TfsMetric
		{
			public int? ParentId { get; set; }

			public int Id { get; set; }

			public int BuildNumber { get; set; }

			public string Name { get; set; }

			public int MetricType { get; set; }

			public int LinesOfCode { get; set; }

			public int MaintainabilityIndex { get; set; }

			public int CyclomaticComplexity { get; set; }

			public int ClassCoupling { get; set; }

			public int? DepthOfInheritance { get; set; }
		}
	}
}
