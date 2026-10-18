using WorkerService;
using WorkerService.ParseContent.Helpers;
using WorkerService.ParseContent.Interfaces;
using WorkerService.ParseContent.Processors;

var builder = WebApplication.CreateBuilder(args);


builder.Services.ConfigureParsers();
builder.Services.AddScoped<JobResultHandler>();
builder.Services.AddSignalR();

//Add CORS
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
	{
		policy.WithOrigins("http://localhost:1111")
			  .SetIsOriginAllowed(_ => true)
			  .AllowAnyMethod()
			  .AllowAnyHeader()
			  .AllowCredentials();
	});
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
