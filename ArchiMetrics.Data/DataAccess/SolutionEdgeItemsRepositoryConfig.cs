// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionEdgeItemsRepositoryConfig.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SolutionEdgeItemsRepositoryConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Data.DataAccess
{
	using System;
	using System.ComponentModel;
	using System.Runtime.CompilerServices;
	using Common;
	using Properties;

	public class SolutionEdgeItemsRepositoryConfig : ISolutionEdgeItemsRepositoryConfig
	{
		private bool _includeCodeReview;
		private string _path;
		private EdgeSource _source;

		public SolutionEdgeItemsRepositoryConfig()
		{
			CutOff = TimeSpan.FromDays(7);
		}

		public TimeSpan CutOff { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public string Path
		{
			get
			{
				return _path;
			}

			set
			{
				if (_path != value)
				{
					_path = value;
					OnPropertyChanged();
				}
			}
		}

		public bool IncludeCodeReview
		{
			get
			{
				return _includeCodeReview;
			}

			set
			{
				if (_includeCodeReview != value)
				{
					_includeCodeReview = value;
					OnPropertyChanged();
				}
			}
		}

		public EdgeSource Source
		{
			get
			{
				return _source;
			}

			set
			{
				if (_source != value)
				{
					_source = value;
					OnPropertyChanged();
				}
			}
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
