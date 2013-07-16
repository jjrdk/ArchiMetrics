// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XamlConverterTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the XamlConverterTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Analysis.Tests.Xaml
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Xml.Linq;
	using NUnit.Framework;
	using Roslyn.Services;

	public class XamlConverterTests
	{
		private XamlConverter _converter;

		[SetUp]
		public void Setup()
		{
			_converter = new XamlConverter();
		}

		[Test]
		public void WhenLoadingProjectThenCanAccessXaml()
		{
			var path = Path.GetFullPath(@"..\..\..\SampleSL\SampleSL.csproj");
			var workspace = Workspace.LoadStandAloneProject(path);
			var project = workspace.CurrentSolution.Projects.Last();
			if (!project.HasDocuments)
			{
				project = GetDocuments(project);
			}

			var isSilverlight = project.CompilationOptions.SubsystemVersion.Major == 4;
			Assert.IsTrue(isSilverlight);
		}

		[Test]
		public void WhenConvertingXamlSnippetThenGeneratesCode()
		{
			var tree = _converter.ConvertSnippet(GetXaml());
			var code = tree.GetRoot().ToFullString();

			Console.WriteLine(code);
			Assert.IsNotEmpty(code);
		}

		[Test]
		public void WhenConvertingComplexXamlSnippetThenGeneratesCode()
		{
			var tree = _converter.ConvertSnippet(GetComplexXaml());
			var code = tree.GetRoot().ToFullString();
			Console.WriteLine(code);
			Assert.IsNotEmpty(code);
		}

		[Test]
		public void WhenConvertingXamlSnippetThenCanCompileCode()
		{
			var tree = _converter.ConvertSnippet(GetXaml());
			ProjectId pid;
			DocumentId did;
			var compilation = Solution.Create(SolutionId.CreateNewId("Metrics"))
								   .AddCSharpProject("testcode.dll", "testcode", out pid)
								   .AddDocument(pid, "TestClass.cs", tree.GetText(), out did)
								   .AddProjectReferences(pid, new ProjectId[0])
								   .Projects.First().GetCompilation();

			Assert.NotNull(compilation.Assembly);
		}

		private IProject GetDocuments(IProject project)
		{
			var doc = XDocument.Load(project.FilePath);
			var defaultNs = doc.Root.GetDefaultNamespace();
			var compiles = doc.Descendants(defaultNs + "Compile");
			var dependents = doc.Descendants(defaultNs + "DependentUpon");
			var filePaths =
				compiles
				   .Select(x => x.Attribute("Include").Value)
				   .Concat(dependents.Select(x => x.Value));

			project = filePaths.Aggregate(
				project, 
				(p, s) =>
					{
						DocumentId did;
						var root = Path.GetDirectoryName(project.FilePath);
						var filepath = Path.Combine(root, s);
						if (s.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase))
						{
							var text = _converter.Convert(filepath);
							return p.AddDocument(s, text.GetText(), out did);
						}
						
						return p.AddDocument(s, out did);
					});
			
			return project;
		}

		private string GetXaml()
		{
			return @"<UserControl x:Class=""ArchiMetrics.UI.View.CodeReviewView""
			 xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
			 xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
			 xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
			 xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
			 xmlns:viewModel=""clr-namespace:ArchiMetrics.UI.ViewModel""
			 mReimers.dkIgnorable=""d""
			 d:DesignHeight=""300""
			 d:DesignWidth=""300""
			 d:DataContext=""{d:DesignInstance Type=viewModel:CodeReviewViewModel}"">
	<Grid>
		<Grid.RowDefinitions>
			<RowDefinition Height=""Auto"" />
			<RowDefinition />
		</Grid.RowDefinitions>
		<Grid.ColumnDefinitions>
			<ColumnDefinition />
			<ColumnDefinition Width=""Auto"" />
		</Grid.ColumnDefinitions>
		<StackPanel Grid.Row=""0""
					Grid.Column=""0""
					Orientation=""Horizontal""
					HorizontalAlignment=""Stretch"">
			<TextBlock Text=""{Binding Path=ErrorsShown, StringFormat='{}{0} errors found '}"" />
			<TextBlock Text=""{Binding Path=FilesWithErrors, StringFormat='in {0} files.'}"" />
			<TextBlock Text=""{Binding Path=BrokenCode, StringFormat=' Lines of code which are counted as broken: {0}.'}"" />
		</StackPanel>
		<Button Grid.Row=""0""
				Grid.Column=""1""
				Content=""Print Report""
				Click=""OnPrintReport"" />
		<DataGrid x:Name=""CodeReviewGrid""
				  Grid.Row=""1""
				  Grid.ColumnSpan=""2""
				  ItemsSource=""{Binding Path=CodeErrors}""
				  AutoGenerateColumns=""False""
				  EnableColumnVirtualization=""True""
				  EnableRowVirtualization=""True""
				  AlternationCount=""2""
				  AlternatingRowBackground=""LightGray""
				  ClipboardCopyMode=""IncludeHeader""
				  GridLinesVisibility=""Horizontal""
				  HeadersVisibility=""Column""
				  CanUserAddRows=""False""
				  CanUserDeleteRows=""False""
				  CanUserReorderColumns=""True""
				  CanUserResizeColumns=""True""
				  CanUserResizeRows=""True""
				  IsReadOnly=""True""
				  CanUserSortColumns=""True"">
			<DataGrid.Columns>
				<!--<DataGridTextColumn Header=""Quality""
									Binding=""{Binding Path=Quality}"" />-->
				<DataGridTextColumn Header=""Comment""
									Binding=""{Binding Path=Comment}"" />
				<DataGridTextColumn Header=""LoC""
									Binding=""{Binding Path=LinesOfCodeAffected}"" />
				<DataGridTextColumn Header=""Snippet""
									Binding=""{Binding Path=Snippet}"" />
				<DataGridTextColumn Header=""File""
									Binding=""{Binding Path=FilePath}"" />
				<DataGridTextColumn Header=""Project""
									Binding=""{Binding Path=ProjectPath}"" />
			</DataGrid.Columns>
		</DataGrid>
		<ProgressBar Grid.Row=""1""
					 Grid.ColumnSpan=""2""
					 Margin=""25,0""
					 Height=""25""
					 MinWidth=""200""
					 IsIndeterminate=""True""
					 Visibility=""{Binding Path=IsLoading, Converter={StaticResource TrueVisibility}}""
					 VerticalAlignment=""Center""
					 HorizontalAlignment=""Stretch"" />
	</Grid>
</UserControl>";
		}

		private string GetComplexXaml()
		{
			return @"<UserControl x:Class=""ArchiMetrics.UI.View.CodeReviewView""
			 xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
			 xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
			 xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
			 xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
			 xmlns:viewModel=""clr-namespace:ArchiMetrics.UI.ViewModel""
			 mReimers.dkIgnorable=""d""
			 d:DesignHeight=""300""
			 d:DesignWidth=""300""
			 d:DataContext=""{d:DesignInstance Type=viewModel:CodeReviewViewModel}"">
	<UserControl.Resources>
		<NullableBoolConverter x:Key=""conv"" />
		<DataTemplate x:Key=""dt"">
			<Border>
				<TextBlock Text=""{Binding Path=Value}"" />
			</Border>
		</DataTemplate>
	</UserControl.Resources>
	<UserControl.RenderTransform>
		<TranslateTransform X=""1""></TranslateTransform>
	</UserControl.RenderTransform>
	<Grid x:Name=""LayoutRoot""
		  Background=""White"">
		<TextBlock Grid.Row=""1""
				   Text=""Hello World"">
			<TextBlock.Style>
				<Style TargetType=""TextBlock"">
					<Setter Property=""Foreground""
							Value=""Red"" />
				</Style>
			</TextBlock.Style>
		</TextBlock>
	</Grid>
</UserControl>";
		}
	}
}
