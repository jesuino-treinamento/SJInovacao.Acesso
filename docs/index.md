# Plano de Testes — Módulo US01 (Acesso)



- **Versão:** 1.0

- **Data:** 10/10/2026

- **Módulo:** US01 — Acesso

- **Responsável:** Dev Backend + QA



---



## 📑 Sumário



- [1. Estratégia de Testes](#1-estrategia-de-testes)
- [2. Pirâmide de Testes](#2-piramide-de-testes)
- [3. Ferramentas](#3-ferramentas)
- [4. Testes Unitários](#4-testes-unitarios)
- [5. Testes de Integração](#5-testes-de-integracao)
- [6. Testes de Auditoria](#6-testes-de-auditoria)
- [7. Testes de Performance](#7-testes-de-performance)
- [8. Testes de Segurança](#8-testes-de-segurança)
- [9. Matriz de Cobertura](#-9matriz-de-cobertura)
- [10. CI/CD](#10-cicd)

---

## 1. Estratégia de Testes

### Objetivo

Garantir **confiabilidade, performance e segurança** do módulo Acesso antes da homologação, com **cobertura mínima de 60%** nas camadas críticas.

### Escopo

| Categoria | Incluído | Fora do escopo |
| :--- | :--- | :--- |
| Domínio | Entidades, Value Objects, regras | — |
| Aplicação | Handlers, Validators, Profiles | — |
| Infraestrutura | Repositories | Migrations (testadas via integração) |
| API | Controllers, Middlewares | Swagger UI |
| Segurança | JWT, BCrypt, Autorização | — |
| Auditoria | Captura, Mascaramento | — |

---

## 2. Pirâmide de Testes



```

       ▲
      ╱ ╲        E2E (manual via Swagger)
     ╱───╲
    ╱ Int ╲      20% — Integração (banco real)
   ╱───────╲
  ╱  Unit   ╲    80% — Unidade (rápidos, isolados)
 ╱───────────╲

```
---

## 3. Ferramentas

| Ferramenta | Uso |
| :--- | :--- |
| xUnit | Framework de testes |
| FluentAssertions | Assertions legíveis |
| Moq | Mock de dependências |
| Testcontainers | Banco PostgreSQL real em integração |
| Coverlet | Cobertura de código |
| ReportGenerator | Relatórios de cobertura |

---

## 4. Testes Unitários

**Local:** `src/Tests/SJInovacao.Acesso.UnitTests/`

### 1. Domínio — Entidades

#### `UserTests.cs`

| Teste | Descrição |
| :--- | :--- |
| `Create_ComDadosValidos_DeveCriarUsuario` | Validar construtor |
| `Create_SemUsername_DeveLancarArgumentNullException` | Validação de parâmetro |
| `Deactivate_DeveMudarStatusParaInactive` | Regra de negócio |
| `Deactivate_DevePreencherUpdatedAt` | Auditoria implícita |
| `Permissions_DeveRetornarPermissoesDiretasEGrupos` | Agregação de permissões |

#### `PermissionTests.cs`

| Teste | Descrição |
| :--- | :--- |
| `Create_ComNomeValido_DeveCriar` | Construtor |
| `Update_DeveMudarDescricao` | Mutabilidade controlada |

### 2. Aplicação — Handlers

#### `CreateUserHandlerTests.cs`

```csharp

[Fact]
public async Task Handle_ComDadosValidos_DeveCriarUsuario()
{
   // Arrange
   var command = new CreateUserCommand("marcelo", "m@empresa.com", "Senha@123", UserRole.Admin);
   _repoMock.Setup(r => r.GetNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
   _repoMock.Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User u, CancellationToken _) => u);

   // Act
   var result = await _handler.Handle(command, CancellationToken.None);

   // Assert
   result.Should().NotBeNull();
   result.Username.Should().Be("marcelo");
   _repoMock.Verify(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
}

[Fact]
public async Task Handle_ComUsernameDuplicado_DeveLancarDomainException()
{
   // Arrange
   _repoMock.Setup(r => r.GetNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
   var command = new CreateUserCommand("marcelo", "m@empresa.com", "Senha@123", UserRole.Admin);

   // Act
   Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

   // Assert
   await act.Should().ThrowAsync<DomainException>();
   _repoMock.Verify(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
}

```

### 3. Aplicação — Validators

#### `CreateUserValidatorTests.cs`

```csharp

[Theory]
[InlineData("", "email@valido.com", "Senha@123", false)]     // username vazio
[InlineData("marcelo", "email-invalido", "Senha@123", false)] // email inválido
[InlineData("marcelo", "email@valido.com", "123", false)]     // senha fraca
[InlineData("marcelo", "email@valido.com", "Senha@123", true)] // válido
public void Validate_CenariosVariados(string username, string email, string password, bool esperado)
{
   var validator = new CreateUserValidator();
   var command = new CreateUserCommand(username, email, password, UserRole.Admin);
   var result = validator.Validate(command);
   result.IsValid.Should().Be(esperado);
}

```

### 4. Domínio — Value Objects

| Classe | Testes |
| :--- | :--- |
| `DocumentValidator` | CPF válido, CPF inválido, CNPJ válido, CNPJ inválido |
| `EmailValidator` | Email válido, inválido, vazio, formato incorreto |
| `NameValidator` | Nome com 2 partes, 1 parte, vazio, caracteres especiais |
| `PhoneValidator` | Fixo, celular, internacional |
| `PasswordValidator` | Mínimo, maiúscula, número, símbolo |

---

## 5. Testes de Integração

**Local:** `src/Tests/SJInovacao.Acesso.IntegrationTests/`

### Configuração com Testcontainers

```csharp

public class PostgresFixture : IAsyncLifetime
{
   private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
       .WithImage("postgres:16")
       .WithDatabase("silverjbase_test")
       .WithUsername("developer")
       .WithPassword("ev@luAt10n")
       .Build();

   public DefaultContext Context { get; private set; } = null!;
   public async Task InitializeAsync()
   {
       await _postgres.StartAsync();
       var options = new DbContextOptionsBuilder<DefaultContext>()
           .UseNpgsql(_postgres.GetConnectionString())
           .Options;
       Context = new DefaultContext(options, NullLogger...);
       await Context.Database.MigrateAsync();
   }

   public async Task DisposeAsync()
   {
       await Context.DisposeAsync();
       await _postgres.DisposeAsync();
   }
}
```

### Suites Planejadas

#### `UserEndpointsTests.cs`

| Teste | Endpoint |
| :--- | :--- |
| `Post_CriaUsuario_Retorna201` | `POST /api/Users` |
| `Get_ListaUsuariosPaginado_Retorna200` | `GET /api/Users?page=1\&size=10` |
| `Get_UsuarioInexistente_Retorna404` | `GET /api/Users/{id}` |
| `Put_AtualizaUsuario_Retorna200` | `PUT /api/Users/{id}` |
| `Delete_DesativaUsuario_Retorna204` | `DELETE /api/Users/{id}` |

#### `AuthEndpointsTests.cs`

| Teste | Endpoint |
| :--- | :--- |
| `Post_LoginValido_RetornaToken` | `POST /api/Auth/Login` |
| `Post_LoginInvalido_Retorna401` | `POST /api/Auth/Login` |
| `Post_LoginUsuarioInativo_Retorna401` | `POST /api/Auth/Login` |
| `Post_RefreshToken_RetornaNovoToken` | `POST /api/Auth/RefreshToken` |

#### `PermissionEndpointsTests.cs`

| Teste | Endpoint |
| :--- | :--- |
| `Post_CriaPermissao_Retorna201` | `POST /api/Permissions` |
| `Post_PermissaoDuplicada_Retorna400` | `POST /api/Permissions` |
| `Get_PermissaoInexistente_Retorna404` | `GET /api/Permissions/{id}` |

#### `OrderingTests.cs`

| Teste | Query |
| :--- | :--- |
| `Get_OrdenacaoSimples_RetornaOrdenado` | `?order=username asc` |
| `Get_OrdenacaoMultipla_RetornaOrdenado` | `?order=username desc, groupname asc` |
| `Get_CampoInvalido_Retorna400` | `?order=campo_invalido asc` |

---

## 6. Testes de Auditoria

### `AuditServiceTests.cs`

| Teste | Descrição |
| :--- | :--- |
| `CaptureChanges_Added_DeveCriarRegistro` | Insert gera `Action=Added` |
| `CaptureChanges_Modified_DeveRegistrarColunasAlteradas` | Update captura `AffectedColumns` |
| `CaptureChanges_Deleted_DeveRegistrarOldValues` | Delete captura valores antigos |
| `CaptureChanges_PasswordMascarado` | `Password` virou `***` |
| `CaptureChanges_RefreshTokenMascarado` | `RefreshToken` virou `***` |
| `CaptureChanges_SemAlteracao_NaoGeraRegistro` | Skip de "Modified falso" |
| `CaptureChanges_MultiplasEntidades_GeraMultiplosRegistros` | Batch |

### Exemplo

```csharp

[Fact]
public async Task CaptureChanges_ComPassword_DeveMascarar()
{
   // Arrange
   var user = new User("marcelo", "m@e.com", "SenhaHash", UserRole.Admin, ...);
   _context.Users.Add(user);
   await _context.SaveChangesAsync();
   // Act
   var logs = await _context.AuditLogs
       .Where(a => a.EntityName == "User" \&\& a.Action == "Added")
       .FirstAsync();
   // Assert
   logs.NewValues.Should().NotContain("SenhaHash");
   logs.NewValues.Should().Contain("\\"Password\\":\\"***\\"");
}

```

---

## 7. Testes de Performance

### Cenários

| Cenário | Meta | Ferramenta |
| :--- | :--- | :--- |
| Query paginada (morna) | < 50ms | Swagger + Stopwatch |
| Query paginada (1000 users) | < 100ms | k6 |
| Login | < 200ms | k6 |
| Criação de usuário | < 300ms | k6 |
| Startup (warmup) | < 5s | Benchmark manual |

### Teste de carga (k6)

```javascript

import http from 'k6/http';
import { check } from 'k6';

export const options = {
 stages: [
   { duration: '30s', target: 10 },
   { duration: '1m', target: 50 },
   { duration: '30s', target: 0 },
 ],
};

export default function () {
 const res = http.get(
   'https://localhost:44382/api/UserGroups/GroupUsersPermissions/paginated?page=1\&size=10\&order=username%20asc',
   {
     headers: {
       Authorization: `Bearer ${__ENV.TOKEN}`,
     },
   }
 );
 check(res, {
   'status is 200': (r) => r.status === 200,
   'duration < 100ms': (r) => r.timings.duration < 100,
 });
}

```
---

## 8. Testes de Segurança

### `AuthenticationTests.cs`

| Teste | Cenário |
| :--- | :--- |
| `Request_SemToken_Retorna401` | Endpoint protegido sem header |
| `Request_TokenInvalido_Retorna401` | JWT com assinatura errada |
| `Request_TokenExpirado_Retorna401` | Token expirado |
| `Request_TokenValidoComPermissao_Retorna200` | Autorização OK |
| `Request_TokenValidoSemPermissao_Retorna403` | Policy bloqueou |

### `BCryptPasswordHasherTests.cs`

| Teste | Descrição |
| :--- | :--- |
| `Hash_DeveGerarHashDiferenteParaMesmaSenha` | Salt aleatório |
| `Verify_HashValido_RetornaTrue` | Autenticação correta |
| `Verify_HashInvalido_RetornaFalse` | Autenticação incorreta |

### `CorrelationIdTests.cs`

| Teste | Descrição |
| :--- | :--- |
| `Request_SemHeader_GeraCorrelationId` | Middleware gera |
| `Request_ComHeader_UsaHeaderFornecido` | Middleware respeita |
| `Response_ContemHeaderCorrelationId` | Header na resposta |

---

## 9. Matriz de Cobertura

### Metas

| Camada | Meta | Atual | Status |
| :--- | ---: | ----: | :----- |
| Domain | 80% | 0% | 🔴 |
| Application (Handlers) | 70% | 0% | 🔴 |
| Application (Validators) | 90% | 0% | 🔴 |
| Infrastructure (Repositories) | 60% | 0% | 🔴 |
| Common (Auditing, Security) | 70% | 0% | 🔴 |
| **Total** | **60%** | **0%** | 🔴 |

### Gerar relatório

```bash

dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

reportgenerator -reports:**/coverage.opencover.xml -targetdir:coverage-report

```

---

## 10. CI/CD

### Pipeline sugerido (GitHub Actions)

```yaml

name: CI



on:

 push:
   branches: [main, develop]
 pull_request:
   branches: [main]



jobs:

 build:
   runs-on: ubuntu-latest
   steps:
     - uses: actions/checkout@v4
     - uses: actions/setup-dotnet@v4
       with:
         dotnet-version: '9.0.x'

     - name: Restore
       run: dotnet restore

     - name: Build (warn as error)
       run: dotnet build --no-restore /warnaserror

     - name: Unit Tests
       run: dotnet test src/Tests/SJInovacao.Acesso.UnitTests --no-build

     - name: Integration Tests
       run: dotnet test src/Tests/SJInovacao.Acesso.IntegrationTests --no-build

     - name: Coverage Report
       run: |
         dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

         reportgenerator -reports:**/coverage.opencover.xml -targetdir:coverage-report

     - name: Upload Coverage

       uses: codecov/codecov-action@v4
       with:
         files: ./coverage-report/Cobertura.xml

```
### Gates de qualidade

| Gate | Requisito |
| :--- | :--- |
| Build | 0 erros, 0 avisos |
| Testes unitários | 100% passando |
| Testes de integração | 100% passando |
| Cobertura | ≥ 60% |
| Vulnerabilidades | 0 críticas |

---

## 🎯 Critérios de Aceite para Homologação



- [ ] Cobertura total ≥ 60%
- [ ] Todos os testes passando em CI
- [ ] Zero avisos no build
- [ ] Testes de carga com resultado < 100ms (p95)
- [ ] Testes de segurança validados
- [ ] Teste de auditoria validado em todos os tipos de operação

---

## 📌 Comandos Rápidos

```bash

# Rodar todos os testes
dotnet test

# Rodar apenas unit tests
dotnet test src/Tests/SJInovacao.Acesso.UnitTests

# Rodar apenas integration tests
dotnet test src/Tests/SJInovacao.Acesso.IntegrationTests

# Com cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Um teste específico
dotnet test --filter "FullyQualifiedName~CreateUserHandlerTests"

# Watch mode (durante desenvolvimento)
dotnet watch --project src/Tests/SJInovacao.Acesso.UnitTests test

```
---

**Plano de Testes — US01 · Versão 1.0 · 10/10/2026**

