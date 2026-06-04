using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OrbitGuardAPI.Entitys
{
    [Table("OG_USUARIO")]
    public class UsuarioEntity
    {
        [Key]
        [Column("ID_USUARIO")]
        public long IdUsuario { get; set; }

        [Required(ErrorMessage = "O nome do usuário é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve conter entre 3 e 100 caracteres.")]
        [Column("NOME")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail do usuário é obrigatório.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        [StringLength(120, ErrorMessage = "O e-mail deve conter no máximo 120 caracteres.")]
        [Column("EMAIL")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O perfil do usuário é obrigatório.")]
        [RegularExpression("CIDADAO|GESTOR|OPERADOR|ADMIN", ErrorMessage = "O perfil deve ser: CIDADAO, GESTOR, OPERADOR ou ADMIN.")]
        [StringLength(30, ErrorMessage = "O perfil deve conter no máximo 30 caracteres.")]
        [Column("PERFIL")]
        public string Perfil { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Informe um telefone válido.")]
        [StringLength(20, ErrorMessage = "O telefone deve conter no máximo 20 caracteres.")]
        [Column("TELEFONE")]
        public string? Telefone { get; set; }

        [Required(ErrorMessage = "A data de cadastro é obrigatória.")]
        [Column("DATA_CADASTRO")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "O campo ativo é obrigatório.")]
        [RegularExpression("S|N", ErrorMessage = "O campo ativo deve ser S ou N.")]
        [StringLength(1, ErrorMessage = "O campo ativo deve conter apenas 1 caractere.")]
        [Column("ATIVO")]
        public string Ativo { get; set; } = "S";

        [JsonIgnore]
        public ICollection<OcorrenciaEntity> Ocorrencias { get; set; } = new List<OcorrenciaEntity>();
    }
}