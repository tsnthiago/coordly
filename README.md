# ProductAPI

## Descrição do Projeto

Este projeto é uma API RESTful construída com **ASP.NET Core 8** e **Entity Framework Core 8**, que oferece operações CRUD (Criar, Ler, Atualizar, Deletar) para o gerenciamento de produtos. Cada produto tem propriedades como `ProductID`, `Name`, `Price` e `StockQuantity`, e a aplicação utiliza boas práticas de arquitetura com camadas de **Models** e **Controllers**.

A aplicação utiliza **SQLite** como banco de dados e oferece documentação interativa de endpoints através do **Swagger**.

## Funcionalidades

- Criar, listar, atualizar e deletar produtos.
- Validações de:
  - Nome (obrigatório).
  - Preço (deve ser maior que zero).
  - Quantidade de estoque (deve ser maior ou igual a zero).
- Documentação interativa com **Swagger**.
- Testes unitários para o `ProductsController`.

## Tecnologias Utilizadas

- **ASP.NET Core 8.0**
- **Entity Framework Core 8.0**
- **SQLite**
- **Swagger para documentação**
- **xUnit para testes unitários**
- **C#**

## Estrutura do Projeto

```bash
ProductAPI/
├── Controllers/
│   └── ProductsController.cs       # Lida com as requisições HTTP
├── Models/
│   ├── AppDbContext.cs             # Configura o contexto do banco de dados
│   └── Product.cs                  # Modelo de domínio Produto
├── appsettings.json                # Configuração de banco de dados e outros parâmetros
├── ProductAPI.csproj               # Arquivo de projeto .NET
└── Program.cs                      # Configuração de inicialização da aplicação
```

## Pré-requisitos

Para rodar este projeto, você precisará ter o **.NET 8.0 SDK** e o **SQL Server** instalados. Se estiver em um ambiente Linux, o WSL pode ser usado para rodar o .NET.

### Instalar .NET SDK

1. Adicione o repositório e instale o SDK do .NET:

   ```bash
   sudo apt update
   sudo apt install -y dotnet-sdk-8.0
   ```

2. Verifique a instalação:
   ```bash
   dotnet --version
   ```

### Configurar SQL Server

No `appsettings.json`, defina a string de conexão para o SQL Server local:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ProductDb;Trusted_Connection=True;"
  }
}
```

Certifique-se de que o SQL Server esteja em execução e configurado corretamente.

## Como Rodar o Projeto

### 1. Clonar o Repositório

Clone este repositório do GitHub para o seu ambiente local:

```bash
git clone https://github.com/seu-usuario/ProductAPI.git
cd ProductAPI
```

### 2. Restaurar Dependências

Restaure os pacotes NuGet do projeto:

```bash
dotnet restore
```

### 3. Aplicar Migrações

Crie as migrações para configurar o banco de dados:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Rodar o Projeto

Inicie a aplicação localmente:

```bash
dotnet run
```

A aplicação será iniciada em `http://localhost:5000`. Você pode acessar a documentação Swagger em:

```bash
http://localhost:5000/swagger
```

### 5. Testar a API

Use o **Swagger** ou ferramentas como **Postman** para testar os endpoints da API.

## Endpoints Disponíveis

| Método | Endpoint           | Descrição                      |
| ------ | ------------------ | ------------------------------ |
| GET    | /api/products      | Listar todos os produtos       |
| POST   | /api/products      | Criar um novo produto          |
| PUT    | /api/products/{id} | Atualizar um produto existente |
| DELETE | /api/products/{id} | Deletar um produto             |

## Como Contribuir

1. Fork este repositório.
2. Crie uma nova branch (`git checkout -b feature/nova-feature`).
3. Faça as suas alterações e commit (`git commit -am 'Adiciona nova feature'`).
4. Envie a branch (`git push origin feature/nova-feature`).
5. Crie um novo Pull Request.

## Licença

Este projeto está licenciado sob os termos da licença MIT. Consulte o arquivo `LICENSE` para mais informações.

---

### Observações Finais

Esse `README.md` cobre todos os requisitos e é totalmente adaptado para rodar a aplicação facilmente. Inclua também o link do repositório do GitHub e certifique-se de que o arquivo `README.md` esteja no diretório raiz do projeto.
