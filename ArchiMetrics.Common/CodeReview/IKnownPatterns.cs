namespace ArchiMetrics.Common.CodeReview
{
	public interface IKnownPatterns
	{
		bool IsExempt(string word);

		void Add(params string[] patterns);

		void Remove(string pattern);

		void Clear();
	}
}
