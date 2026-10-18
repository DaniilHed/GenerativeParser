using Contracts.DTOs;
using Infrastructure.ParseContent.Enums;




namespace Infrastructure.ParseContent.Entities
{
	public sealed class ParseContentJob
	{
		public Guid Id { get; private init; }
		public JobStatus Status { get; private set; }
		public string ContentType { get; private init; }
		public string Content { get; private init; }

		public ParseContentResult? Result { get; private set; }

		public string? ErrorCode { get; private set; }
		public string? ErrorMessage { get; private set; }

		public DateTimeOffset CreatedAt { get; private init; }
		public DateTimeOffset? CompletedAt { get; private set; }
		private ParseContentJob(
			Guid id,
			string type,
			string content)
		{
			Id = id;
			ContentType = type;
			Content = content;
			Status = JobStatus.Pending;
			CreatedAt = DateTimeOffset.UtcNow;
		}
		private ParseContentJob() { }
		public static ParseContentJob Create(
			string type,
			string content)
		{
			if (string.IsNullOrWhiteSpace(content))
				throw new ArgumentException("Content cannot be empty.",nameof(content));

			return new ParseContentJob(
				Guid.NewGuid(),
				type,
				content);
		}


		public void SetProcessing()
		{
			if(Status != JobStatus.Pending)
			{
				throw new InvalidOperationException("Status must be Pending to set Processing.");
			}
			Status = JobStatus.Processing;
		}

		public void SetCompleted(ParseContentResult result)
		{
			if(Status != JobStatus.Processing)
			{
				throw new InvalidOperationException("Status must be Processing to set Completed.");
			}
			Result = result;
			Status = JobStatus.Completed;
			CompletedAt = DateTimeOffset.UtcNow;
		}

		public void SetFailed(string errorCode, string errorMessage)
		{
			if(Status!= JobStatus.Processing)
			{
				throw new InvalidOperationException("Status must be Processing to set Failed");
			}
			Status = JobStatus.Failed;
			ErrorCode = errorCode;
			ErrorMessage= errorMessage;
			CompletedAt = DateTimeOffset.UtcNow;
		}
	}
}
