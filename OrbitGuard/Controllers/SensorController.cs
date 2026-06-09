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
    public class SensorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SensorController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista todos os sensores IoT",
            Description = "Retorna todos os sensores IoT cadastrados no OrbitGuard. Caso não existam registros, retorna 404 Not Found.")]
        [ProducesResponseType(typeof(IEnumerable<SensorEntity>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<SensorEntity>>> Get()
        {
            try
            {
                var sensores = await _context.SensoresIot
                    .AsNoTracking()
                    .ToListAsync();

                if (sensores.Count == 0)
                    return NotFound("Nenhum sensor encontrado.");

                return Ok(sensores);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao buscar sensores.");
            }
        }

        [HttpGet("{id:long}")]
        [SwaggerOperation(
            Summary = "Busca um sensor IoT por ID",
            Description = "Retorna os dados de um sensor IoT específico a partir do ID informado.")]
        [ProducesResponseType(typeof(SensorEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SensorEntity>> GetById(long id)
        {
            try
            {
                var sensor = await _context.SensoresIot
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.IdSensor == id);

                if (sensor == null)
                    return NotFound("Sensor não encontrado.");

                return Ok(sensor);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao buscar sensor.");
            }
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Cadastra um novo sensor IoT",
            Description = "Cria um novo sensor IoT vinculado a uma região monitorada. Valida se a região existe e impede cadastro com código duplicado.")]
        [ProducesResponseType(typeof(SensorEntity), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SensorEntity>> Post([FromBody] SensorEntity sensor)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var regiaoExiste = await _context.Regioes
                    .AsNoTracking()
                    .Where(r => r.IdRegiao == sensor.IdRegiao)
                    .Select(r => r.IdRegiao)
                    .FirstOrDefaultAsync();

                if (regiaoExiste == 0)
                    return NotFound("Região informada não encontrada.");

                var codigoExiste = await _context.SensoresIot
                    .AsNoTracking()
                    .Where(s => s.Codigo.ToLower() == sensor.Codigo.ToLower())
                    .Select(s => s.IdSensor)
                    .FirstOrDefaultAsync();

                if (codigoExiste != 0)
                    return Conflict("Já existe um sensor cadastrado com este código.");

                sensor.Codigo = sensor.Codigo.ToUpper();
                sensor.TipoSensor = sensor.TipoSensor.ToUpper();
                sensor.StatusSensor = sensor.StatusSensor.ToUpper();

                if (sensor.DataInstalacao == default)
                    sensor.DataInstalacao = DateTime.Now;

                _context.SensoresIot.Add(sensor);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = sensor.IdSensor },
                    sensor);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao cadastrar sensor.");
            }
        }

        [HttpPut("{id:long}")]
        [SwaggerOperation(
            Summary = "Atualiza um sensor IoT",
            Description = "Atualiza completamente os dados de um sensor IoT existente. Valida ID, região vinculada e duplicidade de código.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(long id, [FromBody] SensorEntity sensor)
        {
            try
            {
                if (id != sensor.IdSensor)
                    return BadRequest("O ID informado na URL é diferente do ID enviado no corpo da requisição.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var sensorExiste = await _context.SensoresIot
                    .AsNoTracking()
                    .Where(s => s.IdSensor == id)
                    .Select(s => s.IdSensor)
                    .FirstOrDefaultAsync();

                if (sensorExiste == 0)
                    return NotFound("Sensor não encontrado.");

                var regiaoExiste = await _context.Regioes
                    .AsNoTracking()
                    .Where(r => r.IdRegiao == sensor.IdRegiao)
                    .Select(r => r.IdRegiao)
                    .FirstOrDefaultAsync();

                if (regiaoExiste == 0)
                    return NotFound("Região informada não encontrada.");

                var codigoEmUso = await _context.SensoresIot
                    .AsNoTracking()
                    .Where(s =>
                        s.Codigo.ToLower() == sensor.Codigo.ToLower()
                        && s.IdSensor != id)
                    .Select(s => s.IdSensor)
                    .FirstOrDefaultAsync();

                if (codigoEmUso != 0)
                    return Conflict("Já existe outro sensor cadastrado com este código.");

                sensor.Codigo = sensor.Codigo.ToUpper();
                sensor.TipoSensor = sensor.TipoSensor.ToUpper();
                sensor.StatusSensor = sensor.StatusSensor.ToUpper();

                _context.Entry(sensor).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao atualizar sensor.");
            }
        }

        [HttpDelete("{id:long}")]
        [SwaggerOperation(
            Summary = "Remove um sensor IoT",
            Description = "Remove um sensor IoT existente. Caso existam leituras vinculadas ao sensor, retorna 409 Conflict.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var sensor = await _context.SensoresIot
                    .FirstOrDefaultAsync(s => s.IdSensor == id);

                if (sensor == null)
                    return NotFound("Sensor não encontrado.");

                var possuiLeituras = await _context.LeiturasSensor
                    .AsNoTracking()
                    .Where(l => l.IdSensor == id)
                    .Select(l => l.IdLeitura)
                    .FirstOrDefaultAsync();

                if (possuiLeituras != 0)
                    return Conflict("Não é possível remover o sensor, pois existem leituras vinculadas a ele.");

                _context.SensoresIot.Remove(sensor);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao remover sensor.");
            }
        }
    }
}