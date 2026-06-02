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
    public class RegiaoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RegiaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista todas as regiões monitoradas",
            Description = "Retorna todas as regiões cadastradas no sistema OrbitGuard. Caso não existam registros, retorna 404 Not Found.")]
        [ProducesResponseType(typeof(IEnumerable<RegiaoEntity>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<RegiaoEntity>>> Get()
        {
            try
            {
                var regioes = await _context.Regioes
                    .AsNoTracking()
                    .ToListAsync();

                if (!regioes.Any())
                    return NotFound("Nenhuma região encontrada.");

                return Ok(regioes);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao buscar regiões.");
            }
        }

        [HttpGet("{id:long}")]
        [SwaggerOperation(
            Summary = "Busca uma região por ID",
            Description = "Retorna os dados de uma região específica a partir do ID informado.")]
        [ProducesResponseType(typeof(RegiaoEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RegiaoEntity>> GetById(long id)
        {
            try
            {
                var regiao = await _context.Regioes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.IdRegiao == id);

                if (regiao == null)
                    return NotFound("Região não encontrada.");

                return Ok(regiao);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao buscar região.");
            }
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Cadastra uma nova região",
            Description = "Cria uma nova região monitorada no sistema OrbitGuard.")]
        [ProducesResponseType(typeof(RegiaoEntity), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RegiaoEntity>> Post([FromBody] RegiaoEntity regiao)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                regiao.Uf = regiao.Uf.ToUpper();
                regiao.TipoRiscoBase = regiao.TipoRiscoBase.ToUpper();

                _context.Regioes.Add(regiao);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = regiao.IdRegiao },
                    regiao);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao cadastrar região.");
            }
        }

        [HttpPut("{id:long}")]
        [SwaggerOperation(
            Summary = "Atualiza uma região",
            Description = "Atualiza completamente os dados de uma região existente. O ID da URL deve ser igual ao ID enviado no corpo da requisição.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(long id, [FromBody] RegiaoEntity regiao)
        {
            try
            {
                if (id != regiao.IdRegiao)
                    return BadRequest("O ID informado na URL é diferente do ID enviado no corpo da requisição.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var regiaoExiste = await _context.Regioes
                    .AnyAsync(r => r.IdRegiao == id);

                if (!regiaoExiste)
                    return NotFound("Região não encontrada.");

                regiao.Uf = regiao.Uf.ToUpper();
                regiao.TipoRiscoBase = regiao.TipoRiscoBase.ToUpper();

                _context.Entry(regiao).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao atualizar região.");
            }
        }

        [HttpDelete("{id:long}")]
        [SwaggerOperation(
            Summary = "Remove uma região",
            Description = "Remove uma região existente a partir do ID informado.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var regiao = await _context.Regioes
                    .FirstOrDefaultAsync(r => r.IdRegiao == id);

                if (regiao == null)
                    return NotFound("Região não encontrada.");

                var possuiDependencias =
                    await _context.SensoresIot.AnyAsync(s => s.IdRegiao == id) ||
                    await _context.Abrigos.AnyAsync(a => a.IdRegiao == id) ||
                    await _context.HistoricosRisco.AnyAsync(h => h.IdRegiao == id) ||
                    await _context.AlertasRisco.AnyAsync(a => a.IdRegiao == id) ||
                    await _context.Ocorrencias.AnyAsync(o => o.IdRegiao == id);

                if (possuiDependencias)
                    return Conflict("Não é possível remover a região, pois existem registros vinculados a ela.");

                _context.Regioes.Remove(regiao);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao remover região.");
            }
        }
    }
}