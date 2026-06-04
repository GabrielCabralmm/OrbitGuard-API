using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OrbitGuardAPI.Entitys
{
    [Table("OG_HISTORICO_RISCO")]
    public class HistoricoEntity
    {
        [Key]
        [Column("ID_HISTORICO")]
        public long IdHistorico { get; set; }

        [Required(ErrorMessage = "O ID da região é obrigatório.")]
        [Column("ID_REGIAO")]
        public long IdRegiao { get; set; }

        [Required(ErrorMessage = "O índice de risco é obrigatório.")]
        [Range(0, 100, ErrorMessage = "O índice de risco deve estar entre 0 e 100.")]
        [Precision(5, 2)]
        [Column("INDICE_RISCO")]
        public decimal IndiceRisco { get; set; }

        [Required(ErrorMessage = "O nível de risco é obrigatório.")]
        [RegularExpression("NORMAL|ATENCAO|ALTO|CRITICO", ErrorMessage = "O nível de risco deve ser: NORMAL, ATENCAO, ALTO ou CRITICO.")]
        [StringLength(20, ErrorMessage = "O nível de risco deve conter no máximo 20 caracteres.")]
        [Column("NIVEL_RISCO")]
        public string NivelRisco { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "O motivo deve conter no máximo 300 caracteres.")]
        [Column("MOTIVO")]
        public string? Motivo { get; set; }

        [Required(ErrorMessage = "A data de cálculo é obrigatória.")]
        [Column("DATA_CALCULO")]
        public DateTime DataCalculo { get; set; } = DateTime.Now;

        [JsonIgnore]
        [ForeignKey(nameof(IdRegiao))]
        public RegiaoEntity? Regiao { get; set; }

        [JsonIgnore]
        public ICollection<AlertaEntity> Alertas { get; set; } = new List<AlertaEntity>();
    }
}