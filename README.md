# 🚀 SJInovacao.Acesso

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)
![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED?style=for-the-badge&logo=docker)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-13-4169E1?style=for-the-badge&logo=postgresql)
![Redis](https://img.shields.io/badge/Redis-7-DC382D?style=for-the-badge&logo=redis)
![GitHub Actions](https://img.shields.io/badge/CI%2FCD-GitHub%20Actions-2088FF?style=for-the-badge&logo=github-actions)

Uma API RESTful robusta desenvolvida em **.NET 8** para o gerenciamento de acesso e autenticação. O projeto foi construído seguindo as melhores práticas de mercado, utilizando **Clean Architecture**, conteinerização com **Docker**, e um pipeline completo de **CI/CD** com geração de relatórios de testes automatizados.

---

## 📊 Dashboard de Testes Automatizados

Este projeto possui um pipeline de Integração Contínua (CI) configurado no GitHub Actions. A cada `push` na branch principal, os testes são executados e um dashboard interativo é gerado e publicado automaticamente.

🔗 **[Acesse o Dashboard de Testes ao vivo aqui](https://jesuino-treinamento.github.io/SJInovacao.Acesso/)**

*O dashboard exibe a taxa de sucesso, total de testes e gráficos detalhados por categoria (Unitários, Integração, Infraestrutura e Aplicação).*

---

## 🛠️ Tecnologias Utilizadas

- **Backend:** .NET 8, C#
- **Banco de Dados:** PostgreSQL 13
- **Cache:** Redis 7
- **Autenticação:** JWT (JSON Web Tokens) e API Key
- **Documentação:** Swagger (OpenAPI)
- **Conteinerização:** Docker e Docker Compose
- **Testes:** xUnit, Moq, FluentAssertions
- **CI/CD:** GitHub Actions
- **Logs:** Serilog

---

## ✨ Funcionalidades Principais

- ✅ Autenticação e autorização baseada em JWT.
- ✅ Documentação interativa da API com Swagger.
- ✅ Cache distribuído com Redis para alta performance.
- ✅ Proteção do Swagger em ambiente de produção com Basic Auth.
- ✅ Pipeline de testes automatizados com geração de dashboard HTML.

---

## 🚀 Como Executar o Projeto

### Pré-requisitos
Antes de começar, você vai precisar ter instalado em sua máquina:
- [Docker](https://www.docker.com/products/docker-desktop)
- [Docker Compose](https://docs.docker.com/compose/install/)

### Passo a Passo

1. **Clone o repositório:**
   ```bash
   git clone https://github.com/jesuino-treinamento/SJInovacao.Acesso.git
   cd SJInovacao.Acesso