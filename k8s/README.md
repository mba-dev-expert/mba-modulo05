# Kubernetes - Teles Educação

Manifests YAML "puros" (sem Kustomize) para rodar toda a plataforma em um cluster local (Minikube).

## Pré-requisitos

- `kubectl`
- Um cluster local: [Minikube](https://minikube.sigs.k8s.io/)
- As 5 imagens publicadas no Docker Hub pelo workflow `cd.yml` (ou geradas localmente com `docker compose build` e enviadas para um registry acessível pelo cluster)

## Estrutura

```
k8s/
├── namespace.yaml                  # cria o namespace "teleseducacao"
├── configmap-app-config.yaml       # configuração não sensível (ambiente, URLs internas)
├── secret-app-secrets.yaml         # connection strings, credenciais do RabbitMQ/SQL Server
├── infra/
│   ├── sqlserver-{deployment,service,pvc}.yaml
│   └── rabbitmq-{deployment,service,pvc}.yaml
├── auth/{auth-deployment,auth-service}.yaml
├── alunos/{alunos-deployment,alunos-service}.yaml
├── conteudo/{conteudo-deployment,conteudo-service}.yaml
├── pagamentos/{pagamentos-deployment,pagamentos-service}.yaml
└── bff/{bff-deployment,bff-service}.yaml
```

## Passo a passo

### 1. Crie o cluster

```bash
minikube start
```

### 2. Ajuste as imagens

Os 5 arquivos `k8s/*/*-deployment.yaml` referenciam `<DOCKERHUB_USERNAME>/teleseducacao-<servico>:latest`. Substitua `<DOCKERHUB_USERNAME>` pelo usuário/organização configurado nos secrets `DOCKERHUB_USERNAME`/`DOCKERHUB_TOKEN` do workflow `cd.yml`:

```bash
# Linux/macOS
sed -i 's/<DOCKERHUB_USERNAME>/seu-usuario/' k8s/*/*-deployment.yaml

# Windows (PowerShell)
Get-ChildItem k8s\*\*-deployment.yaml | ForEach-Object {
    (Get-Content $_) -replace '<DOCKERHUB_USERNAME>', 'seu-usuario' | Set-Content $_
}
```

### 3. Aplique os manifests

A ordem não é estritamente obrigatória (Deployments em `CrashLoopBackOff` por dependências ainda não prontas se recuperam automaticamente), mas para um primeiro apply organizado:

```bash
kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/configmap-app-config.yaml -f k8s/secret-app-secrets.yaml
kubectl apply -f k8s/infra/ -R
kubectl apply -f k8s/auth/ -f k8s/alunos/ -f k8s/conteudo/ -f k8s/pagamentos/ -f k8s/bff/ -R
```

Ou tudo de uma vez:

```bash
kubectl apply -f k8s/ -R
```

### 4. Acompanhe os pods

```bash
kubectl get pods -n teleseducacao -w
```

O SQL Server costuma levar de 1 a 2 minutos para aceitar conexões; os serviços de API podem reiniciar algumas vezes até o banco e o RabbitMQ ficarem prontos (ver Troubleshooting).

### 5. Acesse o BFF

```bash
kubectl port-forward svc/bff 5035:5035 -n teleseducacao
```

Acesse `http://localhost:5035/swagger`.

## Portas e probes

| Serviço | Porta | Liveness | Readiness |
| :--- | :--- | :--- | :--- |
| Auth | 5101 | `/health/live` | `/health/ready` (SQL Server + RabbitMQ) |
| Alunos | 5201 | `/health/live` | `/health/ready` (SQL Server + RabbitMQ) |
| Conteúdo | 5301 | `/health/live` | `/health/ready` (SQL Server + RabbitMQ) |
| Pagamentos | 5401 | `/health/live` | `/health/ready` (SQL Server + RabbitMQ) |
| BFF | 5035 | `/health/live` | `/health/ready` (sem dependências) |
| database (SQL Server) | 1433 | TCP | TCP |
| rabbitmq | 5672 / 15672 | `rabbitmq-diagnostics ping` | `rabbitmq-diagnostics ping` |

## Troubleshooting

### Pods de API em `CrashLoopBackOff` no início

As APIs rodam migrations no startup e dependem do SQL Server e do RabbitMQ estarem prontos. Como os manifests não usam `initContainers` de espera, é normal que os primeiros pods reiniciem algumas vezes até `database` e `rabbitmq` ficarem `Running`/`Ready`. O Kubernetes reinicia automaticamente os pods com `restartPolicy` padrão; aguarde alguns minutos e confira novamente com `kubectl get pods -n teleseducacao`.

Se o problema persistir, veja os logs:

```bash
kubectl logs -n teleseducacao deployment/auth
kubectl logs -n teleseducacao deployment/database
```

### `ImagePullBackOff`

Confirme que `<DOCKERHUB_USERNAME>` foi substituído nos 5 arquivos `*-deployment.yaml` e que as imagens existem no Docker Hub (publicadas pelo `cd.yml` via push em `main` ou `workflow_dispatch`).

### SQL Server demorando para iniciar no Minikube

O SQL Server pode levar mais tempo em ambientes com poucos recursos. Aumente `initialDelaySeconds` em `k8s/infra/sqlserver-deployment.yaml` se os probes estiverem falhando antes do banco terminar de iniciar.

### Remover tudo

```bash
kubectl delete namespace teleseducacao
```
