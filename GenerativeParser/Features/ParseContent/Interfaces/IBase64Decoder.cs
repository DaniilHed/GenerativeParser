using GenerativeParser.Common;

namespace GenerativeParser.Features.ParseContent.Interfaces
{
	public interface IBase64Decoder
	{
		Result<string> Decode(string content);
	}
}
