using TxSpareParts.Core.CustomEntities;

namespace TxSpareParts.Responses
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public ApiResponse(T data)
        {
            Data = data;
        }

        public ApiResponse()
        {
        }

        public Metadata Metadata { get; set; }
    }
}
