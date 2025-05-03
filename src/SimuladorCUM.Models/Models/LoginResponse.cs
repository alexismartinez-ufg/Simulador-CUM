namespace SimuladorCUM.Models
{
    public class LoginResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string Access_Token { get; set; }
        public long Access_Token_Expires_At { get; set; }
        public string Refresh_Token { get; set; }
        public long Refresh_Token_Expires_At { get; set; }
        public string Program { get; set; }
    }

}
