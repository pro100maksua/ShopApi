using System.ComponentModel.DataAnnotations;

namespace ShopApi.Dtos
{
    public class FetchRequestDto
    {
        public int Skip { get; set; } = 0;

        [Range(1, int.MaxValue)]
        public int Take { get; set; } = 20;

        public string SearchString { get; set; } = string.Empty;
    }
}