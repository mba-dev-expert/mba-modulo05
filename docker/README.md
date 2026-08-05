# Docker Compose - Teles Educação

## Configuração e Execução

### Pré-requisitos

- Docker Desktop instalado
- Docker Compose v2+

### Passo 1: Configurar variáveis de ambiente

Na pasta `docker/`, copie o arquivo de exemplo:

```bash
cp .env.example .env
```

O arquivo `.env` contém as variáveis padrão para desenvolvimento. Você pode editá-lo se desejar usar senhas diferentes:

```env
RABBITMQ_DEFAULT_USER=teleseducacao
RABBITMQ_DEFAULT_PASS=TelesEduca123!
ACCEPT_EULA=Y
MSSQL_SA_PASSWORD=YourStrong!Passw0rd
MSSQL_PID=Developer
```

### Passo 2: Iniciar os serviços

Execute dentro da pasta `docker/`:

```bash
docker compose -f docker-compose.yml up -d --build
```

### O que será iniciado

- **RabbitMQ** (port 5672, Management 15672)
- **SQL Server 2022** (port 1433)
- **Auth API** (port 5101)
- **Alunos API** (port 5201)
- **Conteúdo API** (port 5301)
- **Pagamentos API** (port 5401)
- **BFF Plataforma** (port 5035)

Todos os 7 containers expõem `HEALTHCHECK` (as 5 APIs chamam `/health/live`; RabbitMQ usa `rabbitmq-diagnostics ping`; o SQL Server executa um `SELECT 1` via `sqlcmd`), e o `docker-compose.yml` usa `depends_on` com `condition: service_healthy` para as APIs só subirem depois que `database` e `rabbitmq` estiverem de fato saudáveis.

### Verificando a saúde dos serviços

Aguarde alguns minutos para que todos os serviços iniciem. Os logs mostrarão o progresso. As APIs estarão prontas quando exibirem:

```
Now listening on: http://0.0.0.0:5XXX
Application started. Press Ctrl+C to shut down.
```

### Acessando os serviços

#### RabbitMQ Management
- URL: `http://localhost:15672`
- Usuário: `teleseducacao`
- Senha: `TelesEduca123!`

#### Swagger das APIs
- Auth: `http://localhost:5101/swagger`
- Alunos: `http://localhost:5201/swagger`
- Conteúdo: `http://localhost:5301/swagger`
- BFF: `http://localhost:5035/swagger`

### Parar os serviços

```bash
docker compose down
```

### Remover volumes (limpeza completa)

```bash
docker compose down -v
```

## Configuração de Ambiente

### Como as credenciais chegam às APIs

As 4 APIs de domínio (Auth, Alunos, Conteúdo, Pagamentos) **não leem** credenciais de banco/fila do `appsettings.Docker.json` nem diretamente do `.env`. O `docker-compose.yml` injeta a connection string e a string de conexão do Message Bus como variáveis de ambiente por serviço, interpolando os valores do `.env`:

```yaml
environment:
  - ConnectionStrings__DefaultConnection=Server=database,1433;...;Password=${MSSQL_SA_PASSWORD};...
  - MessageQueueConnection__MessageBus=host=rabbitmq:5672;username=${RABBITMQ_DEFAULT_USER};password=${RABBITMQ_DEFAULT_PASS};...
```

A variável de ambiente tem precedência sobre o `appsettings.Docker.json` no `IConfiguration` do ASP.NET Core — por isso **alterar a senha no `.env`**.

O `appsettings.Docker.json` de cada serviço só contém configuração não sensível (URLs internas dos outros serviços, parâmetros do Message Bus, CORS) — nenhuma credencial fica hardcoded nesses arquivos.

### Arquivos appsettings.Docker.json

- `ASPNETCORE_ENVIRONMENT=Docker` ativa a leitura desses arquivos.
- As URLs internas apontam para os nomes dos serviços do Docker Compose (ex: `database`, `rabbitmq`, `auth`).

### Variáveis de Ambiente

As variáveis de ambiente no `docker-compose.yml`, por serviço:
- `ASPNETCORE_ENVIRONMENT`: define qual arquivo `appsettings.{Environment}.json` será carregado.
- `ASPNETCORE_URLS`: define a porta HTTP.
- `ConnectionStrings__DefaultConnection` (Auth, Alunos, Conteúdo, Pagamentos): connection string do SQL Server, com a senha interpolada de `${MSSQL_SA_PASSWORD}`.
- `MessageQueueConnection__MessageBus` (Auth, Alunos, Conteúdo, Pagamentos): connection string do RabbitMQ, com usuário/senha interpolados de `${RABBITMQ_DEFAULT_USER}`/`${RABBITMQ_DEFAULT_PASS}`.

## Troubleshooting

### As APIs não conseguem conectar ao banco de dados

Verifique se o serviço `database` está saudável. Execute:

```bash
docker compose logs database
```

Aguarde a mensagem `MSSQL is now ready to accept connections`.

### RabbitMQ está em loop de reinicialização

O RabbitMQ pode levar tempo para iniciar. Verifique com:

```bash
docker compose logs rabbitmq
```

### Erro "Connection refused" nas APIs

Certifique-se de que todos os serviços iniciaram completamente. Aguarde alguns minutos e verifique os logs:

```bash
docker compose logs auth
docker compose logs alunos
```

### Alterar senha do SQL Server

Esse procedimento funciona de ponta a ponta: a nova senha é usada tanto para inicializar o container do SQL Server quanto para montar a `ConnectionStrings__DefaultConnection` de cada API, já que ambos leem a mesma variável `${MSSQL_SA_PASSWORD}` do `.env`.

1. Edite o arquivo `.env` e atualize `MSSQL_SA_PASSWORD`
2. Execute `docker compose down -v` para remover o volume anterior (o SQL Server só aplica a senha do `MSSQL_SA_PASSWORD` na criação do banco/volume; sem `-v` a senha antiga permaneceria válida no volume existente)
3. Execute `docker compose up -d --build` novamente

## Desenvolvimento

### Reconstruir um serviço específico

```bash
docker compose up --build auth
```

### Ver logs em tempo real

```bash
docker compose logs -f [nome-do-serviço]
```

Exemplo: `docker compose logs -f alunos`

### Executar comando em um container

```bash
docker compose exec [serviço] [comando]
```

Exemplo: `docker compose exec database /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong!Passw0rd" -Q "SELECT 1" -C`
