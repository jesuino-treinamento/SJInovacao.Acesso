# Arquitetura do Projeto — SJInovacao.Acesso



- **Versão:** 1.0

- **Data:** 10/10/2026

- **Autor:** Tech Lead

- **Público-alvo:** Devs, Arquitetos, DevOps

---

## 📑 Sumario

- [Arquitetura do Projeto — SJInovacao.Acesso](#arquitetura-do-projeto--sjinovacaoacesso)
  - [📑 Sumario](#-sumario)
  - [1. Visao Geral](#1-visao-geral)
    - [Objetivos Arquiteturais](#objetivos-arquiteturais)
  - [2. Principios Arquiteturais](#2-principios-arquiteturais)
    - [1. Separacao de Responsabilidades (SRP)](#1-separacao-de-responsabilidades-srp)
    - [2. Inversao de Dependencia (DIP)](#2-inversao-de-dependencia-dip)
    - [3. Isolamento entre Modulos](#3-isolamento-entre-modulos)
    - [4. Convencao sobre Configuracao](#4-convencao-sobre-configuracao)
    - [5. Auditoria by Default](#5-auditoria-by-default)
  - [3. Estilo Arquitetural](#3-estilo-arquitetural)
  - [🌐 Diagrama de Contexto (C4 Nivel 1)](#-diagrama-de-contexto-c4-nivel-1)
  - [📦 Diagrama de Containers (C4 Nivel 2)](#-diagrama-de-containers-c4-nivel-2)
    - [Containers](#containers)
  - [🔩 Diagrama de Componentes (C4 Nivel 3)](#-diagrama-de-componentes-c4-nivel-3)
  - [🥞 Camadas](#camadas)
    - [1. Domain (Nucleo)](#1-domain-nucleo)
    - [2. Application](#2-application)
    - [3. Infrastructure.ORM](#3-infrastructureorm)
    - [4. WebAPI](#4-webapi)
    - [5. Common (Transversal)](#5-common-transversal)
    - [6. IoC](#6-ioc)
  - [🧩 Estrutura de Modulos](#-estrutura-de-modulos)
    - [Estado atual](#estado-atual)
    - [Estado futuro (9 modulos)](#estado-futuro-9-modulos)
  - [🔄 Fluxo de Requisicao](#-fluxo-de-requisicao)
  - [🔍 Fluxo de Auditoria](#-fluxo-de-auditoria)
  - [🔗 Comunicacao entre Modulos](#-comunicacao-entre-modulos)
    - [Atual (1 modulo)](#atual-1-modulo)
    - [Futuro (9 modulos) — Padrao Provider](#futuro-9-modulos--padrao-provider)
  - [🗄 Banco de Dados](#-banco-de-dados)
    - [Estrategia](#estrategia)
    - [Schemas planejados](#schemas-planejados)
    - [Estado atual (schema public)](#estado-atual-schema-public)
    - [Indices](#indices)
  - [🎨 Padroes de Projeto](#-padroes-de-projeto)
  - [📋 Decisoes Arquiteturais (ADRs)](#-decisoes-arquiteturais-adrs)
  - [🚀 Roadmap Arquitetural](#-roadmap-arquitetural)
    - [Fase 1 — Consolidacao (atual)](#fase-1--consolidacao-atual)
    - [Fase 2 — Extracao de Cadastros (US02)](#fase-2--extracao-de-cadastros-us02)
    - [Fase 3 — Novos Modulos (US02 a US09)](#fase-3--novos-modulos-us02-a-us09)
    - [Fase 4 — Otimizacoes Avancadas](#fase-4--otimizacoes-avancadas)
    - [Fase 5 — Extracao de Microsservicos (se necessario)](#fase-5--extracao-de-microsservicos-se-necessario)
  - [📚 Referencias](#-referencias)



---

## 1. Visão Geral

O **SJInovacao.Acesso** é o módulo de autenticação e autorização do ERP SJInovacao. Foi projetado como **Modular Monolith** — um deploy único, mas com **módulos fortemente isolados logicamente**, preparados para extração futura como microsserviços caso a escala exija.

### Objetivos Arquiteturais

| Objetivo | Como é atendido |
| :--- | :--- |
| **Escalabilidade** | Modular Monolith pronto para extração de serviços |
| **Manutenibilidade** | Clean Architecture + Feature Folders |
| **Observabilidade** | Serilog + Correlation ID + Health Checks |
| **Auditabilidade** | AuditService interceptando `SaveChangesAsync` |
| **Performance** | AsNoTracking + Projections + Índices + Warmup |
| **Segurança** | JWT + BCrypt + Policies + Auditoria |
| **Testabilidade** | Repository Pattern + CQRS + DI |

---

## 2. Princípios Arquiteturais

### 1. Separação de Responsabilidades (SRP)

Cada classe tem **uma única razão para mudar**:

- **Domain** — Regras de negócio

- **Application** — Orquestração de casos de uso

- **Infrastructure** — Persistência e integrações

- **WebAPI** — Exposição HTTP

### 2. Inversão de Dependência (DIP)

Camadas externas dependem de **abstrações** das camadas internas:

```
WebAPI → Application → Domain

                         ↑
                   Infrastructure

```

O **Domain** não conhece EF Core, MediatR, AutoMapper ou qualquer biblioteca externa.

### 3. Isolamento entre Módulos

Módulos **não se referenciam** diretamente. Comunicação via **contratos públicos**:

```csharp

public interface IUserProvider
{
   Task<UserResumo?> ObterAsync(Guid userId, CancellationToken ct);
   Task<bool> ExisteAsync(Guid userId, CancellationToken ct);
}

```
### 4. Convenção sobre Configuração

- Feature Folders em vez de Layered Folders
- `BaseRepository<TEntity>` em vez de repositórios duplicados
- `ExecuteWithLoggingAsync` em vez de try/catch individual

### 5. Auditoria by Default

Toda entidade que implementa `IAuditable` é auditada **automaticamente**, sem esforço por parte do desenvolvedor.

---

## 3. Estilo Arquitetural

| Dimensão | Escolha | Justificativa |
| :--- | :--- | :--- |
| **Estilo** | Modular Monolith | Ver [ADR-001](../adr/001-modular-monolith.md) |
| **Camadas** | Clean Architecture (4 camadas) | Separação clara |
| **Organização** | Feature Folders | Ver [ADR-004](../adr/004-feature-folders.md) |
| **Comunicação interna** | CQRS (MediatR) | Desacoplamento |
| **Persistência** | Repository Pattern | Testabilidade |
| **Autenticação** | JWT Bearer | Stateless |
| **Auditoria** | Interceptação centralizada | Ver [ADR-006](../adr/006-centralized-auditing.md) |

| **Deploy** | Docker + Docker Compose | Portabilidade |

---

## 🌐 Diagrama de Contexto (C4 Nível 1)

Quem usa o sistema e quais são as dependências externas:

```

                   ┌──────────────────────┐
                   │   Usuário Final      │
                   │   (Navegador/App)    │
                   └──────────┬───────────┘
                              │ HTTPS/JWT
                              ▼
       ┌──────────────────────────────────────┐
       │                                      │
       │      SJInovacao.Acesso (WebAPI)      │
       │                                      │
       │  • Autenticação JWT                  │
       │  • Autorização por permissões        │
       │  • Gestão de usuários e grupos       │
       │  • Auditoria automática              │
       │                                      │
       └──────────┬───────────┬───────────────┘
                  │           │
                  │           │
                  ▼           ▼
        ┌─────────────┐ ┌─────────────┐
        │ PostgreSQL  │ │ Redis Cache │
        │ (dados)     │ │ (sessões)   │
        └─────────────┘ └─────────────┘

        Consumidores internos (futuro):
        • Módulo Produtos
        • Módulo Vendas
        • Módulo Financeiro
        • Módulo RH
        • ... (9 módulos)

```


---

## 📦 Diagrama de Containers (C4 Nível 2)

Como o sistema é decomposto em containers executáveis:

```

┌───────────────────────────────────────────────────────────────┐
│                     Docker Network (bridge)                   │
│                                                               │
│  ┌──────────────────────────────┐                             │
│  │  sjinovacao_acesso_api       │                             │
│  │  (.NET 9 Runtime)            │                             │
│  │  Porta: 8080 / 8081          │                             │
│  └──────────┬───────────────────┘                             │
│             │                                                 │
│             │ EF Core                                         │
│             ▼                                                 │
│  ┌──────────────────────────────┐                             │
│  │  postgres:16                 │                             │
│  │  Porta: 5432                 │                             │
│  │  DB: silverjbase             │                             │
│  └──────────────────────────────┘                             │
│                                                               │
│  ┌──────────────────────────────┐                             │
│  │  redis:7-alpine              │                             │
│  │  Porta: 6379                 │                             │
│  └──────────────────────────────┘                             │
│                                                               │
└───────────────────────────────────────────────────────────────┘
        │
        │ HTTP/HTTPS
        ▼
  ┌─────────────┐
  │   Cliente   │
  │  (Swagger,  │
  │  Postman,   │
  │  Frontend)  │
  └─────────────┘

```

### Containers

| Container | Imagem | Porta | Descrição |
| :--- | :--- | :--- | :--- |
| `sjinovacao_acesso_api` | `sjinovacaoacessowebapi:latest` | 8080, 8081 | API .NET 9 |
| `sjinovacao_developer_evaluation_database` | `postgres:16` | 5434 (host) | Banco PostgreSQL |
| `silverj_developer_evaluation_cache` | `redis:7-alpine` | interno | Cache Redis |

---

## 🔩 Diagrama de Componentes (C4 Nível 3)

Detalhamento interno da WebAPI:

```

┌─────────────────────────────────────────────────────────────┐
│                  SJInovacao.Acesso.WebAPI                   │
│                                                             │
│  ┌────────────────────────────────────────────────────┐     │
│  │  Middleware Pipeline                               │     │
│  │  ─────────────────────                             │     │
│  │  1. CorrelationIdMiddleware                        │     │
│  │  2. UseExceptionHandler                            │     │
│  │  3. UseStatusCodePages                             │     │
│  │  4. UseHttpsRedirection                            │     │
│  │  5. UseAuthentication                              │     │
│  │  6. UseAuthorization                               │     │
│  │  7. ValidationExceptionMiddleware                  │     │
│  │  8. UserContextMiddleware                          │     │
│  └────────────────────────────────────────────────────┘     │
│                          │                                  │
│                          ▼                                  │
│  ┌────────────────────────────────────────────────────┐     │
│  │  Controllers                                       │     │
│  │  ────────────                                      │     │
│  │  • AuthController                                  │     │
│  │  • UserController                                  │     │
│  │  • PermissionController                            │     │
│  │  • GroupPermissionsController                      │     │
│  │  • GroupUsersPermissionsController                 │     │
│  │  • UserPermissionsController                       │     │
│  └────────────────────────────────────────────────────┘     │
│                          │                                  │
│                          ▼                                  │
│  ┌────────────────────────────────────────────────────┐     │
│  │  MediatR (CQRS)                                    │     │
│  │  ──────────────                                    │     │
│  │  • ValidationBehavior<,>                           │     │
│  │  • Commands → CommandHandlers                      │     │
│  │  • Queries → QueryHandlers                         │     │
│  └────────────────────────────────────────────────────┘     │
│                          │                                  │
│                          ▼                                  │
│  ┌────────────────────────────────────────────────────┐     │
│  │  Repositories (BaseRepository)                     │     │
│  │  ────────────────────────────                      │     │
│  │  • UserRepository                                  │     │
│  │  • PermissionRepository                            │     │
│  │  • GroupPermissionRepository                       │     │
│  │  • UsersGroupsPermissionsRepository                │     │
│  │  • + AuditService                                  │     │
│  └────────────────────────────────────────────────────┘     │
│                          │                                  │
│                          ▼                                  │
│  ┌────────────────────────────────────────────────────┐     │
│  │  DefaultContext (EF Core)                          │     │
│  │  ───────────────────────                           │     │
│  │  • DbSets                                          │     │
│  │  • Override SaveChangesAsync → Auditoria           │     │
│  └────────────────────────────────────────────────────┘     │
│                          │                                  │
│                          ▼                                  │
│                    PostgreSQL 16                            │
│                                                             │
└─────────────────────────────────────────────────────────────┘

```

---

## 🥞 Camadas

### 1. Domain (Núcleo)

**Responsabilidade:** Regras de negócio, entidades, value objects.

**Não depende de:** nenhuma camada externa (só `Common`).

```

Domain/
├── Entities/              User, Permission, GroupPermission, ...
├── ValueObjects/          Name, Document, Geolocation
├── Enums/                 UserRole, StatusTypes, PhoneType
├── Exceptions/            DomainException
├── Repositories/          Interfaces (IUserRepository, ...)
├── Specifications/        Padrões de consulta
├── Validation/            Regras de validação de domínio
└── Common/                BaseEntity, PaginatedList

```

**Princípios:**

- Zero dependências de infraestrutura
- Regras de negócio explícitas em métodos
- Value Objects imutáveis

### 2. Application

**Responsabilidade:** Orquestração de casos de uso, CQRS, validação.

**Depende de:** Domain.

```
Application/
├── Authentication/        Login, RefreshToken
├── Authorization/         Policies, Providers
├── Users/                 CRUD de usuários
├── Permissions/           CRUD de permissões
├── GroupPermissions/      Gestão de grupos
├── GroupUsersPermissions/ Vínculos grupo-usuário-permissão
├── UserPermissios/        Vínculos diretos
├── DTOs/                  Objetos de transferência
├── Contracts/             IUserAccessModule
└── Configuration/         ICommandHandler, IQueryHandler

```

**Princípios:**

- **Feature Folders** — cada caso de uso em sua pasta
- Handler delega para repositório (não implementa regra)
- Validação com FluentValidation
- Mapping com AutoMapper

### 3. Infrastructure.ORM

**Responsabilidade:** Persistência, acesso a dados.

**Depende de:** Domain.

```
Infrastructure.ORM/
├── DefaultContext.cs      DbContext + SaveChangesAsync override
├── Mapping/               IEntityTypeConfiguration por entidade
├── Repositories/          Implementações
└── Common/                RepositoriesModule

```

**Princípios:**

- `BaseRepository<TEntity>` como base
- `ExecuteWithLoggingAsync` para observabilidade
- `AsNoTracking` em leituras
- Load-then-update em escritas

### 4. WebAPI

**Responsabilidade:** Exposição HTTP, autenticação, middlewares.

**Depende de:** IoC (que referencia tudo).

```
WebAPI/
├── Modules/               Controllers por domínio
├── Middleware/            CorrelationId, Validation, UserContext
├── Common/                ApiResponse, PaginatedResponse, BaseController
├── Program.cs             Bootstrap
└── appsettings.*.json     Configuração por ambiente

```

### 5. Common (Transversal)

**Responsabilidade:** Infra compartilhada entre módulos.

**Não depende de nenhum módulo.**

```
Common/
├── Auditing/              IAuditable, AuditService, AuditLog
├── Security/              JWT, BCrypt, IUserContext, Policies
├── Middleware/            UserContextMiddleware
├── Logging/               Serilog setup
├── HealthChecks/          Health endpoints
└── Validation/            ValidationBehavior, Validator

```

### 6. IoC

**Responsabilidade:** Orquestrar DI.

```
IoC/
├── ModuleInitializers/
│   ├── ApplicationModuleInitializer.cs
│   ├── InfrastructureModuleInitializer.cs
│   └── WebApiModuleInitializer.cs
├── DependencyResolver.cs
└── IModuleInitializer.cs

```
---

## 🧩 Estrutura de Módulos

### Estado atual

```
src/
├── SJInovacao.Acesso.Common/                  → transversal
├── SJInovacao.Acesso.Database/                → migrations
├── SJInovacao.Acesso.IoC/                     → orquestração
├── SJInovacao.Acesso.Modules.UserAccess.*     → US01 (Acesso)
└── SJInovacao.Acesso.WebAPI/                  → 1 API

```

### Estado futuro (9 módulos)

```
src/
├── SJInovacao.Acesso.Common/
├── SJInovacao.Acesso.Database/
├── SJInovacao.Acesso.IoC/
├── SJInovacao.Acesso.WebAPI/
├── Modules/
│   ├── Acesso/                                → US01
│   │   ├── *.Domain/
│   │   ├── *.Application/
│   │   └── *.Infrastructure.ORM/
│   ├── Cadastros/                             → ADR-002
│   ├── Produtos/                              → US02
│   ├── Cliente/                               → US03
│   ├── Colaboradores/                         → US04
│   ├── Fornecedores/                          → US05
│   ├── Vendas/                                → US06
│   ├── Financeiro/                            → US07
│   ├── RH/                                    → US08
│   └── Relatorios/                            → US09
└── Tests/

```

---

## 🔄 Fluxo de Requisição

Fluxo completo de uma requisição HTTP até a resposta:

```
1. Cliente envia request

  ↓

2. CorrelationIdMiddleware

  • Gera/extrai CorrelationId
  • Adiciona ao HttpContext.Items
  • Adiciona ao LogContext (Serilog)
  • Adiciona ao header de resposta
  ↓

3. UseExceptionHandler

  • Captura exceções não tratadas
  • Mapeia para HTTP 400/401/500
  ↓

4. UseStatusCodePages

  • Trata 401/403/404 do framework

  ↓

5. UseHttpsRedirection

  ↓

6. UseRouting

  ↓

7. UseAuthentication

  • Valida JWT
  • Popula context.User com claims
  ↓

8. UseAuthorization

  • Avalia policies
  • Pode retornar 403
  ↓

9. ValidationExceptionMiddleware

  • Trata ValidationException do FluentValidation
  ↓

10. UserContextMiddleware

   • Extrai claims
   • Popula IUserContext
   ↓

11. Controller

   • Recebe o request
   • Envia Command/Query via MediatR
   ↓

12. MediatR Pipeline

   • ValidationBehavior valida o Command
   • Chama o Handler
   ↓

13. Handler

   • Orquestra caso de uso
   • Chama repositório
   ↓

14. Repository (BaseRepository)

   • ExecuteWithLoggingAsync
   • Operação no DefaultContext
   ↓

15. SaveChangesAsync (override)

   • AuditService.CaptureChanges()
   • Adiciona registros em AuditLogs
   • Persiste tudo na mesma transação

   ↓

16. Response volta pelo pipeline

   ↓

17. CorrelationIdMiddleware

   • Loga duração total

   ↓

18. Resposta ao cliente

```
---

## 🔍 Fluxo de Auditoria

Como a auditoria acontece automaticamente:

```
┌─────────────────────────────────────────────────┐
│  Handler executa operação                       │
│  ex: userRepository.UpdateAsync(user)           │
└────────────────────┬────────────────────────────┘

                     ↓

┌─────────────────────────────────────────────────┐
│  BaseRepository.ExecuteWithLoggingAsync         │
│  • Loga início                                  │
│  • Inicia stopwatch                             │
└────────────────────┬────────────────────────────┘

                     ↓

┌─────────────────────────────────────────────────┐
│  Load-then-update                               │
│  • Carrega entidade do banco (tracked)          │
│  • Aplica apenas campos alterados               │
└────────────────────┬────────────────────────────┘

                     ↓

┌─────────────────────────────────────────────────┐
│  DefaultContext.SaveChangesAsync (override)     │
│                                                 │
│  1. if (_auditService != null)                  │
│  2. auditLogs = _auditService.CaptureChanges()  │
│     ├─ DetectChanges()                          │
│     ├─ Itera ChangeTracker.Entries()            │
│     ├─ Filtra IAuditable                        │
│     ├─ Filtra valores idênticos                 │
│     ├─ Mascara sensíveis (***)                  │
│     └─ Cria AuditLog[]                          │
│  3. AuditLogs.AddRangeAsync(auditLogs)          │
│  4. base.SaveChangesAsync() → 1 transação       │
└────────────────────┬────────────────────────────┘

                     ↓

┌─────────────────────────────────────────────────┐
│  Banco persiste:                                │
│  • Alteração na tabela de origem                │
│  • Registro em AuditLogs                        │
│  AMBOS na mesma transação ACID                  │
└─────────────────────────────────────────────────┘

```
---

## 🔗 Comunicação entre Módulos

### Atual (1 módulo)

Sem comunicação cross-module.

### Futuro (9 módulos) — Padrão Provider

```
┌──────────────────┐                  ┌──────────────────┐
│  Vendas          │                  │  Produtos        │
│                  │  IProdutoProvider│                  │
│  VendaService ───┼────────────────▶│  ProdutoService  │
│                  │  (contrato)      │                  │
└──────────────────┘                  └──────────────────┘

        │                                     │
        │                                     │
        ▼                                     ▼
   ┌─────────┐                          ┌─────────┐
   │ Vendas  │                          │ Produtos│
   │ Tables  │                          │ Tables  │
   └─────────┘                          └─────────┘

```

**Regras:**

1. **Nunca** referenciar o `Infrastructure.ORM` de outro módulo
2. **Sempre** usar contratos públicos (`I*Provider`)
3. Retornar **DTOs de resumo**, não entidades
4. Se a comunicação for pesada, considerar **eventos** (MediatR `INotification` ou message broker)

---

## 🗄 Banco de Dados

### Estratégia

**Um único banco PostgreSQL** com **schemas separados por módulo**.

### Schemas planejados

| Schema | Módulo |
| :--- | :--- |
| `acesso` | Acesso |
| `cadastros` | Cadastros (Person) |
| `produtos` | Produtos |
| `cliente` | Cliente |
| `colaboradores` | Colaboradores |
| `fornecedores` | Fornecedores |
| `vendas` | Vendas |
| `financeiro` | Financeiro |
| `rh` | RH |

### Estado atual (schema `public`)

Tabelas já criadas no schema `public`:

- `Persons`, `Users`, `Addresses`, `Phones`
- `Permissions`, `UserPermissions`, `GroupPermissions`, `GroupsPermissions`
- `UsersGroupsPermissions`, `UserGroups`
- `AuditLogs`

### Índices

| Índice | Tabela | Colunas | Propósito |
| :--- | :--- | :--- | :--- |
| `IX_UserGroups_GroupId` | UserGroups | GroupId | Listar usuários por grupo |
| `IX_UGP_UserId_GroupId` | UsersGroupsPermissions | UserId, GroupId | Query de permissões |
| `IX_UGP_PermissionId` | UsersGroupsPermissions | PermissionId | Busca por permissão |
| `IX_GroupPermissions_Name` | GroupPermissions | Name | Ordenação |
| `IX_AuditLogs_EntityName_EntityId` | AuditLogs | EntityName, EntityId | Histórico |
| `IX_AuditLogs_Timestamp` | AuditLogs | Timestamp DESC | Últimas mudanças |
| `IX_AuditLogs_CorrelationId` | AuditLogs | CorrelationId | Rastreio |

---

## 🎨 Padrões de Projeto

| Padrão | Onde | Por quê |
| :--- | :--- | :--- |
| **Repository** | Infrastructure.ORM | Abstrai persistência |
| **Unit of Work** | DefaultContext | Transação implícita |
| **CQRS** | Application (MediatR) | Separa leitura/escrita |
| **Mediator** | MediatR | Desacopla Controller de Handler |
| **Pipeline Behavior** | ValidationBehavior | Cross-cutting |
| **Specification** | Domain/Specifications | Regras reutilizáveis |
| **Value Object** | Domain/ValueObjects | Imutabilidade |
| **DTO** | Application/DTOs | Transporte de dados |
| **Factory** | DefaultContextFactory | Design-time |
| **Marker Interface** | IAuditable | Auditoria automática |
| **Decorator** | BaseRepository | Logging transversal |
| **Provider** | Cross-module | Contratos públicos |

---

## 📋 Decisões Arquiteturais (ADRs)

| ID | Título | Status |
| :--- | :--- | :--- |
| [ADR-001](../adr/001-modular-monolith.md) | Adoção de Modular Monolith | Aceito |
| [ADR-002](../adr/002-person-shared-kernel.md) | Person como Shared Kernel | Aceito |
| [ADR-003](../adr/003-base-repository.md) | BaseRepository com ExecuteWithLoggingAsync | Aceito |
| [ADR-004](../adr/004-feature-folders.md) | Feature Folders | Aceito |
| [ADR-005](../adr/005-tpt-inheritance.md) | TPT Inheritance | Em revisão |
| [ADR-006](../adr/006-centralized-auditing.md) | Auditoria Centralizada | Aceito |

---

## 🚀 Roadmap Arquitetural

### Fase 1 — Consolidação (atual)

- ✅ Módulo Acesso 100% funcional
- ✅ Auditoria automática
- ✅ Observabilidade completa
- 🔄 ADRs, testes, README

### Fase 2 — Extração de Cadastros (US02)

- Criar módulo `Cadastros`
- Migrar `Person` de `Acesso` para `Cadastros`
- Definir `ICadastroProvider`
- Ajustar `User` para referenciar `Person`

### Fase 3 — Novos Módulos (US02 a US09)

- Criar estrutura padrão para cada módulo
- Aplicar os padrões já estabelecidos
- Definir contratos públicos conforme necessidade

### Fase 4 — Otimizações Avançadas

- Cache distribuído (Redis) para consultas frequentes
- CQRS com read models separados
- Eventos de domínio via message broker (RabbitMQ/Kafka)

### Fase 5 — Extração de Microsserviços (se necessário)

- Extrair módulos com carga distinta
- Introduzir API Gateway
- Service discovery

---

## 📚 Referências



- [Backlog US01](../backlog/us01-backlog.md)
- [ADRs](../adr/README.md)
- [API Endpoints](../api/endpoints.md)
- [Plano de Testes](../tests/test-plan.md)
- [README Principal](../../README.md)



---



**Arquitetura — SJInovacao.Acesso · Versão 1.0 · 10/10/2026**

