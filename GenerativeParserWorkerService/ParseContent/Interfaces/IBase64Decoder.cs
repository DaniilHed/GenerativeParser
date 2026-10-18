
using Common;

namespace WorkerService.ParseContent.Interfaces
{
	public interface IBase64Decoder
	{
		Result<string> Decode(string content);
	}
}
