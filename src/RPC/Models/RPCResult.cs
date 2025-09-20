using Newtonsoft.Json;

namespace GrindBreaker.RPC.Models
{
    public enum RPCResultType
    {
        Success,
        Error
    }

    public class RPCResult<T>
    {
        [JsonProperty("isError")]
        public bool IsError { get; set; }

        [JsonProperty("errorMessage")]
        public string? ErrorMessage { get; set; }

        [JsonProperty("data")]
        public T? Data { get; set; }

        public static RPCResult<T> Success(T data)
        {
            return new RPCResult<T>
            {
                IsError = false,
                Data = data
            };
        }

        public static RPCResult<T> Error(string errorMessage)
        {
            return new RPCResult<T>
            {
                IsError = true,
                ErrorMessage = errorMessage
            };
        }
    }
}
