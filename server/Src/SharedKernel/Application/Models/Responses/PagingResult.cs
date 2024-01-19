namespace SharedKernel.Application
{
    public class PagingResult<T>
    {
        public IEnumerable<T> Data { get; set; }

        public long Count { get; set; }
    }
}
