using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OrbitGuardAPI.Entitys
{
    [Table("OG_SENSOR_IOT")]
    public class SensorEntity
    {
        [Key]
        [Column("ID_SENSOR")]
        public long IdSensor { get; set; }

        [Required(ErrorMessage = "O ID da região é obrigatório.")]
        [Column("ID_REGIAO")]
        public long IdRegiao { get; set; }

        [Required(ErrorMessage = "O código do sensor é obrigatório.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "O código deve conter entre 3 e 30 caracteres.")]
        [Column("CODIGO")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo do sensor é obrigatório.")]
        [RegularExpression("NIVEL_AGUA|CHUVA|TEMPERATURA|UMIDADE|FUMACA", ErrorMessage = "O tipo do sensor deve ser: NIVEL_AGUA, CHUVA, TEMPERATURA, UMIDADE ou FUMACA.")]
        [StringLength(30, ErrorMessage = "O tipo do sensor deve conter no máximo 30 caracteres.")]
        [Column("TIPO_SENSOR")]
        public string TipoSensor { get; set; } = string.Empty;

        [Required(ErrorMessage = "O status do sensor é obrigatório.")]
        [RegularExpression("ATIVO|MANUTENCAO|INATIVO", ErrorMessage = "O status do sensor deve ser: ATIVO, MANUTENCAO ou INATIVO.")]
        [StringLength(20, ErrorMessage = "O status do sensor deve conter no máximo 20 caracteres.")]
        [Column("STATUS_SENSOR")]
        public string StatusSensor { get; set; } = "ATIVO";

        [Required(ErrorMessage = "A data de instalação é obrigatória.")]
        [Column("DATA_INSTALACAO")]
        public DateTime DataInstalacao { get; set; } = DateTime.Now;

        [ForeignKey(nameof(IdRegiao))]
        public RegiaoEntity? Regiao { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(IdRegiao))]
        public RegiaoEntity? Regiao { get; set; }

        [JsonIgnore]
        public ICollection<LeituraEntity> Leituras { get; set; } = new List<LeituraEntity>();
    }
}