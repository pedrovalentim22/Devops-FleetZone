# 🛵 FleetZone API – Advanced Business Development with .NET

API RESTful desenvolvida em **.NET 8 (Web API)** como parte do **Challenge 2025** (FIAP – 2º Ano ADS).  
Projeto alinhado com as boas práticas REST e com os requisitos da disciplina **Advanced Business Development with .NET**.

---

## 👨‍💻 Integrantes
- Pedro Valentim Merise – RM 556826
- Miguel Barros Ramos – RM 556652
- Thomas Rodrigues – RM 558042

---

## 📚 Domínio e Arquitetura

- **Entidades principais (mínimo 3):**
  - **Pátio** → representa os locais físicos onde as motos ficam.
  - **Motocicleta** → ativo principal gerenciado.
  - **Movimentação** → histórico de entrada, saída e realocação de motos.

**Justificativa do domínio:** Essas entidades traduzem diretamente o cenário da Mottu:  
controlar a infraestrutura (pátios), gerenciar os ativos (motocicletas) e registrar os eventos operacionais (movimentações).

**Arquitetura utilizada:** Clean Architecture  
- **Domain** → entidades, contratos e regras de negócio.  
- **Application** → DTOs, validações e casos de uso.  
- **Infrastructure** → persistência com EF Core (Oracle/SQLite).  
- **WebApi** → controllers REST, HATEOAS, versionamento, Swagger.

**Justificativa da arquitetura:** Optamos pela Clean Architecture para garantir separação rigorosa de responsabilidades, facilitando a evolução do domínio sem acoplamento às camadas externas. O desenho usa limites bem definidos, o que permite:
- **Testabilidade** — os casos de uso podem ser validados em isolamento, sem precisar subir banco ou web server.
- **Manutenibilidade** — mudanças em infraestrutura (ex.: troca de banco Oracle ↔ SQLite) não impactam regras de negócio.
- **Escalabilidade da equipe** — times distintos podem atuar em camadas diferentes, reduzindo conflitos e acelerando entregas.
- **Extensibilidade** — a entrada de novos canais (ex.: gRPC, filas) exige apenas novas implementações de interface, preservando o core.

---

## 🚀 Tecnologias Utilizadas
- ASP.NET Core 8 (Web API)
- Entity Framework Core (com suporte a Oracle)
- EF Core Migrations
- Swagger / OpenAPI (Swashbuckle.AspNetCore + Filters)
- xUnit para testes automatizados

---

## 🔧 Variáveis de Ambiente

| Variável | Obrigatória? | Descrição | Exemplo (PowerShell) |
| --- | --- | --- | --- |
| `ConnectionStrings__Oracle` | ✅ | String de conexão utilizada pelo EF Core. | `$env:ConnectionStrings__Oracle = "Data Source=oracle.fiap.com.br:1521/orcl;User ID=RM556652;Password=123456;"` |
| `ASPNETCORE_ENVIRONMENT` | ⚙️ | Ambiente de execução (`Development`, `Staging`, `Production`). | `$env:ASPNETCORE_ENVIRONMENT = "Development"` |
| `ASPNETCORE_HTTPS_PORT` | ⚙️ | Porta HTTPS exposta (default: `7208`). | `$env:ASPNETCORE_HTTPS_PORT = "7208"` |
| `ASPNETCORE_URLS` | Opcional | Sobrescreve as URLs de bind do Kestrel. Use se quiser rodar em outra porta/IP. | `$env:ASPNETCORE_URLS = "http://localhost:5049;https://localhost:7208"` |

> 💡 As variáveis podem ser definidas diretamente no PowerShell (válidas apenas para a sessão atual) ou em um arquivo `appsettings.{Environment}.json`.

---

## ⚙️ Como Executar

### Pré-requisitos
- .NET 8 SDK instalado
- Banco Oracle ou SQLite configurado
- EF Core CLI:
  ```
  dotnet tool install --global dotnet-ef
  ```

### Passos
```
# 1. (Somente uma vez) Confiar no certificado HTTPS de desenvolvimento
dotnet dev-certs https --trust

# 2. Restaurar pacotes e aplicar migrations
dotnet restore
dotnet ef database update

# 3. Executar a aplicação (perfil HTTPS recomendado)
dotnet run --launch-profile https
```

A API sobe em:  
➡️ HTTP: `http://localhost:5049`  
➡️ HTTPS: `https://localhost:7208`  
➡️ Swagger UI: `https://localhost:7208/swagger`

---

## 🌐 Endpoints Principais

### Motocicletas
- `GET /api/v1/motocicletas?pageNumber=1&pageSize=10`
- `GET /api/v1/motocicletas/{id}`
- `POST /api/v1/motocicletas`
- `PUT /api/v1/motocicletas/{id}`
- `DELETE /api/v1/motocicletas/{id}`

### Pátios
- `GET /api/v1/patio?pageNumber=1&pageSize=10&nome=&endereco=`
- `GET /api/v1/patio/{id}`
- `POST /api/v1/patio`
- `PUT /api/v1/patio/{id}`
- `DELETE /api/v1/patio/{id}`

### Movimentações
- `GET /api/v1/movimentacoes?pageNumber=1&pageSize=10`
- `GET /api/v1/movimentacoes/{id}`
- `POST /api/v1/movimentacoes`
- `PUT /api/v1/movimentacoes/{id}`
- `DELETE /api/v1/movimentacoes/{id}`

---

## 📌 Exemplos de Uso (cURL)

> Caso ainda não tenha confiado no certificado de desenvolvimento, adicione a opção `-k` aos comandos abaixo.

