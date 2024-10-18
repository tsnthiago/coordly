## Seção 1: SQL  

### 1.1. Otimização de Query

**Pergunta**: Escreva uma query que otimize a seguinte consulta para grandes volumes de dados.

```sql
SELECT * FROM Orders
WHERE OrderDate BETWEEN '2023-01-01' AND '2023-12-31'
ORDER BY CustomerID;
```

**Resposta**:


```sql
SELECT OrderID, CustomerID, OrderDate, ProductID
FROM Orders
WHERE OrderDate BETWEEN '2023-01-01' AND '2023-12-31'
ORDER BY CustomerID
OFFSET 0 ROWS FETCH NEXT 100 ROWS ONLY;
```

Melhorias implementadas:

- Com essa query, trazemos apenas as colunas necessárias para o escopo necessário, melhorando a performance da consulta
- Implementei paginação na query, para que ao invés de trazer um grande volume de dados de uma única vez, ela traga apenas de 100 em 100 registros.
- Eu também criaria índices para as colunas OrderDate e CustomerId para que o banco consiga localizar mais rapidamente os registros desejados. Por exemplo, o CustomerId melhora a ordenação e o OrderDate melhora o filtro por datas.

Para criar os índices dessas colunas eu seguiria os seguintes passos:

1. Verificar a estrutura da tabela e os índices já existentes.

```sql
SHOW INDEXES FROM Orders;
```

2. Criar índice na coluna OrderDate

```sql
CREATE INDEX idx_orderdate ON Orders(OrderDate)
```

Esse índice permite que o banco de dados procure mais rapidamente pelos registros que estão dentro de um intervalo de datas, o que melhora a performance do filtro `BETWEEN`.

3. Criar índice na coluna CustomerId

```sql
CREATE INDEX idx_customerid ON Orders(CustomerID);
```

Esse índice ajuda na execução de consultas que envolvem a cláusula `ORDER BY CustomerID`. Sem um índice, o banco de dados precisaria ordenar os resultados manualmente após buscar os dados, o que é muito mais lento.


### 1.2. Tipos de JOIN

**Pergunta**: Qual é a diferença entre INNER JOIN, LEFT JOIN e CROSS JOIN?

**Resposta**:

- O INNER JOIN retornar apenas registros que possuem correspondência em ambas as tabelas.
	Ex: No caso de uma tabela de **Clientes** e outra de **Pedidos**. Nós usaríamos INNER JOIN para obter os clientes que já tem algum pedido associado.

- LEFT JOIN retorna todos os registros da tabela à esquerda, com os registros correspondentes da tabela à direita (se houver).
	Ex: Se quisermos uma lista de todos os usuários, independente de terem feito pedidos ou não, podemos usar o LEFT JOIN. Dessa forma veríamos todos os clientes, mas com o valor 'NULL' nos campos de pedidos para aqueles que não têm nenhum pedido.

- CROSS JOIN faz uma combinação de cada registro da primeira tabela com cada registro da segunda, independente de uma condição.
	Ex: O CROSS JOIN é mais raro em cenários práticos, pois geralmente tende a gerar grandes volumes de dados. Mas ele pode ser útil em situações de análises, como obter todas as combinações possíveis de opções em um menu de produtos.


---
## Seção 2: Entity Framework
### 2.1. Modos de Carregamento

**Pergunta**: Explique a diferença entre os modos de carregamento Lazy, Eager e Explicit no Entity Framework.

**Resposta**:

- **Lazy**: carrega as entidades relacionadas sob demanda, ou seja, apenas quando tentamos acessar essas entidades. Por padrão as entidades relacionadas não são carregadas imediatamente ao recuperar a entidade principal. 

- **Eager**: carrega as entidades relacionadas imediatamente, junto com a entidade principal em uma única consulta ao banco de dados. Isso é feito usando o método **Include()**.

