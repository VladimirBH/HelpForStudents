namespace WebApi.Classes
{
    public class UserToken : TokenPair
    {
        public string Name {get; set;}
        public string Surname {get; set;}
        public string Email {get; set;}
        public bool EmailConfirmed {get; set;}
    }
}