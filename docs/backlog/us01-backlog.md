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

<table>
<thead>
<tr>
<th>Indicador</th>
<th>Valor</th>
</tr>
</thead>
<tbody>
<tr>
<td>Itens de backlog concluídos</td>
<td>**44 de 46**</td>
</tr>
<tr>
<td>Progresso funcional</td>
<td>**96%**</td>
</tr>
<tr>
<td>Progresso técnico (épicos complementares)</td>
<td>**100%**</td>
</tr>
<tr>
<td>Prazo estimado restante</td>
<td>1 sprint curta (~1 semana)</td>
</tr>
<tr>
<td>Data de homologação prevista</td>
<td>**17/10/2026**</td>
</tr>
<tr>
<td>Build da solução</td>
<td>✅ **0 avisos**</td>
</tr>
<tr>
<td>Performance média (regime morno)</td>
<td>✅ **~20ms por requisição**</td>
</tr>
<tr>
<td>Cobertura técnica transversal</td>
<td>✅ 4 épicos implementados</td>
</tr>
</tbody>
</table>

---

## 3. Backlog Detalhado por Épico

### Épico 1 — Fundação Técnica do Módulo

**Status:** 26 pts (16 originais + 10 extras) · ✅ Concluído

<table>
<thead>
<tr>
<th>ID</th>
<th>Item</th>
<th>Prioridade</th>
<th>Estimativa</th>
<th>Status</th>
</tr>
</thead>
<tbody>
<tr>
<td>US01-001</td>
<td>Estruturar solução do módulo</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-002</td>
<td>Configurar PostgreSQL e connection strings</td>
<td>Alta</td>
<td>2 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-003</td>
<td>Criar `DefaultContext` e registrar DI</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-004</td>
<td>Configurar convenções de entidade base</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-005</td>
<td>Padronizar tratamento global de erros</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-006</td>
<td>Configurar Swagger e versionamento básico</td>
<td>Média</td>
<td>2 pts</td>
<td>✅</td>
</tr>
</tbody>
</table>

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

<table>
<thead>
<tr>
<th>ID</th>
<th>Item</th>
<th>Prioridade</th>
<th>Estimativa</th>
<th>Status</th>
</tr>
</thead>
<tbody>
<tr>
<td>US01-007</td>
<td>Modelar entidade `User`</td>
<td>Alta</td>
<td>5 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-008</td>
<td>Criar `IUserRepository` + implementação</td>
<td>Alta</td>
<td>5 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-009</td>
<td>Implementar `CreateUser`</td>
<td>Alta</td>
<td>5 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-010</td>
<td>Implementar `UpdateUser`</td>
<td>Alta</td>
<td>5 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-011</td>
<td>Implementar `GetUser`</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-012</td>
<td>Implementar `GetAllUsers` (paginado)</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-013</td>
<td>Implementar `DeactivateUser`</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-014</td>
<td>Criar `UserController`</td>
<td>Alta</td>
<td>5 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-015</td>
<td>Implementar autenticação (Login)</td>
<td>Alta</td>
<td>5 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-016</td>
<td>Implementar geração de JWT</td>
<td>Alta</td>
<td>5 pts</td>
<td>✅</td>
</tr>
</tbody>
</table>

**Entregas extras:**

- Refresh Token com expiração configurável
- `UpdateRefreshTokenAsync` otimizado (load-then-update)
- Ordenação dinâmica multi-campo (`username asc, email desc`)
- Validação de campos de ordenação
- Projeção para DTO sem expor `Password` ou `RefreshToken`

---

### Épico 3 — Gestão de Permissões

**Status:** 24 pts · ✅ Concluído

<table>
<thead>
<tr>
<th>ID</th>
<th>Item</th>
<th>Prioridade</th>
<th>Estimativa</th>
<th>Status</th>
</tr>
</thead>
<tbody>
<tr>
<td>US01-017</td>
<td>Modelar entidade `Permission`</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-018</td>
<td>Criar `IPermissionRepository` + implementação</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-019</td>
<td>Implementar `CreatePermission`</td>
<td>Alta</td>
<td>5 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-020</td>
<td>Implementar `ListPermissions`</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-021</td>
<td>Implementar `UpdatePermission`</td>
<td>Alta</td>
<td>5 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-022</td>
<td>Criar `PermissionController`</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-023</td>
<td>Validar unicidade do código da permissão</td>
<td>Alta</td>
<td>2 pts</td>
<td>✅</td>
</tr>
</tbody>
</table>

