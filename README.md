# SmartSell API - Sistema de Transporte Universitário Galdino

Sistema de gerenciamento de transporte universitário desenvolvido em .NET Core e MySQL.

## 🚀 Tecnologias

- **.NET 8.0** - Framework principal
- **MySQL** - Banco de dados
- **Swagger** - Documentação da API
- **BCrypt** - Criptografia de senhas

## 📋 Funcionalidades

### Módulos Principais
- **Gestão de Usuários** - Administradores, motoristas e alunos
- **Gestão de Frota** - Cadastro e controle de ônibus
- **Gestão de Rotas** - Planejamento e acompanhamento de rotas
- **Sistema de Pagamentos** - Controle financeiro e mensalidades
- **Pontos de Embarque** - Gerenciamento de locais de embarque
- **Presença** - Controle de presença dos alunos
- **Emergências** - Sistema de notificação de emergências
- **Notificações** - Sistema de comunicação
- **Instituições** - Cadastro de escolas/universidades

## 🛠️ COMO EXECUTAR O SISTEMA

### Pré-requisitos
- **.NET 8.0 SDK** - [Download aqui](https://dotnet.microsoft.com/download/dotnet/8.0)
- **MySQL Server 8.0+** - [Download aqui](https://dev.mysql.com/downloads/mysql/)
- **Visual Studio 2022** ou **VS Code** com extensão C#

### 📥 Instalação Rápida

1. **Clone o repositório**
```bash
git clone [URL_DO_REPOSITORIO]
cd SmartSell.Api
```

2. **Restaure as dependências**
```bash
dotnet restore
```

3. **Configure o Banco de Dados MySQL**

**Opção A - Usando as configurações padrão (recomendado para teste):**
- Use o MySQL com o seu usuário `x` e senha `x`
- Crie um banco chamado `galdinotransporte`
- As configurações já estão no `appsettings.json`

**Opção B - Personalizando as configurações:**
Edite o arquivo `appsettings.json` e altere a connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR;Database=SEU_BANCO;Uid=SEU_USUARIO;Pwd=SUA_SENHA;"
  }
}
```

4. **Execute o projeto**
```bash
dotnet run
```

5. **Acesse a aplicação**
- **API:** `https://localhost:5064`
- **Swagger (Documentação):** `https://localhost:5064/swagger`

## 🗄️ Configuração do Banco de Dados

### Configurações Atuais (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=galdinotransporte;Uid=root;Pwd=root;" // Minhas config do db que estou usando
  },
  "Jwt": {
    "Key": "sua-chave-jwt-super-secreta-aqui",
    "Issuer": "GaldinoApi",
    "Audience": "GaldinoClient"
  }
}
```

### Script SQL para Criação do Banco
```sql
CREATE DATABASE galdinotransporte;
USE galdinotransporte;

-- As tabelas serão criadas automaticamente pelo Entity Framework
-- na primeira execução da aplicação
```

## 📚 DOCUMENTAÇÃO DA API

### 🔗 Endpoints Principais

#### **Gestão de Usuários**
- `GET /api/students` - Listar alunos
- `POST /api/students` - Criar aluno
- `PUT /api/students/{id}` - Atualizar aluno
- `DELETE /api/students/{id}` - Excluir aluno

#### **Gestão de Motoristas**
- `GET /api/drivers` - Listar motoristas
- `POST /api/drivers` - Criar motorista
- `PUT /api/drivers/{id}` - Atualizar motorista
- `DELETE /api/drivers/{id}` - Excluir motorista

#### **Gestão de Veículos**
- `GET /api/onibus` - Listar ônibus
- `GET /api/onibus/{id}` - Buscar ônibus por ID
- `GET /api/onibus/placa/{placa}` - Buscar por placa
- `POST /api/onibus` - Criar ônibus
- `PUT /api/onibus/{id}` - Atualizar ônibus
- `PATCH /api/onibus/{id}/status` - Atualizar status
- `DELETE /api/onibus/{id}` - Excluir ônibus

#### **Gestão de Rotas**
- `GET /api/routes` - Listar rotas
- `GET /api/routes/{id}` - Buscar rota por ID
- `POST /api/routes` - Criar rota
- `PUT /api/routes/{id}` - Atualizar rota
- `DELETE /api/routes/{id}` - Excluir rota

#### **Gestão de Instituições**
- `GET /api/instituicoes` - Listar instituições
- `GET /api/instituicoes/{id}` - Buscar instituição por ID
- `POST /api/instituicoes` - Criar instituição
- `PUT /api/instituicoes/{id}` - Atualizar instituição
- `DELETE /api/instituicoes/{id}` - Excluir instituição

#### **Sistema de Pagamentos**
- `GET /api/payments` - Listar pagamentos
- `POST /api/payments` - Criar pagamento
- `PUT /api/payments/{id}` - Atualizar pagamento

#### **Controle de Presença**
- `GET /api/attendance` - Listar presenças
- `POST /api/attendance` - Registrar presença
- `PUT /api/attendance/{id}` - Atualizar presença

#### **Pontos de Embarque**
- `GET /api/boarding-points` - Listar pontos
- `POST /api/boarding-points` - Criar ponto
- `PUT /api/boarding-points/{id}` - Atualizar ponto

#### **Emergências**
- `GET /api/emergencies` - Listar emergências
- `POST /api/emergencies` - Criar emergência
- `PUT /api/emergencies/{id}` - Atualizar emergência

#### **Notificações**
- `GET /api/notifications` - Listar notificações
- `POST /api/notifications` - Criar notificação
- `PUT /api/notifications/{id}` - Marcar como lida

## 🧪 TESTANDO A API

### Usando Swagger (Recomendado)
1. Execute a aplicação (`dotnet run`)
2. Acesse `https://localhost:5064/swagger`
3. Teste os endpoints diretamente na interface

