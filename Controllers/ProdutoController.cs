using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceApi.Data;

namespace EcommerceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProdutoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/produto
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var produtos = await _context.Produtos
                .Include(p => p.Categoria)
                .ToListAsync();
            
            return Ok(produtos);
        }

        // GET: api/produto/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var produto = await _context.Produtos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);
                
            if (produto == null)
                return NotFound();
                
            return Ok(produto);
        }

        // POST: api/produto
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DTOs.ProdutoCreateDto dto)
        {
            var produto = new Produto
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                Preco = dto.Preco,
                Estoque = dto.Estoque,
                CategoriaId = dto.CategoriaId
            };

            await _context.Produtos.AddAsync(produto);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(Get), new { id = produto.Id }, produto);
        }

        // PUT: api/produto/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Produto produto)
        {
            if (id != produto.Id)
                return BadRequest("ID da URL não coincide com o ID do corpo");

            // Verificar se a categoria existe
            if (produto.CategoriaId > 0)
            {
                var categoriaExiste = await _context.Categorias
                    .AnyAsync(c => c.Id == produto.CategoriaId);
                    
                if (!categoriaExiste)
                    return BadRequest("Categoria não encontrada");
            }

            _context.Entry(produto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        // DELETE: api/produto/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
                return NotFound();

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
    }
}