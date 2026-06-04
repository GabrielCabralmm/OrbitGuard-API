using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OrbitGuardAPI.Entitys
{
    [Table("OG_ABRIGO")]
    public class AbrigoEntity
    {
        [Key]
        [Column("ID_ABRIGO")]
        public long IdAbrigo { get; set; }

        [Required(ErrorMessage = "O ID da região é obrigatório.")]
        [Column("ID_REGIAO")]
        public long IdRegiao { get; set; }

        [Required(ErrorMessage = "O nome do abrigo é obrigatório.")]
        [StringLength(120, MinimumLength = 3, ErrorMessage = "O nome do abrigo deve conter entre 3 e 120 caracteres.")]
        [Column("NOME")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O endereço do abrigo é obrigatório.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "O endereço deve conter entre 5 e 200 caracteres.")]
        [Column("ENDERECO")]
        public string Endereco { get; set; } = string.Empty;

        [Required(ErrorMessage = "A capacidade total é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "A capacidade total deve ser maior que zero.")]
        [Column("CAPACIDADE_TOTAL")]
        public int CapacidadeTotal { get; set; }

        [Required(ErrorMessage = "A capacidade ocupada é obrigatória.")]
        [Range(0, int.MaxValue, ErrorMessage = "A capacidade ocupada não pode ser negativa.")]
        [Column("CAPACIDADE_OCUPADA")]
        public int CapacidadeOcupada { get; set; }

        [Required(ErrorMessage = "O campo ativo é obrigatório.")]
        [RegularExpression("S|N", ErrorMessage = "O campo ativo deve ser S ou N.")]
        [StringLength(1, ErrorMessage = "O campo ativo deve conter apenas 1 caractere.")]
        [Column("ATIVO")]
        public string Ativo { get; set; } = "S";

        [JsonIgnore]
        [ForeignKey(nameof(IdRegiao))]
        public RegiaoEntity? Regiao { get; set; }

        [JsonIgnore]
        public ICollection<RecursoAbrigoEntity> Recursos { get; set; } = new List<RecursoAbrigoEntity>();
    }
}