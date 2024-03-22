using System.ComponentModel.DataAnnotations;

namespace ApiWithJwtBearer.Models
{
    public class dtoLogin
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
