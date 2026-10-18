using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Infrastructure.ParseContent.Enums
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum JobStatus
	{
		Completed,
		Pending,
		Failed,
		Processing
	}
}
