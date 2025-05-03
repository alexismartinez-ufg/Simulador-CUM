using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SimuladorCUM.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El usuario es obligatorio")]
        [JsonProperty("username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
