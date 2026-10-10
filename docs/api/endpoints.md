# API Endpoints — SJInovacao.Acesso



- **Versão:** 1.0

- **Data:** 10/10/2026

- **Base URL (dev):** `https://localhost:44382`

- **Base URL (Docker):** `http://localhost:8080`

- **Autenticação:** JWT Bearer

- **Formato:** JSON

- **Swagger:** `/swagger`



---



## 📑 Sumário



- [Convenções](#-convenções)

- [Autenticação](#-autenticação)

- [Usuários](#-usuários)

- [Permissões](#-permissões)

- [Grupos e Vínculos](#-grupos-e-vínculos)

- [Vínculos Usuário-Permissão](#-vínculos-usuário-permissão)

- [Health Checks](#-health-checks)

- [Códigos de Status](#-códigos-de-status)

- [Paginação](#-paginação)

- [Ordenação](#-ordenação)

- [Headers](#-headers)

- [Erros](#-erros)



---



## 📐 Convenções



### Base URL



| Ambiente | URL |

| :--- | :--- |

| Desenvolvimento | `https://localhost:44382` |

| Docker | `http://localhost:8080` |

| Produção | `https://api.suaempresa.com.br` |



### Content-Type



- **Request:** `application/json`

- **Response:** `application/json; charset=utf-8`



### Autenticação



Endpoints protegidos requerem o header:



```http

Authorization: Bearer {seu_jwt_token}

```



### Swagger



Documentação interativa disponível em:



- Desenvolvimento: `https://localhost:44382/swagger/index.html`

- Basic Auth (produção): `admin` / senha configurada em `SwaggerAuth__Password`



---



## 🔐 Autenticação



### POST `/api/Auth/Login`



Autentica um usuário e retorna o token JWT.



**Autenticação:** ❌ Não requerida



**Request Body:**



```json

{

&#x20; "username": "marcelo.jesuino",

&#x20; "password": "Senha@123"

}

```



**Response 200 OK:**



```json

{

&#x20; "success": true,

&#x20; "data": {

&#x20;   "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",

&#x20;   "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4...",

&#x20;   "expiresIn": 28800,

&#x20;   "tokenType": "Bearer",

&#x20;   "user": {

&#x20;     "id": "bdae663e-e7bb-426f-90ae-366e1d8839d9",

&#x20;     "username": "marcelo.jesuino",

&#x20;     "email": "marcelo@empresa.com",

&#x20;     "role": "Admin",

&#x20;     "permissions": [

&#x20;       "user.add",

&#x20;       "user.update",

&#x20;       "user.view",

&#x20;       "user.Deactivate"

&#x20;     ],

&#x20;     "groups": ["group.all"]

&#x20;   }

&#x20; }

}

```



**Response 401 Unauthorized:**



```json

{

&#x20; "success": false,

&#x20; "message": "Credenciais inválidas."

}

```



**Códigos:**



| Status | Descrição |

| :--- | :--- |

| 200 | Login OK |

| 400 | Dados inválidos |

| 401 | Credenciais inválidas ou usuário inativo |



---



### POST `/api/Auth/RefreshToken`



Renova o token de acesso usando o refresh token.



**Autenticação:** ❌ Não requerida



**Request Body:**



```json

{

&#x20; "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4..."

}

```



**Response 200 OK:**



```json

{

&#x20; "success": true,

&#x20; "data": {

&#x20;   "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",

&#x20;   "refreshToken": "bm92byByZWZyZXNoIHRva2Vu...",

&#x20;   "expiresIn": 28800,

&#x20;   "tokenType": "Bearer"

&#x20; }

}

```



**Códigos:**



| Status | Descrição |

| :--- | :--- |

| 200 | Token renovado |

| 401 | Refresh token inválido/expirado |



---



### POST `/api/Auth/RevokeToken`



Revoga o refresh token do usuário.



**Autenticação:** ✅ Requerida



**Response 204 No Content**



---



## 👥 Usuários



### GET `/api/Users`



Lista usuários com paginação e ordenação.



**Autenticação:** ✅ Requerida (`user.view`)



**Query Parameters:**



| Parâmetro | Tipo | Padrão | Descrição |

| :--- | :--- | :--- | :--- |

| `page` | int | 1 | Página (1-based) |

| `size` | int | 10 | Itens por página (1-100) |

| `order` | string | `username asc` | Ordenação |



**Exemplo:**



```

GET /api/Users?page=1\&size=10\&order=username asc, email desc

```



**Response 200 OK:**



```json

{

&#x20; "currentPage": 1,

&#x20; "pageSize": 10,

&#x20; "totalPages": 3,

&#x20; "totalCount": 25,

&#x20; "hasPrevious": false,

&#x20; "hasNext": true,

&#x20; "data": [

&#x20;   {

&#x20;     "id": "bdae663e-e7bb-426f-90ae-366e1d8839d9",

&#x20;     "username": "marcelo.jesuino",

&#x20;     "email": "marcelo@empresa.com",

&#x20;     "firstName": "Marcelo",

&#x20;     "lastName": "Jesuino",

&#x20;     "role": "Admin",

&#x20;     "status": "Active",

&#x20;     "createdAt": "2026-10-04T03:10:40Z",

&#x20;     "updatedAt": null

&#x20;   }

&#x20; ],

&#x20; "success": true,

&#x20; "message": "",

&#x20; "errors": []

}

```



**Códigos:**



| Status | Descrição |

| :--- | :--- |

| 200 | Lista retornada |

| 400 | Parâmetros inválidos |

| 401 | Não autenticado |

| 403 | Sem permissão `user.view` |



---



### GET `/api/Users/{id}`



Busca usuário por ID.



**Autenticação:** ✅ Requerida (`user.view`)



**Path Parameters:**



- `id` (uuid) — ID do usuário



**Response 200 OK:**



```json

{

&#x20; "success": true,

&#x20; "data": {

&#x20;   "id": "bdae663e-e7bb-426f-90ae-366e1d8839d9",

&#x20;   "username": "marcelo.jesuino",

&#x20;   "email": "marcelo@empresa.com",

&#x20;   "firstName": "Marcelo",

&#x20;   "lastName": "Jesuino",

&#x20;   "role": "Admin",

&#x20;   "status": "Active",

&#x20;   "addresses": [

&#x20;     {

&#x20;       "id": "...",

&#x20;       "street": "Rua Exemplo",

&#x20;       "number": "123",

&#x20;       "neighborhood": "Centro",

&#x20;       "city": "São Paulo",

&#x20;       "state": "SP",

&#x20;       "zipCode": "01234-567"

&#x20;     }

&#x20;   ],

&#x20;   "phones": [

&#x20;     {

&#x20;       "id": "...",

&#x20;       "number": "+55 11 99999-9999",

&#x20;       "type": "Mobile"

&#x20;     }

&#x20;   ]

&#x20; }

}

```



**Códigos:**



| Status | Descrição |

| :--- | :--- |

| 200 | Usuário encontrado |

| 404 | Usuário não encontrado |



---



### POST `/api/Users`



Cria um novo usuário.



**Autenticação:** ✅ Requerida (`user.add`)



**Request Body:**



```json

{

&#x20; "username": "novo.usuario",

&#x20; "email": "novo@empresa.com",

&#x20; "password": "Senha@123",

&#x20; "role": "User",

&#x20; "name": {

&#x20;   "firstName": "Novo",

&#x20;   "lastName": "Usuário"

&#x20; },

&#x20; "document": {

&#x20;   "number": "12345678900",

&#x20;   "personType": "Physical"

&#x20; },

&#x20; "addresses": [

&#x20;   {

&#x20;     "street": "Rua Exemplo",

&#x20;     "number": "123",

&#x20;     "neighborhood": "Centro",

&#x20;     "city": "São Paulo",

&#x20;     "state": "SP",

&#x20;     "zipCode": "01234-567"

&#x20;   }

&#x20; ],

&#x20; "phones": [

&#x20;   {

&#x20;     "number": "+55 11 99999-9999",

&#x20;     "type": "Mobile"

&#x20;   }

&#x20; ]

}

```



**Response 201 Created:**



```json

{

&#x20; "success": true,

&#x20; "data": {

&#x20;   "id": "novo-uuid-aqui",

&#x20;   "username": "novo.usuario",

&#x20;   "email": "novo@empresa.com",

&#x20;   "createdAt": "2026-10-10T18:00:00Z"

&#x20; }

}

```



**Códigos:**



| Status | Descrição |

| :--- | :--- |

| 201 | Usuário criado |

| 400 | Dados inválidos ou usuário duplicado |

| 401 | Não autenticado |

| 403 | Sem permissão `user.add` |



---



### PUT `/api/Users/{id}`



Atualiza dados de um usuário.



**Autenticação:** ✅ Requerida (`user.update`)



**Request Body:**



```json

{

&#x20; "username": "marcelo.jesuino",

&#x20; "email": "novo.email@empresa.com",

&#x20; "role": "Admin",

&#x20; "status": "Active"

}

```



**Response 200 OK:**



```json

{

&#x20; "success": true,

&#x20; "data": {

&#x20;   "id": "bdae663e-e7bb-426f-90ae-366e1d8839d9",

&#x20;   "email": "novo.email@empresa.com",

&#x20;   "updatedAt": "2026-10-10T18:05:00Z"

&#x20; }

}

```



**Observação:** Apenas campos realmente alterados aparecem em `AffectedColumns` na auditoria.



**Códigos:**



| Status | Descrição |

| :--- | :--- |

| 200 | Atualizado |

| 400 | Dados inválidos |

| 404 | Usuário não encontrado |



---



### DELETE `/api/Users/{id}`



Desativa logicamente um usuário (soft delete).



**Autenticação:** ✅ Requerida (`user.Deactivate`)



**Response 204 No Content**



**Códigos:**



| Status | Descrição |

| :--- | :--- |

| 204 | Desativado |

| 404 | Usuário não encontrado |



---



## 🔑 Permissões



### GET `/api/Permissions`



Lista todas as permissões.



**Autenticação:** ✅ Requerida (`permission.view`)



**Response 200 OK:**



```json

{

&#x20; "success": true,

&#x20; "data": [

&#x20;   {

&#x20;     "id": "770c7a5d-4637-4587-9cc1-34849c31d765",

&#x20;     "name": "user.Deactivate",

&#x20;     "description": "Permite desativar usuários",

&#x20;     "isActive": true,

&#x20;     "createdAt": "2026-10-09T20:23:03.937038Z",

&#x20;     "updatedAt": "2026-10-10T02:24:33.896299Z"

&#x20;   }

&#x20; ]

}

```



---



### GET `/api/Permissions/{id}`



Busca permissão por ID.



**Autenticação:** ✅ Requerida (`permission.view`)



**Response 200 OK:**



```json

{

&#x20; "success": true,

&#x20; "data": {

&#x20;   "id": "770c7a5d-4637-4587-9cc1-34849c31d765",

&#x20;   "name": "user.Deactivate",

&#x20;   "description": "Permite desativar usuários",

&#x20;   "isActive": true

&#x20; }

}

```



---



### POST `/api/Permissions`



Cria uma nova permissão.



**Autenticação:** ✅ Requerida (`permission.add`)



**Request Body:**



```json

{

&#x20; "name": "report.export",

&#x20; "description": "Permite exportar relatórios"

}

```



**Response 201 Created:**



```json

{

&#x20; "success": true,

&#x20; "data": {

&#x20;   "id": "novo-uuid",

&#x20;   "name": "report.export",

&#x20;   "description": "Permite exportar relatórios",

&#x20;   "isActive": true,

&#x20;   "createdAt": "2026-10-10T18:10:00Z"

&#x20; }

}

```



**Códigos:**



| Status | Descrição |

| :--- | :--- |

| 201 | Criada |

| 400 | Nome duplicado ou inválido |



---



### PUT `/api/Permissions/{id}`



Atualiza uma permissão.



**Autenticação:** ✅ Requerida (`permission.update`)



**Request Body:**



```json

{

&#x20; "name": "report.export",

&#x20; "description": "Permite exportar relatórios (atualizado)",

&#x20; "isActive": true

}

```



**Response 200 OK**



---



## 👥 Grupos e Vínculos



### GET `/api/GroupUsersPermissions`



Lista todos os vínculos usuário-grupo-permissão.



**Autenticação:** ✅ Requerida



**Response 200 OK:**



```json

{

&#x20; "success": true,

&#x20; "data": [

&#x20;   {

&#x20;     "userId": "bdae663e-e7bb-426f-90ae-366e1d8839d9",

&#x20;     "userName": "Marcelo Jesuino",

&#x20;     "groupId": "15dfb65d-cb63-4588-bc9f-00e9d3236f0a",

&#x20;     "groupName": "group.all",

&#x20;     "userIsActive": true,

&#x20;     "permissions": [

&#x20;       {

&#x20;         "id": "770c7a5d-4637-4587-9cc1-34849c31d765",

&#x20;         "name": "user.Deactivate",

&#x20;         "description": "O usuário tem permissão de desativar",

&#x20;         "isActive": true

&#x20;       }

&#x20;     ]

&#x20;   }

&#x20; ]

}

```



---



### GET `/api/GroupUsersPermissions/paginated`



Versão paginada do endpoint anterior.



**Autenticação:** ✅ Requerida



**Query Parameters:**



| Parâmetro | Tipo | Padrão | Descrição |

| :--- | :--- | :--- | :--- |

| `page` | int | 1 | Página |

| `size` | int | 10 | Itens por página |

| `order` | string | `username asc` | Ordenação multi-campo |



**Ordenação suportada:**



- `username` — nome do usuário

- `groupname` / `name` — nome do grupo

- `userid` — ID do usuário

- `groupid` — ID do grupo



**Exemplo:**



```

GET /api/GroupUsersPermissions/paginated?page=1\&size=10\&order=username asc, groupname desc

```



**Response 200 OK:**



```json

{

&#x20; "currentPage": 1,

&#x20; "pageSize": 10,

&#x20; "totalPages": 1,

&#x20; "totalCount": 2,

&#x20; "hasPrevious": false,

&#x20; "hasNext": false,

&#x20; "data": [

&#x20;   {

&#x20;     "userId": "bdae663e-e7bb-426f-90ae-366e1d8839d9",

&#x20;     "userName": "Marcelo Jesuino",

&#x20;     "groupId": "15dfb65d-cb63-4588-bc9f-00e9d3236f0a",

&#x20;     "groupName": "group.all",

&#x20;     "userIsActive": true,

&#x20;     "permissions": [...]

&#x20;   }

&#x20; ]

}

```



**Códigos:**



| Status | Descrição |

| :--- | :--- |

| 200 | Lista retornada |

| 400 | Parâmetros de ordenação inválidos |



---



### PUT `/api/UserGroups/GroupUsersPermissions/{userId}/{groupId}/{userIsActive}`



Atualiza vínculos de um usuário em um grupo, incluindo permissões específicas.



**Autenticação:** ✅ Requerida (`group.update`)



**Path Parameters:**



- `userId` (uuid) — ID do usuário

- `groupId` (uuid) — ID do grupo

- `userIsActive` (bool) — Ativa/desativa usuário no grupo



**Request Body:**



```json

{

&#x20; "permissionIds": [

&#x20;   "ac4e76f2-17d4-4f21-9aa3-f2cd42cd3b40",

&#x20;   "770c7a5d-4637-4587-9cc1-34849c31d765"

&#x20; ],

&#x20; "permissionIsActive": true

}

```



**Regras:**



- Se `permissionIds` estiver vazio, apenas o status do usuário no grupo muda

- Se `permissionIsActive` for `null`, apenas o status do usuário muda

- Todas as permissões informadas devem pertencer ao grupo (senão 400)

- Se `userIsActive = false`, todas as permissões também ficam inativas



**Response 200 OK:**



```json

{

&#x20; "success": true,

&#x20; "data": {

&#x20;   "userId": "bdae663e-e7bb-426f-90ae-366e1d8839d9",

&#x20;   "userName": "Marcelo Jesuino",

&#x20;   "groupId": "15dfb65d-cb63-4588-bc9f-00e9d3236f0a",

&#x20;   "groupName": "group.all",

&#x20;   "userIsActive": true,

&#x20;   "permissionIsActive": true,

&#x20;   "permissions": [...]

&#x20; }

}

```



**Códigos:**



| Status | Descrição |

| :--- | :--- |

| 200 | Vínculos atualizados |

| 400 | Usuário/permissão não encontrados ou permissão fora do grupo |

| 404 | Grupo não encontrado |



---



## 🔗 Vínculos Usuário-Permissão



### POST `/api/UserPermissions`



Cria vínculo direto entre usuário e permissão.



**Autenticação:** ✅ Requerida (`permission.grant`)



**Request Body:**



```json

{

&#x20; "userId": "bdae663e-e7bb-426f-90ae-366e1d8839d9",

&#x20; "permissionId": "770c7a5d-4637-4587-9cc1-34849c31d765"

}

```



**Response 201 Created:**



```json

{

&#x20; "success": true,

&#x20; "data": {

&#x20;   "userId": "bdae663e-e7bb-426f-90ae-366e1d8839d9",

&#x20;   "permissionId": "770c7a5d-4637-4587-9cc1-34849c31d765",

&#x20;   "isActive": true,

&#x20;   "createdAt": "2026-10-10T18:15:00Z"

&#x20; }

}

```



**Códigos:**



| Status | Descrição |

| :--- | :--- |

| 201 | Vínculo criado |

| 400 | Usuário/permissão inexistente ou vínculo duplicado |



---



### GET `/api/UserPermissions/{userId}`



Lista permissões diretas de um usuário.



**Autenticação:** ✅ Requerida (`user.view`)



**Response 200 OK:**



```json

{

&#x20; "success": true,

&#x20; "data": [

&#x20;   {

&#x20;     "id": "770c7a5d-4637-4587-9cc1-34849c31d765",

&#x20;     "name": "user.Deactivate",

&#x20;     "description": "Permite desativar usuários",

&#x20;     "isActive": true

&#x20;   }

&#x20; ]

}

```



---



### DELETE `/api/UserPermissions/{userId}/{permissionId}`



Remove vínculo direto.



**Autenticação:** ✅ Requerida (`permission.revoke`)



**Response 204 No Content**



---



## 🏥 Health Checks



### GET `/health`



Health check completo.



**Autenticação:** ❌ Não requerida



**Response 200 OK:**



```json

{

&#x20; "status": "Healthy",

&#x20; "timestamp": "2026-10-10T18:20:00Z",

&#x20; "healthChecks": [

&#x20;   {

&#x20;     "name": "Liveness",

&#x20;     "status": "Healthy",

&#x20;     "description": null,

&#x20;     "errorMessage": null,

&#x20;     "hostEnvironment": "development"

&#x20;   },

&#x20;   {

&#x20;     "name": "Readiness",

&#x20;     "status": "Healthy",

&#x20;     "description": null,

&#x20;     "errorMessage": null,

&#x20;     "hostEnvironment": "development"

&#x20;   }

&#x20; ]

}

```



### GET `/health/live`



Liveness probe — apenas verifica se a API está de pé.



**Response 200 OK:**



```json

{

&#x20; "status": "Healthy",

&#x20; "healthChecks": [

&#x20;   { "name": "Liveness", "status": "Healthy" }

&#x20; ]

}

```



### GET `/health/ready`



Readiness probe — verifica dependências (banco, cache).



**Response 200 OK** ou **503 Service Unavailable**



---



## 📊 Códigos de Status



### Sucesso



| Status | Descrição | Quando usar |

| :--- | :--- | :--- |

| 200 | OK | GET, PUT, PATCH |

| 201 | Created | POST que cria recurso |

| 204 | No Content | DELETE, operações sem retorno |



### Erro do Cliente



| Status | Descrição | Quando usar |

| :--- | :--- | :--- |

| 400 | Bad Request | Validação, `DomainException` |

| 401 | Unauthorized | Sem token ou token inválido |

| 403 | Forbidden | Sem permissão |

| 404 | Not Found | Recurso não existe |



### Erro do Servidor



| Status | Descrição | Quando usar |

| :--- | :--- | :--- |

| 500 | Internal Server Error | Bug, exceção não tratada |

| 503 | Service Unavailable | Health check falhando |



---



## 📄 Paginação



Todos os endpoints paginados seguem o **mesmo formato**:



**Query Parameters:**



| Parâmetro | Tipo | Padrão | Range |

| :--- | :--- | :--- | :--- |

| `page` | int | 1 | ≥ 1 |

| `size` | int | 10 | 1-100 |



**Response envelope:**



```json

{

&#x20; "currentPage": 1,

&#x20; "pageSize": 10,

&#x20; "totalPages": 3,

&#x20; "totalCount": 25,

&#x20; "hasPrevious": false,

&#x20; "hasNext": true,

&#x20; "data": [...],

&#x20; "success": true,

&#x20; "message": "",

&#x20; "errors": []

}

```



---



## 🔤 Ordenação



Formato:



```

?order={campo} {direção}[, {campo2} {direção2}][, ...]

```



### Exemplos válidos



```

?order=username asc

?order=username desc

?order=username asc, email desc

?order=groupname asc, username desc, email asc

```



### Direções



- `asc` (padrão se omitido)

- `desc`



### Campos suportados (exemplos)



| Endpoint | Campos |

| :--- | :--- |

| `/api/Users` | `username`, `email`, `role`, `status`, `createdat`, `updatedat`, `name`, `name.firstname`, `name.lastname`, `address.city`, `address.street`, `phone` |

| `/api/GroupUsersPermissions/paginated` | `username`, `userid`, `groupname`, `name`, `groupid` |



### Validação



Campos inválidos retornam **400 Bad Request**:



```json

{

&#x20; "success": false,

&#x20; "message": "Order inválido. Campos válidos: username, userid, groupname, name, groupid. Direções: asc, desc."

}

```



---



## 📨 Headers



### Request Headers



| Header | Obrigatório | Descrição |

| :--- | :--- | :--- |

| `Authorization` | ✅ (endpoints protegidos) | `Bearer {token}` |

| `Content-Type` | ✅ (POST/PUT) | `application/json` |

| `X-Correlation-ID` | ❌ | ID para rastreamento (gerado automaticamente se ausente) |



### Response Headers



| Header | Descrição |

| :--- | :--- |

| `X-Correlation-ID` | Correlation ID da requisição |

| `Content-Type` | Sempre `application/json; charset=utf-8` |



### Exemplo de uso



```bash

curl -X GET https://localhost:44382/api/Users \\

&#x20; -H "Authorization: Bearer eyJhbGciOi..." \\

&#x20; -H "X-Correlation-ID: minha-correlacao-123" \\

&#x20; -H "Accept: application/json"

```



---



## ⚠️ Erros



Todos os erros seguem o mesmo formato:



### 400 — Validação



```json

{

&#x20; "success": false,

&#x20; "message": "Erro de validação nos dados enviados.",

&#x20; "errors": [

&#x20;   {

&#x20;     "field": "Username",

&#x20;     "message": "O nome de usuário é obrigatório."

&#x20;   },

&#x20;   {

&#x20;     "field": "Email",

&#x20;     "message": "Email inválido."

&#x20;   }

&#x20; ]

}

```



### 400 — Regra de negócio (`DomainException`)



```json

{

&#x20; "success": false,

&#x20; "message": "Já existe uma permissão com o nome 'user.add'."

}

```



### 401 — Não autenticado



```json

{

&#x20; "success": false,

&#x20; "message": "Você não tem autorização para acessar este recurso."

}

```



### 403 — Sem permissão



```json

{

&#x20; "success": false,

&#x20; "message": "Você não tem permissão para acessar este recurso."

}

```



### 404 — Não encontrado



```json

{

&#x20; "success": false,

&#x20; "message": "Recurso não encontrado."

}

```



### 500 — Erro interno



```json

{

&#x20; "success": false,

&#x20; "message": "Erro interno do servidor. Contate o suporte."

}

```



**Nota:** o cliente **nunca** vê stack trace ou detalhes internos. O erro completo é logado no servidor com o `CorrelationId` para rastreamento.



---



## 🧪 Coleção Postman / cURL



### Login



```bash

curl -X POST https://localhost:44382/api/Auth/Login \\

&#x20; -H "Content-Type: application/json" \\

&#x20; -d '{

&#x20;   "username": "marcelo.jesuino",

&#x20;   "password": "Senha@123"

&#x20; }'

```



### Listar usuários



```bash

curl -X GET "https://localhost:44382/api/Users?page=1\&size=10\&order=username%20asc" \\

&#x20; -H "Authorization: Bearer SEU_TOKEN" \\

&#x20; -H "Accept: application/json"

```



### Criar permissão



```bash

curl -X POST https://localhost:44382/api/Permissions \\

&#x20; -H "Authorization: Bearer SEU_TOKEN" \\

&#x20; -H "Content-Type: application/json" \\

&#x20; -d '{

&#x20;   "name": "report.export",

&#x20;   "description": "Permite exportar relatórios"

&#x20; }'

```



### Atualizar vínculos



```bash

curl -X PUT "https://localhost:44382/api/UserGroups/GroupUsersPermissions/BD.../15.../true" \\

&#x20; -H "Authorization: Bearer SEU_TOKEN" \\

&#x20; -H "Content-Type: application/json" \\

&#x20; -d '{

&#x20;   "permissionIds": ["ac4e76f2-17d4-4f21-9aa3-f2cd42cd3b40"],

&#x20;   "permissionIsActive": true

&#x20; }'

```



---



## 📌 Rate Limiting (futuro)



| Endpoint | Limite |

| :--- | :--- |

| `POST /api/Auth/Login` | 5/min por IP |

| `POST /api/Auth/RefreshToken` | 20/min por usuário |

| Demais endpoints | 100/min por usuário |



---



## 📚 Referências



- [Backlog US01](../backlog/us01-backlog.md)

- [Arquitetura](../architecture/overview.md)

- [ADRs](../adr/README.md)

- [Plano de Testes](../tests/test-plan.md)

- Swagger interativo: `/swagger`



---



**API Endpoints — SJInovacao.Acesso · Versão 1.0 · 10/10/2026**

