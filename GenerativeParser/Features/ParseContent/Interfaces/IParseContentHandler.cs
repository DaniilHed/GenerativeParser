using Common;
using Contracts.DTOs;

using MassTransit;


namespace Features.ParseContent.Interfaces
{
	public interface IParseContentHandler
	{
		Task<Result<ParseContentResponse>> Handle(ParseContentRequest request, CancellationToken cancellationToken = default);
	}
}
