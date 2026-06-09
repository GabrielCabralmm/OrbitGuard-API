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
    public class OcorrenciaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OcorrenciaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista todas as ocorrências",
            Description = "Retorna todas as ocorrências registradas no sistema OrbitGuard.")]
        [ProducesResponseType(typeof(IEnumerable<OcorrenciaEntity>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<OcorrenciaEntity>>> Get()
        {
            try
            {
                var ocorrencias = await _context.Ocorrencias
                    .AsNoTracking()
                    .ToListAsync();

                if (ocorrencias.Count == 0)
                    return NotFound("Nenhuma ocorrência encontrada.");

                return Ok(ocorrencias);
            }
            catch
            {
                return StatusCode(500, "Erro interno ao buscar ocorrências.");
            }
        }

        [HttpGet("{id:long}")]
        [SwaggerOperation(
            Summary = "Busca uma ocorrência por ID",
            Description = "Retorna os dados de uma ocorrência específica.")]
        [ProducesResponseType(typeof(OcorrenciaEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OcorrenciaEntity>> GetById(long id)
        {
            try
            {
                var ocorrencia = await _context.Ocorrencias
                    .AsNoTracking()
                    .FirstOrDefaultAsync(o => o.IdOcorrencia == id);

                if (ocorrencia == null)
                    return NotFound("Ocorrência não encontrada.");

                return Ok(ocorrencia);
            }
            catch
            {
                return StatusCode(500, "Erro interno ao buscar ocorrência.");
            }
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Cadastra uma nova ocorrência",
            Description = "Cria uma nova ocorrência vinculada a um usuário e uma região. O alerta é opcional.")]
        [ProducesResponseType(typeof(OcorrenciaEntity), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OcorrenciaEntity>> Post([FromBody] OcorrenciaEntity ocorrencia)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var usuarioExiste = await _context.Usuarios
                    .AsNoTracking()
                    .Where(u => u.IdUsuario == ocorrencia.IdUsuario)
                    .Select(u => u.IdUsuario)
                    .FirstOrDefaultAsync();

                if (usuarioExiste == 0)
                    return NotFound("Usuário informado não encontrado.");

                var regiaoExiste = await _context.Regioes
                    .AsNoTracking()
                    .Where(r => r.IdRegiao == ocorrencia.IdRegiao)
                    .Select(r => r.IdRegiao)
                    .FirstOrDefaultAsync();

                if (regiaoExiste == 0)
                    return NotFound("Região informada não encontrada.");

                if (ocorrencia.IdAlerta.HasValue)
                {
                    var alertaExiste = await _context.AlertasRisco
                        .AsNoTracking()
                        .Where(a => a.IdAlerta == ocorrencia.IdAlerta)
                        .Select(a => a.IdAlerta)
                        .FirstOrDefaultAsync();

                    if (alertaExiste == 0)
                        return NotFound("Alerta informado não encontrado.");
                }

                ocorrencia.TipoOcorrencia = ocorrencia.TipoOcorrencia.ToUpper();
                ocorrencia.StatusOcorrencia = ocorrencia.StatusOcorrencia.ToUpper();

                if (ocorrencia.DataOcorrencia == default)
                    ocorrencia.DataOcorrencia = DateTime.Now;

                _context.Ocorrencias.Add(ocorrencia);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = ocorrencia.IdOcorrencia },
                    ocorrencia);
            }
            catch
            {
                return StatusCode(500, "Erro interno ao cadastrar ocorrência.");
            }
        }

        [HttpPut("{id:long}")]
        [SwaggerOperation(
            Summary = "Atualiza uma ocorrência",
            Description = "Atualiza completamente uma ocorrência existente.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(long id, [FromBody] OcorrenciaEntity ocorrencia)
        {
            try
            {
                if (id != ocorrencia.IdOcorrencia)
                    return BadRequest("O ID informado na URL é diferente do ID enviado no corpo da requisição.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var ocorrenciaExiste = await _context.Ocorrencias
                    .AsNoTracking()
                    .Where(o => o.IdOcorrencia == id)
                    .Select(o => o.IdOcorrencia)
                    .FirstOrDefaultAsync();

                if (ocorrenciaExiste == 0)
                    return NotFound("Ocorrência não encontrada.");

                var usuarioExiste = await _context.Usuarios
                    .AsNoTracking()
                    .Where(u => u.IdUsuario == ocorrencia.IdUsuario)
                    .Select(u => u.IdUsuario)
                    .FirstOrDefaultAsync();

                if (usuarioExiste == 0)
                    return NotFound("Usuário informado não encontrado.");

                var regiaoExiste = await _context.Regioes
                    .AsNoTracking()
                    .Where(r => r.IdRegiao == ocorrencia.IdRegiao)
                    .Select(r => r.IdRegiao)
                    .FirstOrDefaultAsync();

                if (regiaoExiste == 0)
                    return NotFound("Região informada não encontrada.");

                if (ocorrencia.IdAlerta.HasValue)
                {
                    var alertaExiste = await _context.AlertasRisco
                        .AsNoTracking()
                        .Where(a => a.IdAlerta == ocorrencia.IdAlerta)
                        .Select(a => a.IdAlerta)
                        .FirstOrDefaultAsync();

                    if (alertaExiste == 0)
                        return NotFound("Alerta informado não encontrado.");
                }

                ocorrencia.TipoOcorrencia = ocorrencia.TipoOcorrencia.ToUpper();
                ocorrencia.StatusOcorrencia = ocorrencia.StatusOcorrencia.ToUpper();

                _context.Entry(ocorrencia).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(500, "Erro interno ao atualizar ocorrência.");
            }
        }

        [HttpDelete("{id:long}")]
        [SwaggerOperation(
            Summary = "Remove uma ocorrência",
            Description = "Remove uma ocorrência existente do sistema.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var ocorrencia = await _context.Ocorrencias
                    .FirstOrDefaultAsync(o => o.IdOcorrencia == id);

                if (ocorrencia == null)
                    return NotFound("Ocorrência não encontrada.");

                _context.Ocorrencias.Remove(ocorrencia);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(500, "Erro interno ao remover ocorrência.");
            }
        }
    }
}