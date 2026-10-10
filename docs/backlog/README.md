# Backlog — Índice Geral

Este diretório centraliza o planejamento de sprints e o backlog de todos os módulos do sistema **SJInovacao.Acesso**.

## 📚 Backlogs Disponíveis

| Módulo | Documento | Status | Última Atualização |
| :----- | :-------- | :----- | :----------------- |
| US01 — Acesso | [us01-backlog.md](./us01-backlog.md) | ✅ 96% | 10/10/2026 |
| US02 — Produtos | _(a criar)_ | 🔴 Planejado | — |
| US03 — Cliente | _(a criar)_ | 🔴 Planejado | — |
| US04 — Colaboradores | _(a criar)_ | 🔴 Planejado | — |
| US05 — Fornecedores | _(a criar)_ | 🔴 Planejado | — |
| US06 — Vendas | _(a criar)_ | 🔴 Planejado | — |
| US07 — Financeiro | _(a criar)_ | 🔴 Planejado | — |
| US08 — RH | _(a criar)_ | 🔴 Planejado | — |
| US09 — Relatórios | _(a criar)_ | 🔴 Planejado | — |

## 🎯 Convenções

### Identificação de itens

- `US##-XXX` — User Story do módulo ##, item XXX
- Exemplo: `US01-045` — Módulo Acesso, item 45

### Status

- ✅ **Concluído** — Entregue e testado
- ⚠️ **Parcial** — Implementação incompleta
- 🔴 **Pendente** — Não iniciado
- 🔵 **Planejado** — Backlog futuro

### Priorização

- **Alta** — Bloqueia outros itens ou é crítico para o MVP
- **Média** — Importante mas não bloqueante
- **Baixa** — Melhoria, débito técnico, otimização

## 📋 Estrutura de cada Backlog

Cada arquivo `us##-backlog.md` contém:

1. Sumário Executivo
2. Status Geral
3. Backlog por Épico
4. Resumo Consolidado
5. Priorização e MVP
6. Arquitetura (quando aplicável)
7. Métricas (quando aplicável)
8. Cronograma
9. Recomendações de Gestão
10. Próximos Passos

## 🔄 Processo de Atualização

1. **Durante a sprint:** marcar itens como `Concluído` ou `Parcial`
2. **No fim da sprint:** revisar status geral e cronograma
3. **Commit semântico:**

   ```bash
   git commit -m "docs(backlog): atualiza us01 com conclusão da sprint 6"
   ```

## 🔗 Documentos Relacionados

- [ADRs (Architecture Decision Records)](../adr/README.md)
- [Arquitetura do Projeto](../architecture/overview.md)
- [Documentação de API](../api/endpoints.md)
- [Plano de Testes](../tests/test-plan.md)

## 📌 Contato

- **Tech Lead:** _(a definir)_
- **PO:** _(a definir)_
- **QA:** _(a definir)_

---

_Última atualização: 10/10/2026_