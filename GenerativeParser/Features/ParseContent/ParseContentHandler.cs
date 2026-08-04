using GenerativeParser.Common;
using GenerativeParser.Features.ParseContent.DTOs;
using GenerativeParser.Features.ParseContent.Interfaces;

namespace GenerativeParser.Features.ParseContent
{
	// Hadnler that uses specialised classes to handle parsing and return coorrect response
	public class ParseContentHandler(
		IBase64Decoder base64Decoder,
		IContentProcessorFactory processorFactory,
		ILogger<ParseContentHandler> logger
		) : IParseContentHandler
	{
	
		public Result<ParseContentResponse> Handle(ParseContentRequest request)
		{
			logger.LogInformation("Processing parse request for ContentType: {ContentType}", request.Type);
			
			// Decode base64 content with custom decoder class
			var decodedResult = base64Decoder.Decode(request.Content);
			if (decodedResult.IsFailure)
			{
				logger.LogWarning(
					"Base64 decoding failed for ContentType: {ContentType}. Code: {ErrorCode}, Description: {ErrorDescription}",
					request.Type,
					decodedResult.Error.Code,
					decodedResult.Error.Description);
				return decodedResult.Error;
			}
			var decodedContent = decodedResult.Value;
			
			// Get correct processor for supported type using factory pattern
			var processorResult = processorFactory.GetProcessor(request.Type);
			if (processorResult.IsFailure)
			{
				logger.LogWarning(
					"Processor lookup failed for ContentType: {ContentType}. Code: {ErrorCode}, Description: {ErrorDescription}",
					request.Type,
					processorResult.Error.Code,
					processorResult.Error.Description);

				return processorResult.Error;
			}
			var processor = processorResult.Value;

			// Process decoded content 
			var processedResult = processor.Process(decodedContent);

			if (processedResult.IsFailure)
			{
				logger.LogWarning(
					"Content parsing failed for ContentType: {ContentType}. Code: {ErrorCode}, Description: {ErrorDescription}",
					request.Type,
					processedResult.Error.Code,
					processedResult.Error.Description);

				return processedResult.Error;
			}

			var data = processedResult.Value;

			logger.LogInformation(
				"Successfully parsed {RecordCount} records for ContentType: {ContentType}",
				data.Count,
				request.Type);

			return new ParseContentResponse(RecordCount:data.Count,Data: data);
		}
	}
}
