namespace SlambookBackend.Models
{
    public class ServiceResponse<T>
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public string TraceID { get; set; } = string.Empty;
    }

    public class ServiceResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public string TraceID { get; set; } = string.Empty;
    }
}
