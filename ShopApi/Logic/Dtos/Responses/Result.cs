using System.Collections.Generic;

namespace ShopApi.Logic.Dtos.Responses
{
    public class Result<T>
    {
        public T Data { get; set; }

        public IList<string> Errors { get; set; }

        public Result()
        {
            Errors = new List<string>();
        }
    }
}