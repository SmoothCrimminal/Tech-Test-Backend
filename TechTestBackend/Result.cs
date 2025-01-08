namespace TechTestBackend
{
    public class Result
    {
        public string? Message { get; set; } = null;

        public StatusCode StatusCode { get; set; }
    }

    public class Result<T> : Result
    {
        public T? Payload { get; set; } = default;
    }

    public static class ResultExtensions
    {
        public static Result WithMessage(this Result result, string message)
        {
            result.Message = message;
            return result;
        }

        public static Result<T> WiithMessage<T>(this Result<T> result, string message)
        {
            result.Message = message;
            return result;
        }

        public static Result WithStatusCode(this Result result, StatusCode statusCode)
        {
            result.StatusCode = statusCode;
            return result;
        }

        public static Result<T> WithStatusCode<T>(this Result<T> result, StatusCode statusCode)
        {
            result.StatusCode = statusCode;
            return result;
        }

        public static Result<T> WithData<T>(this Result<T> result, T? data)
        {
            result.Payload = data;
            return result;
        }
    }

    public enum StatusCode
    {
        None,
        Success,
        NotFound,
        BadRequest
    }
}
