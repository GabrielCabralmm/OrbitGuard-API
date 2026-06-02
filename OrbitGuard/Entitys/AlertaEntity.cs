using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrbitGuardAPI.Entitys
{
    [Table("OG_ALERTA_RISCO")]
    public class AlertaEntity
    {
        [Key]
        [Column("ID_ALERTA")]
        public long IdAlerta { get; set; }

        [Required(ErrorMessage = "O ID da região é obrigatório.")]
        [Column("ID_REGIAO")]
        public long IdRegiao { get; set; }

        [Column("ID_HISTORICO")]
        public long? IdHistorico { get; set; }

        [Required(ErrorMessage = "O título do alerta é obrigatório.")]
        [StringLength(120, MinimumLength = 3, ErrorMessage = "O título deve conter entre 3 e 120 caracteres.")]
        [Column("TITULO")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A mensagem do alerta é obrigatória.")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "A mensagem deve conter entre 5 e 500 caracteres.")]
        [Column("MENSAGEM")]
        public string Mensagem { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nível de risco é obrigatório.")]
        [RegularExpression("ATENCAO|ALTO|CRITICO", ErrorMessage = "O nível de risco deve ser: ATENCAO, ALTO ou CRITICO.")]
        [StringLength(20, ErrorMessage = "O nível de risco deve conter no máximo 20 caracteres.")]
        [Column("NIVEL_RISCO")]
        public string NivelRisco { get; set; } = string.Empty;

        [Required(ErrorMessage = "O status do alerta é obrigatório.")]
        [RegularExpression("ABERTO|EM_ATENDIMENTO|RESOLVIDO|CANCELADO", ErrorMessage = "O status do alerta deve ser: ABERTO, EM_ATENDIMENTO, RESOLVIDO ou CANCELADO.")]
        [StringLength(20, ErrorMessage = "O status do alerta deve conter no máximo 20 caracteres.")]
        [Column("STATUS_ALERTA")]
        public string StatusAlerta { get; set; } = "ABERTO";

        [Required(ErrorMessage = "A data do alerta é obrigatória.")]
        [Column("DATA_ALERTA")]
        public DateTime DataAlerta { get; set; } = DateTime.Now;

        [ForeignKey(nameof(IdRegiao))]
        public RegiaoEntity? Regiao { get; set; }

        [ForeignKey(nameof(IdHistorico))]
        public HistoricoEntity? Historico { get; set; }

        public ICollection<OcorrenciaEntity> Ocorrencias { get; set; } = new List<OcorrenciaEntity>();
        public ICollection<AuditoriaAlertaEntity> Auditorias { get; set; } = new List<AuditoriaAlertaEntity>();
    }
}