---

### Épico 4 — Usuários e Vínculo de Permissões

**Status:** 27 pts · ✅ Concluído

<table>
<thead>
<tr>
<th>ID</th>
<th>Item</th>
<th>Prioridade</th>
<th>Estimativa</th>
<th>Status</th>
</tr>
</thead>
<tbody>
<tr>
<td>US01-024</td>
<td>Modelar entidade `UserPermission`</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-025</td>
<td>Criar `IUserPermissionRepository`</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-026</td>
<td>Implementar cadastro de `UserPermission`</td>
<td>Alta</td>
<td>5 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-027</td>
<td>Criar `UserPermissionsController`</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-028</td>
<td>Modelar vínculo `Users_Permissions`</td>
<td>Alta</td>
<td>5 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-029</td>
<td>Criar contrato/implementação do vínculo</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-030</td>
<td>Implementar cadastro de `UsersPermissions`</td>
<td>Alta</td>
<td>5 pts</td>
<td>✅</td>
</tr>
</tbody>
</table>

---

### Épico 5 — Grupos e Vínculo de Permissões

**Status:** 30 pts · ✅ Concluído

<table>
<thead>
<tr>
<th>ID</th>
<th>Item</th>
<th>Prioridade</th>
<th>Estimativa</th>
<th>Status</th>
</tr>
</thead>
<tbody>
<tr>
<td>US01-031</td>
<td>Modelar entidade `GroupPermission`</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-032</td>
<td>Criar `IGroupPermissionRepository`</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-032B</td>
<td>Implementar cadastro de `GroupPermission`</td>
<td>Alta</td>
<td>5 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-033</td>
<td>Criar `GroupPermissionsController`</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-034</td>
<td>Modelar vínculo `GroupUsersPermissions`</td>
<td>Alta</td>
<td>5 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-035</td>
<td>Criar contrato/implementação do vínculo</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-036</td>
<td>Implementar cadastro de `GroupUsersPermissions`</td>
<td>Alta</td>
<td>5 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-037</td>
<td>Criar `GroupUsersPermissionsController`</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
</tbody>
</table>

---

### Épico 6 — Autorização, Segurança e Estabilização

**Status:** 34 pts (26 concluídos + 8 pendentes)

<table>
<thead>
<tr>
<th>ID</th>
<th>Item</th>
<th>Prioridade</th>
<th>Estimativa</th>
<th>Status</th>
</tr>
</thead>
<tbody>
<tr>
<td>US01-038</td>
<td>Implementar `GetUserPermissions`</td>
<td>Alta</td>
<td>5 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-039</td>
<td>Configurar policies/claims por permissão</td>
<td>Alta</td>
<td>5 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-040</td>
<td>Impedir login de usuário inativo</td>
<td>Alta</td>
<td>2 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-041</td>
<td>Hash seguro de senha (BCrypt)</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-042</td>
<td>Seed inicial de permissões</td>
<td>Média</td>
<td>3 pts</td>
<td>⚠️ Parcial</td>
</tr>
<tr>
<td>US01-043</td>
<td>Testes unitários de domínio e aplicação</td>
<td>Alta</td>
<td>5 pts</td>
<td>⚠️ Parcial</td>
</tr>
<tr>
<td>US01-044</td>
<td>Testes de integração dos endpoints</td>
<td>Alta</td>
<td>5 pts</td>
<td>⚠️ Parcial</td>
</tr>
<tr>
<td>US01-045</td>
<td>Criar `README.md` técnico-operacional</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-046</td>
<td>Preparar pipeline de publicação</td>
<td>Média</td>
<td>3 pts</td>
<td>✅</td>
</tr>
</tbody>
</table>

---

## 4. Épicos Técnicos Complementares

> Além do backlog originalmente previsto, foram implementados **quatro épicos técnicos transversais**.

### Épico 7 — Observabilidade e Rastreamento 🆕

**Status:** 13 pts · ✅ Concluído

