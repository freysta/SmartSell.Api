using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSell.Api.Data;
using SmartSell.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSell.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoServicoController : ControllerBase
    {
        private readonly SmartSellDbContext _context;

        public ProdutoServicoController(SmartSellDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoServico>>> GetProdutosServicos()
        {
            return await _context.ProdutosServicos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoServico>> GetProdutoServico(int id)
        {
            var produtoServico = await _context.ProdutosServicos.FindAsync(id);
            if (produtoServico == null)
            {
                return NotFound();
            }
            return produtoServico;
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoServico>> CreateProdutoServico(ProdutoServico produtoServico)
        {
            _context.ProdutosServicos.Add(produtoServico);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProdutoServico), new { id = produtoServico.IdProduto }, produtoServico);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProdutoServico(int id, ProdutoServico produtoServico)
        {
            if (id != produtoServico.IdProduto)
            {
                return BadRequest();
            }
            _context.Entry(produtoServico).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoServicoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProdutoServico(int id)
        {
            var produtoServico = await _context.ProdutosServicos.FindAsync(id);
            if (produtoServico == null)
            {
                return NotFound();
            }
            _context.ProdutosServicos.Remove(produtoServico);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool ProdutoServicoExists(int id)
        {
            return _context.ProdutosServicos.Any(e => e.IdProduto == id);
        }
    }
}
