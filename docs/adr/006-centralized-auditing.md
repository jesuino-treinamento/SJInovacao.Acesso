\# ADR-006 — Auditoria Centralizada



\- \*\*Status:\*\* Aceito

\- \*\*Data:\*\* 10/10/2026

\- \*\*Decisores:\*\* Tech Lead, Dev Backend, Compliance



\## Contexto



Requisitos de auditoria:



\- \*\*LGPD:\*\* rastrear quem acessa/alterou dados pessoais

\- \*\*Compliance:\*\* histórico de alterações de permissões

\- \*\*Debug:\*\* entender o que mudou e quando

\- \*\*Rastreabilidade:\*\* correlacionar alteração com requisição HTTP



Sem padronização, cada módulo implementaria sua própria auditoria — inconsistente e cara.



\## Decisão



Auditoria \*\*centralizada no `Common`\*\* com interceptação automática no `SaveChangesAsync`.



\### Componentes



```

Common/Auditing/

├── IAuditable.cs              ← marker interface

├── AuditLog.cs                ← entidade (tabela AuditLogs)

├── IAuditService.cs           ← contrato

├── AuditService.cs            ← implementação

└── AuditLogEntityConfiguration.cs

```



\### Fluxo



```

Request HTTP

&#x20;   ↓

CorrelationIdMiddleware       → gera correlationId

&#x20;   ↓

Controller → Handler → Repository

&#x20;   ↓

SaveChangesAsync

&#x20;   ↓

AuditService.CaptureChanges()  → captura alterações em IAuditable

&#x20;   ↓

AuditLogs.AddRange(...)        → grava na mesma transação

&#x20;   ↓

Commit

```



\### Regras



\- \*\*Auditar:\*\* entidades que implementam `IAuditable`

\- \*\*Mascarar:\*\* `Password`, `RefreshToken`, `PasswordHash`, `SecurityStamp`, `ConcurrencyStamp`

\- \*\*Ignorar:\*\* `RowVersion`, `xmin`

\- \*\*Skip:\*\* se `OldValues == NewValues` (Modified falso)

\- \*\*Formato:\*\* `OldValues`, `NewValues` em JSONB

\- \*\*Metadata:\*\* UserId, UserName, IpAddress, UserAgent, CorrelationId, Timestamp



\## Consequências



\### Positivas



\- ✅ \*\*Zero esforço por módulo\*\* — adicionar `: IAuditable` e pronto

\- ✅ \*\*Consistência\*\* em todas as tabelas

\- ✅ \*\*LGPD ready\*\* — dados sensíveis mascarados

\- ✅ \*\*Rastreável\*\* — CorrelationId em cada registro

\- ✅ \*\*Atomicidade\*\* — audit + alteração na mesma transação

\- ✅ \*\*Performance\*\* — Skip de "Modified falso" evita poluição



\### Negativas



\- ⚠️ Overhead por `SaveChanges` (\~1-2ms em bases pequenas)

\- ⚠️ Crescimento da tabela (mitigado por índices + retenção)

\- ⚠️ Custo de armazenamento (JSONB)



\### Mitigações



\- Índices otimizados: `EntityName/EntityId`, `Timestamp DESC`, `CorrelationId`

\- Política de retenção: 2 anos + arquivamento

\- Consultas paginadas obrigatórias



\## Schema



```sql

CREATE TABLE "AuditLogs" (

&#x20;   "Id"              UUID PRIMARY KEY DEFAULT gen\_random\_uuid(),

&#x20;   "EntityName"      VARCHAR(200) NOT NULL,

&#x20;   "EntityId"        VARCHAR(100) NOT NULL,

&#x20;   "Action"          VARCHAR(20)  NOT NULL,  -- Added | Modified | Deleted

&#x20;   "OldValues"       JSONB,

&#x20;   "NewValues"       JSONB,

&#x20;   "AffectedColumns" TEXT,

&#x20;   "UserId"          UUID,

&#x20;   "UserName"        VARCHAR(200),

&#x20;   "IpAddress"       VARCHAR(45),

&#x20;   "UserAgent"       VARCHAR(500),

&#x20;   "CorrelationId"   VARCHAR(100),

&#x20;   "Timestamp"       TIMESTAMPTZ NOT NULL DEFAULT CURRENT\_TIMESTAMP

);

```



\## Exemplo de Registro



```json

{

&#x20; "EntityName": "Permission",

&#x20; "Action": "Modified",

&#x20; "AffectedColumns": "Email",

&#x20; "OldValues": { "Email": "antigo@empresa.com" },

&#x20; "NewValues": { "Email": "novo@empresa.com" },

&#x20; "UserName": "marcelo.jesuino",

&#x20; "CorrelationId": "00-c41757...-06ccf47e...",

&#x20; "Timestamp": "2026-10-10T17:30:00.928Z"

}

```



\## Referências



\- \[AuditService.cs](../../src/SJInovacao.Acesso.Common/Auditing/AuditService.cs)

\- \[Backlog US01](../backlog/us01-backlog.md)

