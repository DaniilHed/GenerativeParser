


using Common;
using Contracts.DTOs;


namespace WorkerService.ParseContent.Interfaces
{
	public interface IContentParser
	{
		string ContentType { get; }
		Result<IReadOnlyCollection<ParsedItemDto>> Parse(string content);
	}
}
