
using Common;
using Common.Errors;
using System.Text;
using WorkerService.ParseContent.Interfaces;

namespace WorkerService.ParseContent
{
	public class Base64Decoder : IBase64Decoder
	{
		public Result<string> Decode(string base64Content)
		{
			if (string.IsNullOrWhiteSpace(base64Content))
			{
				return ParseContentErrors.EmptyContent;
			}
			// Memory allocation 
			Span<byte> buffer = new Span<byte>(new byte[base64Content.Length]);

			//Format validation and trying to convert string into bytes with allocated memory 
			if(!Convert.TryFromBase64String(base64Content,buffer,out int bytesWritten))
			{
				return ParseContentErrors.InvalidBase64;
			}

			// Trying to decode bytes 
			try
			{
				string decodedContent = Encoding.UTF8.GetString(buffer[..bytesWritten]);
				return decodedContent;
			}
			catch(DecoderFallbackException exception)
			{
				return ParseContentErrors.ProcessingFailed($"Decoded binary payload is not valid UTF-8 text: {exception.Message}");
			}

		}
	}
}