### Listar Motocicletas (paginação + HATEOAS)

```powershell
curl "https://localhost:7208/api/v1/motocicletas?pageNumber=1&pageSize=2"
```

**Resposta (200 OK)**

```json
{
  "pageNumber": 1,
  "pageSize": 2,
  "totalCount": 42,
  "totalPages": 21,
  "links": [
    { "rel": "self", "href": "https://localhost:7208/api/v1/motocicletas?pageNumber=1&pageSize=2", "method": "GET" },
    { "rel": "first", "href": "https://localhost:7208/api/v1/motocicletas?pageNumber=1&pageSize=2", "method": "GET" },
    { "rel": "last", "href": "https://localhost:7208/api/v1/motocicletas?pageNumber=21&pageSize=2", "method": "GET" }
  ],
  "items": [
    {
      "id": 1,
      "placa": "ABC1D23",
      "modelo": "Honda CG 160",
      "status": "Disponivel",
      "patioId": 1,
      "links": [
        { "rel": "self", "href": "https://localhost:7208/api/v1/motocicletas/1", "method": "GET" },
        { "rel": "update", "href": "https://localhost:7208/api/v1/motocicletas/1", "method": "PUT" },
        { "rel": "delete", "href": "https://localhost:7208/api/v1/motocicletas/1", "method": "DELETE" }
      ]
    }
  ]
}
```

### Criar um Pátio

```powershell
curl -X POST "https://localhost:7208/api/v1/patio" -H "Content-Type: application/json" -d '{
  "nome": "Pátio Central",
  "endereco": "Av. das Nações, 1000 - SP",
  "capacidade": 120
}'
```

### Listar Pátios (paginado e filtrado)

```powershell
curl "https://localhost:7208/api/v1/patio?pageNumber=1&pageSize=5&nome=Central"
```

### Criar uma Movimentação

```powershell
curl -X POST "https://localhost:7208/api/v1/movimentacoes" -H "Content-Type: application/json" -d '{
  "tipo": "Entrada",
  "observacao": "Recebida do pátio Unidade 02",
  "motocicletaId": 1,
  "patioId": 1
}'
```

### Exemplo de erro 400 (ValidationProblemDetails)

```powershell
curl -X POST "https://localhost:7208/api/v1/motocicletas" -H "Content-Type: application/json" -d '{}'
```

**Resposta (400 Bad Request)**

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Placa": [ "The Placa field is required." ],
    "Modelo": [ "The Modelo field is required." ],
    "PatioId": [ "The field PatioId must be between 1 and 2147483647." ]
  }
}
```

---

## 📖 Swagger / OpenAPI

O Swagger está configurado com:
- Descrição de endpoints e parâmetros (via **XML Comments**)
- Exemplos de payloads (`SwaggerRequestExample`)
- Exemplos de respostas (`SwaggerResponseExample`)
- Modelos de dados (DTOs) visíveis na UI

➡️ Acesse `https://localhost:7208/swagger` após rodar a aplicação.

---

## 🧪 Testes Automatizados

Os testes estão implementados com **xUnit** e cobrem:
- Regras de negócio das entidades
- Validações básicas
- Testes de integração com `WebApplicationFactory`

Rodar os testes:
```
dotnet test
```
## Scripts para criação do Web App Service e SQL Server na Azure

//Criando o resource group
az group create \
  --name "rg-fleetzone" \
  --location "eastus2"

//Criando Servidor SQL
az sql server create \
  --name "server-fleetzone-sprint3" \
  --resource-group "rg-fleetzone" \
  --location "eastus2" \
  --admin-user "sqladmin" \
  --admin-password "AzureDb123"

//Criando o banco de dados Azure SQL
az sql db create \
  --resource-group "rg-fleetzone" \
  --server "server-fleetzone-sprint3" \
  --name "db-fleetzone" \
  --service-objective S0

//Permitindo o WebApp acessar o banco
az sql server firewall-rule create \
  --resource-group "rg-fleetzone" \
  --server "server-fleetzone-sprint3" \
  --name AllowAzureIps \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

//Criando o App Service Plan
az appservice plan create \
  --name "plan-fleetzone" \
  --resource-group "rg-fleetzone" \
  --sku F1

//Criando o Web App de fato com Dotnet
az webapp create \
  --resource-group "rg-fleetzone" \
  --plan "plan-fleetzone" \
  --name "app-fleetzone" \
  --runtime "dotnet:8"

//Conection String
CONNECTION_STRING="Server=tcp:server-fleetzone-sprint3.database.windows.net,1433;Initial Catalog=db-fleetzone;Persist Security Info=False;User ID=sqladmin;Password=AzureDb123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

//Inserindo a Conection String
az webapp config connection-string set \
  --resource-group "rg-fleetzone" \
  --name "app-fleetzone" \
  --settings DefaultConnection="$CONNECTION_STRING" \
  --connection-string-type SQLAzure

//Aplicando as configs
az webapp deployment source config \
    --name app-fleetzone \
    --resource-group rg-fleetzone \
    --repo-url https://$app-fleetzone@app-fleetzone.scm.azurewebsites.net/app-fleetzone.git \
    --branch main \
    --manual-integration

//Criando as tabelas
dotnet ef database update --context ApplicationDbContext


//Fazer o deploy de fato
git remote add azure https://$app-fleetzone@app-fleetzone.scm.azurewebsites.net/app-fleetzone.git

//Vendo o user e senha
az webapp deployment list-publishing-profiles \
  --resource-group "rg-fleetzone" \
  --name "app-fleetzone" \
  --output table

//Fazendo o Push
git push azure main


