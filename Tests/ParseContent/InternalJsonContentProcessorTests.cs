using FluentAssertions;
using GenerativeParser.Common.Errors;
using GenerativeParser.Features.ParseContent.Processors;
using NSubstitute;
using Xunit;

namespace GenerativeParser.Tests.ParseContent
{
	public class InternalJsonContentProcessorTests
	{
		private readonly ILogger<InternalJsonContentProcessor> _logger = Substitute.For<ILogger<InternalJsonContentProcessor>>();
		private readonly InternalJsonContentProcessor _sut;

		public InternalJsonContentProcessorTests()
		{
			_sut = new InternalJsonContentProcessor(_logger);
		}

		[Fact]
		public void Process_WhenJsonIsArrayOfObjects_ParsesAllItems()
		{
			// Arrange
			const string jsonContent = """
			[
				{ "id": 1, "sku": "A-1", "isAvailable": true },
				{ "id": 2, "sku": "B-2", "isAvailable": false }
			]
			""";

			// Act
			var result = _sut.Process(jsonContent);

			// Assert
			result.IsSuccess.Should().BeTrue();
			result.Value.Should().HaveCount(2);

			var firstRow = result.Value.First().Values;
			firstRow["id"].Should().Be(1L);
			firstRow["sku"].Should().Be("A-1");
			firstRow["isAvailable"].Should().Be(true);
		}

		[Fact]
		public void Process_WhenJsonIsSingleObject_WrapsInSingleElementList()
		{
			// Arrange
			const string jsonContent = """{ "orderId": 99, "amount": 150.50 }""";

			// Act
			var result = _sut.Process(jsonContent);

			// Assert
			result.IsSuccess.Should().BeTrue();
			result.Value.Should().HaveCount(1);
			result.Value.First().Values["orderId"].Should().Be(99L);
		}

		[Fact]
		public void Process_WhenJsonIsInvalidSyntax_ReturnsInvalidJsonError()
		{
			// Arrange
			const string invalidJson = """[ { "id": 1, missing quote } ]""";

			// Act
			var result = _sut.Process(invalidJson);

			// Assert
			result.IsFailure.Should().BeTrue();
			result.Error.Code.Should().Be(ParseContentErrors.InvalidCsvFormat("").Code);
		}
	}
}
