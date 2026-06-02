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
    public class AuditoriaAlertaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuditoriaAlertaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista todas as auditorias de alertas",
            Description = "Retorna todos os registros de auditoria dos alertas cadastrados no sistema.")]
        [ProducesResponseType(typeof(IEnumerable<AuditoriaAlertaEntity>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AuditoriaAlertaEntity>>> Get()
        {
            try
            {
                var auditorias = await _context.AuditoriasAlerta
                    .AsNoTracking()
                    .ToListAsync();

                if (!auditorias.Any())
                    return NotFound("Nenhuma auditoria encontrada.");

                return Ok(auditorias);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao buscar auditorias.");
            }
        }

        [HttpGet("{id:long}")]
        [SwaggerOperation(
            Summary = "Busca uma auditoria por ID",
            Description = "Retorna os dados de uma auditoria específica a partir do ID informado.")]
        [ProducesResponseType(typeof(AuditoriaAlertaEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuditoriaAlertaEntity>> GetById(long id)
        {
            try
            {
                var auditoria = await _context.AuditoriasAlerta
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.IdAuditoria == id);

                if (auditoria == null)
                    return NotFound("Auditoria não encontrada.");

                return Ok(auditoria);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao buscar auditoria.");
            }
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Cadastra uma nova auditoria",
            Description = "Cria um novo registro de auditoria vinculado a um alerta existente.")]
        [ProducesResponseType(typeof(AuditoriaAlertaEntity), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuditoriaAlertaEntity>> Post([FromBody] AuditoriaAlertaEntity auditoria)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var alertaExiste = await _context.AlertasRisco
                    .AnyAsync(a => a.IdAlerta == auditoria.IdAlerta);

                if (!alertaExiste)
                    return NotFound("Alerta informado não encontrado.");

                auditoria.Acao = auditoria.Acao.ToUpper();

                if (!string.IsNullOrWhiteSpace(auditoria.StatusAnterior))
                    auditoria.StatusAnterior = auditoria.StatusAnterior.ToUpper();

                if (!string.IsNullOrWhiteSpace(auditoria.StatusNovo))
                    auditoria.StatusNovo = auditoria.StatusNovo.ToUpper();

                if (auditoria.DataAuditoria == default)
                    auditoria.DataAuditoria = DateTime.Now;

                _context.AuditoriasAlerta.Add(auditoria);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = auditoria.IdAuditoria },
                    auditoria);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao cadastrar auditoria.");
            }
        }

        [HttpPut("{id:long}")]
        [SwaggerOperation(
            Summary = "Atualiza uma auditoria",
            Description = "Atualiza completamente um registro de auditoria existente.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(long id, [FromBody] AuditoriaAlertaEntity auditoria)
        {
            try
            {
                if (id != auditoria.IdAuditoria)
                    return BadRequest("O ID informado na URL é diferente do ID enviado no corpo da requisição.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var auditoriaExiste = await _context.AuditoriasAlerta
                    .AnyAsync(a => a.IdAuditoria == id);

                if (!auditoriaExiste)
                    return NotFound("Auditoria não encontrada.");

                var alertaExiste = await _context.AlertasRisco
                    .AnyAsync(a => a.IdAlerta == auditoria.IdAlerta);

                if (!alertaExiste)
                    return NotFound("Alerta informado não encontrado.");

                auditoria.Acao = auditoria.Acao.ToUpper();

                if (!string.IsNullOrWhiteSpace(auditoria.StatusAnterior))
                    auditoria.StatusAnterior = auditoria.StatusAnterior.ToUpper();

                if (!string.IsNullOrWhiteSpace(auditoria.StatusNovo))
                    auditoria.StatusNovo = auditoria.StatusNovo.ToUpper();

                _context.Entry(auditoria).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao atualizar auditoria.");
            }
        }

        [HttpDelete("{id:long}")]
        [SwaggerOperation(
            Summary = "Remove uma auditoria",
            Description = "Remove um registro de auditoria existente.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var auditoria = await _context.AuditoriasAlerta
                    .FirstOrDefaultAsync(a => a.IdAuditoria == id);

                if (auditoria == null)
                    return NotFound("Auditoria não encontrada.");

                _context.AuditoriasAlerta.Remove(auditoria);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao remover auditoria.");
            }
        }
    }
}