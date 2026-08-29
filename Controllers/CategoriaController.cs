using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceApi.Data;

namespace EcommerceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/categorias
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var categorias = await _context.Categorias
                .Select(c => new {
                    c.Id,
                    c.Nome,
                    c.Descricao,
                    QuantidadeProdutos = c.Produtos.Count()
                })
                .ToListAsync();
            
            return Ok(categorias);
        }

        // GET: api/categorias/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var categoria = await _context.Categorias
                .Include(c => c.Produtos)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (categoria == null)
                return NotFound();

            // Retorna a categoria com seus produtos
            return Ok(new {
                categoria.Id,
                categoria.Nome,
                categoria.Descricao,
                Produtos = categoria.Produtos.Select(p => new {
                    p.Id,
                    p.Nome,
                    p.Descricao,
                    p.Preco,
                    p.Estoque
                })
            });
        }

        // POST: api/categorias
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DTOs.CategoriaCreateDto dto)
        {
            var categoria = new Categoria
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao
            };

            await _context.Categorias.AddAsync(categoria);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(Get), new { id = categoria.Id }, categoria);
        }
        

        // PUT: api/categorias/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Categoria categoria)
        {
            if (id != categoria.Id)
                return BadRequest("ID da URL não coincide com o ID do corpo");

            _context.Entry(categoria).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        // DELETE: api/categorias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _context.Categorias
                .Include(c => c.Produtos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
                return NotFound();

            // Verifica se a categoria tem produtos
            if (categoria.Produtos.Any())
            {
                return BadRequest("Não é possível excluir uma categoria que possui produtos");
            }

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
    }
}