- **Explicit**: você carrega a entidade principal, e quando precisar dos dados relacionados, você instrui explicitamente o Entity Framework a carregá-los usando o método **Load()**.

### 2.2. Uso de AsNoTracking()

**Pergunta**: Descreva um cenário onde o uso de AsNoTracking() seria necessário e explique por que.

**Resposta**:

Um exemplo prático dentro da Coordly onde seria necessário o uso de AsNoTracking() seria por exemplo um endpoint que lista todos os arquivos de um projeto de construção específico, como plantas, relatórios e especificações técnicas para exibição em uma tela de acompanhamento ou de revisão por engenheiros. Esses documentos são apenas para visualização e leitura, sem a necessidade de edição, então nesse caso, seria necessário o uso desta função.

Exemplo:

```cs
var arquivosProjeto = context.ArquivosProjetos
                             .Where(a => a.ProjetoID == projetoID)
                             .AsNoTracking()
                             .ToList();

```

Se um engenheiro deseja acessar rapidamente uma lista de todos os documentos relacionados a uma obra em campo, o uso de `AsNoTracking()` garante que a consulta seja rápida e eficiente, já que o sistema precisa apenas mostrar os documentos para consulta, sem a intenção de modificá-los.

---
## Seção 3: Arquitetura de Software

### 3.1. Arquitetura Monolítica vs Microsserviços
 
**Pergunta**: Explique a diferença entre arquitetura monolítica e arquitetura de microsserviços.

**Resposta**:

Arquitetura monolítica é um tipo de design de software em que todos os componentes e funcionalidades do sistema são integrados e executados como uma única aplicação. Tudo faz parte do mesmo "bloco" de código, incluindo interface de usuário (frontend) e lógica de negócios e acesso a dados (backend).

Como o código é todo unificado, toda vez que uma parte do sistema é atualizada, a aplicação inteira precisa ser redeployada.

Existe forte acoplamento do sistema. Qualquer mudança pode afetar o resto da aplicação.

## Vantagens

- Simples de desenvolver e testar
- Facilidade para debugar
- Menor complexidade inicial

## Desvantagens

- Dificuldade de escalabilidade
- Manutenção complicada
- Implantação arriscada.


Já na arquitetura de **microsserviços** por sua vez, a aplicação é composta por vários serviços independentes, cada um responsável por uma função específica. Esses serviços se comunicam entre si geralmente via API's, mas funcionam de forma independente.

## Vantages

- Escalabilidade independente
- Facilidade de manutenção
- Resiliência

## Desvantagens

- Complexidade na comunicação
- Gerenciamento distribuído
- Sobrecarga operacional


Cada arquitetura possui situações suas vantagens e desvantagens e tem cenários que favorecem mais a adoção de uma ou de outra. Por exemplo, a monolítica pode ser ideal para projetos menores, ou quando desejamos simplificar a gestão e manter o controle centralizado, como uma pequena aplicação empresarial.
Já os microsserviços são preferíveis em sistemas grandes e com necessidade de escalabilidade, ou quando diferentes partes do sistema precisam evoluir de forma independente, como em plataformas complexas de SaaS, como o caso da Coordly.

---
## Seção 4: Problema Prático 

### 4.1. API RESTful com .NET Core e Entity Framework Core

**Tarefa**: Desenvolva uma API simples utilizando .NET Core e Entity Framework Core para o gerenciamento de produtos, com as operações CRUD.

**Requisitos Funcionais**: 

1. Criar, atualizar, listar e deletar produtos.
2. Propriedades dos produtos:
   - ProductID: Identificador único.
   - Name: Nome do produto (obrigatório).
   - Price: Preço do produto (obrigatório, maior que zero).
   - StockQuantity: Quantidade em estoque (obrigatório, maior ou igual a zero).  

**Requisitos Não Funcionais**:

- Arquitetura: Separação de camadas (Models, Repositories, Controllers).
- Validação: Implementar validações para nome, preço e quantidade.
- Documentação da API com Swagger.  
