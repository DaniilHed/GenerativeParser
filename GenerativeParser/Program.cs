using Features.ParseContent.Interfaces;
using FluentValidation;
using GenerativeParser.Features.ParseContent;
using GenerativeParser.Features.ParseContent.Validation;
using GenerativeParser.Middleware;
using MassTransit;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add core services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(configure =>
{
	configure.UsingRabbitMq((context, config) =>
	{
		config.Host(new Uri(builder.Configuration["RabbutMQ:Host"]!), h =>
		{
			h.Username(builder.Configuration["RabbitMQ:Username"]!);
			h.Username(builder.Configuration["RabbitMQ:Password"]!);
		});
		config.ConfigureEndpoints(context);
	});

});

builder.Services.AddInfrastructure(builder.Configuration);


// Register application components and service contracts.
builder.Services.AddScoped<IParseContentHandler, ParseContentHandler>();



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