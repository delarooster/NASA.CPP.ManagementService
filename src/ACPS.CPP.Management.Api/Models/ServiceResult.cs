namespace VOYG.CPP.Management.Api.Models
{
    public class ServiceResult<T> : IServiceResult<T>
    {
        public ErrorResponse ErrorResponse { get; set; } = new ErrorResponse();

        public bool IsInError { get; set; }

        public T Result { get; set; }

        public int StatusCode { get; set; }
    }
}