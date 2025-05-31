namespace UMenagmentService.Models.Authentication.UserResponses
{
    public class CreateUserResponse
    {
        public string Token {  get; set; }

        public User User { get; set; }
    }
}
