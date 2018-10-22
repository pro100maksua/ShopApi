namespace ShopApi.Dtos.Requests
{
    public class FetchRequest
    {
        public int Skip { get; set; }
        
        public int Take { get; set; }

        public string SearchString { get; set; }
    }
}