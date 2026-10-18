using GenerativeParser.Common.Errors;
using GenerativeParser.Common;
using GenerativeParser.Features.ParseContent.DTOs;
using GenerativeParser.Features.ParseContent.Interfaces;
using GenerativeParser.Features.ParseContent;
using NSubstitute;
using Xunit;
using GenerativeParser.Features.ParseContent.Enums;
using FluentAssertions;

namespace GenerativeParser.Tests.ParseContent
{
	public class ParseContentHandlerTests
	{
		private readonly IBase64Decoder _decoder = Substitute.For<IBase64Decoder>();
		private readonly IContentProcessorFactory _processorFactory = Substitute.For<IContentProcessorFactory>();
		private readonly IContentProcessor _processor = Substitute.For<IContentProcessor>();
		private readonly ILogger<ParseContentHandler> _logger = Substitute.For<ILogger<ParseContentHandler>>();
		private readonly ParseContentHandler _sut;

		public ParseContentHandlerTests()
		{
			_sut = new ParseContentHandler(_decoder, _processorFactory, _logger);
		}

		[Fact]
		public void Handle_WhenPipelineSucceeds_ReturnsParseContentResponse()
		{
			// Arrange
			var request = new ParseContentRequest(ContentType.CSV, "ValidBase64String");
			const string decodedContent = "Id,Name\n1,Item";

			var parsedItems = new List<ParsedItemDto>
		{
			new(new Dictionary<string, object?> { { "Id", 1L }, { "Name", "Item" } })
		};

			_decoder.Decode(request.Content).Returns(Result.Success(decodedContent));
			_processorFactory.GetProcessor(request.Type).Returns(Result.Success(_processor));
			_processor.Process(decodedContent).Returns(Result.Success<IReadOnlyCollection<ParsedItemDto>>(parsedItems));

			// Act
			var result = _sut.Handle(request);

			// Assert
			result.IsSuccess.Should().BeTrue();
			result.Value.RecordCount.Should().Be(1);
			result.Value.Data.Should().BeEquivalentTo(parsedItems);
		}

		[Fact]
		public void Handle_WhenBase64DecoderFails_ShortCircuitsAndReturnsError()
		{
			// Arrange
			var request = new ParseContentRequest(ContentType.CSV, "InvalidBase64");
			_decoder.Decode(request.Content).Returns(ParseContentErrors.EmptyContent);

			// Act
			var result = _sut.Handle(request);

			// Assert
			result.IsFailure.Should().BeTrue();
			result.Error.Should().Be(ParseContentErrors.EmptyContent);

			// Ensure processor lookup never executes if decoding fails
			_processorFactory.DidNotReceiveWithAnyArgs().GetProcessor(default);
		}
	}
}
