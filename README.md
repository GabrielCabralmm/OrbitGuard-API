# OrbitGuard API

## Integrantes

* Enzo Monteiro Maciel
* Gabriel Cabral Mendes Mariano
* Matheus de Almeida Sousa

---

# 1. Descrição do Projeto

O OrbitGuard é uma API REST desenvolvida em ASP.NET Core 8 com banco de dados Oracle, criada para auxiliar no monitoramento de riscos ambientais através da integração de sensores IoT, registros históricos, alertas preventivos, ocorrências e gestão de abrigos.

A solução centraliza informações relacionadas a eventos ambientais críticos, permitindo o acompanhamento de regiões monitoradas e o registro de ações preventivas e corretivas.

---

# 2. Objetivo

Desenvolver uma API REST seguindo boas práticas de arquitetura e desenvolvimento, utilizando persistência em banco de dados relacional Oracle, relacionamentos entre entidades e gerenciamento de estrutura através de migrations do Entity Framework Core.

---

# 3. Tecnologias Utilizadas

* ASP.NET Core 8
* C#
* Entity Framework Core 8
* Oracle Database
* Oracle Entity Framework Core Provider
* Swagger
* Swagger Annotations
* OpenAPI
* Visual Studio 2022
* GitHub

---

# 4. Arquitetura do Projeto

O projeto foi desenvolvido utilizando uma arquitetura simples e organizada em camadas:

## Controllers

Responsáveis por receber as requisições HTTP e retornar as respostas da API.

## Entitys

Representam as entidades do banco de dados e suas validações.

## Data

Contém o ApplicationDbContext responsável pela comunicação com o Oracle através do Entity Framework Core.

---

# 5. Estrutura do Projeto

```text
OrbitGuardAPI
│
├── Controllers
│   ├── UsuarioController
│   ├── RegiaoController
│   ├── FonteController
│   ├── SensorController
│   ├── LeituraController
│   ├── AbrigoController
│   ├── RecursoAbrigoController
│   ├── HistoricoController
│   ├── AlertaController
│   ├── OcorrenciaController
│   └── AuditoriaAlertaController
│
├── Data
│   └── ApplicationDbContext
│
├── Entitys
│
├── Migrations
│
├── Program.cs
│
└── appsettings.json
```

---

# 6. Banco de Dados

O banco de dados foi modelado utilizando Oracle Database.

A estrutura possui relacionamentos do tipo:

* 1:N
* N:1

atendendo aos requisitos da disciplina.

## Principais Relacionamentos

### Região → Sensores

Uma região pode possuir vários sensores.

### Região → Abrigos

Uma região pode possuir vários abrigos.

### Região → Históricos

Uma região pode possuir vários históricos de risco.

### Região → Alertas

Uma região pode possuir vários alertas.

### Região → Ocorrências

Uma região pode possuir várias ocorrências.

### Sensor → Leituras

Um sensor pode possuir diversas leituras.

### Abrigo → Recursos

Um abrigo pode possuir diversos recursos.

### Histórico → Alertas

Um histórico pode originar vários alertas.

### Alerta → Auditorias

Um alerta pode possuir diversos registros de auditoria.

### Usuário → Ocorrências

Um usuário pode registrar diversas ocorrências.

---

# 7. Entidades da Aplicação

* Usuario
* Regiao
* Fonte
* Sensor
* Leitura
* Abrigo
* RecursoAbrigo
* Historico
* Alerta
* Ocorrencia
* AuditoriaAlerta

---

# 8. Migrations

As migrations são utilizadas para versionar e controlar a estrutura do banco de dados.

Criação da migration:

```powershell
Add-Migration InitialCreate
```

Aplicação da migration:

```powershell
Update-Database
```

Benefícios:

* Controle de alterações do banco.
* Histórico de mudanças.
* Facilidade de implantação.
* Sincronização entre ambiente de desenvolvimento e banco Oracle.

---

# 9. Documentação Swagger

A documentação da API foi implementada utilizando:

* Swagger
* Swagger Annotations
* XML Documentation

Através do Swagger é possível:

* Visualizar endpoints.
* Testar requisições.
* Verificar status codes.
* Consultar exemplos de retorno.

---

# 10. Endpoints Disponíveis

## Usuários

```text
GET    /api/Usuario
GET    /api/Usuario/{id}
POST   /api/Usuario
PUT    /api/Usuario/{id}
DELETE /api/Usuario/{id}
```

## Regiões

```text
GET    /api/Regiao
GET    /api/Regiao/{id}
POST   /api/Regiao
PUT    /api/Regiao/{id}
DELETE /api/Regiao/{id}
```

## Fontes

```text
GET    /api/Fonte
GET    /api/Fonte/{id}
POST   /api/Fonte
PUT    /api/Fonte/{id}
DELETE /api/Fonte/{id}
```

## Sensores

```text
GET    /api/Sensor
GET    /api/Sensor/{id}
POST   /api/Sensor
PUT    /api/Sensor/{id}
DELETE /api/Sensor/{id}
```

## Leituras

```text
GET    /api/Leitura
GET    /api/Leitura/{id}
POST   /api/Leitura
PUT    /api/Leitura/{id}
DELETE /api/Leitura/{id}
```

## Demais entidades

Seguem o mesmo padrão RESTful.

---

# 11. Como Executar o Projeto

## Clonar o repositório

```bash
git clone URL_DO_REPOSITORIO
```

## Restaurar dependências

```bash
dotnet restore
```

## Criar banco via migrations

```bash
Update-Database
```

## Executar aplicação

```bash
dotnet run
```

---

# 12. Considerações Finais

O OrbitGuard foi desenvolvido seguindo boas práticas de desenvolvimento de APIs REST, utilizando banco de dados Oracle, Entity Framework Core, Swagger e Migrations.

A solução atende aos requisitos técnicos propostos pela disciplina, incluindo persistência relacional, relacionamentos entre entidades, versionamento de banco e documentação completa da API.
