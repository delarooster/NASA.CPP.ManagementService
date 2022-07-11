namespace VOYG.CPP.Management.Api.Models
{
    public interface IServiceResult<T>
    {
        ErrorResponse ErrorResponse { get; set; }

        bool IsInError { get; set; }

        T Result { get; set; }

        int StatusCode { get; set; }
    }
}