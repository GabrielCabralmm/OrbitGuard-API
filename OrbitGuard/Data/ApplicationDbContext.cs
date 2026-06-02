using Microsoft.EntityFrameworkCore;
using OrbitGuardAPI.Entitys;

namespace OrbitGuardAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UsuarioEntity> Usuarios { get; set; }
        public DbSet<RegiaoEntity> Regioes { get; set; }
        public DbSet<FonteEntity> FontesEspaciais { get; set; }
        public DbSet<SensorEntity> SensoresIot { get; set; }
        public DbSet<LeituraEntity> LeiturasSensor { get; set; }
        public DbSet<AbrigoEntity> Abrigos { get; set; }
        public DbSet<RecursoAbrigoEntity> RecursosAbrigo { get; set; }
        public DbSet<HistoricoEntity> HistoricosRisco { get; set; }
        public DbSet<AlertaEntity> AlertasRisco { get; set; }
        public DbSet<OcorrenciaEntity> Ocorrencias { get; set; }
        public DbSet<AuditoriaAlertaEntity> AuditoriasAlerta { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UsuarioEntity>(entity =>
            {
                entity.ToTable("OG_USUARIO");

                entity.HasKey(e => e.IdUsuario);

                entity.Property(e => e.IdUsuario)
                    .HasColumnName("ID_USUARIO")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Nome)
                    .HasColumnName("NOME")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Email)
                    .HasColumnName("EMAIL")
                    .HasMaxLength(120)
                    .IsRequired();

                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasDatabaseName("UK_OG_USUARIO_EMAIL");

                entity.Property(e => e.Perfil)
                    .HasColumnName("PERFIL")
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(e => e.Telefone)
                    .HasColumnName("TELEFONE")
                    .HasMaxLength(20);

                entity.Property(e => e.DataCadastro)
                    .HasColumnName("DATA_CADASTRO")
                    .HasDefaultValueSql("SYSDATE")
                    .IsRequired();

                entity.Property(e => e.Ativo)
                    .HasColumnName("ATIVO")
                    .HasMaxLength(1)
                    .HasDefaultValue("S")
                    .IsRequired();
            });

            modelBuilder.Entity<RegiaoEntity>(entity =>
            {
                entity.ToTable("OG_REGIAO_MONITORADA");

                entity.HasKey(e => e.IdRegiao);

                entity.Property(e => e.IdRegiao)
                    .HasColumnName("ID_REGIAO")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Nome)
                    .HasColumnName("NOME")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Cidade)
                    .HasColumnName("CIDADE")
                    .HasMaxLength(80)
                    .IsRequired();

                entity.Property(e => e.Uf)
                    .HasColumnName("UF")
                    .HasMaxLength(2)
                    .IsRequired();

                entity.Property(e => e.Latitude)
                    .HasColumnName("LATITUDE")
                    .HasPrecision(10, 6)
                    .IsRequired();

                entity.Property(e => e.Longitude)
                    .HasColumnName("LONGITUDE")
                    .HasPrecision(10, 6)
                    .IsRequired();

                entity.Property(e => e.TipoRiscoBase)
                    .HasColumnName("TIPO_RISCO_BASE")
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(e => e.PopulacaoEstimada)
                    .HasColumnName("POPULACAO_ESTIMADA")
                    .HasDefaultValue(0)
                    .IsRequired();
            });

            modelBuilder.Entity<FonteEntity>(entity =>
            {
                entity.ToTable("OG_FONTE_ESPACIAL");

                entity.HasKey(e => e.IdFonte);

                entity.Property(e => e.IdFonte)
                    .HasColumnName("ID_FONTE")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Nome)
                    .HasColumnName("NOME")
                    .HasMaxLength(80)
                    .IsRequired();

                entity.Property(e => e.TipoDado)
                    .HasColumnName("TIPO_DADO")
                    .HasMaxLength(40)
                    .IsRequired();

                entity.Property(e => e.UrlBase)
                    .HasColumnName("URL_BASE")
                    .HasMaxLength(300);

                entity.Property(e => e.PayloadExemplo)
                    .HasColumnName("PAYLOAD_EXEMPLO")
                    .HasColumnType("CLOB");

                entity.Property(e => e.DataColeta)
                    .HasColumnName("DATA_COLETA")
                    .HasDefaultValueSql("SYSDATE")
                    .IsRequired();
            });

            modelBuilder.Entity<SensorEntity>(entity =>
            {
                entity.ToTable("OG_SENSOR_IOT");

                entity.HasKey(e => e.IdSensor);

                entity.Property(e => e.IdSensor)
                    .HasColumnName("ID_SENSOR")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.IdRegiao)
                    .HasColumnName("ID_REGIAO")
                    .IsRequired();

                entity.Property(e => e.Codigo)
                    .HasColumnName("CODIGO")
                    .HasMaxLength(30)
                    .IsRequired();

                entity.HasIndex(e => e.Codigo)
                    .IsUnique()
                    .HasDatabaseName("UK_OG_SENSOR_CODIGO");

                entity.HasIndex(e => e.IdRegiao)
                    .HasDatabaseName("IX_OG_SENSOR_REGIAO");

                entity.Property(e => e.TipoSensor)
                    .HasColumnName("TIPO_SENSOR")
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(e => e.StatusSensor)
                    .HasColumnName("STATUS_SENSOR")
                    .HasMaxLength(20)
                    .HasDefaultValue("ATIVO")
                    .IsRequired();

                entity.Property(e => e.DataInstalacao)
                    .HasColumnName("DATA_INSTALACAO")
                    .HasDefaultValueSql("SYSDATE")
                    .IsRequired();

                entity.HasOne(e => e.Regiao)
                    .WithMany(e => e.Sensores)
                    .HasForeignKey(e => e.IdRegiao)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<LeituraEntity>(entity =>
            {
                entity.ToTable("OG_LEITURA_SENSOR");

                entity.HasKey(e => e.IdLeitura);

                entity.Property(e => e.IdLeitura)
                    .HasColumnName("ID_LEITURA")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.IdSensor)
                    .HasColumnName("ID_SENSOR")
                    .IsRequired();

                entity.Property(e => e.Valor)
                    .HasColumnName("VALOR")
                    .HasPrecision(10, 2)
                    .IsRequired();

                entity.Property(e => e.Unidade)
                    .HasColumnName("UNIDADE")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(e => e.DataLeitura)
                    .HasColumnName("DATA_LEITURA")
                    .HasDefaultValueSql("SYSDATE")
                    .IsRequired();

                entity.Property(e => e.Origem)
                    .HasColumnName("ORIGEM")
                    .HasMaxLength(20)
                    .HasDefaultValue("IOT")
                    .IsRequired();

                entity.HasIndex(e => new { e.IdSensor, e.DataLeitura })
                    .HasDatabaseName("IX_OG_LEITURA_SENSOR_DATA");

                entity.HasOne(e => e.Sensor)
                    .WithMany(e => e.Leituras)
                    .HasForeignKey(e => e.IdSensor)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AbrigoEntity>(entity =>
            {
                entity.ToTable("OG_ABRIGO");

                entity.HasKey(e => e.IdAbrigo);

                entity.Property(e => e.IdAbrigo)
                    .HasColumnName("ID_ABRIGO")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.IdRegiao)
                    .HasColumnName("ID_REGIAO")
                    .IsRequired();

                entity.Property(e => e.Nome)
                    .HasColumnName("NOME")
                    .HasMaxLength(120)
                    .IsRequired();

                entity.Property(e => e.Endereco)
                    .HasColumnName("ENDERECO")
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.CapacidadeTotal)
                    .HasColumnName("CAPACIDADE_TOTAL")
                    .IsRequired();

                entity.Property(e => e.CapacidadeOcupada)
                    .HasColumnName("CAPACIDADE_OCUPADA")
                    .HasDefaultValue(0)
                    .IsRequired();

                entity.Property(e => e.Ativo)
                    .HasColumnName("ATIVO")
                    .HasMaxLength(1)
                    .HasDefaultValue("S")
                    .IsRequired();

                entity.HasOne(e => e.Regiao)
                    .WithMany(e => e.Abrigos)
                    .HasForeignKey(e => e.IdRegiao)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RecursoAbrigoEntity>(entity =>
            {
                entity.ToTable("OG_RECURSO_ABRIGO");

                entity.HasKey(e => e.IdRecurso);

                entity.Property(e => e.IdRecurso)
                    .HasColumnName("ID_RECURSO")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.IdAbrigo)
                    .HasColumnName("ID_ABRIGO")
                    .IsRequired();

                entity.Property(e => e.TipoRecurso)
                    .HasColumnName("TIPO_RECURSO")
                    .HasMaxLength(40)
                    .IsRequired();

                entity.Property(e => e.Quantidade)
                    .HasColumnName("QUANTIDADE")
                    .IsRequired();

                entity.Property(e => e.Unidade)
                    .HasColumnName("UNIDADE")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.HasOne(e => e.Abrigo)
                    .WithMany(e => e.Recursos)
                    .HasForeignKey(e => e.IdAbrigo)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<HistoricoEntity>(entity =>
            {
                entity.ToTable("OG_HISTORICO_RISCO");

                entity.HasKey(e => e.IdHistorico);

                entity.Property(e => e.IdHistorico)
                    .HasColumnName("ID_HISTORICO")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.IdRegiao)
                    .HasColumnName("ID_REGIAO")
                    .IsRequired();

                entity.Property(e => e.IndiceRisco)
                    .HasColumnName("INDICE_RISCO")
                    .HasPrecision(5, 2)
                    .IsRequired();

                entity.Property(e => e.NivelRisco)
                    .HasColumnName("NIVEL_RISCO")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(e => e.Motivo)
                    .HasColumnName("MOTIVO")
                    .HasMaxLength(300);

                entity.Property(e => e.DataCalculo)
                    .HasColumnName("DATA_CALCULO")
                    .HasDefaultValueSql("SYSDATE")
                    .IsRequired();

                entity.HasOne(e => e.Regiao)
                    .WithMany(e => e.HistoricosRisco)
                    .HasForeignKey(e => e.IdRegiao)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AlertaEntity>(entity =>
            {
                entity.ToTable("OG_ALERTA_RISCO");

                entity.HasKey(e => e.IdAlerta);

                entity.Property(e => e.IdAlerta)
                    .HasColumnName("ID_ALERTA")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.IdRegiao)
                    .HasColumnName("ID_REGIAO")
                    .IsRequired();

                entity.Property(e => e.IdHistorico)
                    .HasColumnName("ID_HISTORICO");

                entity.Property(e => e.Titulo)
                    .HasColumnName("TITULO")
                    .HasMaxLength(120)
                    .IsRequired();

                entity.Property(e => e.Mensagem)
                    .HasColumnName("MENSAGEM")
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(e => e.NivelRisco)
                    .HasColumnName("NIVEL_RISCO")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(e => e.StatusAlerta)
                    .HasColumnName("STATUS_ALERTA")
                    .HasMaxLength(20)
                    .HasDefaultValue("ABERTO")
                    .IsRequired();

                entity.Property(e => e.DataAlerta)
                    .HasColumnName("DATA_ALERTA")
                    .HasDefaultValueSql("SYSDATE")
                    .IsRequired();

                entity.HasIndex(e => new { e.IdRegiao, e.StatusAlerta })
                    .HasDatabaseName("IX_OG_ALERTA_REGIAO_STATUS");

                entity.HasOne(e => e.Regiao)
                    .WithMany(e => e.AlertasRisco)
                    .HasForeignKey(e => e.IdRegiao)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Historico)
                    .WithMany(e => e.Alertas)
                    .HasForeignKey(e => e.IdHistorico)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<OcorrenciaEntity>(entity =>
            {
                entity.ToTable("OG_OCORRENCIA");

                entity.HasKey(e => e.IdOcorrencia);

                entity.Property(e => e.IdOcorrencia)
                    .HasColumnName("ID_OCORRENCIA")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.IdUsuario)
                    .HasColumnName("ID_USUARIO")
                    .IsRequired();

                entity.Property(e => e.IdRegiao)
                    .HasColumnName("ID_REGIAO")
                    .IsRequired();

                entity.Property(e => e.IdAlerta)
                    .HasColumnName("ID_ALERTA");

                entity.Property(e => e.TipoOcorrencia)
                    .HasColumnName("TIPO_OCORRENCIA")
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(e => e.Descricao)
                    .HasColumnName("DESCRICAO")
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(e => e.StatusOcorrencia)
                    .HasColumnName("STATUS_OCORRENCIA")
                    .HasMaxLength(20)
                    .HasDefaultValue("ABERTA")
                    .IsRequired();

                entity.Property(e => e.DataOcorrencia)
                    .HasColumnName("DATA_OCORRENCIA")
                    .HasDefaultValueSql("SYSDATE")
                    .IsRequired();

                entity.HasIndex(e => e.IdRegiao)
                    .HasDatabaseName("IX_OG_OCORRENCIA_REGIAO");

                entity.HasOne(e => e.Usuario)
                    .WithMany(e => e.Ocorrencias)
                    .HasForeignKey(e => e.IdUsuario)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Regiao)
                    .WithMany(e => e.Ocorrencias)
                    .HasForeignKey(e => e.IdRegiao)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Alerta)
                    .WithMany(e => e.Ocorrencias)
                    .HasForeignKey(e => e.IdAlerta)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AuditoriaAlertaEntity>(entity =>
            {
                entity.ToTable("OG_AUDITORIA_ALERTA");

                entity.HasKey(e => e.IdAuditoria);

                entity.Property(e => e.IdAuditoria)
                    .HasColumnName("ID_AUDITORIA")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.IdAlerta)
                    .HasColumnName("ID_ALERTA")
                    .IsRequired();

                entity.Property(e => e.Acao)
                    .HasColumnName("ACAO")
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(e => e.StatusAnterior)
                    .HasColumnName("STATUS_ANTERIOR")
                    .HasMaxLength(20);

                entity.Property(e => e.StatusNovo)
                    .HasColumnName("STATUS_NOVO")
                    .HasMaxLength(20);

                entity.Property(e => e.DataAuditoria)
                    .HasColumnName("DATA_AUDITORIA")
                    .HasDefaultValueSql("SYSDATE")
                    .IsRequired();

                entity.HasOne(e => e.Alerta)
                    .WithMany(e => e.Auditorias)
                    .HasForeignKey(e => e.IdAlerta)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}