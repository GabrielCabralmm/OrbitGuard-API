using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrbitGuardAPI.Data;
using OrbitGuardAPI.Entitys;
using Swashbuckle.AspNetCore.Annotations;

namespace OrbitGuardAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class FonteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FonteController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista todas as fontes espaciais")]
        [ProducesResponseType(typeof(IEnumerable<FonteEntity>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<FonteEntity>>> Get()
        {
            try
            {
                var fontes = await _context.FontesEspaciais
                    .AsNoTracking()
                    .ToListAsync();

                if (fontes.Count == 0)
                    return NotFound("Nenhuma fonte espacial encontrada.");

                return Ok(fontes);
            }
            catch
            {
                return StatusCode(500, "Erro interno ao buscar fontes espaciais.");
            }
        }

        [HttpGet("{id:long}")]
        [SwaggerOperation(Summary = "Busca uma fonte espacial por ID")]
        [ProducesResponseType(typeof(FonteEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FonteEntity>> GetById(long id)
        {
            try
            {
                var fonte = await _context.FontesEspaciais
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.IdFonte == id);

                if (fonte == null)
                    return NotFound("Fonte espacial não encontrada.");

                return Ok(fonte);
            }
            catch
            {
                return StatusCode(500, "Erro interno ao buscar fonte espacial.");
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastra uma nova fonte espacial")]
        [ProducesResponseType(typeof(FonteEntity), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FonteEntity>> Post([FromBody] FonteEntity fonte)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                fonte.Nome = fonte.Nome.Trim();
                fonte.TipoDado = fonte.TipoDado.Trim().ToUpper();

                if (!string.IsNullOrWhiteSpace(fonte.UrlBase))
                    fonte.UrlBase = fonte.UrlBase.Trim();

                var nomeExiste = await _context.FontesEspaciais
                    .AsNoTracking()
                    .Where(f => f.Nome.ToLower() == fonte.Nome.ToLower())
                    .Select(f => f.IdFonte)
                    .FirstOrDefaultAsync();

                if (nomeExiste != 0)
                    return Conflict("Já existe uma fonte espacial cadastrada com este nome.");

                if (fonte.DataColeta == default)
                    fonte.DataColeta = DateTime.Now;

                _context.FontesEspaciais.Add(fonte);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = fonte.IdFonte },
                    fonte);
            }
            catch
            {
                return StatusCode(500, "Erro interno ao cadastrar fonte espacial.");
            }
        }

        [HttpPut("{id:long}")]
        [SwaggerOperation(Summary = "Atualiza uma fonte espacial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(long id, [FromBody] FonteEntity fonte)
        {
            try
            {
                if (id != fonte.IdFonte)
                    return BadRequest("O ID informado na URL é diferente do ID enviado no corpo da requisição.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                fonte.Nome = fonte.Nome.Trim();
                fonte.TipoDado = fonte.TipoDado.Trim().ToUpper();

                if (!string.IsNullOrWhiteSpace(fonte.UrlBase))
                    fonte.UrlBase = fonte.UrlBase.Trim();

                var nomeEmUso = await _context.FontesEspaciais
                    .AsNoTracking()
                    .Where(f => f.Nome.ToLower() == fonte.Nome.ToLower() && f.IdFonte != id)
                    .Select(f => f.IdFonte)
                    .FirstOrDefaultAsync();

                if (nomeEmUso != 0)
                    return Conflict("Já existe outra fonte espacial cadastrada com este nome.");

                var fonteBanco = await _context.FontesEspaciais
                    .FirstOrDefaultAsync(f => f.IdFonte == id);

                if (fonteBanco == null)
                    return NotFound("Fonte espacial não encontrada.");

                fonteBanco.Nome = fonte.Nome;
                fonteBanco.TipoDado = fonte.TipoDado;
                fonteBanco.UrlBase = fonte.UrlBase;
                fonteBanco.PayloadExemplo = fonte.PayloadExemplo;
                fonteBanco.DataColeta = fonte.DataColeta == default
                    ? fonteBanco.DataColeta
                    : fonte.DataColeta;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(500, "Erro interno ao atualizar fonte espacial.");
            }
        }

        [HttpDelete("{id:long}")]
        [SwaggerOperation(Summary = "Remove uma fonte espacial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var fonte = await _context.FontesEspaciais
                    .FirstOrDefaultAsync(f => f.IdFonte == id);

                if (fonte == null)
                    return NotFound("Fonte espacial não encontrada.");

                _context.FontesEspaciais.Remove(fonte);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(500, "Erro interno ao remover fonte espacial.");
            }
        }
    }
}