<table>
<thead>
<tr>
<th>ID</th>
<th>Item</th>
<th>Prioridade</th>
<th>Estimativa</th>
<th>Status</th>
</tr>
</thead>
<tbody>
<tr>
<td>US01-047</td>
<td>Serilog com sinks por ambiente</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-048</td>
<td>`CorrelationIdMiddleware`</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-049</td>
<td>Structured logging nos repositórios</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-050</td>
<td>Health checks</td>
<td>Média</td>
<td>2 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-051</td>
<td>Padronização de logs por camada</td>
<td>Média</td>
<td>2 pts</td>
<td>✅</td>
</tr>
</tbody>
</table>

---

### Épico 8 — Performance e Escala 🆕

**Status:** 25 pts · ✅ Concluído · Ganho: **~3.000ms → ~20ms (150x)**

<table>
<thead>
<tr>
<th>ID</th>
<th>Item</th>
<th>Prioridade</th>
<th>Estimativa</th>
<th>Status</th>
</tr>
</thead>
<tbody>
<tr>
<td>US01-052</td>
<td>Refatorar repositórios com `ExecuteWithLoggingAsync`</td>
<td>Alta</td>
<td>5 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-053</td>
<td>`AsNoTracking` em leituras</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-054</td>
<td>Projections (records) para listagens</td>
<td>Alta</td>
<td>5 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-055</td>
<td>Corrigir cartesian explosion em `GetAllUsers`</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-056</td>
<td>Adicionar índices em tabelas críticas</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-057</td>
<td>`QueryWarmupService`</td>
<td>Média</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-058</td>
<td>Remover `Password`/`RefreshToken` do SELECT</td>
<td>Crítica</td>
<td>3 pts</td>
<td>✅</td>
</tr>
</tbody>
</table>

---

### Épico 9 — Auditoria 🆕

**Status:** 17 pts · ✅ Concluído

<table>
<thead>
<tr>
<th>ID</th>
<th>Item</th>
<th>Prioridade</th>
<th>Estimativa</th>
<th>Status</th>
</tr>
</thead>
<tbody>
<tr>
<td>US01-059</td>
<td>Criar tabela `AuditLogs` (com JSONB)</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-060</td>
<td>`IAuditable` + `AuditService`</td>
<td>Alta</td>
<td>5 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-061</td>
<td>Override de `SaveChangesAsync` no `DefaultContext`</td>
<td>Alta</td>
<td>2 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-062</td>
<td>Mascarar `Password`/`RefreshToken`</td>
<td>Alta</td>
<td>2 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-063</td>
<td>Filtrar "Modified falso"</td>
<td>Alta</td>
<td>2 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-064</td>
<td>Índices em `AuditLogs`</td>
<td>Média</td>
<td>2 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-065</td>
<td>Propagação do `CorrelationId` no audit</td>
<td>Média</td>
<td>1 pt</td>
<td>✅</td>
</tr>
</tbody>
</table>

---

### Épico 10 — Refatorações e Débito Técnico 🆕

**Status:** 19 pts · ✅ Concluído · Resultado: **Build em 0 avisos**

<table>
<thead>
<tr>
<th>ID</th>
<th>Item</th>
<th>Prioridade</th>
<th>Estimativa</th>
<th>Status</th>
</tr>
</thead>
<tbody>
<tr>
<td>US01-066</td>
<td>Corrigir shadowing `CS0108`</td>
<td>Média</td>
<td>1 pt</td>
<td>✅</td>
</tr>
<tr>
<td>US01-067</td>
<td>Corrigir `CS8619` nos PolicyProviders</td>
<td>Média</td>
<td>2 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-068</td>
<td>Refatorar `GroupPermissionRepository.UpdateAsync`</td>
<td>Média</td>
<td>2 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-069</td>
<td>Refatorar `PermissionRepository`</td>
<td>Baixa</td>
<td>2 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-070</td>
<td>Refatorar `UserPermissionRepository`</td>
<td>Baixa</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-071</td>
<td>Substituir `.Update()` por load-then-update</td>
<td>Alta</td>
<td>3 pts</td>
<td>✅</td>
</tr>
<tr>
<td>US01-072</td>
<td>Suprimir `NU1903` (AutoMapper)</td>
<td>Baixa</td>
<td>1 pt</td>
<td>✅</td>
</tr>
<tr>
<td>US01-073</td>
<td>Corrigir os 29 avisos nullable</td>
<td>Média</td>
<td>5 pts</td>
<td>✅</td>
</tr>
</tbody>
</table>

