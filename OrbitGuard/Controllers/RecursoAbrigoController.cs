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
    public class RecursoAbrigoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RecursoAbrigoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista todos os recursos dos abrigos",
            Description = "Retorna todos os recursos cadastrados nos abrigos. Caso não existam registros, retorna 404 Not Found.")]
        [ProducesResponseType(typeof(IEnumerable<RecursoAbrigoEntity>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<RecursoAbrigoEntity>>> Get()
        {
            try
            {
                var recursos = await _context.RecursosAbrigo
                    .AsNoTracking()
                    .ToListAsync();

                if (!recursos.Any())
                    return NotFound("Nenhum recurso de abrigo encontrado.");

                return Ok(recursos);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao buscar recursos de abrigo.");
            }
        }

        [HttpGet("{id:long}")]
        [SwaggerOperation(
            Summary = "Busca um recurso de abrigo por ID",
            Description = "Retorna os dados de um recurso específico a partir do ID informado.")]
        [ProducesResponseType(typeof(RecursoAbrigoEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RecursoAbrigoEntity>> GetById(long id)
        {
            try
            {
                var recurso = await _context.RecursosAbrigo
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.IdRecurso == id);

                if (recurso == null)
                    return NotFound("Recurso de abrigo não encontrado.");

                return Ok(recurso);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao buscar recurso de abrigo.");
            }
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Cadastra um novo recurso de abrigo",
            Description = "Cria um novo recurso vinculado a um abrigo existente.")]
        [ProducesResponseType(typeof(RecursoAbrigoEntity), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RecursoAbrigoEntity>> Post([FromBody] RecursoAbrigoEntity recurso)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var abrigoExiste = await _context.Abrigos
                    .AnyAsync(a => a.IdAbrigo == recurso.IdAbrigo);

                if (!abrigoExiste)
                    return NotFound("Abrigo informado não encontrado.");

                recurso.TipoRecurso = recurso.TipoRecurso.ToUpper();
                recurso.Unidade = recurso.Unidade.ToUpper();

                _context.RecursosAbrigo.Add(recurso);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = recurso.IdRecurso },
                    recurso);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao cadastrar recurso de abrigo.");
            }
        }

        [HttpPut("{id:long}")]
        [SwaggerOperation(
            Summary = "Atualiza um recurso de abrigo",
            Description = "Atualiza completamente os dados de um recurso existente. Valida ID e abrigo vinculado.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(long id, [FromBody] RecursoAbrigoEntity recurso)
        {
            try
            {
                if (id != recurso.IdRecurso)
                    return BadRequest("O ID informado na URL é diferente do ID enviado no corpo da requisição.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var recursoExiste = await _context.RecursosAbrigo
                    .AnyAsync(r => r.IdRecurso == id);

                if (!recursoExiste)
                    return NotFound("Recurso de abrigo não encontrado.");

                var abrigoExiste = await _context.Abrigos
                    .AnyAsync(a => a.IdAbrigo == recurso.IdAbrigo);

                if (!abrigoExiste)
                    return NotFound("Abrigo informado não encontrado.");

                recurso.TipoRecurso = recurso.TipoRecurso.ToUpper();
                recurso.Unidade = recurso.Unidade.ToUpper();

                _context.Entry(recurso).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao atualizar recurso de abrigo.");
            }
        }

        [HttpDelete("{id:long}")]
        [SwaggerOperation(
            Summary = "Remove um recurso de abrigo",
            Description = "Remove um recurso existente a partir do ID informado.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var recurso = await _context.RecursosAbrigo
                    .FirstOrDefaultAsync(r => r.IdRecurso == id);

                if (recurso == null)
                    return NotFound("Recurso de abrigo não encontrado.");

                _context.RecursosAbrigo.Remove(recurso);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao remover recurso de abrigo.");
            }
        }
    }
}