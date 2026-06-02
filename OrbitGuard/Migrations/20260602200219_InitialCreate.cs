using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrbitGuard.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OG_FONTE_ESPACIAL",
                columns: table => new
                {
                    ID_FONTE = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false),
                    TIPO_DADO = table.Column<string>(type: "NVARCHAR2(40)", maxLength: 40, nullable: false),
                    URL_BASE = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: true),
                    PAYLOAD_EXEMPLO = table.Column<string>(type: "CLOB", nullable: true),
                    DATA_COLETA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "SYSDATE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OG_FONTE_ESPACIAL", x => x.ID_FONTE);
                });

            migrationBuilder.CreateTable(
                name: "OG_REGIAO_MONITORADA",
                columns: table => new
                {
                    ID_REGIAO = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    CIDADE = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false),
                    UF = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false),
                    LATITUDE = table.Column<decimal>(type: "DECIMAL(10,6)", precision: 10, scale: 6, nullable: false),
                    LONGITUDE = table.Column<decimal>(type: "DECIMAL(10,6)", precision: 10, scale: 6, nullable: false),
                    TIPO_RISCO_BASE = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    POPULACAO_ESTIMADA = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OG_REGIAO_MONITORADA", x => x.ID_REGIAO);
                });

            migrationBuilder.CreateTable(
                name: "OG_USUARIO",
                columns: table => new
                {
                    ID_USUARIO = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    PERFIL = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    TELEFONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    DATA_CADASTRO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "SYSDATE"),
                    ATIVO = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: false, defaultValue: "S")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OG_USUARIO", x => x.ID_USUARIO);
                });

            migrationBuilder.CreateTable(
                name: "OG_ABRIGO",
                columns: table => new
                {
                    ID_ABRIGO = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_REGIAO = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    NOME = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    ENDERECO = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    CAPACIDADE_TOTAL = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CAPACIDADE_OCUPADA = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValue: 0),
                    ATIVO = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: false, defaultValue: "S")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OG_ABRIGO", x => x.ID_ABRIGO);
                    table.ForeignKey(
                        name: "FK_OG_ABRIGO_OG_REGIAO_MONITORADA_ID_REGIAO",
                        column: x => x.ID_REGIAO,
                        principalTable: "OG_REGIAO_MONITORADA",
                        principalColumn: "ID_REGIAO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OG_HISTORICO_RISCO",
                columns: table => new
                {
                    ID_HISTORICO = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_REGIAO = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    INDICE_RISCO = table.Column<decimal>(type: "DECIMAL(5,2)", precision: 5, scale: 2, nullable: false),
                    NIVEL_RISCO = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    MOTIVO = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: true),
                    DATA_CALCULO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "SYSDATE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OG_HISTORICO_RISCO", x => x.ID_HISTORICO);
                    table.ForeignKey(
                        name: "FK_OG_HISTORICO_RISCO_OG_REGIAO_MONITORADA_ID_REGIAO",
                        column: x => x.ID_REGIAO,
                        principalTable: "OG_REGIAO_MONITORADA",
                        principalColumn: "ID_REGIAO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OG_SENSOR_IOT",
                columns: table => new
                {
                    ID_SENSOR = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_REGIAO = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    CODIGO = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    TIPO_SENSOR = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    STATUS_SENSOR = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false, defaultValue: "ATIVO"),
                    DATA_INSTALACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "SYSDATE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OG_SENSOR_IOT", x => x.ID_SENSOR);
                    table.ForeignKey(
                        name: "FK_OG_SENSOR_IOT_OG_REGIAO_MONITORADA_ID_REGIAO",
                        column: x => x.ID_REGIAO,
                        principalTable: "OG_REGIAO_MONITORADA",
                        principalColumn: "ID_REGIAO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OG_RECURSO_ABRIGO",
                columns: table => new
                {
                    ID_RECURSO = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_ABRIGO = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    TIPO_RECURSO = table.Column<string>(type: "NVARCHAR2(40)", maxLength: 40, nullable: false),
                    QUANTIDADE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    UNIDADE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OG_RECURSO_ABRIGO", x => x.ID_RECURSO);
                    table.ForeignKey(
                        name: "FK_OG_RECURSO_ABRIGO_OG_ABRIGO_ID_ABRIGO",
                        column: x => x.ID_ABRIGO,
                        principalTable: "OG_ABRIGO",
                        principalColumn: "ID_ABRIGO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OG_ALERTA_RISCO",
                columns: table => new
                {
                    ID_ALERTA = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_REGIAO = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ID_HISTORICO = table.Column<long>(type: "NUMBER(19)", nullable: true),
                    TITULO = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    MENSAGEM = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    NIVEL_RISCO = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    STATUS_ALERTA = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false, defaultValue: "ABERTO"),
                    DATA_ALERTA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "SYSDATE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OG_ALERTA_RISCO", x => x.ID_ALERTA);
                    table.ForeignKey(
                        name: "FK_OG_ALERTA_RISCO_OG_HISTORICO_RISCO_ID_HISTORICO",
                        column: x => x.ID_HISTORICO,
                        principalTable: "OG_HISTORICO_RISCO",
                        principalColumn: "ID_HISTORICO",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OG_ALERTA_RISCO_OG_REGIAO_MONITORADA_ID_REGIAO",
                        column: x => x.ID_REGIAO,
                        principalTable: "OG_REGIAO_MONITORADA",
                        principalColumn: "ID_REGIAO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OG_LEITURA_SENSOR",
                columns: table => new
                {
                    ID_LEITURA = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_SENSOR = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    VALOR = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false),
                    UNIDADE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    DATA_LEITURA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "SYSDATE"),
                    ORIGEM = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false, defaultValue: "IOT")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OG_LEITURA_SENSOR", x => x.ID_LEITURA);
                    table.ForeignKey(
                        name: "FK_OG_LEITURA_SENSOR_OG_SENSOR_IOT_ID_SENSOR",
                        column: x => x.ID_SENSOR,
                        principalTable: "OG_SENSOR_IOT",
                        principalColumn: "ID_SENSOR",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OG_AUDITORIA_ALERTA",
                columns: table => new
                {
                    ID_AUDITORIA = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_ALERTA = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ACAO = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    STATUS_ANTERIOR = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    STATUS_NOVO = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    DATA_AUDITORIA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "SYSDATE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OG_AUDITORIA_ALERTA", x => x.ID_AUDITORIA);
                    table.ForeignKey(
                        name: "FK_OG_AUDITORIA_ALERTA_OG_ALERTA_RISCO_ID_ALERTA",
                        column: x => x.ID_ALERTA,
                        principalTable: "OG_ALERTA_RISCO",
                        principalColumn: "ID_ALERTA",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OG_OCORRENCIA",
                columns: table => new
                {
                    ID_OCORRENCIA = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_USUARIO = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ID_REGIAO = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ID_ALERTA = table.Column<long>(type: "NUMBER(19)", nullable: true),
                    TIPO_OCORRENCIA = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    DESCRICAO = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    STATUS_OCORRENCIA = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false, defaultValue: "ABERTA"),
                    DATA_OCORRENCIA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "SYSDATE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OG_OCORRENCIA", x => x.ID_OCORRENCIA);
                    table.ForeignKey(
                        name: "FK_OG_OCORRENCIA_OG_ALERTA_RISCO_ID_ALERTA",
                        column: x => x.ID_ALERTA,
                        principalTable: "OG_ALERTA_RISCO",
                        principalColumn: "ID_ALERTA",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OG_OCORRENCIA_OG_REGIAO_MONITORADA_ID_REGIAO",
                        column: x => x.ID_REGIAO,
                        principalTable: "OG_REGIAO_MONITORADA",
                        principalColumn: "ID_REGIAO",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OG_OCORRENCIA_OG_USUARIO_ID_USUARIO",
                        column: x => x.ID_USUARIO,
                        principalTable: "OG_USUARIO",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OG_ABRIGO_ID_REGIAO",
                table: "OG_ABRIGO",
                column: "ID_REGIAO");

            migrationBuilder.CreateIndex(
                name: "IX_OG_ALERTA_REGIAO_STATUS",
                table: "OG_ALERTA_RISCO",
                columns: new[] { "ID_REGIAO", "STATUS_ALERTA" });

            migrationBuilder.CreateIndex(
                name: "IX_OG_ALERTA_RISCO_ID_HISTORICO",
                table: "OG_ALERTA_RISCO",
                column: "ID_HISTORICO");

            migrationBuilder.CreateIndex(
                name: "IX_OG_AUDITORIA_ALERTA_ID_ALERTA",
                table: "OG_AUDITORIA_ALERTA",
                column: "ID_ALERTA");

            migrationBuilder.CreateIndex(
                name: "IX_OG_HISTORICO_RISCO_ID_REGIAO",
                table: "OG_HISTORICO_RISCO",
                column: "ID_REGIAO");

            migrationBuilder.CreateIndex(
                name: "IX_OG_LEITURA_SENSOR_DATA",
                table: "OG_LEITURA_SENSOR",
                columns: new[] { "ID_SENSOR", "DATA_LEITURA" });

            migrationBuilder.CreateIndex(
                name: "IX_OG_OCORRENCIA_ID_ALERTA",
                table: "OG_OCORRENCIA",
                column: "ID_ALERTA");

            migrationBuilder.CreateIndex(
                name: "IX_OG_OCORRENCIA_ID_USUARIO",
                table: "OG_OCORRENCIA",
                column: "ID_USUARIO");

            migrationBuilder.CreateIndex(
                name: "IX_OG_OCORRENCIA_REGIAO",
                table: "OG_OCORRENCIA",
                column: "ID_REGIAO");

            migrationBuilder.CreateIndex(
                name: "IX_OG_RECURSO_ABRIGO_ID_ABRIGO",
                table: "OG_RECURSO_ABRIGO",
                column: "ID_ABRIGO");

            migrationBuilder.CreateIndex(
                name: "IX_OG_SENSOR_REGIAO",
                table: "OG_SENSOR_IOT",
                column: "ID_REGIAO");

            migrationBuilder.CreateIndex(
                name: "UK_OG_SENSOR_CODIGO",
                table: "OG_SENSOR_IOT",
                column: "CODIGO",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UK_OG_USUARIO_EMAIL",
                table: "OG_USUARIO",
                column: "EMAIL",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OG_AUDITORIA_ALERTA");

            migrationBuilder.DropTable(
                name: "OG_FONTE_ESPACIAL");

            migrationBuilder.DropTable(
                name: "OG_LEITURA_SENSOR");

            migrationBuilder.DropTable(
                name: "OG_OCORRENCIA");

            migrationBuilder.DropTable(
                name: "OG_RECURSO_ABRIGO");

            migrationBuilder.DropTable(
                name: "OG_SENSOR_IOT");

            migrationBuilder.DropTable(
                name: "OG_ALERTA_RISCO");

            migrationBuilder.DropTable(
                name: "OG_USUARIO");

            migrationBuilder.DropTable(
                name: "OG_ABRIGO");

            migrationBuilder.DropTable(
                name: "OG_HISTORICO_RISCO");

            migrationBuilder.DropTable(
                name: "OG_REGIAO_MONITORADA");
        }
    }
}
