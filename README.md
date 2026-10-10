# SJInovacao.Acesso

![.NET](https://img.shields.io/badge/.NET-9.0-purple)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue)
![Docker](https://img.shields.io/badge/Docker-ready-blue)
![Build](https://img.shields.io/badge/build-passing-brightgreen)
![Warnings](https://img.shields.io/badge/warnings-0-brightgreen)
![Performance](https://img.shields.io/badge/response-~20ms-success)

Sistema de gestão de **acesso, usuários, permissões e grupos** com auditoria automática, construído em **.NET 9** com **Modular Monolith** e preparado para escalar para **9 módulos de negócio**.

---

## 📑 Sumário

- [Sobre o Projeto](#-sobre-o-projeto)
- [Stack Tecnológica](#-stack-tecnológica)
- [Arquitetura](#-arquitetura)
- [Pré-requisitos](#-pré-requisitos)
- [Como Executar](#-como-executar)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Endpoints Principais](#-endpoints-principais)
- [Padrões de Código](#-padrões-de-código)
- [Auditoria](#-auditoria)
- [Observabilidade](#-observabilidade)
- [Testes](#-testes)
- [Migrations](#-migrations)
- [Docker](#-docker)
- [Documentação Adicional](#-documentação-adicional)
- [Contribuindo](#-contribuindo)

---

## 📖 Sobre o Projeto

O **SJInovacao.Acesso** é o módulo de autenticação e autorização do sistema ERP SJInovacao. Fornece:

- **Autenticação JWT** com refresh token
- **Autorização** por permissões diretas ou via grupos
- **Auditoria automática** de todas as operações de escrita
- **Rastreamento** ponta a ponta via Correlation ID
- **Performance otimizada** (~20ms em regime morno)

### Status Atual

| Indicador | Valor |
| :--- | :--- |
| Progresso funcional | 96% |
| Progresso técnico | 100% |
| Build | 0 avisos |
| Performance (morno) | ~20ms |
| Cobertura de auditoria | 100% |

Ver [backlog completo](docs/backlog/us01-backlog.md).

---

## 🛠 Stack Tecnológica

| Categoria | Tecnologia |
| :--- | :--- |
| Runtime | .NET 9 |
| Linguagem | C# 13 |
| Banco de Dados | PostgreSQL 16 |
| ORM | Entity Framework Core 9 |
| Mediator | MediatR (CQRS) |
| Validação | FluentValidation |
| Mapeamento | AutoMapper |
| Autenticação | JWT Bearer + BCrypt |
| Logging | Serilog (Console + File) |
| Documentação | Swagger/OpenAPI 3.0 |
| Containerização | Docker + Docker Compose |
| Testes | xUnit + FluentAssertions |

---

## 🏗 Arquitetura

O projeto segue **Modular Monolith** com **Clean Architecture**, onde cada módulo é dividido em 3 projetos:

```
Modulo/
├── Modulo.Domain/              → Entidades, Value Objects, Interfaces
├── Modulo.Application/         → Commands, Queries, Handlers, Validators
└── Modulo.Infrastructure.ORM/  → DbContext, Repositories, Mappings
```

### Diagrama de Camadas

```
┌─────────────────────────────────────────────┐
│  WebAPI (única API HTTP)                    │
└─────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────┐
│  IoC (Module Initializers + DI)             │
└─────────────────────────────────────────────┘
                    ↓
┌──────────────────┬──────────────────────────┐
│  Application     │  Infrastructure.ORM      │
│  (Use Cases)     │  (Repositories, EF Core) │
└──────────────────┴──────────────────────────┘
                    ↓
┌─────────────────────────────────────────────┐
│  Domain (Entidades + Regras de Negócio)     │
└─────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────┐
│  Common (Auditing, Security, Logging)       │
└─────────────────────────────────────────────┘
```

Para detalhes completos, ver [docs/architecture/overview.md](docs/architecture/overview.md).

### Decisões Arquiteturais

- [ADR-001 — Modular Monolith](docs/adr/001-modular-monolith.md)
- [ADR-002 — Person como Shared Kernel](docs/adr/002-person-shared-kernel.md)
- [ADR-003 — BaseRepository](docs/adr/003-base-repository.md)
- [ADR-004 — Feature Folders](docs/adr/004-feature-folders.md)
- [ADR-005 — TPT Inheritance](docs/adr/005-tpt-inheritance.md)
- [ADR-006 — Auditoria Centralizada](docs/adr/006-centralized-auditing.md)

---

## ⚙ Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (para execução containerizada)
- [PostgreSQL 16](https://www.postgresql.org/download/) (ou via Docker)
- [DBeaver](https://dbeaver.io/) (opcional, para inspecionar o banco)
- [Git](https://git-scm.com/)

---

## 🚀 Como Executar

### Opção 1 — Docker Compose (recomendado)

```bash
# 1. Clonar
git clone https://github.com/sua-empresa/SJInovacao.Acesso.git
cd SJInovacao.Acesso

# 2. Configurar .env (copiar do .env.example)
cp .env.example .env

# 3. Subir os serviços
docker-compose up -d --build

# 4. Verificar
docker ps
# Deve mostrar: sjinovacao_acesso_api, sjinovacao_developer_evaluation_database, silverj_developer_evaluation_cache
```

Acesse o Swagger em: **https://localhost:44382/swagger**

### Opção 2 — Execução Local

```bash
# 1. Restaurar
dotnet restore

# 2. Aplicar migrations
dotnet ef database update \
    -p src/SJInovacao.Acesso.Database \
    -s src/SJInovacao.Acesso.WebAPI

# 3. Rodar
cd src/SJInovacao.Acesso.WebAPI
dotnet run -c Release

# 4. Acessar
# https://localhost:44382/swagger
```

### Variáveis de Ambiente

Configure em `.env` (Docker) ou `appsettings.{Environment}.json`:

| Variável | Descrição | Exemplo |
| :--- | :--- | :--- |
| `POSTGRES_USER` | Usuário do banco | `developer` |
| `POSTGRES_PASSWORD` | Senha do banco | `ev@luAt10n` |
| `POSTGRES_DB` | Nome do banco | `silverjbase` |
| `REDIS_PASSWORD` | Senha do Redis | `ev@luAt10n` |
| `JWT_SECRET_KEY` | Chave JWT | `d82hd72hdb12b8bd1bdu17bdu817db12` |
| `SwaggerAuth__Username` | Basic auth do Swagger | `admin` |
| `SwaggerAuth__Password` | Senha do Swagger | `...` |

---

## 📂 Estrutura do Projeto

```text
SJInovacao.Acesso/
├── docs/                                    → documentação
│   ├── adr/                                 → decisões arquiteturais
│   ├── backlog/                             → planejamento
│   ├── architecture/                        → visão de arquitetura
│   ├── api/                                 → contratos de API
│   └── tests/                               → plano de testes
├── src/
│   ├── SJInovacao.Acesso.Common/            → transversal
│   │   ├── Auditing/                        → auditoria automática
│   │   ├── HealthChecks/
│   │   ├── Logging/
│   │   ├── Middleware/
│   │   ├── Security/
│   │   └── Validation/
│   ├── SJInovacao.Acesso.Database/          → migrations
│   ├── SJInovacao.Acesso.IoC/               → module initializers
│   ├── SJInovacao.Acesso.Modules.UserAccess.Application/
│   ├── SJInovacao.Acesso.Modules.UserAccess.Domain/
│   ├── SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM/
│   ├── SJInovacao.Acesso.WebAPI/            → API HTTP
│   └── Tests/
├── docker-compose.yml
├── docker-compose.override.yml
├── Dockerfile
└── README.md
```

---

## 🔌 Endpoints Principais

### Autenticação

| Método | Rota | Descrição |
| :--- | :--- | :--- |
| POST | `/api/Auth/Login` | Login com JWT |
| POST | `/api/Auth/RefreshToken` | Renovar token |
| POST | `/api/Auth/RevokeToken` | Revogar refresh token |

### Usuários

| Método | Rota | Descrição |
| :--- | :--- | :--- |
| GET | `/api/Users` | Listar usuários (paginado, ordenável) |
| GET | `/api/Users/{id}` | Buscar por ID |
| POST | `/api/Users` | Criar usuário |
| PUT | `/api/Users/{id}` | Atualizar usuário |
| DELETE | `/api/Users/{id}` | Desativar usuário |

### Permissões

| Método | Rota | Descrição |
| :--- | :--- | :--- |
| GET | `/api/Permissions` | Listar permissões |
| GET | `/api/Permissions/{id}` | Buscar por ID |
| POST | `/api/Permissions` | Criar permissão |
| PUT | `/api/Permissions/{id}` | Atualizar permissão |

### Grupos e Vínculos

| Método | Rota | Descrição |
| :--- | :--- | :--- |
| GET | `/api/GroupUsersPermissions` | Vínculos usuário-grupo-permissão |
| GET | `/api/GroupUsersPermissions/paginated` | Listagem paginada |
| PUT | `/api/UserGroups/GroupUsersPermissions/{userId}/{groupId}/{userIsActive}` | Atualizar vínculos |

### Health

| Método | Rota | Descrição |
| :--- | :--- | :--- |
| GET | `/health` | Health check completo |
| GET | `/health/live` | Liveness |
| GET | `/health/ready` | Readiness |

Documentação completa em [docs/api/endpoints.md](docs/api/endpoints.md).

---

## 📐 Padrões de Código

### Feature Folders

Cada caso de uso tem sua própria pasta:

```text
CreateUser/
├── CreateUserCommand.cs
├── CreateUserHandler.cs
├── CreateUserValidator.cs
└── CreateUserProfile.cs
```

### Repository Pattern

Todos os repositórios herdam de `BaseRepository<TEntity>`:

```csharp
public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository
{
    public Task<Permission> CreateAsync(Permission permission, CancellationToken ct)
        => ExecuteWithLoggingAsync(
            $"Create:{permission.Name}",
            async () =>
            {
                var exists = await _context.Permissions
                    .AnyAsync(p => p.Name == permission.Name, ct);

                if (exists)
                    throw new DomainException("A permissão já existe.");

                _context.Permissions.Add(permission);
                await _context.SaveChangesAsync(ct);
                return permission;
            });
}
```

### Load-then-Update

**NUNCA** usar `_context.X.Update(entity)` em entidades rastreadas:

```csharp
// ❌ Errado
_context.Users.Update(user);

// ✅ Correto
var existing = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id, ct);
existing.Email = user.Email;
existing.UpdatedAt = DateTime.UtcNow;
await _context.SaveChangesAsync(ct);
```

### DomainException

Erros de negócio usam `DomainException` (mapeada para HTTP 400):

```csharp
throw new DomainException($"Usuário {userId} não encontrado.");
```

### Ordenação

Formatos suportados:

- `username asc`
- `username desc, email asc`
- `groupname asc, username desc`

---

## 🔍 Auditoria

Toda entidade que implementa `IAuditable` é auditada automaticamente:

```csharp
public class Permission : BaseEntity, IAuditable { ... }
```

### Estrutura da tabela

```sql
SELECT 
    "EntityName", "Action", "AffectedColumns",
    "OldValues", "NewValues",
    "UserName", "IpAddress", "CorrelationId", "Timestamp"
FROM "AuditLogs"
ORDER BY "Timestamp" DESC
LIMIT 10;
```

### Regras

- **Mascara:** `Password`, `RefreshToken`, `PasswordHash`, `SecurityStamp`, `ConcurrencyStamp`
- **Ignora:** `RowVersion`, `xmin`
- **Skip:** registros com `OldValues == NewValues` (Modified falso)

Ver [ADR-006](docs/adr/006-centralized-auditing.md).

---

## 📊 Observabilidade

### Correlation ID

Todo request é rastreável pelo header `X-Correlation-ID`:

```bash
curl -H "X-Correlation-ID: minha-correlation-id" https://localhost:44382/health
```

O ID é propagado em:

- Resposta HTTP (header)
- Logs estruturados (Serilog)
- Auditoria (coluna `CorrelationId`)

### Logs

Estruturados com Serilog:

```
[2026-10-10 17:30:00.928 -03:00] [INF] GetAllUserGroupsPaginated:1:10 em UserGroup | Duração: 20ms
[2026-10-10 17:30:01.123 -03:00] [INF] CorrelationIdMiddleware ⬅️ GET /api/Users - 200 - 25ms
```

### Health Checks

```bash
curl https://localhost:44382/health
```

Retorna:

```json
{
  "status": "Healthy",
  "timestamp": "2026-10-10T17:30:00Z",
  "checks": [
    { "name": "Liveness", "status": "Healthy" },
    { "name": "Readiness", "status": "Healthy" }
  ]
}
```

---

## 🧪 Testes

### Executar

```bash
# Todos
dotnet test

# Unidade
dotnet test src/Tests/SJInovacao.Acesso.UnitTests

# Integração
dotnet test src/Tests/SJInovacao.Acesso.IntegrationTests

# Com cobertura
dotnet test /p:CollectCoverage=true
```

Ver [plano de testes completo](docs/tests/test-plan.md).

---

## 🗃 Migrations

### Criar nova migration

```bash
dotnet ef migrations add NomeDaMigration \
    -p src/SJInovacao.Acesso.Database \
    -s src/SJInovacao.Acesso.WebAPI
```

### Aplicar

```bash
dotnet ef database update \
    -p src/SJInovacao.Acesso.Database \
    -s src/SJInovacao.Acesso.WebAPI
```

### Reverter

```bash
dotnet ef database update NomeDaMigrationAnterior \
    -p src/SJInovacao.Acesso.Database \
    -s src/SJInovacao.Acesso.WebAPI
```

### Histórico aplicado

```sql
SELECT "MigrationId", "ProductVersion"
FROM "__EFMigrationsHistory"
ORDER BY "MigrationId" DESC;
```

---

## 🐳 Docker

### Subir

```bash
docker-compose up -d --build
```

### Parar

```bash
docker-compose down
```

### Parar + limpar volumes

```bash
docker-compose down -v
```

### Ver logs

```bash
docker-compose logs -f sjinovacao.acesso.webapi
```

### Acessar container

```bash
docker exec -it sjinovacao_acesso_api /bin/bash
```

---

## 📚 Documentação Adicional

- [Backlog US01](docs/backlog/us01-backlog.md)
- [Backlog — Índice](docs/backlog/README.md)
- [ADRs](docs/adr/README.md)
- [Arquitetura](docs/architecture/overview.md)
- [API Endpoints](docs/api/endpoints.md)
- [Plano de Testes](docs/tests/test-plan.md)

---

## 🤝 Contribuindo

### Fluxo de trabalho

```bash
# 1. Criar branch
git checkout -b feature/US01-045-readme

# 2. Desenvolver
# ... código ...

# 3. Commits semânticos
git commit -m "feat(users): adiciona endpoint de busca por email"

# 4. Push
git push origin feature/US01-045-readme

# 5. Abrir PR
```

### Convenções de commit

- `feat(scope): descrição` — Nova funcionalidade
- `fix(scope): descrição` — Correção de bug
- `docs(scope): descrição` — Documentação
- `refactor(scope): descrição` — Refatoração
- `test(scope): descrição` — Testes
- `chore(scope): descrição` — Tarefas auxiliares

### Checklist antes do PR

- [ ] Build sem avisos (`dotnet build`)
- [ ] Testes passando (`dotnet test`)
- [ ] Código formatado
- [ ] Documentação atualizada
- [ ] Sem `Console.WriteLine` ou código de debug
- [ ] Sem secrets no código

---

## 📄 Licença

Este projeto é de uso interno da SJInovacao. Todos os direitos reservados.

---

**SJInovacao.Acesso** · Módulo de Acesso · Versão 2.0 · Última atualização: 10/10/2026