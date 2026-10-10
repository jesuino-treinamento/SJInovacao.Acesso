# ADR-004 — Feature Folders vs. Layered Folders

- **Status:** Aceito
- **Data:** 04/10/2026
- **Decisores:** Tech Lead, Dev Backend

## Contexto

O módulo Acesso tem **~40 casos de uso** (Commands + Queries). Organizar por camada (`Commands/`, `Handlers/`, `Validators/`, `Profiles/`) fragmenta o código relacionado em 4 pastas diferentes.

## Decisão

Adotar **Feature Folders** — agrupar todos os arquivos de um caso de uso em uma pasta por feature.

### Estrutura de cada feature

```
Application/
└── GroupUsersPermissions/
    └── CreateGroupUsersPermission/
        ├── CreateGroupUsersPermissionCommand.cs
        ├── CreateGroupUsersPermissionHandler.cs
        ├── CreateGroupUsersPermissionValidator.cs
        └── CreateGroupUsersPermissionProfile.cs
```

## Consequências

### Positivas

- ✅ **Coesão:** tudo relacionado ao "CreateUser" está junto
- ✅ **Navegação:** localizar arquivo por nome de caso de uso
- ✅ **Escala:** adicionar feature = adicionar pasta, não editar 4 lugares
- ✅ **Deleção:** apagar feature = apagar pasta
- ✅ **Code review:** diff focado em uma funcionalidade

### Negativas

- ⚠️ Duplicação de estrutura (mesma estrutura em N pastas)
- ⚠️ Time precisa conhecer o padrão

### Mitigações

- Templates de feature prontos no IDE
- Code review como gate

## Comparação

<table>
<thead>
<tr>
<th>Aspecto</th>
<th>Layered Folders</th>
<th>Feature Folders</th>
</tr>
</thead>
<tbody>
<tr>
<td>Localizar "CreateUser"</td>
<td>4 pastas</td>
<td>1 pasta</td>
</tr>
<tr>
<td>Adicionar caso de uso</td>
<td>Editar 4 pastas</td>
<td>Adicionar 1 pasta</td>
</tr>
<tr>
<td>Coesão</td>
<td>Baixa</td>
<td>**Alta**</td>
</tr>
<tr>
<td>Navegação IDE</td>
<td>Fragmentada</td>
<td>**Fluida**</td>
</tr>
</tbody>
</table>

## Referências

- [Feature Folders — Jimmy Bogard](https://jimmybogard.com/vertical-slice-architecture/)
- [Backlog US01](../backlog/us01-backlog.md)