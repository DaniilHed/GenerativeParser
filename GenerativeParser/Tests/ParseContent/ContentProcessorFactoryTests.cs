using GenerativeParser.Features.ParseContent.Interfaces;
using GenerativeParser.Features.ParseContent;
using Xunit;
using NSubstitute;
using GenerativeParser.Features.ParseContent.Enums;
using FluentAssertions;
using GenerativeParser.Common.Errors;

namespace GenerativeParser.Tests.ParseContent
{
	public class ContentProcessorFactoryTests
	{
		private readonly IContentProcessor _csvProcessor = Substitute.For<IContentProcessor>();
		private readonly IContentProcessor _jsonProcessor = Substitute.For<IContentProcessor>();
		private readonly ContentProcessorFactory _sut;

		public ContentProcessorFactoryTests()
		{
			_csvProcessor.Type.Returns(ContentType.CSV);
			_jsonProcessor.Type.Returns(ContentType.INTERNAL_JSON);

			_sut = new ContentProcessorFactory([_csvProcessor, _jsonProcessor]);
		}

		[Theory]
		[InlineData(ContentType.CSV)]
		[InlineData(ContentType.INTERNAL_JSON)]
		public void GetProcessor_WhenProcessorIsRegistered_ReturnsSuccessWithProcessor(ContentType contentType)
		{
			// Act
			var result = _sut.GetProcessor(contentType);

			// Assert
			result.IsSuccess.Should().BeTrue();
			result.Value.Type.Should().Be(contentType);
		}

		[Fact]
		public void GetProcessor_WhenProcessorIsNotRegistered_ReturnsUnsupportedTypeError()
		{
			// Arrange
			const ContentType unregisteredType = (ContentType)999;

			// Act
			var result = _sut.GetProcessor(unregisteredType);

			// Assert
			result.IsFailure.Should().BeTrue();
			result.Error.Code.Should().Be(ParseContentErrors.UnsupportedType.Code);
		}
	}
}
