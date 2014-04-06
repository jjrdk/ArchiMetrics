namespace ArchiMetrics.Common.Metrics
{
	using System;

	public interface IHalsteadMetrics
	{
		int NumberOfOperands { get; }

		int NumberOfOperators { get; }

		int NumberOfUniqueOperands { get; }

		int NumberOfUniqueOperators { get; }

		int GetBugs();

		double GetDifficulty();

		TimeSpan GetEffort();

		int GetLength();

		int GetVocabulary();

		double GetVolume();

		IHalsteadMetrics Merge(IHalsteadMetrics metrics);
	}
}