---

## 5. Resumo Consolidado

<table>
<thead>
<tr>
<th>Épico</th>
<th>Pontos</th>
<th>Concluído</th>
<th>Pendente</th>
</tr>
</thead>
<tbody>
<tr>
<td>1. Fundação Técnica</td>
<td>26</td>
<td>26</td>
<td>0</td>
</tr>
<tr>
<td>2. Usuários + Auth</td>
<td>44</td>
<td>44</td>
<td>0</td>
</tr>
<tr>
<td>3. Permissões</td>
<td>24</td>
<td>24</td>
<td>0</td>
</tr>
<tr>
<td>4. Usuários ↔ Permissões</td>
<td>27</td>
<td>27</td>
<td>0</td>
</tr>
<tr>
<td>5. Grupos ↔ Permissões</td>
<td>30</td>
<td>30</td>
<td>0</td>
</tr>
<tr>
<td>6. Autorização + Estabilização</td>
<td>34</td>
<td>26</td>
<td>**8**</td>
</tr>
<tr>
<td>7. Observabilidade</td>
<td>13</td>
<td>13</td>
<td>0</td>
</tr>
<tr>
<td>8. Performance</td>
<td>25</td>
<td>25</td>
<td>0</td>
</tr>
<tr>
<td>9. Auditoria</td>
<td>17</td>
<td>17</td>
<td>0</td>
</tr>
<tr>
<td>10. Refatorações</td>
<td>19</td>
<td>19</td>
<td>0</td>
</tr>
<tr>
<td>**TOTAL**</td>
<td>**259**</td>
<td>**251 (97%)**</td>
<td>**8 (3%)**</td>
</tr>
</tbody>
</table>

---

## 6. Priorização e MVP

### ✅ MVP Obrigatório — Concluído

US01-001 a US01-041 + US01-046.

### ✅ MVP com Qualidade — Concluído

Épicos 7, 8, 9 e 10 (técnicos, transversais). Entrega de auditoria completa, performance otimizada e build sem avisos.

### ⚠️ Pendências para Homologação

<table>
<thead>
<tr>
<th>ID</th>
<th>Item</th>
<th>Estimativa</th>
<th>Justificativa</th>
</tr>
</thead>
<tbody>
<tr>
<td>US01-043</td>
<td>Testes unitários (cobertura ≥ 60%)</td>
<td>3 pts</td>
<td>Confiabilidade</td>
</tr>
<tr>
<td>US01-044</td>
<td>Testes de integração dos endpoints</td>
<td>2 pts</td>
<td>Regressão</td>
</tr>
<tr>
<td>US01-042</td>
<td>Seed inicial de permissões</td>
<td>2 pts</td>
<td>Setup de ambiente</td>
</tr>
<tr>
<td>**TOTAL**</td>
<td></td>
<td>**7 pts**</td>
<td>**~1 sprint curta**</td>
</tr>
</tbody>
</table>

---

## 7. Cronograma

### Sprints Concluídas

<table>
<thead>
<tr>
<th>Sprint</th>
<th>Período</th>
<th>Foco Principal</th>
<th>Status</th>
</tr>
</thead>
<tbody>
<tr>
<td>Sprint 0</td>
<td>03/10 a 05/10</td>
<td>Foundation técnica</td>
<td>✅</td>
</tr>
<tr>
<td>Sprint 1</td>
<td>05/10 a 07/10</td>
<td>Usuários + Autenticação</td>
<td>✅</td>
</tr>
<tr>
<td>Sprint 2</td>
<td>07/10 a 08/10</td>
<td>Permissões</td>
<td>✅</td>
</tr>
<tr>
<td>Sprint 3</td>
<td>08/10 a 09/10</td>
<td>Vínculos usuário-permissão</td>
<td>✅</td>
</tr>
<tr>
<td>Sprint 4</td>
<td>09/10 a 10/10</td>
<td>Grupos + Vínculos</td>
<td>✅</td>
</tr>
<tr>
<td>Sprint 5</td>
<td>10/10</td>
<td>Autorização + Auditoria</td>
<td>✅</td>
</tr>
<tr>
<td>Sprint 6</td>
<td>10/10</td>
<td>Performance + Refatorações</td>
<td>✅</td>
</tr>
</tbody>
</table>

