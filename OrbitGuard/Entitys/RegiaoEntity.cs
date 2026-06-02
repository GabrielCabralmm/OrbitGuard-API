using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrbitGuardAPI.Entitys
{
    [Table("OG_REGIAO_MONITORADA")]
    public class RegiaoEntity
    {
        [Key]
        [Column("ID_REGIAO")]
        public long IdRegiao { get; set; }

        [Required(ErrorMessage = "O nome da região é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome da região deve conter entre 3 e 100 caracteres.")]
        [Column("NOME")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "A cidade é obrigatória.")]
        [StringLength(80, MinimumLength = 2, ErrorMessage = "A cidade deve conter entre 2 e 80 caracteres.")]
        [Column("CIDADE")]
        public string Cidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "A UF é obrigatória.")]
        [RegularExpression("^[A-Z]{2}$", ErrorMessage = "A UF deve conter exatamente 2 letras maiúsculas.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "A UF deve conter exatamente 2 caracteres.")]
        [Column("UF")]
        public string Uf { get; set; } = string.Empty;

        [Required(ErrorMessage = "A latitude é obrigatória.")]
        [Range(-90, 90, ErrorMessage = "A latitude deve estar entre -90 e 90.")]
        [Precision(10, 6)]
        [Column("LATITUDE")]
        public decimal Latitude { get; set; }

        [Required(ErrorMessage = "A longitude é obrigatória.")]
        [Range(-180, 180, ErrorMessage = "A longitude deve estar entre -180 e 180.")]
        [Precision(10, 6)]
        [Column("LONGITUDE")]
        public decimal Longitude { get; set; }

        [Required(ErrorMessage = "O tipo de risco base é obrigatório.")]
        [RegularExpression("ENCHENTE|QUEIMADA|CALOR_EXTREMO|DESLIZAMENTO", ErrorMessage = "O tipo de risco base deve ser: ENCHENTE, QUEIMADA, CALOR_EXTREMO ou DESLIZAMENTO.")]
        [StringLength(30, ErrorMessage = "O tipo de risco base deve conter no máximo 30 caracteres.")]
        [Column("TIPO_RISCO_BASE")]
        public string TipoRiscoBase { get; set; } = string.Empty;

        [Required(ErrorMessage = "A população estimada é obrigatória.")]
        [Range(0, int.MaxValue, ErrorMessage = "A população estimada não pode ser negativa.")]
        [Column("POPULACAO_ESTIMADA")]
        public int PopulacaoEstimada { get; set; }

        public ICollection<SensorEntity> Sensores { get; set; } = new List<SensorEntity>();
        public ICollection<AbrigoEntity> Abrigos { get; set; } = new List<AbrigoEntity>();
        public ICollection<HistoricoEntity> HistoricosRisco { get; set; } = new List<HistoricoEntity>();
        public ICollection<AlertaEntity> AlertasRisco { get; set; } = new List<AlertaEntity>();
        public ICollection<OcorrenciaEntity> Ocorrencias { get; set; } = new List<OcorrenciaEntity>();
    }
}