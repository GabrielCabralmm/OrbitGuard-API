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
    public class AlertaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AlertaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista todos os alertas",
            Description = "Retorna todos os alertas de risco cadastrados no sistema.")]
        [ProducesResponseType(typeof(IEnumerable<AlertaEntity>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AlertaEntity>>> Get()
        {
            try
            {
                var alertas = await _context.AlertasRisco
                    .AsNoTracking()
                    .ToListAsync();

                if (!alertas.Any())
                    return NotFound("Nenhum alerta encontrado.");

                return Ok(alertas);
            }
            catch
            {
                return StatusCode(500, "Erro interno ao buscar alertas.");
            }
        }

        [HttpGet("{id:long}")]
        [SwaggerOperation(
            Summary = "Busca um alerta por ID",
            Description = "Retorna os dados de um alerta específico.")]
        [ProducesResponseType(typeof(AlertaEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AlertaEntity>> GetById(long id)
        {
            try
            {
                var alerta = await _context.AlertasRisco
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.IdAlerta == id);

                if (alerta == null)
                    return NotFound("Alerta não encontrado.");

                return Ok(alerta);
            }
            catch
            {
                return StatusCode(500, "Erro interno ao buscar alerta.");
            }
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Cadastra um novo alerta",
            Description = "Cria um novo alerta de risco para uma região monitorada.")]
        [ProducesResponseType(typeof(AlertaEntity), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AlertaEntity>> Post([FromBody] AlertaEntity alerta)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var regiaoExiste = await _context.Regioes
                    .AnyAsync(r => r.IdRegiao == alerta.IdRegiao);

                if (!regiaoExiste)
                    return NotFound("Região informada não encontrada.");

                if (alerta.IdHistorico.HasValue)
                {
                    var historicoExiste = await _context.HistoricosRisco
                        .AnyAsync(h => h.IdHistorico == alerta.IdHistorico);

                    if (!historicoExiste)
                        return NotFound("Histórico informado não encontrado.");
                }

                alerta.NivelRisco = alerta.NivelRisco.ToUpper();
                alerta.StatusAlerta = alerta.StatusAlerta.ToUpper();

                if (alerta.DataAlerta == default)
                    alerta.DataAlerta = DateTime.Now;

                _context.AlertasRisco.Add(alerta);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = alerta.IdAlerta },
                    alerta);
            }
            catch
            {
                return StatusCode(500, "Erro interno ao cadastrar alerta.");
            }
        }

        [HttpPut("{id:long}")]
        [SwaggerOperation(
            Summary = "Atualiza um alerta",
            Description = "Atualiza completamente um alerta existente.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(long id, [FromBody] AlertaEntity alerta)
        {
            try
            {
                if (id != alerta.IdAlerta)
                    return BadRequest("O ID informado na URL é diferente do ID enviado no corpo da requisição.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var alertaExiste = await _context.AlertasRisco
                    .AnyAsync(a => a.IdAlerta == id);

                if (!alertaExiste)
                    return NotFound("Alerta não encontrado.");

                var regiaoExiste = await _context.Regioes
                    .AnyAsync(r => r.IdRegiao == alerta.IdRegiao);

                if (!regiaoExiste)
                    return NotFound("Região informada não encontrada.");

                if (alerta.IdHistorico.HasValue)
                {
                    var historicoExiste = await _context.HistoricosRisco
                        .AnyAsync(h => h.IdHistorico == alerta.IdHistorico);

                    if (!historicoExiste)
                        return NotFound("Histórico informado não encontrado.");
                }

                alerta.NivelRisco = alerta.NivelRisco.ToUpper();
                alerta.StatusAlerta = alerta.StatusAlerta.ToUpper();

                _context.Entry(alerta).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(500, "Erro interno ao atualizar alerta.");
            }
        }

        [HttpDelete("{id:long}")]
        [SwaggerOperation(
            Summary = "Remove um alerta",
            Description = "Remove um alerta existente. Caso existam ocorrências ou auditorias vinculadas, retorna 409 Conflict.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var alerta = await _context.AlertasRisco
                    .FirstOrDefaultAsync(a => a.IdAlerta == id);

                if (alerta == null)
                    return NotFound("Alerta não encontrado.");

                var possuiOcorrencias = await _context.Ocorrencias
                    .AnyAsync(o => o.IdAlerta == id);

                var possuiAuditorias = await _context.AuditoriasAlerta
                    .AnyAsync(a => a.IdAlerta == id);

                if (possuiOcorrencias || possuiAuditorias)
                    return Conflict("Não é possível remover o alerta, pois existem registros vinculados a ele.");

                _context.AlertasRisco.Remove(alerta);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(500, "Erro interno ao remover alerta.");
            }
        }
    }
}