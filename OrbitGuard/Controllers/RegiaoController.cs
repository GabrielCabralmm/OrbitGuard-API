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

                if (regioes.Count == 0)
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
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RegiaoEntity>> Post([FromBody] RegiaoEntity regiao)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                regiao.Nome = regiao.Nome.Trim();
                regiao.Cidade = regiao.Cidade.Trim();
                regiao.Uf = regiao.Uf.Trim().ToUpper();
                regiao.TipoRiscoBase = regiao.TipoRiscoBase.Trim().ToUpper();

                var regiaoExiste = await _context.Regioes
                    .AsNoTracking()
                    .Where(r =>
                        r.Nome.ToLower() == regiao.Nome.ToLower()
                        && r.Cidade.ToLower() == regiao.Cidade.ToLower()
                        && r.Uf.ToLower() == regiao.Uf.ToLower())
                    .Select(r => r.IdRegiao)
                    .FirstOrDefaultAsync();

                if (regiaoExiste != 0)
                    return Conflict("Já existe uma região cadastrada com este nome, cidade e UF.");

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
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(long id, [FromBody] RegiaoEntity regiao)
        {
            try
            {
                if (id != regiao.IdRegiao)
                    return BadRequest("O ID informado na URL é diferente do ID enviado no corpo da requisição.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                regiao.Nome = regiao.Nome.Trim();
                regiao.Cidade = regiao.Cidade.Trim();
                regiao.Uf = regiao.Uf.Trim().ToUpper();
                regiao.TipoRiscoBase = regiao.TipoRiscoBase.Trim().ToUpper();

                var regiaoEmUso = await _context.Regioes
                    .AsNoTracking()
                    .Where(r =>
                        r.Nome.ToLower() == regiao.Nome.ToLower()
                        && r.Cidade.ToLower() == regiao.Cidade.ToLower()
                        && r.Uf.ToLower() == regiao.Uf.ToLower()
                        && r.IdRegiao != id)
                    .Select(r => r.IdRegiao)
                    .FirstOrDefaultAsync();

                if (regiaoEmUso != 0)
                    return Conflict("Já existe outra região cadastrada com este nome, cidade e UF.");

                var regiaoBanco = await _context.Regioes
                    .FirstOrDefaultAsync(r => r.IdRegiao == id);

                if (regiaoBanco == null)
                    return NotFound("Região não encontrada.");

                regiaoBanco.Nome = regiao.Nome;
                regiaoBanco.Cidade = regiao.Cidade;
                regiaoBanco.Uf = regiao.Uf;
                regiaoBanco.Latitude = regiao.Latitude;
                regiaoBanco.Longitude = regiao.Longitude;
                regiaoBanco.TipoRiscoBase = regiao.TipoRiscoBase;
                regiaoBanco.PopulacaoEstimada = regiao.PopulacaoEstimada;

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
            Description = "Remove uma região existente a partir do ID informado. Caso existam registros vinculados, retorna 409 Conflict.")]
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

                var possuiSensor = await _context.SensoresIot
                    .AsNoTracking()
                    .Where(s => s.IdRegiao == id)
                    .Select(s => s.IdSensor)
                    .FirstOrDefaultAsync();

                if (possuiSensor != 0)
                    return Conflict("Não é possível remover a região, pois existem sensores vinculados a ela.");

                var possuiAbrigo = await _context.Abrigos
                    .AsNoTracking()
                    .Where(a => a.IdRegiao == id)
                    .Select(a => a.IdAbrigo)
                    .FirstOrDefaultAsync();

                if (possuiAbrigo != 0)
                    return Conflict("Não é possível remover a região, pois existem abrigos vinculados a ela.");

                var possuiHistorico = await _context.HistoricosRisco
                    .AsNoTracking()
                    .Where(h => h.IdRegiao == id)
                    .Select(h => h.IdHistorico)
                    .FirstOrDefaultAsync();

                if (possuiHistorico != 0)
                    return Conflict("Não é possível remover a região, pois existem históricos vinculados a ela.");

                var possuiAlerta = await _context.AlertasRisco
                    .AsNoTracking()
                    .Where(a => a.IdRegiao == id)
                    .Select(a => a.IdAlerta)
                    .FirstOrDefaultAsync();

                if (possuiAlerta != 0)
                    return Conflict("Não é possível remover a região, pois existem alertas vinculados a ela.");

                var possuiOcorrencia = await _context.Ocorrencias
                    .AsNoTracking()
                    .Where(o => o.IdRegiao == id)
                    .Select(o => o.IdOcorrencia)
                    .FirstOrDefaultAsync();

                if (possuiOcorrencia != 0)
                    return Conflict("Não é possível remover a região, pois existem ocorrências vinculadas a ela.");

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