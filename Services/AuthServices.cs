using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using EcommerceApi.Data;

namespace EcommerceApi.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<Usuario> Registrar(string nome, string email, string senha)
        {
            var usuarioExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);
            
            if (usuarioExistente != null)
                throw new Exception("Usuário já existe!");

            var senhaHash = BCrypt.Net.BCrypt.HashPassword(senha, workFactor: 11);

            var usuario = new Usuario
            {
                Nome = nome,
                Email = email,
                Senha = senhaHash,
                Role = "User",
                CriadoEm = DateTime.UtcNow
            };

            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();

            return usuario;
        }

        public async Task<Usuario> Login(string email, string senha)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);
            
            if (usuario == null)
                throw new Exception("E-mail ou senha incorretos!");

            var senhaValida = BCrypt.Net.BCrypt.Verify(senha, usuario.Senha);
            
            if (!senhaValida)
                throw new Exception("E-mail ou senha incorretos!");

            return usuario;
        }

        public string GerarToken(Usuario usuario)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"] ?? "chave_super_secreta_32_caracteres_aqui")
            );
            
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Role ?? "User"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"] ?? "EcommerceApi",
                audience: _configuration["JwtSettings:Audience"] ?? "EcommerceApiUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}