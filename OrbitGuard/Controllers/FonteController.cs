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
        [SwaggerOperation(
            Summary = "Lista todas as fontes espaciais",
            Description = "Retorna todas as fontes espaciais cadastradas no OrbitGuard. Caso não existam registros, retorna 404 Not Found.")]
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

                if (!fontes.Any())
                    return NotFound("Nenhuma fonte espacial encontrada.");

                return Ok(fontes);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao buscar fontes espaciais.");
            }
        }

        [HttpGet("{id:long}")]
        [SwaggerOperation(
            Summary = "Busca uma fonte espacial por ID",
            Description = "Retorna os dados de uma fonte espacial específica a partir do ID informado.")]
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
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao buscar fonte espacial.");
            }
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Cadastra uma nova fonte espacial",
            Description = "Cria uma nova fonte espacial no sistema OrbitGuard, como NASA POWER, NASA FIRMS ou outra origem externa de dados ambientais.")]
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

                var nomeExiste = await _context.FontesEspaciais
                    .AnyAsync(f => f.Nome.ToLower() == fonte.Nome.ToLower());

                if (nomeExiste)
                    return Conflict("Já existe uma fonte espacial cadastrada com este nome.");

                fonte.TipoDado = fonte.TipoDado.ToUpper();

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
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao cadastrar fonte espacial.");
            }
        }

        [HttpPut("{id:long}")]
        [SwaggerOperation(
            Summary = "Atualiza uma fonte espacial",
            Description = "Atualiza completamente os dados de uma fonte espacial existente. O ID da URL deve ser igual ao ID enviado no corpo da requisição.")]
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

                var fonteExiste = await _context.FontesEspaciais
                    .AnyAsync(f => f.IdFonte == id);

                if (!fonteExiste)
                    return NotFound("Fonte espacial não encontrada.");

                var nomeEmUso = await _context.FontesEspaciais
                    .AnyAsync(f =>
                        f.Nome.ToLower() == fonte.Nome.ToLower()
                        && f.IdFonte != id);

                if (nomeEmUso)
                    return Conflict("Já existe outra fonte espacial cadastrada com este nome.");

                fonte.TipoDado = fonte.TipoDado.ToUpper();

                _context.Entry(fonte).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao atualizar fonte espacial.");
            }
        }

        [HttpDelete("{id:long}")]
        [SwaggerOperation(
            Summary = "Remove uma fonte espacial",
            Description = "Remove uma fonte espacial existente a partir do ID informado.")]
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
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao remover fonte espacial.");
            }
        }
    }
}