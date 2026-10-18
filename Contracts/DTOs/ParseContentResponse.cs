using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.DTOs
{
	public sealed record ParseContentResponse(Guid JobId, string Status);
}
