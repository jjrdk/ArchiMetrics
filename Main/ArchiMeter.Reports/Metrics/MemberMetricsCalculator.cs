namespace ArchiMeter.Reports.Metrics
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Core.Metrics;
	using Roslyn.Compilers.Common;

	internal sealed class MemberMetricsCalculator : SemanticModelMetricsCalculator
	{
		// Methods
		public MemberMetricsCalculator(ISemanticModel semanticModel)
			: base(semanticModel)
		{
		}

		public IEnumerable<MemberMetric> Calculate(TypeDeclarationSyntaxInfo typeNode)
		{
			IEnumerable<MemberNode> members = new MemberCollectorSyntaxWalker(Root).GetMembers(Model, typeNode);
			return this.CalculateMemberMetrics(members).ToArray();
		}

		private IEnumerable<string> CalculateClassCoupling(MemberNode node)
		{
			var provider = new MemberClassCouplingAnalyzer(Model);
			return provider.Calculate(node);
		}

		private static int CalculateCyclomaticComplexity(MemberNode node)
		{
			var provider = new CyclomaticComplexityAnalyzer();
			return provider.Calculate(node);
		}

		private static int CalculateLinesOfCode(MemberNode node)
		{
			var provider = new StatementsAnalyzer();
			return provider.Calculate(node);
		}

		private static int CalculateLogicalComplexity(MemberNode node)
		{
			var provider = new LogicalComplexityAnalyzer();
			return provider.Calculate(node);
		}

		private static double CalculateMaintainablityIndex(double cyclomaticComplexity, double linesOfCode, IHalsteadMetrics halsteadMetrics)
		{
			if (linesOfCode.Equals(0.0))
			{
				return 100.0;
			}
			if (halsteadMetrics.NumOperands.Equals(0) || halsteadMetrics.NumOperators.Equals(0))
			{
				return 0.0;
			}

			double? volume = halsteadMetrics.GetVolume();
			double num = 1.0;
			if (volume.HasValue)
			{
				num = Math.Log(volume.Value);
			}
			double num2 = ((((171.0 - (5.2 * num)) - (0.23 * cyclomaticComplexity)) - (16.2 * Math.Log(linesOfCode))) * 100.0) / 171.0;
			return Math.Max(0.0, num2);
		}

		private IEnumerable<MemberMetric> CalculateMemberMetrics(IEnumerable<MemberNode> nodes)
		{
			foreach (MemberNode node in nodes)
			{
				var syntaxNode = node.SyntaxNode;
				var memberMetricKind = GetMemberMetricKind(node);
				var analyzer = new HalsteadAnalyzer();
				var halsteadMetrics = analyzer.Calculate(node);
				if (halsteadMetrics != null)
				{
					var source = CalculateClassCoupling(node);
					var complexity = CalculateCyclomaticComplexity(node);
					var logicalComplexity = CalculateLogicalComplexity(node);
					var linesOfCode = CalculateLinesOfCode(node);
					var numberOfParameters = this.CalculateNumberOfParameters(syntaxNode);
					var numberOfLocalVariables = this.CalculateNumberOfLocalVariables(syntaxNode);
					double maintainabilityIndex = CalculateMaintainablityIndex(complexity, linesOfCode, halsteadMetrics);
					MemberMetric iteratorVariable12 = new MemberMetric(
						node.CodeFile,
						halsteadMetrics,
						memberMetricKind,
						node.LineNumber,
						linesOfCode,
						maintainabilityIndex,
						complexity,
						node.DisplayName,
						logicalComplexity,
						source.ToArray(),
						numberOfParameters,
						numberOfLocalVariables);
					yield return iteratorVariable12;
				}
			}
		}

		private int CalculateNumberOfLocalVariables(CommonSyntaxNode node)
		{
			MethodLocalVariablesAnalyzer analyzer = new MethodLocalVariablesAnalyzer();
			return analyzer.Calculate(node);
		}

		private int CalculateNumberOfParameters(CommonSyntaxNode node)
		{
			MethodParameterAnalyzer analyzer = new MethodParameterAnalyzer();
			return analyzer.Calculate(node);
		}

		private static MemberMetricKind GetMemberMetricKind(MemberNode memberNode)
		{
			switch (memberNode.Kind)
			{
				case MemberKind.Method:
				case MemberKind.Constructor:
				case MemberKind.Destructor:
					return MemberMetricKind.Method;

				case MemberKind.GetProperty:
				case MemberKind.SetProperty:
					return MemberMetricKind.PropertyAccessor;

				case MemberKind.AddEventHandler:
				case MemberKind.RemoveEventHandler:
					return MemberMetricKind.EventAccessor;
			}
			return MemberMetricKind.Unknown;
		}

		//	// Nested Types
		//	[CompilerGenerated]
		//	private sealed class <CalculateIEnumerable<MemberMetric>>d__c : IEnumerable<MemberMetric>, IEnumerable, IEnumerator<MemberMetric>, IEnumerator, IDisposable
		//	{
		//		// Fields
		//	private int <>1__state;
		//	private MemberMetric <>2__current;
		//	public IEnumerable<MemberNode> <>3__nodes;
		//	public MemberMetricsCalculator <>4__this;
		//	public IEnumerator<MemberNode> <>7__wrap19;
		//	public MemberMetric <>g__initLocal3;
		//	public List<MetricResult> <>g__initLocal4;
		//	public MetricResult <>g__initLocal5;
		//	public MetricResult <>g__initLocal6;
		//	public MetricResult <>g__initLocal7;
		//	public ClassCouplingMetricResult <>g__initLocal8;
		//	public MetricResult <>g__initLocal9;
		//	public MetricResult <>g__initLocala;
		//	public MetricResult <>g__initLocalb;
		//	private int <>l__initialThreadId;
		//	public IEnumerable<string> <coupledClasses>5__12;
		//	public int <cyclomaticComplexity>5__13;
		//	public IHalsteadMetricsProvider <halstead>5__10;
		//	public IHalsteadMetrics <halsteadMetrics>5__11;
		//	public int <linesOfCode>5__15;
		//	public int <logicalComplexity>5__14;
		//	public double <maintainabilityIndex>5__18;
		//	public MemberNode <memberNode>5__d;
		//	public MemberMetricKind <metricKind>5__f;
		//	public CommonSyntaxNode <node>5__e;
		//	public int <numLocalVariables>5__17;
		//	public int <numParameters>5__16;
		//	public IEnumerable<MemberNode> nodes;

		//		// Methods
		//	[DebuggerHidden]
		//public <CalculateIEnumerable<MemberMetric>>d__c(int <>1__state)
		//		{
		//			this.<>1__state = <>1__state;
		//			this.<>l__initialThreadId = Environment.CurrentManagedThreadId;
		//		}

		//	private void <>m__Finally1a()
		//		{
		//			this.<>1__state = -1;
		//			if (this.<>7__wrap19 != null)
		//			{
		//				this.<>7__wrap19.Dispose();
		//			}
		//		}

		//	private bool MoveNext()
		//		{
		//			bool flag;
		//			try
		//			{
		//				switch (this.<>1__state)
		//				{
		//					case 0:
		//					this.<>1__state = -1;
		//					this.<>7__wrap19 = this.nodes.GetEnumerator();
		//					this.<>1__state = 1;
		//					goto Label_03C3;

		//					case 2:
		//					this.<>1__state = 1;
		//					goto Label_03C3;

		//					default:
		//					goto Label_03D9;
		//				}
		//				Label_0042:
		//				this.<memberNode>5__d = this.<>7__wrap19.Current;
		//				this.<node>5__e = this.<memberNode>5__d.SyntaxNode;
		//				this.<metricKind>5__f = MemberMetricsCalculator.GetMemberMetricKind(this.<memberNode>5__d);
		//				this.<halstead>5__10 = new HalsteadAnalyzer();
		//				this.<halsteadMetrics>5__11 = this.<halstead>5__10.Calculate(this.<memberNode>5__d);
		//				if (this.<halsteadMetrics>5__11 != null)
		//				{
		//					this.<coupledClasses>5__12 = this.<>4__this.CalculateClassCoupling(this.<memberNode>5__d);
		//					this.<cyclomaticComplexity>5__13 = MemberMetricsCalculator.CalculateCyclomaticComplexity(this.<memberNode>5__d);
		//					this.<logicalComplexity>5__14 = MemberMetricsCalculator.CalculateLogicalComplexity(this.<memberNode>5__d);
		//					this.<linesOfCode>5__15 = MemberMetricsCalculator.CalculateLinesOfCode(this.<memberNode>5__d);
		//					this.<numParameters>5__16 = this.<>4__this.CalculateNumberOfParameters(this.<node>5__e);
		//					this.<numLocalVariables>5__17 = this.<>4__this.CalculateNumberOfLocalVariables(this.<node>5__e);
		//					this.<maintainabilityIndex>5__18 = MemberMetricsCalculator.CalculateMaintainablityIndex((double) this.<cyclomaticComplexity>5__13, (double) this.<linesOfCode>5__15, this.<halsteadMetrics>5__11);
		//					this.<>g__initLocal3 = new MemberMetric();
		//					this.<>g__initLocal3.Name = this.<memberNode>5__d.DisplayName;
		//					this.<>g__initLocal3.CodeFile = this.<memberNode>5__d.CodeFile;
		//					this.<>g__initLocal3.LineNumber = this.<memberNode>5__d.LineNumber;
		//					this.<>g__initLocal3.Halstead = this.<halsteadMetrics>5__11;
		//					this.<>g__initLocal3.Kind = this.<metricKind>5__f;
		//					this.<>g__initLocal4 = new List<MetricResult>();
		//					this.<>g__initLocal5 = new MetricResult();
		//					this.<>g__initLocal5.Name = "MaintainabilityIndex";
		//					this.<>g__initLocal5.Value = this.<maintainabilityIndex>5__18;
		//					this.<>g__initLocal4.Add(this.<>g__initLocal5);
		//					this.<>g__initLocal6 = new MetricResult();
		//					this.<>g__initLocal6.Name = "CyclomaticComplexity";
		//					this.<>g__initLocal6.Value = this.<cyclomaticComplexity>5__13;
		//					this.<>g__initLocal4.Add(this.<>g__initLocal6);
		//					this.<>g__initLocal7 = new MetricResult();
		//					this.<>g__initLocal7.Name = "LogicalComplexity";
		//					this.<>g__initLocal7.Value = this.<logicalComplexity>5__14;
		//					this.<>g__initLocal4.Add(this.<>g__initLocal7);
		//					this.<>g__initLocal8 = new ClassCouplingMetricResult();
		//					this.<>g__initLocal8.Name = "ClassCoupling";
		//					this.<>g__initLocal8.Value = this.<coupledClasses>5__12.Count<string>();
		//					this.<>g__initLocal8.Types = this.<coupledClasses>5__12;
		//					this.<>g__initLocal4.Add(this.<>g__initLocal8);
		//					this.<>g__initLocal9 = new MetricResult();
		//					this.<>g__initLocal9.Name = "LinesOfCode";
		//					this.<>g__initLocal9.Value = this.<linesOfCode>5__15;
		//					this.<>g__initLocal4.Add(this.<>g__initLocal9);
		//					this.<>g__initLocala = new MetricResult();
		//					this.<>g__initLocala.Name = "NumberOfParameters";
		//					this.<>g__initLocala.Value = this.<numParameters>5__16;
		//					this.<>g__initLocal4.Add(this.<>g__initLocala);
		//					this.<>g__initLocalb = new MetricResult();
		//					this.<>g__initLocalb.Name = "NumberOfLocalVariables";
		//					this.<>g__initLocalb.Value = this.<numLocalVariables>5__17;
		//					this.<>g__initLocal4.Add(this.<>g__initLocalb);
		//					this.<>g__initLocal3.Metrics = this.<>g__initLocal4;
		//					this.<>2__current = this.<>g__initLocal3;
		//					this.<>1__state = 2;
		//					return true;
		//				}
		//				Label_03C3:
		//				if (this.<>7__wrap19.MoveNext())
		//				{
		//					goto Label_0042;
		//				}
		//				this.<>m__Finally1a();
		//				Label_03D9:
		//				flag = false;
		//			}
		//			fault
		//			{
		//				this.System.IDisposable.Dispose();
		//			}
		//			return flag;
		//		}

		//	[DebuggerHidden]
		//		IEnumerator<MemberMetric> IEnumerable<MemberMetric>.GetEnumerator()
		//		{
		//			MemberMetricsCalculator.<CalculateIEnumerable<MemberMetric>>d__c _c;
		//			if ((Environment.CurrentManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
		//			{
		//				this.<>1__state = 0;
		//				_c = this;
		//			}
		//		else
		//			{
		//				_c = new MemberMetricsCalculator.<CalculateIEnumerable<MemberMetric>>d__c(0) {
		//				<>4__this = this.<>4__this
		//				};
		//			}
		//			_c.nodes = this.<>3__nodes;
		//			return _c;
		//		}

		//	[DebuggerHidden]
		//		IEnumerator IEnumerable.GetEnumerator()
		//		{
		//			return this.System.Collections.Generic.IEnumerable<RoslynMetrics.Contracts.MemberMetric>.GetEnumerator();
		//		}

		//	[DebuggerHidden]
		//		void IEnumerator.Reset()
		//		{
		//			throw new NotSupportedException();
		//		}

		//		void IDisposable.Dispose()
		//		{
		//			switch (this.<>1__state)
		//			{
		//				case 1:
		//				case 2:
		//				try
		//				{
		//				}
		//				finally
		//				{
		//					this.<>m__Finally1a();
		//				}
		//				return;
		//			}
		//		}

		//		// Properties
		//		MemberMetric IEnumerator<MemberMetric>.Current
		//		{
		//		[DebuggerHidden]
		//			get
		//			{
		//				return this.<>2__current;
		//			}
		//		}

		//		object IEnumerator.Current
		//		{
		//		[DebuggerHidden]
		//			get
		//			{
		//				return this.<>2__current;
		//			}
		//		}
		//	}
	}
}