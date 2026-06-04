using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OrbitGuardAPI.Entitys
{
    [Table("OG_OCORRENCIA")]
    public class OcorrenciaEntity
    {
        [Key]
        [Column("ID_OCORRENCIA")]
        public long IdOcorrencia { get; set; }

        [Required(ErrorMessage = "O ID do usuário é obrigatório.")]
        [Column("ID_USUARIO")]
        public long IdUsuario { get; set; }

        [Required(ErrorMessage = "O ID da região é obrigatório.")]
        [Column("ID_REGIAO")]
        public long IdRegiao { get; set; }

        [Column("ID_ALERTA")]
        public long? IdAlerta { get; set; }

        [Required(ErrorMessage = "O tipo da ocorrência é obrigatório.")]
        [RegularExpression("ENCHENTE|QUEIMADA|CALOR_EXTREMO|DESLIZAMENTO", ErrorMessage = "O tipo da ocorrência deve ser: ENCHENTE, QUEIMADA, CALOR_EXTREMO ou DESLIZAMENTO.")]
        [StringLength(30, ErrorMessage = "O tipo da ocorrência deve conter no máximo 30 caracteres.")]
        [Column("TIPO_OCORRENCIA")]
        public string TipoOcorrencia { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição da ocorrência é obrigatória.")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "A descrição deve conter entre 5 e 500 caracteres.")]
        [Column("DESCRICAO")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O status da ocorrência é obrigatório.")]
        [RegularExpression("ABERTA|EM_ANALISE|ATENDIDA|CANCELADA", ErrorMessage = "O status da ocorrência deve ser: ABERTA, EM_ANALISE, ATENDIDA ou CANCELADA.")]
        [StringLength(20, ErrorMessage = "O status da ocorrência deve conter no máximo 20 caracteres.")]
        [Column("STATUS_OCORRENCIA")]
        public string StatusOcorrencia { get; set; } = "ABERTA";

        [Required(ErrorMessage = "A data da ocorrência é obrigatória.")]
        [Column("DATA_OCORRENCIA")]
        public DateTime DataOcorrencia { get; set; } = DateTime.Now;

        [JsonIgnore]
        [ForeignKey(nameof(IdUsuario))]
        public UsuarioEntity? Usuario { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(IdRegiao))]
        public RegiaoEntity? Regiao { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(IdAlerta))]
        public AlertaEntity? Alerta { get; set; }
    }
}