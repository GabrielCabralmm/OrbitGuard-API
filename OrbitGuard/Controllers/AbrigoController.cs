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
    public class AbrigoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AbrigoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista todos os abrigos",
            Description = "Retorna todos os abrigos cadastrados no OrbitGuard. Caso não existam registros, retorna 404 Not Found.")]
        [ProducesResponseType(typeof(IEnumerable<AbrigoEntity>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AbrigoEntity>>> Get()
        {
            try
            {
                var abrigos = await _context.Abrigos
                    .AsNoTracking()
                    .ToListAsync();

                if (!abrigos.Any())
                    return NotFound("Nenhum abrigo encontrado.");

                return Ok(abrigos);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao buscar abrigos.");
            }
        }

        [HttpGet("{id:long}")]
        [SwaggerOperation(
            Summary = "Busca um abrigo por ID",
            Description = "Retorna os dados de um abrigo específico a partir do ID informado.")]
        [ProducesResponseType(typeof(AbrigoEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AbrigoEntity>> GetById(long id)
        {
            try
            {
                var abrigo = await _context.Abrigos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.IdAbrigo == id);

                if (abrigo == null)
                    return NotFound("Abrigo não encontrado.");

                return Ok(abrigo);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao buscar abrigo.");
            }
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Cadastra um novo abrigo",
            Description = "Cria um novo abrigo vinculado a uma região existente. Valida a região, capacidade total e capacidade ocupada.")]
        [ProducesResponseType(typeof(AbrigoEntity), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AbrigoEntity>> Post([FromBody] AbrigoEntity abrigo)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var regiaoExiste = await _context.Regioes
                    .AnyAsync(r => r.IdRegiao == abrigo.IdRegiao);

                if (!regiaoExiste)
                    return NotFound("Região informada não encontrada.");

                if (abrigo.CapacidadeOcupada > abrigo.CapacidadeTotal)
                    return BadRequest("A capacidade ocupada não pode ser maior que a capacidade total.");

                abrigo.Ativo = abrigo.Ativo.ToUpper();

                _context.Abrigos.Add(abrigo);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = abrigo.IdAbrigo },
                    abrigo);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao cadastrar abrigo.");
            }
        }

        [HttpPut("{id:long}")]
        [SwaggerOperation(
            Summary = "Atualiza um abrigo",
            Description = "Atualiza completamente os dados de um abrigo existente. Valida ID, região e capacidade.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(long id, [FromBody] AbrigoEntity abrigo)
        {
            try
            {
                if (id != abrigo.IdAbrigo)
                    return BadRequest("O ID informado na URL é diferente do ID enviado no corpo da requisição.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var abrigoExiste = await _context.Abrigos
                    .AnyAsync(a => a.IdAbrigo == id);

                if (!abrigoExiste)
                    return NotFound("Abrigo não encontrado.");

                var regiaoExiste = await _context.Regioes
                    .AnyAsync(r => r.IdRegiao == abrigo.IdRegiao);

                if (!regiaoExiste)
                    return NotFound("Região informada não encontrada.");

                if (abrigo.CapacidadeOcupada > abrigo.CapacidadeTotal)
                    return BadRequest("A capacidade ocupada não pode ser maior que a capacidade total.");

                abrigo.Ativo = abrigo.Ativo.ToUpper();

                _context.Entry(abrigo).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao atualizar abrigo.");
            }
        }

        [HttpDelete("{id:long}")]
        [SwaggerOperation(
            Summary = "Remove um abrigo",
            Description = "Remove um abrigo existente. Caso existam recursos vinculados, retorna 409 Conflict.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var abrigo = await _context.Abrigos
                    .FirstOrDefaultAsync(a => a.IdAbrigo == id);

                if (abrigo == null)
                    return NotFound("Abrigo não encontrado.");

                var possuiRecursos = await _context.RecursosAbrigo
                    .AnyAsync(r => r.IdAbrigo == id);

                if (possuiRecursos)
                    return Conflict("Não é possível remover o abrigo, pois existem recursos vinculados a ele.");

                _context.Abrigos.Remove(abrigo);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao remover abrigo.");
            }
        }
    }
}