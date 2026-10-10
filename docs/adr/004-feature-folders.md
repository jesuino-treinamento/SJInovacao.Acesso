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

| Aspecto                | Layered Folders | Feature Folders |
| :--------------------- | :-------------- | :-------------- |
| Localizar "CreateUser" | 4 pastas        | 1 pasta         |
| Adicionar caso de uso  | Editar 4 pastas | Adicionar 1 pasta |
| Coesão                 | Baixa           | **Alta**        |
| Navegação IDE          | Fragmentada     | **Fluida**      |

## Referências

- [Feature Folders — Jimmy Bogard](https://jimmybogard.com/vertical-slice-architecture/)
- [Backlog US01](../backlog/us01-backlog.md)