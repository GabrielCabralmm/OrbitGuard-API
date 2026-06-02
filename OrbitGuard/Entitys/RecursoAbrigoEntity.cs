using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrbitGuardAPI.Entitys
{
    [Table("OG_RECURSO_ABRIGO")]
    public class RecursoAbrigoEntity
    {
        [Key]
        [Column("ID_RECURSO")]
        public long IdRecurso { get; set; }

        [Required(ErrorMessage = "O ID do abrigo é obrigatório.")]
        [Column("ID_ABRIGO")]
        public long IdAbrigo { get; set; }

        [Required(ErrorMessage = "O tipo de recurso é obrigatório.")]
        [RegularExpression("AGUA|ALIMENTO|COLCHAO|REMEDIO|VAGA|KIT_HIGIENE", ErrorMessage = "O tipo de recurso deve ser: AGUA, ALIMENTO, COLCHAO, REMEDIO, VAGA ou KIT_HIGIENE.")]
        [StringLength(40, ErrorMessage = "O tipo de recurso deve conter no máximo 40 caracteres.")]
        [Column("TIPO_RECURSO")]
        public string TipoRecurso { get; set; } = string.Empty;

        [Required(ErrorMessage = "A quantidade é obrigatória.")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade não pode ser negativa.")]
        [Column("QUANTIDADE")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "A unidade é obrigatória.")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "A unidade deve conter entre 1 e 20 caracteres.")]
        [Column("UNIDADE")]
        public string Unidade { get; set; } = string.Empty;

        [ForeignKey(nameof(IdAbrigo))]
        public AbrigoEntity? Abrigo { get; set; }
    }
}