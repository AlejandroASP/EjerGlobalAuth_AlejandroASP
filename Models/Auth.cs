using System.ComponentModel;

namespace APIMusicaAuth_SerafinParedesAlejandro.Models
{
    public class Auth
    {
        [DisplayName("Usuario")]
        public string User { get; set; }
        [DisplayName("Contraseña")]
        public string Password { get; set; }
    }
}
