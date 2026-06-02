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
    public class HistoricoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HistoricoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista todos os históricos de risco",
            Description = "Retorna todos os históricos de risco cadastrados. Caso não existam registros, retorna 404 Not Found.")]
        [ProducesResponseType(typeof(IEnumerable<HistoricoEntity>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<HistoricoEntity>>> Get()
        {
            try
            {
                var historicos = await _context.HistoricosRisco
                    .AsNoTracking()
                    .ToListAsync();

                if (!historicos.Any())
                    return NotFound("Nenhum histórico de risco encontrado.");

                return Ok(historicos);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao buscar históricos de risco.");
            }
        }

        [HttpGet("{id:long}")]
        [SwaggerOperation(
            Summary = "Busca um histórico de risco por ID",
            Description = "Retorna os dados de um histórico específico a partir do ID informado.")]
        [ProducesResponseType(typeof(HistoricoEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<HistoricoEntity>> GetById(long id)
        {
            try
            {
                var historico = await _context.HistoricosRisco
                    .AsNoTracking()
                    .FirstOrDefaultAsync(h => h.IdHistorico == id);

                if (historico == null)
                    return NotFound("Histórico de risco não encontrado.");

                return Ok(historico);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao buscar histórico de risco.");
            }
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Cadastra um novo histórico de risco",
            Description = "Cria um novo histórico de risco para uma região monitorada existente.")]
        [ProducesResponseType(typeof(HistoricoEntity), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<HistoricoEntity>> Post([FromBody] HistoricoEntity historico)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var regiaoExiste = await _context.Regioes
                    .AnyAsync(r => r.IdRegiao == historico.IdRegiao);

                if (!regiaoExiste)
                    return NotFound("Região informada não encontrada.");

                historico.NivelRisco = historico.NivelRisco.ToUpper();

                if (historico.DataCalculo == default)
                    historico.DataCalculo = DateTime.Now;

                _context.HistoricosRisco.Add(historico);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = historico.IdHistorico },
                    historico);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao cadastrar histórico de risco.");
            }
        }

        [HttpPut("{id:long}")]
        [SwaggerOperation(
            Summary = "Atualiza um histórico de risco",
            Description = "Atualiza completamente os dados de um histórico de risco existente.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(long id, [FromBody] HistoricoEntity historico)
        {
            try
            {
                if (id != historico.IdHistorico)
                    return BadRequest("O ID informado na URL é diferente do ID enviado no corpo da requisição.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var historicoExiste = await _context.HistoricosRisco
                    .AnyAsync(h => h.IdHistorico == id);

                if (!historicoExiste)
                    return NotFound("Histórico de risco não encontrado.");

                var regiaoExiste = await _context.Regioes
                    .AnyAsync(r => r.IdRegiao == historico.IdRegiao);

                if (!regiaoExiste)
                    return NotFound("Região informada não encontrada.");

                historico.NivelRisco = historico.NivelRisco.ToUpper();

                _context.Entry(historico).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao atualizar histórico de risco.");
            }
        }

        [HttpDelete("{id:long}")]
        [SwaggerOperation(
            Summary = "Remove um histórico de risco",
            Description = "Remove um histórico de risco. Caso existam alertas vinculados, retorna 409 Conflict.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var historico = await _context.HistoricosRisco
                    .FirstOrDefaultAsync(h => h.IdHistorico == id);

                if (historico == null)
                    return NotFound("Histórico de risco não encontrado.");

                var possuiAlertas = await _context.AlertasRisco
                    .AnyAsync(a => a.IdHistorico == id);

                if (possuiAlertas)
                    return Conflict("Não é possível remover o histórico, pois existem alertas vinculados a ele.");

                _context.HistoricosRisco.Remove(historico);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao remover histórico de risco.");
            }
        }
    }
}