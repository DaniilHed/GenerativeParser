using System.Text.Json.Serialization;

namespace GenerativeParser.Features.ParseContent.Enums
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum ContentType
	{
		CSV,
		INTERNAL_JSON
	}
}
