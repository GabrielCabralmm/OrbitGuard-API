using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrbitGuardAPI.Entitys
{
    [Table("OG_AUDITORIA_ALERTA")]
    public class AuditoriaAlertaEntity
    {
        [Key]
        [Column("ID_AUDITORIA")]
        public long IdAuditoria { get; set; }

        [Required(ErrorMessage = "O ID do alerta é obrigatório.")]
        [Column("ID_ALERTA")]
        public long IdAlerta { get; set; }

        [Required(ErrorMessage = "A ação da auditoria é obrigatória.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "A ação deve conter entre 3 e 30 caracteres.")]
        [Column("ACAO")]
        public string Acao { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "O status anterior deve conter no máximo 20 caracteres.")]
        [Column("STATUS_ANTERIOR")]
        public string? StatusAnterior { get; set; }

        [StringLength(20, ErrorMessage = "O status novo deve conter no máximo 20 caracteres.")]
        [Column("STATUS_NOVO")]
        public string? StatusNovo { get; set; }

        [Required(ErrorMessage = "A data da auditoria é obrigatória.")]
        [Column("DATA_AUDITORIA")]
        public DateTime DataAuditoria { get; set; } = DateTime.Now;

        [ForeignKey(nameof(IdAlerta))]
        public AlertaEntity? Alerta { get; set; }
    }
}