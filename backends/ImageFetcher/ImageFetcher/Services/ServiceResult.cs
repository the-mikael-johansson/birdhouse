namespace ImageFetcher.Services
{
    public class ServiceResult<T> : ServiceResult
    {
        public T Data { get; set; }

        public bool IsSuccessCode => Code == (int)ServiceResultCode.Ok;

        public static ServiceResult<T> OkWithContent(T content)
        {
            return new ServiceResult<T>
            {
                Code = (int)ServiceResultCode.Ok,
                Data = content
            };
        }

        public static ServiceResult<T> Ok(T bytes)
        {
            return new ServiceResult<T>
            {
                Code = (int)ServiceResultCode.Ok,
                Data = bytes
            };
        }

        public static ServiceResult<T> Error(string message)
        {
            return new ServiceResult<T>
            {
                Code = (int)ServiceResultCode.InternalError,
                Message = message
            };
        }

    }

    public class ServiceResult
    {
        public int Code { get; set; }
        public string Message { get; internal set; }
    }

    /// <summary>
    /// 0-1024 is reserved
    /// </summary>
    public enum ServiceResultCode {
        Ok = 200,
        InternalError = 500
    }
}
