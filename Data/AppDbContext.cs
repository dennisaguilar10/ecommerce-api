using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }    
        public DbSet<Categoria> Categorias { get; set; } // 👈
        public DbSet<Usuario> Usuarios { get; set; }
    }

    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public int Estoque { get; set; }
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } // Navegação
    }

    public class Categoria
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public ICollection<Produto> Produtos { get; set; }
    }

    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }

    public class CategoriaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
    }

    // DTO para categoria com produtos
    public class CategoriaComProdutosDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public List<ProdutoDto> Produtos { get; set; }
    }

    public class ProdutoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
    }
}