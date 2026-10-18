using GenerativeParser.Common.Errors;
using GenerativeParser.Features.ParseContent;
using System.Text;
using Xunit;
using FluentAssertions;

namespace GenerativeParser.Tests.ParseContent
{
	public class Base64DecoderTests
	{
		private readonly Base64Decoder _sut = new();

		[Fact]
		public void Decode_WhenContentIsValidBase64_ReturnsSuccessWithDecodedString()
		{
			// Arrange
			const string rawText = "Hello, World!";
			var base64Input = Convert.ToBase64String(Encoding.UTF8.GetBytes(rawText));

			// Act
			var result = _sut.Decode(base64Input);

			// Assert
			result.IsSuccess.Should().BeTrue();
			result.Value.Should().Be(rawText);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("   ")]
		public void Decode_WhenContentIsNullOrWhitespace_ReturnsEmptyContentError(string? invalidInput)
		{
			// Act
			var result = _sut.Decode(invalidInput!);

			// Assert
			result.IsFailure.Should().BeTrue();
			result.Error.Should().Be(ParseContentErrors.EmptyContent);
		}

		[Fact]
		public void Decode_WhenContentIsInvalidBase64_ReturnsInvalidBase64Error()
		{
			// Arrange
			const string invalidBase64 = "!!!NOT_BASE64_STRING!!!";

			// Act
			var result = _sut.Decode(invalidBase64);

			// Assert
			result.IsFailure.Should().BeTrue();
			// Compare against your actual static error or error code property:
			result.Error.Code.Should().Be(ParseContentErrors.InvalidBase64.Code);
		}
	}
}
