namespace EventStaf.Models
{
	public class Result<T>
	{

		public bool IsSuccess { get; set; }
		public T Value { get; set; }
		public List<string> Errors { get; set; } = new List<string>();

		public static Result<T> Success(T value) => new Result<T> { IsSuccess = true, Value = value };
		public static Result<T> Failure(string error) => new Result<T> { IsSuccess = false, Errors = new List<string> { error } };
		public static Result<T> Failure(List<string> errors) => new Result<T> { IsSuccess = false, Errors = errors };

	}
}
