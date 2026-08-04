using GenerativeParser.Common.Errors;

namespace GenerativeParser.Common
{
	public class Result
	{
		public bool IsSuccess { get; }
		public bool IsFailure => !IsSuccess;
		public Error Error { get; }

		protected Result(bool isSuccess, Error error)
		{
			if (isSuccess && error != Error.None)
			{
				throw new ArgumentException("A success result cannot contain an Error.", nameof(error));
			}
			if (!isSuccess && error == Error.None)
			{
				throw new ArgumentException("A failure result must contain an Error.", nameof(error));
			}
			IsSuccess = isSuccess;
			this.Error = error;
		}
		public static Result Success() => new(true, Error.None);
		public static Result Failure(Error error) => new(false, error);

		public static Result<TValue> Success<TValue>(TValue value) => new(value,true, Error.None);
		public static Result<TValue> Failure<TValue>(Error error) => new(default,false, error);
	}

	public class Result<TValue>: Result
	{
		private readonly TValue? _value;

		public TValue Value=>IsSuccess
			? _value!
			: throw new InvalidOperationException("The value of a failure result cannot be accessed.");

		protected internal Result (TValue? value, bool isSuccess, Error error): base(isSuccess, error)
		{
			_value = value;
		}

		public static implicit operator Result<TValue>(TValue? value) =>
			value is not null
			? Success<TValue>(value)
			: Failure<TValue>(Error.NullValue);

		public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);

	} 
}
