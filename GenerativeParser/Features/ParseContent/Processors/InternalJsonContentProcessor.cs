using GenerativeParser.Common.Errors;
using GenerativeParser.Common;
using GenerativeParser.Features.ParseContent.DTOs;
using GenerativeParser.Features.ParseContent.Interfaces;
using System.Text.Json;
using GenerativeParser.Features.ParseContent.Enums;

namespace GenerativeParser.Features.ParseContent.Processors
{
	// Reads JSON text and converts it into a list of parsed items
	public sealed class InternalJsonContentProcessor(ILogger<InternalJsonContentProcessor> logger) : IContentProcessor
	{
		public ContentType Type => ContentType.INTERNAL_JSON;

		public Result<IReadOnlyCollection<ParsedItemDto>> Process(string content)
		{
			if (string.IsNullOrWhiteSpace(content))
			{
				return ParseContentErrors.EmptyContent;
			}

			logger.LogDebug("Starting JSON parsing for payload of length {PayloadLength}.", content.Length);

			try
			{
				using var document = JsonDocument.Parse(content);
				var root = document.RootElement;

				// Handle either a JSON array or a single JSON object
				var items = root.ValueKind switch
				{
					JsonValueKind.Array => ProcessArray(root),
					JsonValueKind.Object => ProcessSingleObject(root),
					_ => null
				};

				// Return error if JSON root is not an object or array of objects
				if (items is null)
				{
					logger.LogWarning("JSON parsing failed: root JSON element was of kind {ValueKind}, expected Object or Array.", root.ValueKind);

					return ParseContentErrors.InvalidCsvFormat("JSON root must be an object or an array of objects.");
				}
				logger.LogDebug("Successfully parsed JSON payload into {RecordCount} records.", items.Count);
				return Result.Success(items);
			}
			catch (JsonException ex)
			{
				// Catch broken JSON syntax errors
				logger.LogWarning(ex, "JSON parsing failed due to invalid syntax or structure: {Message}", ex.Message);
				return ParseContentErrors.InvalidCsvFormat($"Invalid JSON structure: {ex.Message}");
			}
		}

		// Parses a JSON array into items and returns null if any array element is not an object
		private static IReadOnlyCollection<ParsedItemDto>? ProcessArray(JsonElement arrayElement)
		{
			var items = new List<ParsedItemDto>();

			foreach (var element in arrayElement.EnumerateArray())
			{
				if (element.ValueKind != JsonValueKind.Object)
				{
					// Reject arrays containing simple values
					return null;
				}

				items.Add(new ParsedItemDto(ConvertObjectToDictionary(element)));
			}

			return items;
		}

		// Wraps a single JSON object into a list with one item
		private static IReadOnlyCollection<ParsedItemDto> ProcessSingleObject(JsonElement objectElement)
		{
			return [new ParsedItemDto(ConvertObjectToDictionary(objectElement))];
		}

		// Converts a JSON properties into dictionary
		private static Dictionary<string, object?> ConvertObjectToDictionary(JsonElement element)
		{
			var dictionary = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

			foreach (var property in element.EnumerateObject())
			{
				dictionary[property.Name] = ConvertJsonElement(property.Value);
			}

			return dictionary;
		}

		// Converts a raw JSON value to a standard C# primitive type
		private static object? ConvertJsonElement(JsonElement element)
		{
			return element.ValueKind switch
			{
				JsonValueKind.Null or JsonValueKind.Undefined => null,

				JsonValueKind.True => true,

				JsonValueKind.False => false,

				JsonValueKind.String => element.GetString(),

				JsonValueKind.Number when element.TryGetInt64(out var integer) => integer,

				JsonValueKind.Number when element.TryGetDecimal(out var decimalValue) => decimalValue,

				JsonValueKind.Array => element
					.EnumerateArray()
					.Select(ConvertJsonElement)
					.ToList(),

				JsonValueKind.Object => ConvertObjectToDictionary(element),

				_ => element.GetRawText()
			};
		}
	}
}
