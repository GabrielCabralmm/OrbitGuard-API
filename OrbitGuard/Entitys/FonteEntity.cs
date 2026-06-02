using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrbitGuardAPI.Entitys
{
    [Table("OG_FONTE_ESPACIAL")]
    public class FonteEntity
    {
        [Key]
        [Column("ID_FONTE")]
        public long IdFonte { get; set; }

        [Required(ErrorMessage = "O nome da fonte espacial é obrigatório.")]
        [StringLength(80, MinimumLength = 2, ErrorMessage = "O nome deve conter entre 2 e 80 caracteres.")]
        [Column("NOME")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo de dado é obrigatório.")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "O tipo de dado deve conter entre 2 e 40 caracteres.")]
        [Column("TIPO_DADO")]
        public string TipoDado { get; set; } = string.Empty;

        [Url(ErrorMessage = "Informe uma URL válida.")]
        [StringLength(300, ErrorMessage = "A URL base deve conter no máximo 300 caracteres.")]
        [Column("URL_BASE")]
        public string? UrlBase { get; set; }

        [Column("PAYLOAD_EXEMPLO")]
        public string? PayloadExemplo { get; set; }

        [Required(ErrorMessage = "A data de coleta é obrigatória.")]
        [Column("DATA_COLETA")]
        public DateTime DataColeta { get; set; } = DateTime.Now;
    }
}