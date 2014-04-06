namespace ArchiMetrics.Common.Tests.Xaml
{
	using System.Xml.Linq;
	using ArchiMetrics.Common.Xaml;
	using NUnit.Framework;

	public sealed class XamlNodeTests
	{
		private XamlNodeTests()
		{
		}

		public class GivenAXamlSnippet
		{
			private string _snippet;

			[SetUp]
			public void Setup()
			{
				_snippet = @"<UserControl x:Class=""ArchiMetrics.UI.View.CodeReviewView""
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

			[Test]
			public void CanParseXaml()
			{
				Assert.DoesNotThrow(() => new XamlNode(null, XElement.Parse(_snippet)));
			}
		}
	}
}