### Sprint Final — Sprint 7

<table>
<thead>
<tr>
<th>Sprint</th>
<th>Período</th>
<th>Foco</th>
<th>Entregável</th>
</tr>
</thead>
<tbody>
<tr>
<td>**Sprint 7**</td>
<td>**11/10 a 17/10**</td>
<td>Docs + Testes + Seed</td>
<td>Módulo pronto para homologação</td>
</tr>
</tbody>
</table>

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

<table>
<thead>
<tr>
<th>Ordem</th>
<th>Ação</th>
<th>Responsável</th>
<th>Prazo</th>
</tr>
</thead>
<tbody>
<tr>
<td>1</td>
<td>Adicionar testes unitários (cobertura ≥ 60%)</td>
<td>Dev backend + QA</td>
<td>15/10</td>
</tr>
<tr>
<td>2</td>
<td>Adicionar testes de integração</td>
<td>Dev backend + QA</td>
<td>16/10</td>
</tr>
<tr>
<td>3</td>
<td>Seed inicial de permissões</td>
<td>Dev backend</td>
<td>17/10</td>
</tr>
<tr>
<td>4</td>
<td>Homologação com PO</td>
<td>PO + QA</td>
<td>17/10</td>
</tr>
<tr>
<td>5</td>
<td>Início do US02 (Produtos)</td>
<td>Dev backend</td>
<td>18/10</td>
</tr>
<tr>
<td>6</td>
<td>Criar ADRs de arquitetura</td>
<td>Dev backend</td>
<td>18/10</td>
</tr>
<tr>
<td>7</td>
<td>Configurar pipeline CI/CD</td>
<td>DevOps</td>
<td>20/10</td>
</tr>
</tbody>
</table>

---

## 10. Anexos

### Anexo A — Glossário

<table>
<thead>
<tr>
<th>Termo</th>
<th>Definição</th>
</tr>
</thead>
<tbody>
<tr>
<td>CQRS</td>
<td>Command Query Responsibility Segregation</td>
</tr>
<tr>
<td>DTO</td>
<td>Data Transfer Object</td>
</tr>
<tr>
<td>JWT</td>
<td>JSON Web Token</td>
</tr>
<tr>
<td>TPT</td>
<td>Table-per-Type</td>
</tr>
<tr>
<td>Warmup</td>
<td>Pré-compilação JIT em background</td>
</tr>
<tr>
<td>Correlation ID</td>
<td>Identificador único propagado na requisição</td>
</tr>
</tbody>
</table>

### Anexo B — ADRs

- [ADR-001 — Modular Monolith](../adr/001-modular-monolith.md)
- [ADR-002 — Person como Shared Kernel](../adr/002-person-shared-kernel.md)
- [ADR-003 — BaseRepository](../adr/003-base-repository.md)
- [ADR-004 — Feature Folders](../adr/004-feature-folders.md)
- [ADR-005 — TPT Inheritance](../adr/005-tpt-inheritance.md)
- [ADR-006 — Auditoria Centralizada](../adr/006-centralized-auditing.md)

### Anexo C — Riscos Mapeados

<table>
<thead>
<tr>
<th>Risco</th>
<th>Impacto</th>
<th>Mitigação</th>
</tr>
</thead>
<tbody>
<tr>
<td>Migração de `Person` para módulo Cadastros</td>
<td>Médio</td>
<td>Executar antes do US02</td>
</tr>
<tr>
<td>Crescimento da base de auditoria</td>
<td>Médio</td>
<td>Índices + política de retenção</td>
</tr>
<tr>
<td>Acoplamento entre módulos futuros</td>
<td>Alto</td>
<td>Providers públicos antes do US02</td>
</tr>
<tr>
<td>Cobertura de testes insuficiente</td>
<td>Médio</td>
<td>Meta mínima 60%</td>
</tr>
</tbody>
</table>

---

**Prazo executivo sugerido: 17/10/2026**

---

_SJInovacao.Acesso — Backlog e Planejamento de Sprints · Módulo US01 · Versão 2.0 · 10/10/2026_