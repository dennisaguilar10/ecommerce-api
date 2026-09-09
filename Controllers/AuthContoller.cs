using Microsoft.AspNetCore.Mvc;
using EcommerceApi.DTOs;
using EcommerceApi.Services;
using Microsoft.EntityFrameworkCore;
using EcommerceApi.Data;

namespace EcommerceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] UsuarioRegistroDto dto)
        {
            try
            {
                var usuario = await _authService.Registrar(
                    dto.Nome, 
                    dto.Email, 
                    dto.Senha
                );

                var token = _authService.GerarToken(usuario);

                return CreatedAtAction(nameof(Registrar), new TokenResponseDto
                {
                    Token = token,
                    ExpiraEm = DateTime.UtcNow.AddHours(8),
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Role = usuario.Role ?? "User"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDto dto)
        {
            try
            {
                var usuario = await _authService.Login(dto.Email, dto.Senha);
                
                var token = _authService.GerarToken(usuario);

                return Ok(new TokenResponseDto
                {
                    Token = token,
                    ExpiraEm = DateTime.UtcNow.AddHours(8),
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Role = usuario.Role ?? "User"
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { erro = ex.Message });
            }
        }
    }
}