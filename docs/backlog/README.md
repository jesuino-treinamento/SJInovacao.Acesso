# Backlog — Índice Geral

Este diretório centraliza o planejamento de sprints e o backlog de todos os módulos do sistema **SJInovacao.Acesso**.

## 📚 Backlogs Disponíveis

<table>
<thead>
<tr>
<th>Módulo</th>
<th>Documento</th>
<th>Status</th>
<th>Última Atualização</th>
</tr>
</thead>
<tbody>
<tr>
<td>US01 — Acesso</td>
<td>[us01-backlog.md](./us01-backlog.md)</td>
<td>✅ 96%</td>
<td>10/10/2026</td>
</tr>
<tr>
<td>US02 — Produtos</td>
<td>_(a criar)_</td>
<td>🔴 Planejado</td>
<td>—</td>
</tr>
<tr>
<td>US03 — Cliente</td>
<td>_(a criar)_</td>
<td>🔴 Planejado</td>
<td>—</td>
</tr>
<tr>
<td>US04 — Colaboradores</td>
<td>_(a criar)_</td>
<td>🔴 Planejado</td>
<td>—</td>
</tr>
<tr>
<td>US05 — Fornecedores</td>
<td>_(a criar)_</td>
<td>🔴 Planejado</td>
<td>—</td>
</tr>
<tr>
<td>US06 — Vendas</td>
<td>_(a criar)_</td>
<td>🔴 Planejado</td>
<td>—</td>
</tr>
<tr>
<td>US07 — Financeiro</td>
<td>_(a criar)_</td>
<td>🔴 Planejado</td>
<td>—</td>
</tr>
<tr>
<td>US08 — RH</td>
<td>_(a criar)_</td>
<td>🔴 Planejado</td>
<td>—</td>
</tr>
<tr>
<td>US09 — Relatórios</td>
<td>_(a criar)_</td>
<td>🔴 Planejado</td>
<td>—</td>
</tr>
</tbody>
</table>

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