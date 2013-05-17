namespace ArchiMeter.Reports.Metrics
{
	using Core.Metrics;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Compilers.Common;

	internal sealed class StatementsAnalyzer : SyntaxWalker
	{
		// Fields
		private int _counter;

		// Methods
		public StatementsAnalyzer()
			: base(SyntaxWalkerDepth.Node)
		{
		}

		public int Calculate(MemberNode node)
		{
			var syntax = MemberBodySelector.FindBody(node);
			if (syntax != null)
			{
				this.Visit(syntax);
			}
			this.CalculateConstructorStatements(node);
			this.CalculateCompilerGeneratedPropertyStatements(node);
			return _counter;
		}

		private void CalculateCompilerGeneratedPropertyStatements(MemberNode node)
		{
			switch (node.Kind)
			{
				case MemberKind.GetProperty:
				case MemberKind.SetProperty:
					if (MemberBodySelector.FindBody(node) == null)
					{
						_counter++;
					}
					return;
			}
		}

		private void CalculateConstructorStatements(MemberNode node)
		{
			ConstructorDeclarationSyntax syntax;
			if (((node.Kind == MemberKind.Constructor) && ((syntax = node.SyntaxNode as ConstructorDeclarationSyntax) != null)) && (syntax.Initializer != null))
			{
				this.Visit(syntax.Initializer);
				_counter++;
			}
		}

		public override void VisitBreakStatement(BreakStatementSyntax node)
		{
			base.VisitBreakStatement(node);
			_counter++;
		}

		public override void VisitCheckedStatement(CheckedStatementSyntax node)
		{
			base.VisitCheckedStatement(node);
			_counter++;
		}

		public override void VisitContinueStatement(ContinueStatementSyntax node)
		{
			base.VisitContinueStatement(node);
			_counter++;
		}

		public override void VisitDoStatement(DoStatementSyntax node)
		{
			base.VisitDoStatement(node);
			_counter++;
		}

		public override void VisitEmptyStatement(EmptyStatementSyntax node)
		{
			base.VisitEmptyStatement(node);
			_counter++;
		}

		public override void VisitExpressionStatement(ExpressionStatementSyntax node)
		{
			base.VisitExpressionStatement(node);
			_counter++;
		}

		public override void VisitFixedStatement(FixedStatementSyntax node)
		{
			base.VisitFixedStatement(node);
			_counter++;
		}

		public override void VisitForEachStatement(ForEachStatementSyntax node)
		{
			base.VisitForEachStatement(node);
			_counter++;
		}

		public override void VisitForStatement(ForStatementSyntax node)
		{
			base.VisitForStatement(node);
			_counter++;
		}

		public override void VisitGlobalStatement(GlobalStatementSyntax node)
		{
			base.VisitGlobalStatement(node);
			_counter++;
		}

		public override void VisitGotoStatement(GotoStatementSyntax node)
		{
			base.VisitGotoStatement(node);
			_counter++;
		}

		public override void VisitIfStatement(IfStatementSyntax node)
		{
			base.VisitIfStatement(node);
			_counter++;
		}

		public override void VisitInitializerExpression(InitializerExpressionSyntax node)
		{
			base.VisitInitializerExpression(node);
			_counter += node.Expressions.Count;
		}

		public override void VisitLabeledStatement(LabeledStatementSyntax node)
		{
			base.VisitLabeledStatement(node);
			_counter++;
		}

		public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
		{
			base.VisitLocalDeclarationStatement(node);
			if (!node.Modifiers.Any(SyntaxKind.ConstKeyword))
			{
				_counter++;
			}
		}

		public override void VisitLockStatement(LockStatementSyntax node)
		{
			base.VisitLockStatement(node);
			_counter++;
		}

		public override void VisitReturnStatement(ReturnStatementSyntax node)
		{
			base.VisitReturnStatement(node);
			_counter++;
		}

		public override void VisitSwitchStatement(SwitchStatementSyntax node)
		{
			base.VisitSwitchStatement(node);
			_counter++;
		}

		public override void VisitThrowStatement(ThrowStatementSyntax node)
		{
			base.VisitThrowStatement(node);
			_counter++;
		}

		public override void VisitUnsafeStatement(UnsafeStatementSyntax node)
		{
			base.VisitUnsafeStatement(node);
			_counter++;
		}

		public override void VisitUsingDirective(UsingDirectiveSyntax node)
		{
			base.VisitUsingDirective(node);
			_counter++;
		}

		public override void VisitUsingStatement(UsingStatementSyntax node)
		{
			base.VisitUsingStatement(node);
			_counter++;
		}

		public override void VisitWhileStatement(WhileStatementSyntax node)
		{
			base.VisitWhileStatement(node);
			_counter++;
		}

		public override void VisitYieldStatement(YieldStatementSyntax node)
		{
			base.VisitYieldStatement(node);
			_counter++;
		}
	}
}