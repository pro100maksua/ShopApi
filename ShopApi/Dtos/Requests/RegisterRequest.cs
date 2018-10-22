namespace ShopApi.Dtos.Requests
{
    public class RegisterRequest
    {
        public string UserName { get; set; }
        
        public string Password { get; set; }
        
        public string ConfirmPassword { get; set; }
    }
}