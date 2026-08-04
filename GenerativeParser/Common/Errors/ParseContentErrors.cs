namespace GenerativeParser.Common.Errors
{
	// List of expected errors for result pattern
	public static class ParseContentErrors
	{
		public static readonly Error EmptyContent = new Error(
		"Parse.EmptyContent",
		"Field 'content' cannot be empty.");

		public static readonly Error InvalidBase64 = new Error(
			"Parse.InvalidBase64",
			"Field 'content' is not a valid Base64 encoded string.");

		public static readonly Error UnsupportedType = new Error(
			"Parse.UnsupportedType",
			"The requested content type 'type' is missing or not supported.");

		// Parse errors for supported formats (INTERNAL_JSON / CSV)
		public static Error InvalidInternalJson(string details) => new Error(
			"Parse.InvalidInternalJson",
			$"Invalid internal JSON format: {details}");

		public static Error InvalidCsvFormat(string details) => new Error(
			"Parse.InvalidCsvFormat",
			$"Failed to parse CSV payload: {details}");

		// Common parse error
		public static Error ProcessingFailed(string details) => new Error(
			"Parse.ProcessingFailed",
			$"An Error occurred while processing the payload: {details}");
	}
}
