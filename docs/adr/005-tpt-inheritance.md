# ADR-005 — Estratégia de Herança: TPT



- **Status:** Em revisão (aguardando ADR-002)

- **Data:** 10/10/2026

- **Decisores:** Tech Lead, Arquiteto



## Contexto



O modelo de dados tem herança entre `Person` e seus roles (`User`, `Customer`, `Supplier`, `Employee`). O EF Core oferece 3 estratégias:



| Estratégia | Tabelas                    | Query     | Insert/Update |

| :--------- | :------------------------- | :-------- | :------------ |

| TPH        | 1 tabela (com discriminator) | Rápida    | Rápida        |

| TPT        | 1 tabela por tipo          | Lenta (JOINs) | Rápida    |

| TPC        | 1 tabela por tipo (completa)| Lenta    | Rápida        |



## Decisão Atual



**TPT** — `User` herda de `Person` com `User.Id = Person.Id`.



## Problemas Identificados



1. **Acoplamento:** `User` (Acesso) depende de `Person` (base compartilhada)

2. **Duplicação:** se a mesma pessoa é cliente E usuário, precisa ser 2 `Persons`

3. **JOIN pesado:** toda query em `User` faz `INNER JOIN Persons`

4. **Escala ruim:** cada novo role adiciona herança na hierarquia



## Decisão Proposta



Migrar para **referência** (não herança) — ver **ADR-002**.



```

Person (raiz isolada)

&#x20; ↑

&#x20; ├─ User { Id, PersonId, ... }

&#x20; ├─ Customer { Id, PersonId, ... }

&#x20; └─ Employee { Id, PersonId, ... }

```



Cada role **referencia** `Person` por FK, sem herança.



## Consequências



### Se mantiver TPT



- ⚠️ Acoplamento crescente entre módulos

- ⚠️ Bug do "cliente que também é usuário" continua

- ✅ Menor refactor agora



### Se migrar para referência (ADR-002)



- ✅ Isolamento correto entre módulos

- ✅ Pessoa com múltiplos papéis sem duplicação

- ✅ Preparado para extração de microsserviço

- ⚠️ Refactor + migration necessários



## Recomendação



**Migrar para referência** — o custo se paga rapidamente na escala de 9 módulos.



## Referências



- [ADR-002 — Person como Shared Kernel](002-person-shared-kernel.md)

- [Inheritance — EF Core Docs](https://learn.microsoft.com/en-us/ef/core/modeling/inheritance)

