namespace ArchiMeter.Reports.Metrics
{
	using System;
	using Core.Metrics;

	internal sealed class HalsteadMetrics : IHalsteadMetrics
	{
		public static readonly IHalsteadMetrics GenericInstanceGetPropertyMetrics;
		public static readonly IHalsteadMetrics GenericInstanceSetPropertyMetrics;
		public static readonly IHalsteadMetrics GenericStaticGetPropertyMetrics;
		public static readonly IHalsteadMetrics GenericStaticSetPropertyMetrics;

		public HalsteadMetrics(int numOperands, int numOperators, int numUniqueOperands, int numUniqueOperators)
		{
			NumOperands = numOperands;
			NumOperators = numOperators;
			NumUniqueOperands = numUniqueOperands;
			NumUniqueOperators = numUniqueOperators;
		}

		public IHalsteadMetrics Merge(IHalsteadMetrics other)
		{
			if (other == null)
			{
				return this;
			}
			return new HalsteadMetrics(
				NumOperands + other.NumOperands,
				NumOperators + other.NumOperators,
				NumUniqueOperands + other.NumUniqueOperands,
				NumUniqueOperators + other.NumUniqueOperators);
		}

		public int NumOperands { get; private set; }

		public int NumOperators { get; private set; }

		public int NumUniqueOperands { get; private set; }

		public int NumUniqueOperators { get; private set; }

		static HalsteadMetrics()
		{
			GenericInstanceSetPropertyMetrics = new HalsteadMetrics(5, 3, 4, 3);
			GenericStaticSetPropertyMetrics = new HalsteadMetrics(4, 3, 3, 3);
			GenericInstanceGetPropertyMetrics = new HalsteadMetrics(3, 2, 3, 2);
			GenericStaticGetPropertyMetrics = new HalsteadMetrics(2, 1, 2, 1);
		}

		public int GetBugs()
		{
			double volume = GetVolume();

			return (int)(volume / 3000);
		}

		public double GetDifficulty()
		{
			return this.NumUniqueOperands == 0
				? 0
				: ((this.NumUniqueOperators / 2.0) * (this.NumOperands / ((double)this.NumUniqueOperands)));
		}

		public TimeSpan GetEffort()
		{
			var effort = GetDifficulty() * GetVolume();
			if (effort.Equals(0.0))
			{
				Console.WriteLine(NumOperands);
			}

			return TimeSpan.FromSeconds(effort / 18.0);
		}

		public int GetLength()
		{
			return (NumOperators + NumOperands);
		}

		public int GetVocabulary()
		{
			return (this.NumUniqueOperators + this.NumUniqueOperands);
		}

		public double GetVolume()
		{
			const double NewBase = 2.0;
			double vocabulary = GetVocabulary();
			double length = GetLength();
			if (vocabulary.Equals(0.0))
			{
				return 0.0;
			}
			return length * Math.Log(vocabulary, NewBase);
		}
	}
}