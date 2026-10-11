# ADR-006 — Auditoria Centralizada



- **Status:** Aceito

- **Data:** 10/10/2026

- **Decisores:** Tech Lead, Dev Backend, Compliance

## Contexto


Requisitos de auditoria:


- **LGPD:** rastrear quem acessa/alterou dados pessoais
- **Compliance:** histórico de alterações de permissões
- **Debug:** entender o que mudou e quando
- **Rastreabilidade:** correlacionar alteração com requisição HTTP

Sem padronização, cada módulo implementaria sua própria auditoria — inconsistente e cara.


## Decisão

Auditoria **centralizada no `Common`** com interceptação automática no `SaveChangesAsync`.


### Componentes


```

Common/Auditing/
├── IAuditable.cs              ← marker interface
├── AuditLog.cs                ← entidade (tabela AuditLogs)
├── IAuditService.cs           ← contrato
├── AuditService.cs            ← implementação
└── AuditLogEntityConfiguration.cs

```

### Fluxo

```

Request HTTP
   ↓
CorrelationIdMiddleware       → gera correlationId
   ↓
Controller → Handler → Repository
   ↓
SaveChangesAsync
   ↓
AuditService.CaptureChanges()  → captura alterações em IAuditable
   ↓
AuditLogs.AddRange(...)        → grava na mesma transação
   ↓
Commit

```

### Regras


- **Auditar:** entidades que implementam `IAuditable`
- **Mascarar:** `Password`, `RefreshToken`, `PasswordHash`, `SecurityStamp`, `ConcurrencyStamp`
- **Ignorar:** `RowVersion`, `xmin`
- **Skip:** se `OldValues == NewValues` (Modified falso)
- **Formato:** `OldValues`, `NewValues` em JSONB
- **Metadata:** UserId, UserName, IpAddress, UserAgent, CorrelationId, Timestamp


## Consequências


### Positivas


- ✅ **Zero esforço por módulo** — adicionar `: IAuditable` e pronto
- ✅ **Consistência** em todas as tabelas
- ✅ **LGPD ready** — dados sensíveis mascarados
- ✅ **Rastreável** — CorrelationId em cada registro
- ✅ **Atomicidade** — audit + alteração na mesma transação
- ✅ **Performance** — Skip de "Modified falso" evita poluição


### Negativas

- ⚠️ Overhead por `SaveChanges` (~1-2ms em bases pequenas)
- ⚠️ Crescimento da tabela (mitigado por índices + retenção)
- ⚠️ Custo de armazenamento (JSONB)


### Mitigações


- Índices otimizados: `EntityName/EntityId`, `Timestamp DESC`, `CorrelationId`
- Política de retenção: 2 anos + arquivamento
- Consultas paginadas obrigatórias


## Schema


```sql

CREATE TABLE "AuditLogs" (
   "Id"              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
   "EntityName"      VARCHAR(200) NOT NULL,
   "EntityId"        VARCHAR(100) NOT NULL,
   "Action"          VARCHAR(20)  NOT NULL,  -- Added | Modified | Deleted
   "OldValues"       JSONB,
   "NewValues"       JSONB,
   "AffectedColumns" TEXT,
   "UserId"          UUID,
   "UserName"        VARCHAR(200),
   "IpAddress"       VARCHAR(45),
   "UserAgent"       VARCHAR(500),
   "CorrelationId"   VARCHAR(100),
   "Timestamp"       TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
);

```


## Exemplo de Registro


```json

{
 "EntityName": "Permission",
 "Action": "Modified",
 "AffectedColumns": "Email",
 "OldValues": { "Email": "antigo@empresa.com" },
 "NewValues": { "Email": "novo@empresa.com" },
 "UserName": "marcelo.jesuino",
 "CorrelationId": "00-c41757...-06ccf47e...",
 "Timestamp": "2026-10-10T17:30:00.928Z"
}

```

## Referências

- [AuditService.cs](../../src/SJInovacao.Acesso.Common/Auditing/AuditService.cs)
- [Backlog US01](../backlog/us01-backlog.md)

