# Erros encontrados e correções — parte 2 (sessão de 08/07/2026)

Continuação do [erros.md](erros.md), depois que a plataforma já estava de pé. Esses bugs apareceram testando os fluxos reais (login, refresh de token e matrícula em curso).

---

## 5. Refresh token dando erro 500 (em vez de "sessão expirada")

**Onde:** `src/services/TelesEducacao.Auth.Application/Services/AuthService.cs`

**O que acontecia:** o código buscava o refresh token no banco e, **antes de checar se ele existia**, já tentava ler a data de expiração dele:

```csharp
var token = await _context.RefreshTokens...FirstOrDefaultAsync(...);
var expirationDate = token.ExpirationDate.ToLocalTime(); // quebra se token for null
return token != null && expirationDate > DateTime.Now ? token : null;
```

Se o token não existisse no banco (já foi trocado por um login mais novo, ou o valor estava errado/expirado), `token` vinha `null`, e a linha seguinte tentava acessar `.ExpirationDate` de um objeto nulo — estourava exceção e a API respondia `500`.

**Correção:** checar `if (token == null) return null;` antes de acessar qualquer propriedade dele. Agora, token inválido/inexistente responde `401 Sessão expirada`, como já era a intenção original do código.

---

## 6. Matrícula em curso devolvendo 400 sem explicação nenhuma

**Onde:** `src/api-gateways/TelesEducacao.Bff.Plataforma/Services/AlunoService.cs`

**O que acontecia:** o BFF montava o corpo da requisição pra mandar pro serviço `alunos`, mas fazia isso errado — serializava o objeto **duas vezes**:

```csharp
var conteudo = ObterConteudo(matriculaDto);  // já transforma em texto/JSON
var response = await _httpClient.PostAsJsonAsync(url, conteudo, ...); // tenta serializar de novo, agora o "texto" inteiro
```

`PostAsJsonAsync` espera receber o **objeto original** pra serializar sozinho — não um JSON que já foi montado à mão. Ao receber o conteúdo já pronto, ele serializava *isso* (a "caixa" que guarda o texto), e não os dados de verdade (nome do aluno, número do cartão, etc.). Resultado: o serviço `alunos` recebia um corpo vazio, sem os campos esperados, e recusava com `400` genérico — sem nenhuma mensagem útil pra entender o motivo.

**Correção:** parar de montar o conteúdo manualmente e deixar o `PostAsJsonAsync` serializar o objeto original direto.

---

## 7. Matrícula com aluno inexistente derrubando o servidor com 500

**Onde:** `src/services/TelesEducacao.Alunos.Application/Commands/AdicionarMatriculaCommandHandler.cs`

**O que acontecia:** ao tentar matricular um `alunoId` que não existe na base (ex.: digitado errado, ou é o ID de login em vez do ID do aluno), o sistema não validava se o aluno existia antes de tentar gravar a matrícula. O banco recusava a gravação (regra de integridade: não pode ter matrícula de um aluno que não existe) e essa recusa virava uma exceção não tratada — a API caía com `500`.

Esse `500`, por sua vez, ativava um mecanismo de proteção do BFF (o "circuit breaker", que existe pra parar de bater numa API que está com problema sério) e passava a **bloquear todas as chamadas seguintes por 30 segundos**, mesmo as válidas — um efeito colateral chato pra quem estivesse testando.

**Correção:** checar explicitamente se o aluno existe antes de tentar criar a matrícula. Se não existir, devolve `400` com uma mensagem clara ("Aluno não encontrado"), em vez de deixar o banco recusar e a exceção estourar.

---

## Achado, mas ainda não corrigido

Durante essa investigação apareceu outro problema, num fluxo diferente (recusa de pagamento): quando um pagamento é recusado, o sistema tenta cancelar a matrícula automaticamente, mas essa parte do código **não está configurada corretamente** e quebra silenciosamente em segundo plano (não afeta a resposta que o usuário vê, mas a matrícula fica "presa" sem ser cancelada). Fica registrado pra corrigir depois.

---

## Resumo rápido

| # | Sintoma visível | Causa real |
|---|---|---|
| 5 | Refresh token sempre dá erro 500 | Código acessava dado de um token nulo antes de checar se ele existia |
| 6 | Matrícula retorna 400 sem explicação | BFF serializava o corpo da requisição duas vezes, mandando dados vazios |
| 7 | Matrícula com aluno errado derruba com 500 (e trava chamadas seguintes) | Faltava checar se o aluno existe antes de gravar a matrícula |

Todos os 3 itens foram corrigidos, reimplantados no Minikube e testados com sucesso.
