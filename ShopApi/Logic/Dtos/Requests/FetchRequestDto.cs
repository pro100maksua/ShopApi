namespace ShopApi.Logic.Dtos.Requests
{
    public class FetchRequestDto
    {
        public int Skip { get; set; }

        public int Take { get; set; } = 10;

        public string SearchString { get; set; }
    }
}