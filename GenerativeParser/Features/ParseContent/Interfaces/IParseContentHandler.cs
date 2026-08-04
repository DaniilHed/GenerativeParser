using GenerativeParser.Common;
using GenerativeParser.Features.ParseContent.DTOs;

namespace GenerativeParser.Features.ParseContent.Interfaces
{
	public interface IParseContentHandler
	{
		Result<ParseContentResponse> Handle(ParseContentRequest request);
	}
}
