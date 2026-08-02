# Kubernetes - Teles Educação

Manifests organizados com **Kustomize** para rodar toda a plataforma em um cluster local (Minikube/Kind), com autoscaling, armazenamento estável para o SQL Server e Ingress.

## Pré-requisitos

- `kubectl` (traz suporte a Kustomize embutido, usado por `kubectl apply -k`)
- Um cluster local: [Minikube](https://minikube.sigs.k8s.io/) ou [Kind](https://kind.sigs.k8s.io/)
- As 5 imagens (`teleseducacao-auth`, `teleseducacao-alunos`, `teleseducacao-conteudo`, `teleseducacao-pagamentos`, `teleseducacao-bff`) publicadas no Docker Hub pelo workflow `cd.yml`, **ou** construídas localmente e carregadas no cluster (ver [Passo 2](#2-construa-ou-obtenha-as-imagens))
- Para autoscaling (HPA): [`metrics-server`](https://github.com/kubernetes-sigs/metrics-server) instalado no cluster (`minikube addons enable metrics-server` no Minikube)
- Para o Ingress: um ingress controller — [ingress-nginx](https://github.com/kubernetes/ingress-nginx) (`minikube addons enable ingress` no Minikube)

## Estrutura

```
k8s/
├── base/                              # manifestos genéricos (sem usuário de registry, sem segredos reais)
│   ├── namespace.yaml                 # cria o namespace "teleseducacao"
│   ├── configmap-app-config.yaml      # configuração não sensível (ambiente, URLs internas, CORS)
│   ├── ingress.yaml                   # Ingress nginx: BFF na raiz, APIs por path (/auth, /alunos, ...)
│   ├── kustomization.yaml             # agrega todos os recursos abaixo
│   ├── infra/
│   │   ├── sqlserver-statefulset.yaml # SQL Server como StatefulSet (volumeClaimTemplates)
│   │   ├── sqlserver-service.yaml
│   │   ├── rabbitmq-{deployment,service,pvc}.yaml
│   ├── auth/{auth-deployment,auth-service,auth-hpa}.yaml
│   ├── alunos/{alunos-deployment,alunos-service,alunos-hpa}.yaml
│   ├── conteudo/{conteudo-deployment,conteudo-service,conteudo-hpa}.yaml
│   ├── pagamentos/{pagamentos-deployment,pagamentos-service,pagamentos-hpa}.yaml
│   └── bff/{bff-deployment,bff-service,bff-hpa}.yaml
└── overlays/
    └── local/                          # overlay para cluster local (Minikube/Kind)
        ├── kustomization.yaml          # resolve tag das imagens + secretGenerator
        ├── app-secrets.env.example     # versionado: template das credenciais
        └── app-secrets.env             # NÃO versionado (.gitignore): credenciais reais locais
```

A base não é pensada para ser aplicada sozinha (`kubectl apply -k k8s/base` falha: falta a Secret `app-secrets`, gerada só pelo overlay). Aplique sempre um overlay — hoje só existe `overlays/local`.

## Passo a passo

### 1. Crie o cluster

```bash
minikube start
minikube addons enable ingress
minikube addons enable metrics-server
```

### 2. Construa ou obtenha as imagens

O overlay `local` referencia as imagens como `teleseducacao-<serviço>:local`. Duas formas de obtê-las:

**a) Build local (sem depender do Docker Hub), a partir da raiz do repositório:**
```bash
docker build -t teleseducacao-auth:local -f docker/Dockerfile.auth .
docker build -t teleseducacao-alunos:local -f docker/Dockerfile.alunos .
docker build -t teleseducacao-conteudo:local -f docker/Dockerfile.conteudo .
docker build -t teleseducacao-pagamentos:local -f docker/Dockerfile.pagamentos .
docker build -t teleseducacao-bff:local -f docker/Dockerfile.bff .

# Minikube:
minikube image load teleseducacao-auth:local
minikube image load teleseducacao-alunos:local
minikube image load teleseducacao-conteudo:local
minikube image load teleseducacao-pagamentos:local
minikube image load teleseducacao-bff:local

# Kind (alternativa ao passo acima):
# kind load docker-image teleseducacao-auth:local
```

**b) Imagens publicadas pelo `cd.yml`:** ajuste `k8s/overlays/local/kustomization.yaml` (bloco `images:`) para apontar `newName` para `<usuário-docker-hub>/teleseducacao-<serviço>` e `newTag` para a tag desejada (`latest` ou o SHA do commit), em vez de editar manualmente cada `*-deployment.yaml` — é o Kustomize quem resolve nome e tag para os 5 serviços de uma vez.

### 3. Configure os segredos

```bash
cp k8s/overlays/local/app-secrets.env.example k8s/overlays/local/app-secrets.env
```

Ajuste as senhas em `app-secrets.env` se desejar (o arquivo não é versionado — está no `.gitignore` via o padrão `**.env`). O `secretGenerator` do overlay gera a Secret `app-secrets` a partir desse arquivo.

### 4. Aplique os manifests

```bash
kubectl apply -k k8s/overlays/local
```

Esse único comando renderiza a base inteira (namespace, ConfigMap, Ingress, StatefulSet do SQL Server, RabbitMQ, os 5 serviços com seus HPAs) com a Secret gerada e as imagens resolvidas, e envia tudo ao cluster.

Para conferir o que seria aplicado sem enviar nada ao cluster:

```bash
kubectl kustomize k8s/overlays/local
```

### 5. Acompanhe os pods

```bash
kubectl get pods -n teleseducacao -w
```

O SQL Server (pod `database-0`, gerenciado pelo StatefulSet) costuma levar de 1 a 2 minutos para aceitar conexões. Os `initContainers` das APIs (`wait-for-dependencies`) esperam ativamente `database`/`rabbitmq` (e, no caso do BFF, `auth`/`alunos`/`conteudo`) responderem na porta antes do container principal iniciar — isso evita a maior parte dos `CrashLoopBackOff` por dependência não pronta, mas as migrations e o `readinessProbe` de cada API ainda podem levar mais alguns ciclos até `Ready` (ver Troubleshooting).

### 6. Acesse a aplicação

**Via Ingress (recomendado):**

```bash
# Minikube: exponha o IP do controller
minikube tunnel
# ou resolva o host manualmente:
echo "$(minikube ip) teleseducacao.local" | sudo tee -a /etc/hosts
```

Acesse `http://teleseducacao.local/swagger` (BFF, na raiz) ou `http://teleseducacao.local/auth/swagger`, `/alunos`, `/conteudo`, `/pagamentos` para as APIs de domínio.

**Via port-forward (alternativa sem Ingress):**

```bash
kubectl port-forward svc/bff 5035:5035 -n teleseducacao
```

Acesse `http://localhost:5035/swagger`.

## Autoscaling (HPA)

Os 5 serviços stateless (Auth, Alunos, Conteúdo, Pagamentos, BFF) têm `replicas: 2` no Deployment e um `HorizontalPodAutoscaler` próprio (`autoscaling/v2`) escalando de **2 a 5 réplicas** por **70% de utilização média de CPU**. Requer `metrics-server` instalado no cluster — sem ele, `kubectl get hpa` mostra `<unknown>` nas métricas e o HPA não escala. Para observar em ação:

```bash
kubectl get hpa -n teleseducacao -w
```

## Ingress

O Ingress (`k8s/base/ingress.yaml`) usa `ingressClassName: nginx` e espera o [ingress-nginx](https://github.com/kubernetes/ingress-nginx) instalado no cluster. O BFF fica na raiz (`/`) como ponto de entrada único para o front-end; as 4 APIs de domínio também são expostas por path (`/auth`, `/alunos`, `/conteudo`, `/pagamentos`) para chamadas diretas e depuração, com `rewrite-target` removendo o prefixo antes de encaminhar ao Service.

## StatefulSet do SQL Server

O SQL Server roda como `StatefulSet` (não `Deployment` + PVC avulso), com `volumeClaimTemplates` gerenciando o PVC ligado ao pod `database-0` — garante identidade estável de armazenamento mesmo que o pod seja reagendado. `serviceName: database` aponta para um Service `ClusterIP` comum (não headless): como há uma única réplica, um `ClusterIP` já dá o nome estável (`database`) usado pelas connection strings, sem depender de o cliente re-resolver DNS a cada reagendamento.

## Segredos (`app-secrets.env`)

Nenhuma credencial fica hardcoded nos manifestos da base. O overlay `k8s/overlays/local/kustomization.yaml` declara um `secretGenerator` que lê `app-secrets.env` (não versionado) e gera a Secret `app-secrets` — o Kustomize sufixa o nome com um hash do conteúdo e atualiza automaticamente as referências (`secretKeyRef`) nos Deployments/StatefulSet, então eles continuam apontando só para `app-secrets`. Só `app-secrets.env.example` é versionado, como template.

## Portas e probes

| Serviço | Porta | Liveness | Readiness |
| :--- | :--- | :--- | :--- |
| Auth | 5101 | `/health/live` | `/health/ready` (SQL Server + RabbitMQ) |
| Alunos | 5201 | `/health/live` | `/health/ready` (SQL Server + RabbitMQ) |
| Conteúdo | 5301 | `/health/live` | `/health/ready` (SQL Server + RabbitMQ) |
| Pagamentos | 5401 | `/health/live` | `/health/ready` (SQL Server + RabbitMQ) |
| BFF | 5035 | `/health/live` | `/health/ready` (Auth + Alunos + Conteúdo downstream) |
| database (SQL Server, StatefulSet) | 1433 | TCP | TCP |
| rabbitmq | 5672 / 15672 | `rabbitmq-diagnostics ping` | `rabbitmq-diagnostics ping` |

Os 5 serviços stateless também trazem annotations `prometheus.io/scrape: "true"`, `prometheus.io/port` e `prometheus.io/path: "/metrics"` nos pods, para descoberta automática por um Prometheus operando no cluster.

## Troubleshooting

### Pods de API demorando para ficar `Ready`

Os `initContainers` (`wait-for-dependencies`) já evitam a maior parte dos reinícios por dependência ausente: as APIs de domínio esperam `database`+`rabbitmq` responderem na porta, e o BFF espera `auth`+`alunos`+`conteudo`. Ainda assim, é esperado que o pod leve um tempo a mais para `Ready`: depois que o container principal sobe, ele roda as migrations no startup, e o `readinessProbe` de `/health/ready` só fica verde quando SQL Server e RabbitMQ respondem de fato (não apenas com a porta TCP aberta). Aguarde alguns minutos e confira com:

```bash
kubectl get pods -n teleseducacao
```

Se algum pod ficar preso em `Init:0/1`, o `initContainer` ainda está esperando a dependência:

```bash
kubectl logs -n teleseducacao <pod> -c wait-for-dependencies
```

Se o problema persistir após o `Init` completar, veja os logs do container principal:

```bash
kubectl logs -n teleseducacao deployment/auth
kubectl logs -n teleseducacao statefulset/database
```

### `ImagePullBackOff`

Confirme que as imagens `teleseducacao-<serviço>:local` existem no cluster (`minikube image ls | grep teleseducacao` ou `docker exec <kind-node> crictl images`) se você seguiu o fluxo de build local, ou que `k8s/overlays/local/kustomization.yaml` aponta para um usuário/tag existente no Docker Hub se optou por consumir as imagens publicadas pelo `cd.yml`.

### HPA mostra `<unknown>` em `TARGETS`

`metrics-server` não está instalado no cluster. No Minikube: `minikube addons enable metrics-server`. Sem ele, o HPA não tem como ler a utilização de CPU dos pods e não escala.

### Ingress não resolve `teleseducacao.local`

Confirme que o ingress controller está instalado (`kubectl get pods -n ingress-nginx`) e que o host `teleseducacao.local` está mapeado para o IP do controller — via `/etc/hosts` (`minikube ip`) ou `minikube tunnel`.

### SQL Server demorando para iniciar no Minikube

O SQL Server pode levar mais tempo em ambientes com poucos recursos. Aumente `initialDelaySeconds` em `k8s/base/infra/sqlserver-statefulset.yaml` se os probes estiverem falhando antes do banco terminar de iniciar.

### Remover tudo

```bash
kubectl delete -k k8s/overlays/local
```

Ou, para remover o namespace inteiro (mais direto, mas também apaga qualquer recurso criado manualmente nele):

```bash
kubectl delete namespace teleseducacao
```
