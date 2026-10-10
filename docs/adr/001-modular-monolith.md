# ADR-001 — Adoção de Modular Monolith

- **Status:** Aceito
- **Data:** 03/10/2026
- **Decisores:** Tech Lead, Arquiteto

## Contexto

O sistema **SJInovacao.Acesso** precisa suportar **9 módulos de negócio** (Acesso, Produtos, Cliente, Colaboradores, Fornecedores, Vendas, Financeiro, RH, Relatórios) em uma base de código única.

A escolha entre **Monolith**, **Modular Monolith** ou **Microsserviços** impacta diretamente:

- Complexidade operacional
- Velocidade de entrega inicial
- Custo de infraestrutura
- Facilidade de manutenção

## Decisão

Adotar **Modular Monolith** com as seguintes características:

- **1 deploy único** (única WebAPI)
- **Módulos isolados logicamente** por pastas e namespaces
- **3 projetos por módulo** (Domain, Application, Infrastructure.ORM)
- **Comunicação entre módulos** via contratos públicos (`IProvider`)
- **Banco único PostgreSQL** com schemas separados por módulo

## Consequências

### Positivas

- ✅ Deploy único simplifica CI/CD
- ✅ Transações ACID entre módulos (quando necessário)
- ✅ Sem latência de rede entre módulos
- ✅ Debug e tracing simplificados
- ✅ Baixo custo inicial de infraestrutura
- ✅ Fácil extração futura para microsserviços (módulos já isolados)

### Negativas

- ⚠️ Deploy acoplado (um bug pode derrubar todo o sistema)
- ⚠️ Escala vertical apenas (não horizontal por módulo)
- ⚠️ Requer disciplina para manter isolamento
- ⚠️ Build time cresce com o número de módulos

### Mitigações

- Contratos públicos (`IProvider`) impedem acoplamento direto
- Testes de arquitetura validam limites entre módulos
- Docker Compose permite isolar serviços de infraestrutura

## Alternativas Consideradas

<table>
<thead>
<tr>
<th>Alternativa</th>
<th>Por que foi rejeitada</th>
</tr>
</thead>
<tbody>
<tr>
<td>Monolith tradicional</td>
<td>Falta de isolamento dificulta manutenção</td>
</tr>
<tr>
<td>Microsserviços</td>
<td>Complexidade operacional alta para time pequeno</td>
</tr>
<tr>
<td>Serverless</td>
<td>Custo imprevisível e modelo não se aplica ao domínio</td>
</tr>
</tbody>
</table>

## Referências

- [Modular Monoliths — Simon Brown](https://www.youtube.com/watch?v=5OjqD-ow8GE)
- [Monolith to Microservices — Sam Newman](https://samnewman.io/books/monolith-to-microservices/)