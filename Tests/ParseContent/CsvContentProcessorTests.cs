using FluentAssertions;
using GenerativeParser.Common.Errors;
using GenerativeParser.Features.ParseContent.Processors;
using NSubstitute;
using Xunit;

namespace GenerativeParser.Tests.ParseContent
{
	public class CsvContentProcessorTests
	{
		private readonly ILogger<CsvContentProcessor> _logger = Substitute.For<ILogger<CsvContentProcessor>>();
		private readonly CsvContentProcessor _sut;

		public CsvContentProcessorTests()
		{
			_sut = new CsvContentProcessor(_logger);
		}

		[Fact]
		public void Process_WhenCsvIsValid_ParsesRowsAndTypesCorrectly()
		{
			// Arrange
			const string csvContent = "Id,Name,Price,IsActive\n101,\"John, Doe\",29.99,true";

			// Act
			var result = _sut.Process(csvContent);

			// Assert
			result.IsSuccess.Should().BeTrue();
			result.Value.Should().HaveCount(1);

			var row = result.Value.First().Values;
			row["Id"].Should().Be(101L);
			row["Name"].Should().Be("John, Doe");
			row["Price"].Should().Be(29.99m);
			row["IsActive"].Should().Be(true);
		}

		[Fact]
		public void Process_WhenDuplicateHeadersExist_ReturnsInvalidCsvFormatError()
		{
			// Arrange
			const string csvContent = "Id,Name,Name\n1,Alex,Smith";

			// Act
			var result = _sut.Process(csvContent);

			// Assert
			result.IsFailure.Should().BeTrue();
			result.Error.Description.Should().Contain("Duplicate column name detected");
		}

		[Fact]
		public void Process_WhenHeaderIsEmpty_GeneratesFallbackColumnName()
		{
			// Arrange
			const string csvContent = "Id,,Status\n1,Val,OK";

			// Act
			var result = _sut.Process(csvContent);

			// Assert
			result.IsSuccess.Should().BeTrue();
			var row = result.Value.First().Values;
			row.Should().ContainKey("Column_2");
			row["Column_2"].Should().Be("Val");
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("   ")]
		public void Process_WhenContentIsEmpty_ReturnsEmptyContentError(string? emptyContent)
		{
			// Act
			var result = _sut.Process(emptyContent!);

			// Assert
			result.IsFailure.Should().BeTrue();
			result.Error.Code.Should().Be(ParseContentErrors.EmptyContent.Code);
		}
	}
}
