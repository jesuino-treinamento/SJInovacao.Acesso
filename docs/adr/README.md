# Architecture Decision Records (ADRs)

Registro das decisões arquiteturais importantes do projeto **SJInovacao.Acesso**.

## 📚 Índice

<table>
<thead>
<tr>
<th>ID</th>
<th>Título</th>
<th>Status</th>
<th>Data</th>
</tr>
</thead>
<tbody>
<tr>
<td>001</td>
<td>[Adoção de Modular Monolith](001-modular-monolith.md)</td>
<td>Aceito</td>
<td>03/10/2026</td>
</tr>
<tr>
<td>002</td>
<td>[Person como Shared Kernel](002-person-shared-kernel.md)</td>
<td>Aceito</td>
<td>09/10/2026</td>
</tr>
<tr>
<td>003</td>
<td>[BaseRepository com ExecuteWithLoggingAsync](003-base-repository.md)</td>
<td>Aceito</td>
<td>04/10/2026</td>
</tr>
<tr>
<td>004</td>
<td>[Feature Folders](004-feature-folders.md)</td>
<td>Aceito</td>
<td>04/10/2026</td>
</tr>
<tr>
<td>005</td>
<td>[Estratégia de Herança: TPT](005-tpt-inheritance.md)</td>
<td>Em revisão</td>
<td>10/10/2026</td>
</tr>
<tr>
<td>006</td>
<td>[Auditoria Centralizada](006-centralized-auditing.md)</td>
<td>Aceito</td>
<td>10/10/2026</td>
</tr>
</tbody>
</table>

## 🎯 O que é um ADR?

Architecture Decision Record é um documento curto que captura **uma decisão arquitetural importante** e seu contexto. Serve para:

- **Documentar o "porquê"** das decisões (não apenas o "o quê")
- **Preservar contexto histórico** para novos membros
- **Facilitar revisões** futuras quando as premissas mudarem
- **Evitar decisões reversas** sem entender as consequências

## 📋 Formato

Cada ADR segue o formato:

- **Título** — Decisão em formato imperativo
- **Status** — Proposto / Aceito / Deprecado / Substituído
- **Contexto** — Situação que exige a decisão
- **Decisão** — O que foi decidido
- **Consequências** — Trade-offs (positivas e negativas)

## 🔄 Status Possíveis

- **Proposto** — Em discussão
- **Aceito** — Decisão tomada e em vigor
- **Deprecado** — Não é mais recomendado
- **Substituído por ADR-XXX** — Trocado por outro

## 🔗 Referências

- [Backlogs](../backlog/README.md)
- [Arquitetura](../architecture/overview.md)

---

_Última atualização: 10/10/2026_