### Usando cURL
```bash
# Listar todos os ônibus
curl -X GET "https://localhost:5064/api/onibus" -H "accept: application/json"

# Criar um novo ônibus
curl -X POST "https://localhost:5064/api/onibus" \
  -H "accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
    "placa": "ABC-1234",
    "modelo": "Mercedes-Benz",
    "capacidade": 45,
    "ano": 2020,
    "status": "Ativo"
  }'
```

## 🗄️ Estrutura do Banco de Dados

### Tabelas Principais
- **`usuario`** - Dados básicos dos usuários (nome, email, senha)
- **`aluno`** - Informações específicas dos alunos
- **`motorista`** - Dados dos motoristas (CNH, CPF, etc.)
- **`onibus`** - Cadastro da frota (placa, modelo, capacidade)
- **`rota`** - Rotas de transporte (data, horário, destino)
- **`instituicao`** - Escolas/universidades
- **`pagamento`** - Controle financeiro
- **`presenca`** - Registro de presença dos alunos
- **`emergencia`** - Ocorrências e emergências
- **`notificacao`** - Sistema de comunicação
- **`ponto_embarque`** - Locais de embarque

### Relacionamentos
- Aluno → Usuário (1:1)
- Motorista → Usuário (1:1)
- Rota → Motorista (N:1)
- Rota → Ônibus (N:1)
- Rota → Instituição (N:1)
- Pagamento → Aluno (N:1)
- Presença → Aluno (N:1)

## 🔧 Configurações Técnicas

### CORS
O sistema aceita requisições de:
- `http://localhost:3000` (React)
- `http://localhost:3001` (Next.js)
- `http://localhost:5173` (Vite)
- `http://localhost:8080` (Vue.js)

### JWT
- **Issuer:** GaldinoApi
- **Audience:** GaldinoClient
- **Chave:** Configurada no appsettings.json

### Logging
- **Nível padrão:** Information
- **ASP.NET Core:** Warning

## ⚠️ NOTAS IMPORTANTES

1. **Banco de Dados:** O sistema usa Entity Framework Code First. As tabelas são criadas automaticamente na primeira execução.

2. **Configurações:** As configurações estão expostas no `appsettings.json` para facilitar a avaliação acadêmica.

3. **Segurança:** Em produção, as configurações sensíveis devem ser movidas para variáveis de ambiente.

4. **Testes:** Use o Swagger para testar todos os endpoints de forma interativa.

## 🚨 Solução de Problemas

### Erro de Conexão com Banco
- Verifique se o MySQL está rodando
- Confirme usuário/senha no appsettings.json
- Teste a conexão: `mysql -u root -p`

### Porta em Uso
- A API roda na porta 5064 (HTTPS) e 5063 (HTTP)
- Se estiver em uso, altere em `Properties/launchSettings.json`

### Dependências
- Execute `dotnet restore` se houver erros de pacotes
- Verifique se o .NET 8.0 SDK está instalado: `dotnet --version`

## 📞 Suporte Acadêmico

Este projeto foi desenvolvido para fins acadêmicos. Para dúvidas sobre implementação ou funcionalidades, consulte:

1. **Documentação Swagger:** `https://localhost:5064/swagger`
2. **Logs da aplicação:** Console do Visual Studio
3. **Estrutura do código:** Organizada em Controllers, Models, DAO e Services

---

**Desenvolvido para o curso de Programação Orientada a Objetos do professor Elias de Abreus, INSTITUTO FEDERAL DE RONDÔNIA** 🎓

**Sistema de Transporte Universitário Galdino** 🚌
