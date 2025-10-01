## 📋 Sobre o Projeto

Aplicação web que permite:
- ✅ Gerar usuários aleatórios via API Random User Generator
- ✅ Criar, listar, editar e excluir usuários (CRUD completo)
- ✅ Busca e paginação de usuários
- ✅ Seleção múltipla para exclusão em lote
- ✅ Interface moderna com notificações toast
- ✅ Persistência em PostgreSQL

## 🛠️ Tecnologias Utilizadas

### Backend
- **C# / .NET 9.0** (compatível também com .NET 8.0, basta ajustar no .csproj)
- **ASP.NET Core 9.0** - Framework  web
- **Entity Framework Core 8.0** - ORM
- **PostgreSQL** - Banco de dados
- **AutoMapper** - Mapeamento de objetos
- **Npgsql** - Provider PostgreSQL para .NET

### Frontend
- **HTML5 / CSS3 / JavaScript**
- **Bootstrap 5** - Framework CSS
- **Font Awesome 6** - Ícones

## 📦 Pré-requisitos

Antes de começar, certifique-se de ter instalado:

### 1. .NET 9.0 SDK
> 💡 Caso prefira usar .NET 8.0, altere o arquivo `.csproj`:
> ```xml
> <TargetFramework>net8.0</TargetFramework>
> ```
> Os pacotes são compatíveis.

```bash
# Verificar instalação
dotnet --version
```
**Download:** https://dotnet.microsoft.com/download/dotnet/9.0

### 2. PostgreSQL 12+
```bash
# Verificar instalação
psql --version
```
**Download:** https://www.postgresql.org/download/

### 3. Git
```bash
# Verificar instalação
git --version
```
**Download:** https://git-scm.com/downloads

### 4. Editor de Código (Opcional)
- **Visual Studio 2022** (Community, Professional ou Enterprise)
- **Visual Studio Code** com extensão C#
- **JetBrains Rider**

## 🚀 Como Executar o Projeto

### Passo 1: Clonar o Repositório

```bash
git clone git@github.com:samuel-sabinodasilva1303/randomuserproject.git
cd randomuserproject
```

### Passo 2: Configurar o Banco de Dados PostgreSQL

#### Opção A: Usar banco existente

1. Abra o pgAdmin ou psql
2. Crie um banco de dados:

```sql
CREATE DATABASE randomuserdb;
```

#### Opção B: Usar instalação padrão

Se você instalou o PostgreSQL com as configurações padrão, pode usar o banco `postgres` existente.

### Passo 3: Configurar a Connection String

1. Copie o arquivo de exemplo:

```bash
# Windows (PowerShell)
copy appsettings.json.example appsettings.json

# Linux/Mac
cp appsettings.json.example appsettings.json
```

2. Edite `appsettings.json` com suas credenciais do PostgreSQL:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=randomuserdb;Username=postgres;Password=sua_senha;Pooling=true;"
  }
}
```

**Parâmetros:**
- `Host`: Endereço do servidor PostgreSQL (padrão: localhost)
- `Port`: Porta do PostgreSQL (padrão: 5432)
- `Database`: Nome do banco de dados
- `Username`: Usuário do PostgreSQL (padrão: postgres)
- `Password`: Senha do usuário PostgreSQL

### Passo 4: Restaurar Dependências

```bash
dotnet restore
```

### Passo 5: Executar o Projeto

```bash
dotnet run
```

### Passo 6: Acessar a Aplicação

Abra seu navegador em:
- **URL:** http://localhost:5000

## 📖 Como Usar

### Adicionar Usuários

1. Clique no botão **"Adicionar Usuários"**
2. Digite a quantidade desejada (1 a 100)
3. Aguarde a geração

Os usuários são gerados pela API Random User Generator em lotes de 20, evitando travar as requisiçoes

### Buscar Usuários

Digite no campo de busca para filtrar por:
- Nome
- Email
- Cidade
- País


### Editar Usuário

1. Clique no ícone de **edição** (✏️) na linha do usuário
2. Modifique os campos desejados
3. Clique em **"Salvar"**

### Excluir Usuários

#### Exclusão Individual
Clique no ícone de **exclusão** (🗑️) na linha do usuário.

#### Exclusão em Lote
1. Marque os checkboxes dos usuários desejados
2. Clique em **"Excluir Selecionados"** na barra de ações
3. Confirme a exclusão

Limite: 100 usuários por operação

### Paginação

- Use os botões de navegação no rodapé da tabela
- Ajuste o número de itens por página (5, 10, 25, 50)

## 🏗️ Estrutura do Projeto

```
RandomUserProject/
├── Controllers/
│   └── UsersController.cs          # Endpoints da API
├── Data/
│   └── ApplicationDbContext.cs     # Contexto EF Core
├── DTOs/
│   ├── UserDto.cs                  # DTO de resposta
│   ├── CreateUserDto.cs            # DTO de criação
│   ├── UpdateUserDto.cs            # DTO de atualização
│   └── PagedResultDto.cs           # DTO de paginação
├── Mappings/
│   └── MappingProfile.cs           # Perfis AutoMapper
├── Middleware/
│   └── GlobalExceptionHandler.cs   # Tratamento de erros
├── Models/
│   └── User.cs                     # Entidade User
├── Repositories/
│   ├── IUserRepository.cs          # Interface do repositório
│   └── UserRepository.cs           # Implementação do repositório
├── Services/
│   ├── IRandomUserService.cs       # Interface do serviço
│   └── RandomUserService.cs        # Serviço da API externa
├── wwwroot/
│   ├── index.html                  # Frontend
│   └── app.js                      # JavaScript da aplicação
├── Program.cs                      # Entry point
├── appsettings.json.example        # Exemplo de configuração
├── appsettings.json                # Configuração (não versionado)
└── README.md                       # Este arquivo
```

## 🔌 Endpoints da API

### Usuários

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/users` | Lista usuários (paginado) |
| GET | `/api/users/{id}` | Busca usuário por ID |
| POST | `/api/users` | Cria novo usuário |
| PUT | `/api/users/{id}` | Atualiza usuário |
| DELETE | `/api/users/{id}` | Exclui usuário |
| POST | `/api/users/add?count=N` | Adiciona N usuários (1-100) |
| POST | `/api/users/delete-multiple` | Exclui múltiplos usuários |
| GET | `/api/users/stats` | Retorna estatísticas |

