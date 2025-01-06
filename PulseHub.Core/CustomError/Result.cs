namespace PulseHub.Core.CustomError
{
    public class Result
    {
        public bool IsSuccess { get; }
        public Error? Error { get; }

        private Result(bool isSuccess, Error? error = null)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new Result(true);

        public static Result Failure(Error error) => new Result(false, error);
    }
}
