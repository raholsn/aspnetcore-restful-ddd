namespace API.Contracts
{
    public class MetadataResourceParameters
    {
        private const int MaxPageSize = 20;

        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string Filter { get; set; }

        public string SearchQuery { get; set; }

    }
}
