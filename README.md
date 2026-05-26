## Objetivo

Implementar uma API REST em C# utilizando o ecossistema .NET Core para gerenciamento de uma tabela de usuários, seguindo as especificações a seguir.

---

## 1.1. Tabela de usuário

| Campo | Formato | Descrição | Observações |
|---|---|---|---|
| Id | integer | Identificador único do registro | Campo gerenciado pelo banco de dados |
| Nome | varchar(50) | Nome do usuário |  |
| E-mail | varchar(254) | E-mail do usuário | Não é permitido existir mais de 1 registro no banco com mesmo endereço de e-mail |
| Data de nascimento | date | Data de nascimento do usuário | Não é permitido cadastrar um usuário com data de nascimento no futuro |
| Data de criação | date | Data de criação do registro | Campo gerenciado pela API |
| Data de edição | date | Data de edição do registro | Campo gerenciado pela API |

---

## 1.2. Operações

### 1.2.1. POST `/users`

Recurso para criação de um usuário.

### 1.2.2. GET `/users/{id}`

Recurso para ler dados de um usuário de acordo com o ID informado.

### 1.2.3. PUT `/users/{id}`

Recurso para edição de dados de um usuário de acordo com o ID informado.

### 1.2.4. DELETE `/users/{id}`

Recurso para remover o registro de um usuário de acordo com o ID informado.

### 1.2.5. GET `/users`

Recurso para ler todos usuários.

**Bônus:** Suporte a consulta paginada.

---

## 1.3. Informações adicionais

- As requisições devem ter seus parâmetros devidamente validados.
- Criar testes unitários para validação dos serviços implementados.
- Pode ser utilizado Entity Framework Core ou outro ORM de sua preferência.
- Adicionar suporte a Swagger para documentação da API.
- Não é obrigatório provisionar o banco de dados via Docker, mas será considerado um diferencial.
