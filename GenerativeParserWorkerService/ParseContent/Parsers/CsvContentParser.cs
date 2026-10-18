
using Common;
using Common.Errors;
using Contracts.DTOs;

using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using WorkerService.ParseContent.Interfaces;


namespace WorkerService.ParseContent.Processors
{
	public sealed class CsvContentParser (ILogger<CsvContentParser> logger): IContentParser
	{
		public string ContentType => "CSV";

		// Core processing method that manages validation, header extraction, and row parsing
		public Result<IReadOnlyCollection<ParsedItemDto>> Parse(string content)
		{
			//Validate if content string is not empty 
			if (string.IsNullOrWhiteSpace(content))
			{
				logger.LogWarning("CSV parsing aborted: input content is null or empty.");
				return ParseContentErrors.EmptyContent;
			}

			logger.LogDebug("Starting CSV parsing for content payload of length {Length}", content.Length);

			try
			{
				using var reader = new StringReader(content);
				using var parser = CreateParser(reader);

				//Handle empty files or stream end
				if (parser.EndOfData)
				{
					logger.LogDebug("CSV parsing completed: stream reached end of data immediately.");
					return Result.Success<IReadOnlyCollection<ParsedItemDto>>(Array.Empty<ParsedItemDto>());
				}

				//Extract and validate column headers
				var headerResult = ExtractHeaders(parser);
				if (headerResult.IsFailure)
				{
					return headerResult.Error;
				}

				//Map content rows into structured DTOs
				var items = ParseRows(parser, headerResult.Value);

				logger.LogDebug("Successfully parsed CSV payload into {RecordCount} records.", items.Count);
				return Result.Success<IReadOnlyCollection<ParsedItemDto>>(items);
			}
			catch (MalformedLineException ex)
			{
				// Catch native parser exception for bad formatting 
				logger.LogWarning(ex, "CSV parsing failed due to malformed row at line {LineNumber}: {Message}", ex.LineNumber, ex.Message);
				return ParseContentErrors.InvalidCsvFormat($"Malformed CSV at line {ex.LineNumber}: {ex.Message}");
			}
		}
		// Configures the string parser rules (quotes, delimiters, whitespace)
		private static TextFieldParser CreateParser(TextReader reader)
		{
			var parser = new TextFieldParser(reader)
			{
				TextFieldType = FieldType.Delimited,
				HasFieldsEnclosedInQuotes = true,
				TrimWhiteSpace = true
			};
			parser.SetDelimiters(",");
			return parser;
		}

		// Reads the header row, checks missing headers and ensures that columns are unique
		private Result<string[]> ExtractHeaders(TextFieldParser parser)
		{
			var rawHeaders = parser.ReadFields();
			if (rawHeaders is null || rawHeaders.Length == 0)
			{
				logger.LogWarning("CSV parsing failed: missing or empty header row.");
				return ParseContentErrors.InvalidCsvFormat("CSV must contain a header row.");
			}

			var headers = new string[rawHeaders.Length];
			var seenHeaders = new HashSet<string>(rawHeaders.Length, StringComparer.OrdinalIgnoreCase);

			for (var i = 0; i < rawHeaders.Length; i++)
			{
				// Fall back to Column_N for empty header strings
				var headerName = string.IsNullOrWhiteSpace(rawHeaders[i]) ? $"Column_{i + 1}" : rawHeaders[i].Trim();

				// Fail early if duplicate column names exist
				if (!seenHeaders.Add(headerName))
				{
					logger.LogWarning("CSV parsing failed due to duplicate column header: '{HeaderName}'.", headerName);
					return ParseContentErrors.InvalidCsvFormat($"Duplicate column name detected: '{headerName}'.");
				}

				headers[i] = headerName;
			}

			return headers;
		}

		// Iterates through remaining lines and ignores blank rows
		private List<ParsedItemDto> ParseRows(TextFieldParser parser, string[] headers)
		{
			var items = new List<ParsedItemDto>();

			while (!parser.EndOfData)
			{
				var fields = parser.ReadFields();

				// Skip empty or blank lines
				if (fields is null || fields.All(string.IsNullOrWhiteSpace))
				{
					continue; 
				}

				var row = ParseRow(fields, headers);
				items.Add(new ParsedItemDto(row));
			}

			return items;
		}

		// Maps field values ​​to header keys and converts string values ​​to native types
		private static Dictionary<string, object?> ParseRow(string[] fields, string[] headers)
		{
			var row = new Dictionary<string, object?>(headers.Length, StringComparer.OrdinalIgnoreCase);

			for (var i = 0; i < headers.Length; i++)
			{
				row[headers[i]] = ParseValue(i < fields.Length ? fields[i] : null);
			}

			return row;
		}

		// Attempts to convert types into booleans, integers and decimals
		private static object? ParseValue(string? value)
		{
			value = value?.Trim();

			if (string.IsNullOrEmpty(value))
				return null;

			if (bool.TryParse(value, out var boolean))
				return boolean;

			if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var integer))
				return integer;

			if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var decimalValue))
				return decimalValue;

			return value;
		}
	}
}
