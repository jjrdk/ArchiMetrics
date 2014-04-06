namespace ArchiMetrics.Common
{
	public interface IAvailability
	{
		bool IsAvailable { get; set; }

		string Title { get; }
	}
}