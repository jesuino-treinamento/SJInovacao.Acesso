\# Plano de Testes — Módulo US01 (Acesso)



\- \*\*Versão:\*\* 1.0

\- \*\*Data:\*\* 10/10/2026

\- \*\*Módulo:\*\* US01 — Acesso

\- \*\*Responsável:\*\* Dev Backend + QA



\---



\## 📑 Sumário



\- \[Estratégia de Testes](#-estratégia-de-testes)

\- \[Pirâmide de Testes](#-pirâmide-de-testes)

\- \[Ferramentas](#-ferramentas)

\- \[Testes Unitários](#-testes-unitários)

\- \[Testes de Integração](#-testes-de-integração)

\- \[Testes de Auditoria](#-testes-de-auditoria)

\- \[Testes de Performance](#-testes-de-performance)

\- \[Testes de Segurança](#-testes-de-segurança)

\- \[Matriz de Cobertura](#-matriz-de-cobertura)

\- \[CI/CD](#-cicd)



\---



\## 🎯 Estratégia de Testes



\### Objetivo



Garantir \*\*confiabilidade, performance e segurança\*\* do módulo Acesso antes da homologação, com \*\*cobertura mínima de 60%\*\* nas camadas críticas.



\### Escopo



| Categoria | Incluído | Fora do escopo |

| :--- | :--- | :--- |

| Domínio | Entidades, Value Objects, regras | — |

| Aplicação | Handlers, Validators, Profiles | — |

| Infraestrutura | Repositories | Migrations (testadas via integração) |

| API | Controllers, Middlewares | Swagger UI |

| Segurança | JWT, BCrypt, Autorização | — |

| Auditoria | Captura, Mascaramento | — |



\---



\## 🔺 Pirâmide de Testes



```

&#x20;       ▲

&#x20;      ╱ ╲        E2E (manual via Swagger)

&#x20;     ╱───╲

&#x20;    ╱ Int ╲      20% — Integração (banco real)

&#x20;   ╱───────╲

&#x20;  ╱  Unit   ╲    80% — Unidade (rápidos, isolados)

&#x20; ╱───────────╲

```



\---



\## 🛠 Ferramentas



| Ferramenta | Uso |

| :--- | :--- |

| xUnit | Framework de testes |

| FluentAssertions | Assertions legíveis |

| Moq | Mock de dependências |

| Testcontainers | Banco PostgreSQL real em integração |

| Coverlet | Cobertura de código |

| ReportGenerator | Relatórios de cobertura |



\---



\## 🧪 Testes Unitários



\*\*Local:\*\* `src/Tests/SJInovacao.Acesso.UnitTests/`



\### 1. Domínio — Entidades



\#### `UserTests.cs`



| Teste | Descrição |

| :--- | :--- |

| `Create\_ComDadosValidos\_DeveCriarUsuario` | Validar construtor |

| `Create\_SemUsername\_DeveLancarArgumentNullException` | Validação de parâmetro |

| `Deactivate\_DeveMudarStatusParaInactive` | Regra de negócio |

| `Deactivate\_DevePreencherUpdatedAt` | Auditoria implícita |

| `Permissions\_DeveRetornarPermissoesDiretasEGrupos` | Agregação de permissões |



\#### `PermissionTests.cs`



| Teste | Descrição |

| :--- | :--- |

| `Create\_ComNomeValido\_DeveCriar` | Construtor |

| `Update\_DeveMudarDescricao` | Mutabilidade controlada |



\### 2. Aplicação — Handlers



\#### `CreateUserHandlerTests.cs`



```csharp

\[Fact]

public async Task Handle\_ComDadosValidos\_DeveCriarUsuario()

{

&#x20;   // Arrange

&#x20;   var command = new CreateUserCommand("marcelo", "m@empresa.com", "Senha@123", UserRole.Admin);

&#x20;   \_repoMock.Setup(r => r.GetNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))

&#x20;            .ReturnsAsync(false);

&#x20;   \_repoMock.Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))

&#x20;            .ReturnsAsync((User u, CancellationToken \_) => u);



&#x20;   // Act

&#x20;   var result = await \_handler.Handle(command, CancellationToken.None);



&#x20;   // Assert

&#x20;   result.Should().NotBeNull();

&#x20;   result.Username.Should().Be("marcelo");

&#x20;   \_repoMock.Verify(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);

}



\[Fact]

public async Task Handle\_ComUsernameDuplicado\_DeveLancarDomainException()

{

&#x20;   // Arrange

&#x20;   \_repoMock.Setup(r => r.GetNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))

&#x20;            .ReturnsAsync(true);

&#x20;   var command = new CreateUserCommand("marcelo", "m@empresa.com", "Senha@123", UserRole.Admin);



&#x20;   // Act

&#x20;   Func<Task> act = () => \_handler.Handle(command, CancellationToken.None);



&#x20;   // Assert

&#x20;   await act.Should().ThrowAsync<DomainException>();

&#x20;   \_repoMock.Verify(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);

}

```



\### 3. Aplicação — Validators



\#### `CreateUserValidatorTests.cs`



```csharp

\[Theory]

\[InlineData("", "email@valido.com", "Senha@123", false)]     // username vazio

\[InlineData("marcelo", "email-invalido", "Senha@123", false)] // email inválido

\[InlineData("marcelo", "email@valido.com", "123", false)]     // senha fraca

\[InlineData("marcelo", "email@valido.com", "Senha@123", true)] // válido

public void Validate\_CenariosVariados(string username, string email, string password, bool esperado)

{

&#x20;   var validator = new CreateUserValidator();

&#x20;   var command = new CreateUserCommand(username, email, password, UserRole.Admin);



&#x20;   var result = validator.Validate(command);



&#x20;   result.IsValid.Should().Be(esperado);

}

```



\### 4. Domínio — Value Objects



| Classe | Testes |

| :--- | :--- |

| `DocumentValidator` | CPF válido, CPF inválido, CNPJ válido, CNPJ inválido |

| `EmailValidator` | Email válido, inválido, vazio, formato incorreto |

| `NameValidator` | Nome com 2 partes, 1 parte, vazio, caracteres especiais |

| `PhoneValidator` | Fixo, celular, internacional |

| `PasswordValidator` | Mínimo, maiúscula, número, símbolo |



\---



\## 🔗 Testes de Integração



\*\*Local:\*\* `src/Tests/SJInovacao.Acesso.IntegrationTests/`



\### Configuração com Testcontainers



```csharp

public class PostgresFixture : IAsyncLifetime

{

&#x20;   private readonly PostgreSqlContainer \_postgres = new PostgreSqlBuilder()

&#x20;       .WithImage("postgres:16")

&#x20;       .WithDatabase("silverjbase\_test")

&#x20;       .WithUsername("developer")

&#x20;       .WithPassword("ev@luAt10n")

&#x20;       .Build();



&#x20;   public DefaultContext Context { get; private set; } = null!;



&#x20;   public async Task InitializeAsync()

&#x20;   {

&#x20;       await \_postgres.StartAsync();

&#x20;       var options = new DbContextOptionsBuilder<DefaultContext>()

&#x20;           .UseNpgsql(\_postgres.GetConnectionString())

&#x20;           .Options;

&#x20;       Context = new DefaultContext(options, NullLogger...);

&#x20;       await Context.Database.MigrateAsync();

&#x20;   }



&#x20;   public async Task DisposeAsync()

&#x20;   {

&#x20;       await Context.DisposeAsync();

&#x20;       await \_postgres.DisposeAsync();

&#x20;   }

}

```



\### Suites Planejadas



\#### `UserEndpointsTests.cs`



| Teste | Endpoint |

| :--- | :--- |

| `Post\_CriaUsuario\_Retorna201` | `POST /api/Users` |

| `Get\_ListaUsuariosPaginado\_Retorna200` | `GET /api/Users?page=1\&size=10` |

| `Get\_UsuarioInexistente\_Retorna404` | `GET /api/Users/{id}` |

| `Put\_AtualizaUsuario\_Retorna200` | `PUT /api/Users/{id}` |

| `Delete\_DesativaUsuario\_Retorna204` | `DELETE /api/Users/{id}` |



\#### `AuthEndpointsTests.cs`



| Teste | Endpoint |

| :--- | :--- |

| `Post\_LoginValido\_RetornaToken` | `POST /api/Auth/Login` |

| `Post\_LoginInvalido\_Retorna401` | `POST /api/Auth/Login` |

| `Post\_LoginUsuarioInativo\_Retorna401` | `POST /api/Auth/Login` |

| `Post\_RefreshToken\_RetornaNovoToken` | `POST /api/Auth/RefreshToken` |



\#### `PermissionEndpointsTests.cs`



| Teste | Endpoint |

| :--- | :--- |

| `Post\_CriaPermissao\_Retorna201` | `POST /api/Permissions` |

| `Post\_PermissaoDuplicada\_Retorna400` | `POST /api/Permissions` |

| `Get\_PermissaoInexistente\_Retorna404` | `GET /api/Permissions/{id}` |



\#### `OrderingTests.cs`



| Teste | Query |

| :--- | :--- |

| `Get\_OrdenacaoSimples\_RetornaOrdenado` | `?order=username asc` |

| `Get\_OrdenacaoMultipla\_RetornaOrdenado` | `?order=username desc, groupname asc` |

| `Get\_CampoInvalido\_Retorna400` | `?order=campo\_invalido asc` |



\---



\## 🔍 Testes de Auditoria



\### `AuditServiceTests.cs`



| Teste | Descrição |

| :--- | :--- |

| `CaptureChanges\_Added\_DeveCriarRegistro` | Insert gera `Action=Added` |

| `CaptureChanges\_Modified\_DeveRegistrarColunasAlteradas` | Update captura `AffectedColumns` |

| `CaptureChanges\_Deleted\_DeveRegistrarOldValues` | Delete captura valores antigos |

| `CaptureChanges\_PasswordMascarado` | `Password` virou `\*\*\*` |

| `CaptureChanges\_RefreshTokenMascarado` | `RefreshToken` virou `\*\*\*` |

| `CaptureChanges\_SemAlteracao\_NaoGeraRegistro` | Skip de "Modified falso" |

| `CaptureChanges\_MultiplasEntidades\_GeraMultiplosRegistros` | Batch |



\### Exemplo



```csharp

\[Fact]

public async Task CaptureChanges\_ComPassword\_DeveMascarar()

{

&#x20;   // Arrange

&#x20;   var user = new User("marcelo", "m@e.com", "SenhaHash", UserRole.Admin, ...);

&#x20;   \_context.Users.Add(user);

&#x20;   await \_context.SaveChangesAsync();



&#x20;   // Act

&#x20;   var logs = await \_context.AuditLogs

&#x20;       .Where(a => a.EntityName == "User" \&\& a.Action == "Added")

&#x20;       .FirstAsync();



&#x20;   // Assert

&#x20;   logs.NewValues.Should().NotContain("SenhaHash");

&#x20;   logs.NewValues.Should().Contain("\\"Password\\":\\"\*\*\*\\"");

}

```



\---



\## ⚡ Testes de Performance



\### Cenários



| Cenário | Meta | Ferramenta |

| :--- | :--- | :--- |

| Query paginada (morna) | < 50ms | Swagger + Stopwatch |

| Query paginada (1000 users) | < 100ms | k6 |

| Login | < 200ms | k6 |

| Criação de usuário | < 300ms | k6 |

| Startup (warmup) | < 5s | Benchmark manual |



\### Teste de carga (k6)



```javascript

import http from 'k6/http';

import { check } from 'k6';



export const options = {

&#x20; stages: \[

&#x20;   { duration: '30s', target: 10 },

&#x20;   { duration: '1m', target: 50 },

&#x20;   { duration: '30s', target: 0 },

&#x20; ],

};



export default function () {

&#x20; const res = http.get(

&#x20;   'https://localhost:44382/api/UserGroups/GroupUsersPermissions/paginated?page=1\&size=10\&order=username%20asc',

&#x20;   {

&#x20;     headers: {

&#x20;       Authorization: `Bearer ${\_\_ENV.TOKEN}`,

&#x20;     },

&#x20;   }

&#x20; );

&#x20; check(res, {

&#x20;   'status is 200': (r) => r.status === 200,

&#x20;   'duration < 100ms': (r) => r.timings.duration < 100,

&#x20; });

}

```



\---



\## 🔐 Testes de Segurança



\### `AuthenticationTests.cs`



| Teste | Cenário |

| :--- | :--- |

| `Request\_SemToken\_Retorna401` | Endpoint protegido sem header |

| `Request\_TokenInvalido\_Retorna401` | JWT com assinatura errada |

| `Request\_TokenExpirado\_Retorna401` | Token expirado |

| `Request\_TokenValidoComPermissao\_Retorna200` | Autorização OK |

| `Request\_TokenValidoSemPermissao\_Retorna403` | Policy bloqueou |



\### `BCryptPasswordHasherTests.cs`



| Teste | Descrição |

| :--- | :--- |

| `Hash\_DeveGerarHashDiferenteParaMesmaSenha` | Salt aleatório |

| `Verify\_HashValido\_RetornaTrue` | Autenticação correta |

| `Verify\_HashInvalido\_RetornaFalse` | Autenticação incorreta |



\### `CorrelationIdTests.cs`



| Teste | Descrição |

| :--- | :--- |

| `Request\_SemHeader\_GeraCorrelationId` | Middleware gera |

| `Request\_ComHeader\_UsaHeaderFornecido` | Middleware respeita |

| `Response\_ContemHeaderCorrelationId` | Header na resposta |



\---



\## 📊 Matriz de Cobertura



\### Metas



| Camada | Meta | Atual | Status |

| :--- | ---: | ----: | :----- |

| Domain | 80% | 0% | 🔴 |

| Application (Handlers) | 70% | 0% | 🔴 |

| Application (Validators) | 90% | 0% | 🔴 |

| Infrastructure (Repositories) | 60% | 0% | 🔴 |

| Common (Auditing, Security) | 70% | 0% | 🔴 |

| \*\*Total\*\* | \*\*60%\*\* | \*\*0%\*\* | 🔴 |



\### Gerar relatório



```bash

dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

reportgenerator -reports:\*\*/coverage.opencover.xml -targetdir:coverage-report

```



\---



\## 🔄 CI/CD



\### Pipeline sugerido (GitHub Actions)



```yaml

name: CI



on:

&#x20; push:

&#x20;   branches: \[main, develop]

&#x20; pull\_request:

&#x20;   branches: \[main]



jobs:

&#x20; build:

&#x20;   runs-on: ubuntu-latest

&#x20;   steps:

&#x20;     - uses: actions/checkout@v4



&#x20;     - uses: actions/setup-dotnet@v4

&#x20;       with:

&#x20;         dotnet-version: '9.0.x'



&#x20;     - name: Restore

&#x20;       run: dotnet restore



&#x20;     - name: Build (warn as error)

&#x20;       run: dotnet build --no-restore /warnaserror



&#x20;     - name: Unit Tests

&#x20;       run: dotnet test src/Tests/SJInovacao.Acesso.UnitTests --no-build



&#x20;     - name: Integration Tests

&#x20;       run: dotnet test src/Tests/SJInovacao.Acesso.IntegrationTests --no-build



&#x20;     - name: Coverage Report

&#x20;       run: |

&#x20;         dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

&#x20;         reportgenerator -reports:\*\*/coverage.opencover.xml -targetdir:coverage-report



&#x20;     - name: Upload Coverage

&#x20;       uses: codecov/codecov-action@v4

&#x20;       with:

&#x20;         files: ./coverage-report/Cobertura.xml

```



\### Gates de qualidade



| Gate | Requisito |

| :--- | :--- |

| Build | 0 erros, 0 avisos |

| Testes unitários | 100% passando |

| Testes de integração | 100% passando |

| Cobertura | ≥ 60% |

| Vulnerabilidades | 0 críticas |



\---



\## 🎯 Critérios de Aceite para Homologação



\- \[ ] Cobertura total ≥ 60%

\- \[ ] Todos os testes passando em CI

\- \[ ] Zero avisos no build

\- \[ ] Testes de carga com resultado < 100ms (p95)

\- \[ ] Testes de segurança validados

\- \[ ] Teste de auditoria validado em todos os tipos de operação



\---



\## 📌 Comandos Rápidos



```bash

\# Rodar todos os testes

dotnet test



\# Rodar apenas unit tests

dotnet test src/Tests/SJInovacao.Acesso.UnitTests



\# Rodar apenas integration tests

dotnet test src/Tests/SJInovacao.Acesso.IntegrationTests



\# Com cobertura

dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover



\# Um teste específico

dotnet test --filter "FullyQualifiedName\~CreateUserHandlerTests"



\# Watch mode (durante desenvolvimento)

dotnet watch --project src/Tests/SJInovacao.Acesso.UnitTests test

```



\---



\*\*Plano de Testes — US01 · Versão 1.0 · 10/10/2026\*\*

