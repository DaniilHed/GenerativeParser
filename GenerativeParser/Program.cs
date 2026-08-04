using FluentValidation;
using GenerativeParser.Features.ParseContent;
using GenerativeParser.Features.ParseContent.Interfaces;
using GenerativeParser.Features.ParseContent.Processors;
using GenerativeParser.Features.ParseContent.Validation;
using GenerativeParser.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add core services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register application components and service contracts.
builder.Services.AddSingleton<IBase64Decoder, Base64Decoder>();
builder.Services.AddScoped<IParseContentHandler, ParseContentHandler>();

// Register content processors for factory injection.
builder.Services.AddSingleton<IContentProcessor, CsvContentProcessor>();
builder.Services.AddSingleton<IContentProcessor, InternalJsonContentProcessor>();
builder.Services.AddSingleton<IContentProcessorFactory, ContentProcessorFactory>();

// Register FluentValidation validators from assembly.
builder.Services.AddValidatorsFromAssemblyContaining<ParseContentRequestValidator>();

var app = builder.Build();

// Configure global exception handling middleware.
app.UseMiddleware<GlobalExceptionMiddleware>();


if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.MapParseContentEndpoint();

app.Run();