### Exemplos de Requisições

#### Listar Usuários (Paginado)
```bash
GET /api/users?page=1&pageSize=10&searchTerm=john

Response:
{
  "data": [...],
  "totalCount": 78,
  "page": 1,
  "pageSize": 10,
  "totalPages": 8,
  "hasPrevious": false,
  "hasNext": true
}
```

#### Adicionar Usuários
```bash
POST /api/users/add?count=50

Response:
{
  "success": true,
  "message": "50 usuários adicionados com sucesso!",
  "added": 50,
  "previousTotal": 78,
  "currentTotal": 128,
  "batches": 3
}
```

#### Excluir Múltiplos
```bash
POST /api/users/delete-multiple
Content-Type: application/json

[1, 2, 3, 4, 5]

Response:
{
  "success": true,
  "message": "5 usuários excluídos com sucesso",
  "deletedCount": 5,
  "notFoundCount": 0,
  "notFoundIds": []
}
```

## 🎨 Features Implementadas

### Backend
- ✅ CRUD completo de usuários
- ✅ Integração com Random User Generator API
- ✅ Paginação server-side
- ✅ Busca por múltiplos campos
- ✅ Validação de emails duplicados
- ✅ Exclusão em lote (até 100 registros)
- ✅ Global Exception Handler
- ✅ Repository Pattern
- ✅ DTOs para separação de responsabilidades
- ✅ AutoMapper para mapeamento de objetos
- ✅ Tratamento de DateTime UTC para PostgreSQL
- ✅ Documentação Swagger/OpenAPI

### Frontend
- ✅ Interface moderna com gradientes
- ✅ Sistema de notificações toast
- ✅ Modal de confirmação para exclusões
- ✅ Progress bar para operações longas
- ✅ Seleção múltipla de usuários
- ✅ Busca com debounce
- ✅ Paginação dinâmica
- ✅ Responsivo (Bootstrap 5)
- ✅ Animações CSS
- ✅ Loading overlay
- ✅ Cache busting para JavaScript

## 🐛 Troubleshooting

### Erro: "Cannot write DateTime with Kind=Local to PostgreSQL"

**Solução:** Já implementada no código. O `Program.cs` contém:
```csharp
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
```

### Erro: "Connection refused" ao conectar no PostgreSQL

**Verificações:**
1. PostgreSQL está rodando?
   ```bash
   # Windows
   Get-Service postgresql*

   # Linux
   sudo systemctl status postgresql
   ```

2. Credenciais corretas no `appsettings.json`?

3. Porta 5432 está aberta?
   ```bash
   netstat -an | findstr 5432
   ```

### Erro: "Failed to load users" no frontend

**Verificações:**
1. Abra o Console do navegador (F12)
2. Verifique os logs coloridos (🔵 e ❌)
3. Limpe o cache do navegador (Ctrl + Shift + Delete)
4. Faça hard refresh (Ctrl + Shift + R)

### Aplicação não cria as tabelas automaticamente

**Solução:**
```bash
# Deletar e recriar o banco
dotnet ef database drop --force
dotnet run
```

## 📚 Documentação da API Externa

Este projeto consome a **Random User Generator API**:
- **URL:** https://randomuser.me/
- **Documentação:** https://randomuser.me/documentation
- **Endpoint usado:** `https://randomuser.me/api/?results=20&nat=us,br,gb`

## 📊 Estrutura do Banco de Dados

### Tabela `Users`
| Coluna | Tipo | Descrição |
|--------|------|-----------|
| Id | INTEGER | Chave primária (auto-increment) |
| FirstName | VARCHAR(100) | Nome (obrigatório) |
| LastName | VARCHAR(100) | Sobrenome (obrigatório) |
| Email | VARCHAR(200) | Email (obrigatório, único) |
| Phone | VARCHAR(20) | Telefone |
| Street | VARCHAR(200) | Endereço |
| City | VARCHAR(100) | Cidade |
| State | VARCHAR(100) | Estado |
| PostalCode | VARCHAR(20) | CEP |
| Country | VARCHAR(100) | País |
| DateOfBirth | TIMESTAMP | Data de nascimento |
| Gender | VARCHAR(10) | Gênero |
| PictureUrl | VARCHAR(500) | URL da foto |
| CreatedAt | TIMESTAMP | Data de criação |

**Nota:** As tabelas são criadas automaticamente pelo Entity Framework Core na primeira execução através do método `EnsureCreated()` no `Program.cs`.

## 👤 Autor

**Samuel Silva**