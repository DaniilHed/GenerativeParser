

using Common;


namespace WorkerService.ParseContent.Interfaces
{
	public interface IContentParserFactory
	{
		Result<IContentParser> GetParser(string type);
	}
}
