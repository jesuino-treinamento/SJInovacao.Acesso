using SJInovacao.Acesso.Common.Validation;

namespace SJInovacao.Acesso.WebAPI.Common
{
    public class ApiResponseWithData<T> : ApiResponse
    {
        public T? Data { get; set; }

        public static ApiResponseWithData<T> SuccessResponse(T data, string message = "")
        {
            return new ApiResponseWithData<T>
            {                
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponseWithData<T> ErrorResponse(string message, IEnumerable<ValidationErrorDetail>? errors = null, T? data = default)
        {
            return new ApiResponseWithData<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? Enumerable.Empty<ValidationErrorDetail>(),
                Data = data
            };
        }
    }
}
