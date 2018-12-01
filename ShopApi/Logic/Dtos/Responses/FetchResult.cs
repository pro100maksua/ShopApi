using System.Collections.Generic;

namespace ShopApi.Logic.Dtos.Responses
{
    public class FetchResult<T>
    {
        public int Count { get; set; }

        public IEnumerable<T> Data { get; set; }
    }
}