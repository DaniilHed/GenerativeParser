using Common;

namespace Common.Errors
{
	//Common error record to use it for expected errors
	public  record  Error (string Code, string? Description = null)
	{
		public static readonly Error None = new(string.Empty);
		public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.");
		public static implicit operator Result(Error error) => Result.Failure(error);
	}
}
