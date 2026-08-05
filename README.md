# **Teles Educação - Plataforma Educacional**

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=flat-square&logo=microsoft-sql-server&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=flat-square&logo=swagger&logoColor=black)

[![CI](https://github.com/mba-dev-expert/mba-modulo05/actions/workflows/ci.yml/badge.svg)](https://github.com/mba-dev-expert/mba-modulo05/actions/workflows/ci.yml)
[![CD](https://github.com/mba-dev-expert/mba-modulo05/actions/workflows/cd.yml/badge.svg)](https://github.com/mba-dev-expert/mba-modulo05/actions/workflows/cd.yml)

## **1. Apresentação**
Bem-vindo ao repositório do projeto **Teles Educação**. Esta plataforma é o resultado prático do MBA DevXpert Full Stack .NET, focada no módulo de **DevOps**.

O projeto consiste no desenvolvimento de uma plataforma educacional robusta, aplicando padrões de design de software modernos para gerir eficientemente conteúdos, alunos e processos financeiros, utilizando uma arquitetura de microserviços. Nesta etapa, o foco foi levar essa arquitetura a um pipeline de entrega contínua: build e testes automatizados antes da publicação, containerização, orquestração em Kubernetes e observabilidade.

---

## **2. Pilares Técnicos**
Para garantir uma aplicação escalável e de fácil manutenção, foram aplicados os seguintes conceitos:

* **DDD (Domain-Driven Design):** Modelagem orientada ao negócio com separação clara de domínios.
* **Bounded Contexts:** Cada contexto possui autonomia total e isolamento de responsabilidades.
* **CQRS:** Segregação de responsabilidades de leitura e escrita.
* **TDD (Test Driven Development):** Desenvolvimento orientado a testes para garantir a qualidade do código.
* **Testes automatizados:** testes de domínio, executados no CI antes de qualquer publicação de imagem. Ver [seção 8](#8-qualidade-testes-e-pipeline-cicd).
* **ACL (Anti-Corruption Layer):** Implementada no contexto de pagamentos para proteger o domínio interno de integrações externas.
* **Microserviços:** Arquitetura distribuída com comunicação via Message Bus.
* **API Gateway/BFF:** Backend for Frontend para otimizar a experiência do usuário.
* **CI/CD e observabilidade:** Pipeline com build e testes antes da publicação, containerização com healthcheck, orquestração em Kubernetes com autoscaling, logs estruturados e métricas Prometheus. Ver [seção 8](#8-qualidade-testes-e-pipeline-cicd) e [seção 9](#9-observabilidade-e-resiliência).

---

## **3. Tecnologias Utilizadas**
| Categoria | Tecnologia |
| :--- | :--- |
| **Linguagem** | C# 13 / .NET 9 |
| **Framework Web** | ASP.NET Core Web API |
| **ORM** | Entity Framework Core |
| **Banco de Dados** | SQL Server |
| **Mensageria** | RabbitMQ (via Message Bus) |
| **Segurança** | ASP.NET Core Identity & JWT (JSON Web Token) |
| **Documentação** | Swagger (OpenAPI) |

---

## **4. Estrutura do Projeto (Bounded Contexts)**
A solução foi desenhada seguindo a premissa de **Contextos Delimitados** e **Microserviços**. Cada BC possui as camadas necessárias para implementar as soluções de cada problema específico de negócio, funcionando de forma independente:

* **Building Blocks:**
  * **TelesEducacao.Core:** Interfaces, entidades base e notificações compartilhadas (*Shared Kernel*).
  * **TelesEducacao.MessageBus:** Implementação do barramento de mensagens para comunicação entre serviços.
  * **TelesEducacao.WebAPI.Core:** Componentes compartilhados para APIs (controllers base, autenticação, etc.).

* **API Gateways:**
  * **TelesEducacao.Bff.Plataforma:** Backend for Frontend para a plataforma, agregando dados de múltiplos serviços.

* **Serviços:**
  * **TelesEducacao.Alunos:** Gestão completa do ciclo de vida do aluno.
    * `API`, `Application`, `Data`, `Domain`: Regras de negócio, comandos/queries e endpoints REST.
  * **TelesEducacao.Auth:** Autenticação e autorização.
    * `API`, `Application`, `Data`: Gestão de usuários, roles e tokens.
  * **TelesEducacao.Conteudo:** Gestão pedagógica (Cursos e Aulas).
    * `API`, `Application`, `Data`, `Domain`: Modelagem e operações de conteúdo educacional.
  * **TelesEducacao.Pagamentos:** Processamento financeiro.
    * `AntiCorruption`, `API`, `Business`, `Data`: Regras de negócio, persistência e integração com gateways.
---

## **5. Funcionalidades Implementadas**
* **Gestão de Cursos e Aulas:** CRUD completo para administração de conteúdo educacional.
* **Gestão de Alunos:** Matrículas, progresso e certificados.
* **Autenticação e Autorização:** Registro de usuários, login e controle de acesso via Roles.
* **Processamento de Pagamentos:** Integração com gateways externos via Anti-Corruption Layer.
* **API RESTful:** Endpoints padronizados para integração com Front-ends ou Apps.
* **Mensageria:** Comunicação assíncrona entre serviços via Message Bus.
* **Documentação Interativa:** Interface Swagger para exploração em tempo real.

---

## **6. Como Executar o Projeto**

### **Opção 1 — Docker Compose (recomendado)**
Sobe as 7 dependências/serviços (RabbitMQ, SQL Server e as 5 APIs/BFF) com um único comando, com as credenciais injetadas por variável de ambiente a partir de um `.env` local.

**Pré-requisitos:** Docker Desktop (ou engine equivalente) com Docker Compose v2+.

```bash
cp docker/.env.example docker/.env
docker compose -f docker/docker-compose.yml --env-file docker/.env up -d --build
```

Guia completo (variáveis, troubleshooting, troca de senha, healthcheck): [docker/README.md](docker/README.md).

### **Opção 2 — Kubernetes**
Manifests organizados em `k8s/base/` (genéricos) + `k8s/overlays/local/` (Kustomize), com HPA, StatefulSet para o SQL Server e Ingress nginx.

**Pré-requisitos:** `kubectl`, um cluster local (Minikube/Kind) e `kustomize` (embutido no `kubectl`).

```bash
cp k8s/overlays/local/app-secrets.env.example k8s/overlays/local/app-secrets.env
kubectl apply -k k8s/overlays/local
```

Guia completo (build/carregamento de imagens locais, HPA, Ingress, troubleshooting): [k8s/README.md](k8s/README.md).

### **Opção 3 — Execução local com `dotnet run`**
Útil para depurar um único serviço fora de um container.

#### **Pré-requisitos**
* .NET SDK 9.0
* SQL Server (ambiente Staging) — em Development os serviços usam SQLite
* IDE de sua preferência (Visual Studio 2022, Rider ou VS Code)

### **Passos para Execução**

1.  **Clone o Repositório:**
    ```bash
    git clone https://github.com/mba-dev-expert/mba-modulo05.git
    cd mba-modulo05
    ```

2.  **Configuração do Banco de Dados:**
    * No arquivo `appsettings.json` (em cada serviço API), configure sua string de conexão com SQL Server (Staging) ou use o padrão SQLite (Development).
    * O projeto possui configuração de **Seed**. Ao rodar, o banco será criado e populado automaticamente.

3.  **Usuários de Teste (Seed):**
    | Perfil | Email | Senha |
    | :--- | :--- | :--- |
    | Admin | `admin@mail.com` | `Dev@123` |
    | Aluno | `aluno1@mail.com` | `Dev@123` |
    | Aluno | `aluno2@mail.com` | `Dev@123` |

4.  **Executar os Serviços:**

    * Em Ambiente **Development** (padrão e SQLite):
      * Inicie o RabbitMQ com a configuração abaixo (docker):
        ```
        docker run -d \
          --name rabbitmq \
          -p 5672:5672 \
          -p 15672:15672 \
          -e RABBITMQ_DEFAULT_USER=teleseducacao \
          -e RABBITMQ_DEFAULT_PASS=TelesEduca123! \
          rabbitmq:management-alpine
        ```
      * Execute cada serviço API individualmente:
        ```bash
        dotnet run --project src/services/TelesEducacao.Alunos.API
        dotnet run --project src/services/TelesEducacao.Auth.API
        dotnet run --project src/services/TelesEducacao.Conteudo.API
        dotnet run --project src/services/TelesEducacao.Pagamentos.API
        dotnet run --project src/api-gateways/TelesEducacao.Bff.Plataforma
        ```

     * Em Ambiente **Staging** (SQL Server):
       * Inicie o RabbitMQ (Igual ao passo para Development):
       * Inicie o serviço do SQL Server local
       * Execute cada serviço API individualmente:
          ```bash
          dotnet run --project src/services/TelesEducacao.Alunos.API --launch-profile "Staging"
          dotnet run --project src/services/TelesEducacao.Auth.Api --launch-profile "Staging"
          dotnet run --project src/services/TelesEducacao.Conteudo.API --launch-profile "Staging"
          dotnet run --project src/services/TelesEducacao.Pagamentos.API --launch-profile "Staging"
          dotnet run --project src/api-gateways/TelesEducacao.Bff.Plataforma --launch-profile "Staging"
          ```

5.  **Acessar os Endpoints:**
    * **BFF (Frontend Gateway):** `http://localhost:5035`
    * **Alunos API:** `http://localhost:5201`
    * **Auth API:** `http://localhost:5101`
    * **Conteudo API:** `http://localhost:5301`
    * **Pagamentos API:** `http://localhost:5401`

---

## **7. Documentação da API**
A documentação completa dos endpoints, modelos de entrada e saída pode ser acessada via Swagger após o início da aplicação (local, Docker ou Kubernetes/`port-forward`) nos endereços abaixo.

  * **BFF (Frontend Gateway):** `http://localhost:5035/swagger`
  * **Alunos API:** `http://localhost:5201/swagger`
  * **Auth API:** `http://localhost:5101/swagger`
  * **Conteudo API:** `http://localhost:5301/swagger`

---

## **8. Qualidade, Testes e Pipeline CI/CD**

### Testes
* **Testes de domínio**, em `tests/TelesEducacao.Alunos.Domain.Tests` e `tests/TelesEducacao.Conteudo.Domain.Tests`.
* Execute localmente com `dotnet test TelesEducacao.sln`.
* Ainda não há medição de cobertura nem testes de integração.

### Pipeline CI/CD
* **`ci.yml`** roda em push e PR para `main`: restore, build em Release, lint (`dotnet format --verify-no-changes --severity error`) e testes.
* **`cd.yml`** roda em push para `main` (ou `workflow_dispatch`) com dois jobs: `validate` (restore, build, testes) e `build-and-push`, ligado por `needs:` — nenhuma imagem é publicada sem build e testes verdes.
* As 5 imagens são construídas em matriz e publicadas no **Docker Hub** com as tags `latest` e `<sha do commit>`, usando cache do GitHub Actions por serviço.
* Duas vulnerabilidades transitivas foram eliminadas por atualização de versão, verificadas com `dotnet list package --vulnerable --include-transitive`.

### Imagens no Docker Hub
As 5 imagens (`teleseducacao-auth`, `teleseducacao-alunos`, `teleseducacao-conteudo`, `teleseducacao-pagamentos`, `teleseducacao-bff`) são publicadas por `cd.yml` sob o usuário/organização configurado no secret `DOCKERHUB_USERNAME` do repositório. O overlay `k8s/overlays/local/kustomization.yaml` resolve nome e tag da imagem via o transformer `images:` do Kustomize — não há mais placeholder para editar manualmente.

---

## **9. Observabilidade e Resiliência**
* **Logs estruturados:** Serilog com saída JSON compacta e enrichers, facilitando agregação em ferramentas externas.
* **Métricas Prometheus:** endpoint `/metrics` exposto via OpenTelemetry em todos os serviços; os pods Kubernetes trazem as annotations `prometheus.io/scrape`, `prometheus.io/port` e `prometheus.io/path` para descoberta automática.
* **Resiliência HTTP:** os 3 `HttpClient`s do BFF (Auth, Alunos, Conteúdo) usam retry com Polly.
* **Readiness real do BFF:** `/health/ready` verifica os 3 serviços downstream (antes era um endpoint vazio que sempre respondia `Healthy`) — responde `503` se algum estiver fora e `200` quando todos voltam.
* **Mensageria resiliente:** consumo de eventos via EasyNetQ com retry, backoff e *dead-letter queue* para mensagens que falham repetidamente, validado contra RabbitMQ real.
* **Segurança de rede:** CORS restrito a uma lista de origens explícitas fora de Development (uma lista vazia derruba o startup com um erro claro, em vez de silenciosamente aceitar qualquer origem); validação de certificado TLS deixa de ser desabilitada fora de Development.

---

## **10. Avaliação e Contribuições**
* Este é um projeto acadêmico; contribuições externas não são aceitas no momento.
* Para dúvidas, utilize a aba de **Issues**.
* O arquivo `FEEDBACK.md` é reservado exclusivamente para as avaliações do instrutor.

---


### Desenvolvido por
- [Guilherme Sant'Anna](https://github.com/svcguilherme)
- [Jefferson Molaz](https://github.com/jmolaz)
- [Karollainny Teles](https://github.com/karollainnyteles)
- [Rafael Secco](https://github.com/rafsecco)
