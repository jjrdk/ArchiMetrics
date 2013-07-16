// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionEdgeItemsRepositoryConfig.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SolutionEdgeItemsRepositoryConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.UI.DataAccess
{
	using System;
	using System.ComponentModel;
	using System.Runtime.CompilerServices;

	using ArchiMetrics.Common;

	public class SolutionEdgeItemsRepositoryConfig : ISolutionEdgeItemsRepositoryConfig
	{
		private bool _includeCodeReview;
		private string _path;
		private EdgeSource _source;

		public SolutionEdgeItemsRepositoryConfig()
		{
			this.CutOff = TimeSpan.FromDays(7);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public TimeSpan CutOff { get; set; }

		public string Path
		{
			get
			{
				return this._path;
			}

			set
			{
				if (this._path != value)
				{
					this._path = value;
					this.OnPropertyChanged();
				}
			}
		}

		public bool IncludeCodeReview
		{
			get
			{
				return this._includeCodeReview;
			}

			set
			{
				if (this._includeCodeReview != value)
				{
					this._includeCodeReview = value;
					this.OnPropertyChanged();
				}
			}
		}

		public EdgeSource Source
		{
			get
			{
				return this._source;
			}

			set
			{
				if (this._source != value)
				{
					this._source = value;
					this.OnPropertyChanged();
				}
			}
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = this.PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
