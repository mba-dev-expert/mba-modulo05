# Erros encontrados e correções (sessão de 08/07/2026)

Contexto: ao subir a plataforma no Minikube, o Swagger do BFF (`http://localhost:5035/swagger`) não abria. Investigando isso, foram encontrados 4 problemas em cadeia — de infraestrutura e de código — que impediam a aplicação de funcionar de ponta a ponta.

---

## 1. RabbitMQ nunca ficava "pronto" → tudo travava

**Onde:** `k8s/infra/rabbitmq-deployment.yaml`

**O que acontecia:** o Kubernetes testa se o RabbitMQ está saudável rodando o comando `rabbitmq-diagnostics ping`. Esse teste tinha um limite de tempo de apenas 1 segundo (o padrão do Kubernetes), e no Minikube o comando às vezes demora um pouco mais que isso. Resultado: o teste "falhava" por lentidão, não porque o RabbitMQ estivesse com problema de verdade.

**Efeito cascata:** quando um pod não passa nesse teste, o Kubernetes o tira de circulação — literalmente remove ele da lista de endereços que os outros serviços conseguem alcançar. E os outros serviços (`auth`, `alunos`, `conteudo`, `pagamentos`, `bff`) têm uma etapa de inicialização que fica esperando "consigo falar com o RabbitMQ?" antes de ligar. Como nunca conseguiam, ficavam travados para sempre em "inicializando".

**Correção:** aumentar o tempo limite do teste de 1s para 10s.

---

## 2. Reiniciar o banco de dados (SQL Server) quebrava tudo

**Onde:** `k8s/infra/sqlserver-deployment.yaml`

**O que acontecia:** por padrão, quando o Kubernetes reinicia um serviço, ele sobe a cópia nova **antes** de desligar a cópia antiga (pra não ter downtime). Isso é ótimo para serviços sem estado, mas o SQL Server guarda seus arquivos de dados num disco compartilhado (PVC) que só pode ser escrito por um processo de cada vez. Com as duas cópias ativas ao mesmo tempo, a nova brigava com a antiga pelos mesmos arquivos e travava com erro de permissão ("Access is denied").

**Correção:** configurar a estratégia de atualização como "Recreate" — ou seja, desligar a cópia antiga por completo antes de subir a nova.

---

## 3. Cadastro de usuário (`/Auth/registrar`) devolvia erro 500 / 406

**Onde:** `src/api-gateways/TelesEducacao.Bff.Plataforma/Program.cs`

**O que acontecia:** havia um trecho de código no BFF (o "porteiro" que recebe as chamadas do Swagger e repassa para os serviços internos) que forçava manualmente o cabeçalho `Content-Type` da resposta antes de o serviço terminar de montar essa resposta. Isso confundia o ASP.NET Core: quando a resposta ficava pronta, o framework não sabia mais como formatá-la e devolvia um erro genérico "não consigo responder isso" (406), no lugar do resultado real do cadastro.

**Correção:** remover esse trecho — o framework já formata a resposta como JSON corretamente sozinho, esse código extra não era necessário e estava quebrando tudo.

**Detalhe técnico à parte:** para essa correção (e a do item 4) valerem, foi preciso também ensinar o Kubernetes a não ficar sempre baixando a versão antiga da imagem publicada na internet (Docker Hub) toda vez que o serviço reiniciava — ele ignorava a versão corrigida que construímos localmente. Ajustado para usar a imagem local quando ela já existe.

---

## 4. Login (`/Auth/acessar`) sempre devolvia "usuário ou senha incorretos", mesmo com a senha certa

**Onde:** `src/services/TelesEducacao.Auth.Data/AuthDbContext.cs`

**O que acontecia:** esse era o bug mais escondido. Existia uma regra genérica no código dizendo "toda informação do tipo texto guardada no banco tem no máximo 100 caracteres" (um atalho para evitar colunas gigantes sem necessidade). O problema é que essa regra também se aplicava, sem querer, à tabela que guarda a chave secreta usada para assinar o token de login (JWT) — e essa chave, quando convertida pra texto, tem bem mais que 100 caracteres.

Então, toda vez que alguém tentava logar, o sistema tentava gerar o token, tentava salvar a chave no banco, o banco recusava porque não cabia no espaço reservado, e a operação inteira falhava silenciosamente — o usuário só via "senha incorreta", mesmo digitando certo. **Ninguém jamais conseguiria logar nesse sistema, não importa o usuário.**

**Correção:** abrir uma exceção só para essa coluna específica, permitindo texto de tamanho ilimitado, e gerar/aplicar uma migration de banco de dados para isso.

---

## Resumo rápido

| # | Sintoma visível | Causa real |
|---|---|---|
| 1 | Pods presos em "inicializando" pra sempre | Teste de saúde do RabbitMQ com tempo curto demais |
| 2 | Banco trava com "Access is denied" ao reiniciar | Kubernetes sobe pod novo antes de desligar o antigo |
| 3 | Cadastro de usuário retorna erro genérico | Código do BFF mexendo onde não devia no cabeçalho da resposta |
| 4 | Login sempre nega, mesmo com senha certa | Coluna do banco pequena demais para a chave de assinatura do JWT |

Todos os 4 itens foram corrigidos e testados de ponta a ponta (registro de usuário e login retornando `201`/`200` com sucesso).
