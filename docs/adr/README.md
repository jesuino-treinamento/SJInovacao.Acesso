# Architecture Decision Records (ADRs)

Registro das decisões arquiteturais importantes do projeto **SJInovacao.Acesso**.

## 📚 Índice

| ID  | Título                                              | Status   | Data       |
| :-- | :-------------------------------------------------- | :------- | :--------- |
| 001 | [Adoção de Modular Monolith](001-modular-monolith.md) | Aceito | 03/10/2026 |
| 002 | [Person como Shared Kernel](002-person-shared-kernel.md) | Aceito | 09/10/2026 |
| 003 | [BaseRepository com ExecuteWithLoggingAsync](003-base-repository.md) | Aceito | 04/10/2026 |
| 004 | [Feature Folders](004-feature-folders.md)           | Aceito   | 04/10/2026 |
| 005 | [Estratégia de Herança: TPT](005-tpt-inheritance.md) | Em revisão | 10/10/2026 |
| 006 | [Auditoria Centralizada](006-centralized-auditing.md) | Aceito   | 10/10/2026 |

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