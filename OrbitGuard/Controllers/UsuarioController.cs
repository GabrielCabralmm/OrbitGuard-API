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
    public class UsuarioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista todos os usuários",
            Description = "Retorna todos os usuários cadastrados no sistema OrbitGuard. Caso não exista nenhum usuário cadastrado, retorna 404 Not Found.")]
        [ProducesResponseType(typeof(IEnumerable<UsuarioEntity>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UsuarioEntity>>> Get()
        {
            try
            {
                var usuarios = await _context.Usuarios
                    .AsNoTracking()
                    .ToListAsync();

                if (usuarios.Count == 0)
                    return NotFound("Nenhum usuário encontrado.");

                return Ok(usuarios);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao buscar usuários.");
            }
        }

        [HttpGet("{id:long}")]
        [SwaggerOperation(
            Summary = "Busca um usuário por ID",
            Description = "Retorna os dados de um usuário específico a partir do ID informado. Caso o ID não exista, retorna 404 Not Found.")]
        [ProducesResponseType(typeof(UsuarioEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UsuarioEntity>> GetById(long id)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.IdUsuario == id);

                if (usuario == null)
                    return NotFound("Usuário não encontrado.");

                return Ok(usuario);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao buscar usuário.");
            }
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Cadastra um novo usuário",
            Description = "Cria um novo usuário no sistema OrbitGuard. Valida os dados recebidos, impede cadastro com e-mail duplicado e retorna 201 Created em caso de sucesso.")]
        [ProducesResponseType(typeof(UsuarioEntity), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UsuarioEntity>> Post([FromBody] UsuarioEntity usuario)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                usuario.Nome = usuario.Nome.Trim();
                usuario.Email = usuario.Email.Trim().ToLower();
                usuario.Perfil = usuario.Perfil.Trim().ToUpper();
                usuario.Ativo = usuario.Ativo.Trim().ToUpper();

                if (!string.IsNullOrWhiteSpace(usuario.Telefone))
                    usuario.Telefone = usuario.Telefone.Trim();

                var emailExiste = await _context.Usuarios
                    .AsNoTracking()
                    .Where(u => u.Email.ToLower() == usuario.Email)
                    .Select(u => u.IdUsuario)
                    .FirstOrDefaultAsync();

                if (emailExiste != 0)
                    return Conflict("Já existe um usuário cadastrado com este e-mail.");

                if (usuario.DataCadastro == default)
                    usuario.DataCadastro = DateTime.Now;

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = usuario.IdUsuario },
                    usuario);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao cadastrar usuário.");
            }
        }

        [HttpPut("{id:long}")]
        [SwaggerOperation(
            Summary = "Atualiza um usuário",
            Description = "Atualiza completamente os dados de um usuário existente. O ID informado na URL deve ser igual ao ID enviado no corpo da requisição.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(long id, [FromBody] UsuarioEntity usuario)
        {
            try
            {
                if (id != usuario.IdUsuario)
                    return BadRequest("O ID informado na URL é diferente do ID enviado no corpo da requisição.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                usuario.Nome = usuario.Nome.Trim();
                usuario.Email = usuario.Email.Trim().ToLower();
                usuario.Perfil = usuario.Perfil.Trim().ToUpper();
                usuario.Ativo = usuario.Ativo.Trim().ToUpper();

                if (!string.IsNullOrWhiteSpace(usuario.Telefone))
                    usuario.Telefone = usuario.Telefone.Trim();

                var emailEmUso = await _context.Usuarios
                    .AsNoTracking()
                    .Where(u => u.Email.ToLower() == usuario.Email && u.IdUsuario != id)
                    .Select(u => u.IdUsuario)
                    .FirstOrDefaultAsync();

                if (emailEmUso != 0)
                    return Conflict("Já existe outro usuário cadastrado com este e-mail.");

                var usuarioBanco = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.IdUsuario == id);

                if (usuarioBanco == null)
                    return NotFound("Usuário não encontrado.");

                usuarioBanco.Nome = usuario.Nome;
                usuarioBanco.Email = usuario.Email;
                usuarioBanco.Perfil = usuario.Perfil;
                usuarioBanco.Telefone = usuario.Telefone;
                usuarioBanco.Ativo = usuario.Ativo;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao atualizar usuário.");
            }
        }

        [HttpDelete("{id:long}")]
        [SwaggerOperation(
            Summary = "Remove um usuário",
            Description = "Remove um usuário existente do sistema OrbitGuard a partir do ID informado. Caso o usuário não exista, retorna 404 Not Found.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.IdUsuario == id);

                if (usuario == null)
                    return NotFound("Usuário não encontrado.");

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao remover usuário.");
            }
        }
    }
}