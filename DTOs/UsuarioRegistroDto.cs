
using System.ComponentModel.DataAnnotations;

namespace EcommerceApi.DTOs
{
    public class UsuarioRegistroDto
    {
        [Required]
        public string Nome { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [MinLength(6)]
        public string Senha { get; set; } = string.Empty;
    }

    public class UsuarioLoginDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Senha { get; set; } = string.Empty;
    }
}