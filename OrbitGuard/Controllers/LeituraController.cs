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
    public class LeituraController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LeituraController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista todas as leituras dos sensores",
            Description = "Retorna todas as leituras registradas pelos sensores IoT. Caso não existam registros, retorna 404 Not Found.")]
        [ProducesResponseType(typeof(IEnumerable<LeituraEntity>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<LeituraEntity>>> Get()
        {
            try
            {
                var leituras = await _context.LeiturasSensor
                    .AsNoTracking()
                    .ToListAsync();

                if (!leituras.Any())
                    return NotFound("Nenhuma leitura encontrada.");

                return Ok(leituras);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao buscar leituras.");
            }
        }

        [HttpGet("{id:long}")]
        [SwaggerOperation(
            Summary = "Busca uma leitura por ID",
            Description = "Retorna os dados de uma leitura específica a partir do ID informado.")]
        [ProducesResponseType(typeof(LeituraEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LeituraEntity>> GetById(long id)
        {
            try
            {
                var leitura = await _context.LeiturasSensor
                    .AsNoTracking()
                    .FirstOrDefaultAsync(l => l.IdLeitura == id);

                if (leitura == null)
                    return NotFound("Leitura não encontrada.");

                return Ok(leitura);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao buscar leitura.");
            }
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Cadastra uma nova leitura",
            Description = "Registra uma nova leitura para um sensor IoT existente.")]
        [ProducesResponseType(typeof(LeituraEntity), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LeituraEntity>> Post([FromBody] LeituraEntity leitura)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var sensorExiste = await _context.SensoresIot
                    .AnyAsync(s => s.IdSensor == leitura.IdSensor);

                if (!sensorExiste)
                    return NotFound("Sensor informado não encontrado.");

                leitura.Origem = leitura.Origem.ToUpper();

                if (leitura.DataLeitura == default)
                    leitura.DataLeitura = DateTime.Now;

                _context.LeiturasSensor.Add(leitura);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = leitura.IdLeitura },
                    leitura);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao cadastrar leitura.");
            }
        }

        [HttpPut("{id:long}")]
        [SwaggerOperation(
            Summary = "Atualiza uma leitura",
            Description = "Atualiza completamente uma leitura existente. O ID da URL deve ser igual ao ID enviado no corpo da requisição.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(long id, [FromBody] LeituraEntity leitura)
        {
            try
            {
                if (id != leitura.IdLeitura)
                    return BadRequest("O ID informado na URL é diferente do ID enviado no corpo da requisição.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var leituraExiste = await _context.LeiturasSensor
                    .AnyAsync(l => l.IdLeitura == id);

                if (!leituraExiste)
                    return NotFound("Leitura não encontrada.");

                var sensorExiste = await _context.SensoresIot
                    .AnyAsync(s => s.IdSensor == leitura.IdSensor);

                if (!sensorExiste)
                    return NotFound("Sensor informado não encontrado.");

                leitura.Origem = leitura.Origem.ToUpper();

                _context.Entry(leitura).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao atualizar leitura.");
            }
        }

        [HttpDelete("{id:long}")]
        [SwaggerOperation(
            Summary = "Remove uma leitura",
            Description = "Remove uma leitura existente a partir do ID informado.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var leitura = await _context.LeiturasSensor
                    .FirstOrDefaultAsync(l => l.IdLeitura == id);

                if (leitura == null)
                    return NotFound("Leitura não encontrada.");

                _context.LeiturasSensor.Remove(leitura);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao remover leitura.");
            }
        }
    }
}