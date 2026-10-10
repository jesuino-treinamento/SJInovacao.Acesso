# Backlog e Planejamento de Sprints — Módulo Acesso (US01)

> **Versão:** 2.0
> **Data de atualização:** 10/10/2026
> **Status:** 96% concluído
> **Homologação prevista:** 17/10/2026
> **Classificação:** Documento técnico-executivo

---

## 📑 Sumário

- [1. Sumário Executivo](#1-sumário-executivo)
- [2. Status Geral do Módulo](#2-status-geral-do-módulo)
- [3. Backlog Detalhado por Épico](#3-backlog-detalhado-por-épico)
- [4. Épicos Técnicos Complementares](#4-épicos-técnicos-complementares)
- [5. Resumo Consolidado](#5-resumo-consolidado)
- [6. Priorização e MVP](#6-priorização-e-mvp)
- [7. Cronograma](#7-cronograma)
- [8. Recomendações de Gestão](#8-recomendações-de-gestão)
- [9. Próximos Passos](#9-próximos-passos)
- [10. Anexos](#10-anexos)

---

## 1. Sumário Executivo

O módulo **US01 — Acesso** encontra-se em estágio final de desenvolvimento, com **96% do backlog originalmente planejado já entregue**. Todos os épicos funcionais foram concluídos, e adicionalmente foram implementados **quatro épicos técnicos transversais** (Observabilidade, Performance, Auditoria e Refatorações) que elevam o módulo a um patamar de qualidade de produção.

O sistema está apto a entrar em produção com autenticação JWT, autorização por permissões e grupos, CRUD completo, auditoria automática de todas as operações e performance otimizada em **~20ms por requisição em regime morno**.

> **Principais entregas já disponíveis:**
>
> - ✅ Autenticação JWT com refresh token
> - ✅ Autorização por permissões diretas e por grupos
> - ✅ CRUD completo de usuários, permissões, grupos e vínculos
> - ✅ Auditoria automática com `OldValues`, `NewValues`, `AffectedColumns`
> - ✅ Observabilidade com Correlation ID e logs estruturados
> - ✅ Performance otimizada: **150x mais rápido**
> - ✅ Build em **0 avisos**

---

## 2. Status Geral do Módulo

| Indicador | Valor |
| :--- | :--- |
| Itens de backlog concluídos | **44 de 46** |
| Progresso funcional | **96%** |
| Progresso técnico (épicos complementares) | **100%** |
| Prazo estimado restante | 1 sprint curta (~1 semana) |
| Data de homologação prevista | **17/10/2026** |
| Build da solução | ✅ **0 avisos** |
| Performance média (regime morno) | ✅ **~20ms por requisição** |
| Cobertura técnica transversal | ✅ 4 épicos implementados |

---

## 3. Backlog Detalhado por Épico

### Épico 1 — Fundação Técnica do Módulo

**Status:** 26 pts (16 originais + 10 extras) · ✅ Concluído

| ID | Item | Prioridade | Estimativa | Status |
| :--- | :--- | :--- | :--- | :--- |
| US01-001 | Estruturar solução do módulo | Alta | 3 pts | ✅ |
| US01-002 | Configurar PostgreSQL e connection strings | Alta | 2 pts | ✅ |
| US01-003 | Criar `DefaultContext` e registrar DI | Alta | 3 pts | ✅ |
| US01-004 | Configurar convenções de entidade base | Alta | 3 pts | ✅ |
| US01-005 | Padronizar tratamento global de erros | Alta | 3 pts | ✅ |
| US01-006 | Configurar Swagger e versionamento básico | Média | 2 pts | ✅ |

**Entregas extras:**

- `BaseRepository<TEntity>` com `ExecuteWithLoggingAsync`
- `CorrelationIdMiddleware` — rastreamento distribuído
- `UserContextMiddleware` — injeção de claims no `IUserContext`
- `ValidationExceptionMiddleware` — tratamento FluentValidation
- `QueryWarmupService` — aquecimento JIT em background
- `UseExceptionHandler` customizado (400/401/500)
- Serilog estruturado por ambiente
- Health checks (`/health`, `/health/live`, `/health/ready`)

---

### Épico 2 — Domínio de Usuários e Autenticação

**Status:** 44 pts + extras · ✅ Concluído

| ID | Item | Prioridade | Estimativa | Status |
| :--- | :--- | :--- | :--- | :--- |
| US01-007 | Modelar entidade `User` | Alta | 5 pts | ✅ |
| US01-008 | Criar `IUserRepository` + implementação | Alta | 5 pts | ✅ |
| US01-009 | Implementar `CreateUser` | Alta | 5 pts | ✅ |
| US01-010 | Implementar `UpdateUser` | Alta | 5 pts | ✅ |
| US01-011 | Implementar `GetUser` | Alta | 3 pts | ✅ |
| US01-012 | Implementar `GetAllUsers` (paginado) | Alta | 3 pts | ✅ |
| US01-013 | Implementar `DeactivateUser` | Alta | 3 pts | ✅ |
| US01-014 | Criar `UserController` | Alta | 5 pts | ✅ |
| US01-015 | Implementar autenticação (Login) | Alta | 5 pts | ✅ |
| US01-016 | Implementar geração de JWT | Alta | 5 pts | ✅ |

**Entregas extras:**

- Refresh Token com expiração configurável
- `UpdateRefreshTokenAsync` otimizado (load-then-update)
- Ordenação dinâmica multi-campo (`username asc, email desc`)
- Validação de campos de ordenação
- Projeção para DTO sem expor `Password` ou `RefreshToken`

---

### Épico 3 — Gestão de Permissões

**Status:** 24 pts · ✅ Concluído

| ID | Item | Prioridade | Estimativa | Status |
| :--- | :--- | :--- | :--- | :--- |
| US01-017 | Modelar entidade `Permission` | Alta | 3 pts | ✅ |
| US01-018 | Criar `IPermissionRepository` + implementação | Alta | 3 pts | ✅ |
| US01-019 | Implementar `CreatePermission` | Alta | 5 pts | ✅ |
| US01-020 | Implementar `ListPermissions` | Alta | 3 pts | ✅ |
| US01-021 | Implementar `UpdatePermission` | Alta | 5 pts | ✅ |
| US01-022 | Criar `PermissionController` | Alta | 3 pts | ✅ |
| US01-023 | Validar unicidade do código da permissão | Alta | 2 pts | ✅ |

---

### Épico 4 — Usuários e Vínculo de Permissões

**Status:** 27 pts · ✅ Concluído

| ID | Item | Prioridade | Estimativa | Status |
| :--- | :--- | :--- | :--- | :--- |
| US01-024 | Modelar entidade `UserPermission` | Alta | 3 pts | ✅ |
| US01-025 | Criar `IUserPermissionRepository` | Alta | 3 pts | ✅ |
| US01-026 | Implementar cadastro de `UserPermission` | Alta | 5 pts | ✅ |
| US01-027 | Criar `UserPermissionsController` | Alta | 3 pts | ✅ |
| US01-028 | Modelar vínculo `Users_Permissions` | Alta | 5 pts | ✅ |
| US01-029 | Criar contrato/implementação do vínculo | Alta | 3 pts | ✅ |
| US01-030 | Implementar cadastro de `UsersPermissions` | Alta | 5 pts | ✅ |

---

### Épico 5 — Grupos e Vínculo de Permissões

**Status:** 30 pts · ✅ Concluído

| ID | Item | Prioridade | Estimativa | Status |
| :--- | :--- | :--- | :--- | :--- |
| US01-031 | Modelar entidade `GroupPermission` | Alta | 3 pts | ✅ |
| US01-032 | Criar `IGroupPermissionRepository` | Alta | 3 pts | ✅ |
| US01-032B | Implementar cadastro de `GroupPermission` | Alta | 5 pts | ✅ |
| US01-033 | Criar `GroupPermissionsController` | Alta | 3 pts | ✅ |
| US01-034 | Modelar vínculo `GroupUsersPermissions` | Alta | 5 pts | ✅ |
| US01-035 | Criar contrato/implementação do vínculo | Alta | 3 pts | ✅ |
| US01-036 | Implementar cadastro de `GroupUsersPermissions` | Alta | 5 pts | ✅ |
| US01-037 | Criar `GroupUsersPermissionsController` | Alta | 3 pts | ✅ |

---

### Épico 6 — Autorização, Segurança e Estabilização

**Status:** 34 pts (26 concluídos + 8 pendentes)

| ID | Item | Prioridade | Estimativa | Status |
| :--- | :--- | :--- | :--- | :--- |
| US01-038 | Implementar `GetUserPermissions` | Alta | 5 pts | ✅ |
| US01-039 | Configurar policies/claims por permissão | Alta | 5 pts | ✅ |
| US01-040 | Impedir login de usuário inativo | Alta | 2 pts | ✅ |
| US01-041 | Hash seguro de senha (BCrypt) | Alta | 3 pts | ✅ |
| US01-042 | Seed inicial de permissões | Média | 3 pts | ⚠️ Parcial |
| US01-043 | Testes unitários de domínio e aplicação | Alta | 5 pts | ⚠️ Parcial |
| US01-044 | Testes de integração dos endpoints | Alta | 5 pts | ⚠️ Parcial |
| US01-045 | Criar `README.md` técnico-operacional | Alta | 3 pts | ✅ |
| US01-046 | Preparar pipeline de publicação | Média | 3 pts | ✅ |

---

## 4. Épicos Técnicos Complementares

> Além do backlog originalmente previsto, foram implementados **quatro épicos técnicos transversais**.

### Épico 7 — Observabilidade e Rastreamento 🆕

**Status:** 13 pts · ✅ Concluído

| ID | Item | Prioridade | Estimativa | Status |
| :--- | :--- | :--- | :--- | :--- |
| US01-047 | Serilog com sinks por ambiente | Alta | 3 pts | ✅ |
| US01-048 | `CorrelationIdMiddleware` | Alta | 3 pts | ✅ |
| US01-049 | Structured logging nos repositórios | Alta | 3 pts | ✅ |
| US01-050 | Health checks | Média | 2 pts | ✅ |
| US01-051 | Padronização de logs por camada | Média | 2 pts | ✅ |

---

### Épico 8 — Performance e Escala 🆕

**Status:** 25 pts · ✅ Concluído · Ganho: **~3.000ms → ~20ms (150x)**

| ID | Item | Prioridade | Estimativa | Status |
| :--- | :--- | :--- | :--- | :--- |
| US01-052 | Refatorar repositórios com `ExecuteWithLoggingAsync` | Alta | 5 pts | ✅ |
| US01-053 | `AsNoTracking` em leituras | Alta | 3 pts | ✅ |
| US01-054 | Projections (records) para listagens | Alta | 5 pts | ✅ |
| US01-055 | Corrigir cartesian explosion em `GetAllUsers` | Alta | 3 pts | ✅ |
| US01-056 | Adicionar índices em tabelas críticas | Alta | 3 pts | ✅ |
| US01-057 | `QueryWarmupService` | Média | 3 pts | ✅ |
| US01-058 | Remover `Password`/`RefreshToken` do SELECT | Crítica | 3 pts | ✅ |

---

### Épico 9 — Auditoria 🆕

**Status:** 17 pts · ✅ Concluído

| ID | Item | Prioridade | Estimativa | Status |
| :--- | :--- | :--- | :--- | :--- |
| US01-059 | Criar tabela `AuditLogs` (com JSONB) | Alta | 3 pts | ✅ |
| US01-060 | `IAuditable` + `AuditService` | Alta | 5 pts | ✅ |
| US01-061 | Override de `SaveChangesAsync` no `DefaultContext` | Alta | 2 pts | ✅ |
| US01-062 | Mascarar `Password`/`RefreshToken` | Alta | 2 pts | ✅ |
| US01-063 | Filtrar "Modified falso" | Alta | 2 pts | ✅ |
| US01-064 | Índices em `AuditLogs` | Média | 2 pts | ✅ |
| US01-065 | Propagação do `CorrelationId` no audit | Média | 1 pt | ✅ |

---

### Épico 10 — Refatorações e Débito Técnico 🆕

**Status:** 19 pts · ✅ Concluído · Resultado: **Build em 0 avisos**

| ID | Item | Prioridade | Estimativa | Status |
| :--- | :--- | :--- | :--- | :--- |
| US01-066 | Corrigir shadowing `CS0108` | Média | 1 pt | ✅ |
| US01-067 | Corrigir `CS8619` nos PolicyProviders | Média | 2 pts | ✅ |
| US01-068 | Refatorar `GroupPermissionRepository.UpdateAsync` | Média | 2 pts | ✅ |
| US01-069 | Refatorar `PermissionRepository` | Baixa | 2 pts | ✅ |
| US01-070 | Refatorar `UserPermissionRepository` | Baixa | 3 pts | ✅ |
| US01-071 | Substituir `.Update()` por load-then-update | Alta | 3 pts | ✅ |
| US01-072 | Suprimir `NU1903` (AutoMapper) | Baixa | 1 pt | ✅ |
| US01-073 | Corrigir os 29 avisos nullable | Média | 5 pts | ✅ |

---

## 5. Resumo Consolidado

| Épico | Pontos | Concluído | Pendente |
| :--- | ---: | ---: | ---: |
| 1. Fundação Técnica | 26 | 26 | 0 |
| 2. Usuários + Auth | 44 | 44 | 0 |
| 3. Permissões | 24 | 24 | 0 |
| 4. Usuários ↔ Permissões | 27 | 27 | 0 |
| 5. Grupos ↔ Permissões | 30 | 30 | 0 |
| 6. Autorização + Estabilização | 34 | 26 | **8** |
| 7. Observabilidade | 13 | 13 | 0 |
| 8. Performance | 25 | 25 | 0 |
| 9. Auditoria | 17 | 17 | 0 |
| 10. Refatorações | 19 | 19 | 0 |
| **TOTAL** | **259** | **251 (97%)** | **8 (3%)** |

---

## 6. Priorização e MVP

### ✅ MVP Obrigatório — Concluído

US01-001 a US01-041 + US01-046.

### ✅ MVP com Qualidade — Concluído

Épicos 7, 8, 9 e 10 (técnicos, transversais). Entrega de auditoria completa, performance otimizada e build sem avisos.

### ⚠️ Pendências para Homologação

| ID | Item | Estimativa | Justificativa |
| :--- | :--- | :--- | :--- |
| US01-043 | Testes unitários (cobertura ≥ 60%) | 3 pts | Confiabilidade |
| US01-044 | Testes de integração dos endpoints | 2 pts | Regressão |
| US01-042 | Seed inicial de permissões | 2 pts | Setup de ambiente |
| **TOTAL** | | **7 pts** | **~1 sprint curta** |

---

## 7. Cronograma

### Sprints Concluídas

| Sprint | Período | Foco Principal | Status |
| :--- | :--- | :--- | :--- |
| Sprint 0 | 03/10 a 05/10 | Foundation técnica | ✅ |
| Sprint 1 | 05/10 a 07/10 | Usuários + Autenticação | ✅ |
| Sprint 2 | 07/10 a 08/10 | Permissões | ✅ |
| Sprint 3 | 08/10 a 09/10 | Vínculos usuário-permissão | ✅ |
| Sprint 4 | 09/10 a 10/10 | Grupos + Vínculos | ✅ |
| Sprint 5 | 10/10 | Autorização + Auditoria | ✅ |
| Sprint 6 | 10/10 | Performance + Refatorações | ✅ |

### Sprint Final — Sprint 7

| Sprint | Período | Foco | Entregável |
| :--- | :--- | :--- | :--- |
| **Sprint 7** | **11/10 a 17/10** | Docs + Testes + Seed | Módulo pronto para homologação |

**Itens da Sprint 7:**

- [ ] **US01-043** — Testes unitários dos handlers e domínio
- [ ] **US01-044** — Testes de integração dos endpoints
- [ ] **US01-042** — Seed inicial de permissões (`Permissions.g.cs`)

**Prazo executivo total: 1 semana — até 17/10/2026**

---

## 8. Recomendações de Gestão

> **Aprovação Formal**
>
> O módulo **US01 (Acesso)** está **96% concluído**, com todos os épicos funcionais entregues e todos os épicos técnicos implementados. Está apto a entrar em produção após a conclusão dos 3 itens pendentes (7 pts).

### Capacidades já disponíveis em produção

- ✅ Autenticação JWT com refresh token
- ✅ Autorização por permissões e grupos
- ✅ CRUD completo de usuários, permissões e vínculos
- ✅ Auditoria automática de todas as entidades
- ✅ Performance otimizada (~20ms morno)
- ✅ Observabilidade (Correlation ID, logs estruturados)
- ✅ Docker + migrations + 7 índices no banco

### Decisões de Gestão Recomendadas

1. Homologação prevista para **17/10/2026**
2. Iniciar **US02 (Produtos)** em paralelo, usando o Acesso como template
3. Criar `docs/adr/` com decisões arquiteturais antes de escalar para os 9 módulos
4. Definir política institucional de retenção da auditoria (sugestão: **2 anos + arquivamento**)
5. Configurar pipeline CI/CD antes do início do US02

---

## 9. Próximos Passos

| Ordem | Ação | Responsável | Prazo |
| :---: | :--- | :--- | :--- |
| 1 | Adicionar testes unitários (cobertura ≥ 60%) | Dev backend + QA | 15/10 |
| 2 | Adicionar testes de integração | Dev backend + QA | 16/10 |
| 3 | Seed inicial de permissões | Dev backend | 17/10 |
| 4 | Homologação com PO | PO + QA | 17/10 |
| 5 | Início do US02 (Produtos) | Dev backend | 18/10 |
| 6 | Criar ADRs de arquitetura | Dev backend | 18/10 |
| 7 | Configurar pipeline CI/CD | DevOps | 20/10 |

---

## 10. Anexos

### Anexo A — Glossário

| Termo | Definição |
| :--- | :--- |
| CQRS | Command Query Responsibility Segregation |
| DTO | Data Transfer Object |
| JWT | JSON Web Token |
| TPT | Table-per-Type |
| Warmup | Pré-compilação JIT em background |
| Correlation ID | Identificador único propagado na requisição |

### Anexo B — ADRs

- [ADR-001 — Modular Monolith](../adr/001-modular-monolith.md)
- [ADR-002 — Person como Shared Kernel](../adr/002-person-shared-kernel.md)
- [ADR-003 — BaseRepository](../adr/003-base-repository.md)
- [ADR-004 — Feature Folders](../adr/004-feature-folders.md)
- [ADR-005 — TPT Inheritance](../adr/005-tpt-inheritance.md)
- [ADR-006 — Auditoria Centralizada](../adr/006-centralized-auditing.md)

### Anexo C — Riscos Mapeados

| Risco | Impacto | Mitigação |
| :--- | :--- | :--- |
| Migração de `Person` para módulo Cadastros | Médio | Executar antes do US02 |
| Crescimento da base de auditoria | Médio | Índices + política de retenção |
| Acoplamento entre módulos futuros | Alto | Providers públicos antes do US02 |
| Cobertura de testes insuficiente | Médio | Meta mínima 60% |

---

**Prazo executivo sugerido: 17/10/2026**

---

_SJInovacao.Acesso — Backlog e Planejamento de Sprints · Módulo US01 · Versão 2.0 · 10/10/2026_