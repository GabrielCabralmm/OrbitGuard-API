using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OrbitGuardAPI.Entitys
{
    [Table("OG_LEITURA_SENSOR")]
    public class LeituraEntity
    {
        [Key]
        [Column("ID_LEITURA")]
        public long IdLeitura { get; set; }

        [Required(ErrorMessage = "O ID do sensor é obrigatório.")]
        [Column("ID_SENSOR")]
        public long IdSensor { get; set; }

        [Required(ErrorMessage = "O valor da leitura é obrigatório.")]
        [Precision(10, 2)]
        [Column("VALOR")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "A unidade da leitura é obrigatória.")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "A unidade deve conter entre 1 e 20 caracteres.")]
        [Column("UNIDADE")]
        public string Unidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data da leitura é obrigatória.")]
        [Column("DATA_LEITURA")]
        public DateTime DataLeitura { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "A origem da leitura é obrigatória.")]
        [RegularExpression("IOT|NASA_POWER|NASA_FIRMS|MANUAL", ErrorMessage = "A origem deve ser: IOT, NASA_POWER, NASA_FIRMS ou MANUAL.")]
        [StringLength(20, ErrorMessage = "A origem deve conter no máximo 20 caracteres.")]
        [Column("ORIGEM")]
        public string Origem { get; set; } = "IOT";

        [JsonIgnore]
        [ForeignKey(nameof(IdSensor))]
        public SensorEntity? Sensor { get; set; }
    }
}