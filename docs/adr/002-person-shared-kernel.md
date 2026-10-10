# ADR-002 — Person como Shared Kernel

- **Status:** Aceito
- **Data:** 09/10/2026
- **Decisores:** Tech Lead, Arquiteto, PO

## Contexto

A entidade `Person` (com `Name`, `Document`, `Addresses`, `Phones`) é usada por **múltiplos módulos**:

- **Acesso** — `User` herda de `Person`
- **Cliente** — `Customer` referencia `Person`
- **Fornecedores** — `Supplier` referencia `Person`
- **Colaboradores** — `Employee` referencia `Person`

O modelo atual usa **TPT (Table-per-Type)** onde cada role herda de `Person`. Isso gera:

- Acoplamento forte entre módulos
- Duplicação se uma pessoa tem múltiplos papéis (cliente E usuário)
- Dificuldade de evolução independente

## Decisão

Adotar **`Person` como Shared Kernel**, extraída para um módulo **`Cadastros`** dedicado. Cada role (`User`, `Customer`, `Supplier`, `Employee`) **referencia** `Person` por ID, sem herança.

```
Cadastros (módulo novo)
  ├── Person (raiz)
  ├── Address (1:N)
  └── Phone (1:N)

Acesso         → User { PersonId, Password, ... }
Cliente        → Customer { PersonId, LimiteCredito, ... }
Fornecedores   → Supplier { PersonId, PrazoPagamento, ... }
Colaboradores  → Employee { PersonId, Salario, ... }
```

Comunicação via contrato público:

```csharp
public interface ICadastroProvider
{
    Task<Guid> CriarPessoaAsync(PessoaInput input, CancellationToken ct);
    Task<PessoaResumo?> ObterAsync(Guid personId, CancellationToken ct);
    Task AtualizarEnderecosAsync(Guid personId, List<EnderecoInput> enderecos, CancellationToken ct);
}
```

## Consequências

### Positivas

- ✅ Uma única `Person` pode ter múltiplos papéis
- ✅ Endereço/telefone cadastrado uma vez, disponível em todos os módulos
- ✅ Módulos ficam isolados (só conhecem `PersonId`)
- ✅ Adicionar novo papel (ex: `Motorista`, `Vendedor`) não impacta os outros
- ✅ Preparado para extração futura como microsserviço
- ✅ LGPD: dados cadastrais centralizados facilitam gestão

### Negativas

- ⚠️ Requer refactor do modelo atual (TPT → referência)
- ⚠️ Precisa de migration de dados
- ⚠️ Mais uma camada de indireção
- ⚠️ Transações cross-module se tornam explícitas

### Mitigações

- Migração incremental (2 deploys, sem downtime)
- `ICadastroProvider` abstrai a complexidade
- Views materializadas para relatórios cross-module

## Plano de Migração

<table>
<thead>
<tr>
<th>Fase</th>
<th>Ação</th>
<th>Duração</th>
</tr>
</thead>
<tbody>
<tr>
<td>1</td>
<td>Criar módulo `Cadastros`</td>
<td>3 dias</td>
</tr>
<tr>
<td>2</td>
<td>Migrar `Person`, `Address`, `Phone`</td>
<td>2 dias</td>
</tr>
<tr>
<td>3</td>
<td>Refatorar `User` para referenciar `Person`</td>
<td>2 dias</td>
</tr>
<tr>
<td>4</td>
<td>Refatorar futuros `Customer`, `Supplier`, `Employee`</td>
<td>Por módulo</td>
</tr>
<tr>
<td>5</td>
<td>Remover TPT antigo</td>
<td>1 dia</td>
</tr>
</tbody>
</table>

## Referências

- [Shared Kernel — Eric Evans, DDD](https://martinfowler.com/bliki/BoundedContext.html)
- [Backlog US01](../backlog/us01-backlog.md)