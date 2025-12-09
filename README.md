# 🔧 Orçamentos rápidos, computadorizados e automatizados (ORCA)

## 📌 Descrição do Projeto

O ORCA é um sistema desktop desenvolvido como Trabalho de Conclusão de Curso (TCC) do Técnico em Desenvolvimento de Sistemas.
Seu objetivo é automatizar orçamentos manuais, reduzindo erros, aumentando a produtividade e garantindo padronização no atendimento de pequenas e médias empresas.

Ele permite criar modelos personalizados de orçamento, inserir valores e gerar PDFs profissionais com rapidez e consistência.

O projeto atual (versão base) foi desenvolvido em C# + WPF + MySQL, funcionando totalmente em ambiente local.

---
## 🎯 Objetivo do Sistema

- Substituir orçamentos feitos manualmente.

- Automatizar cálculos e fórmulas.

- Padronizar a geração de PDFs.

- Facilitar o trabalho de micro e pequenas empresas.

- Reduzir erros humanos.
---

## ⚙️ Funcionalidades Disponíveis (Versão Base – TCC)

#### Login com usuários cadastrados no banco de dados (ADM e USR).

- ## Criação de modelos de orçamento:

- Nome do modelo

- Quantidade de colunas

- Tipos de dados

- Fórmulas personalizadas

- Edição e exclusão de modelos criados.

- Criação de orçamentos utilizando os modelos existentes.

- Execução automática de fórmulas ao pressionar ENTER.

- Armazenamento de orçamentos no banco de dados.

- ## Exportação para PDF com:

- Cabeçalho personalizado

- Rodapé personalizado

- Tabela formatada

- Texto explicativo e valores totais

---

## 👤 Público-Alvo

Micro e pequenas empresas que dependem de **orçamentos frequentes** ou negócios que buscam profissionalizar seus atendimentos.

Empreendedores e gestores que buscam **economia de tempo e redução de erros manuais.**

---

## 🛠️ Tecnologias Utilizadas

- **Linguagem:** C# (.NET 8)
- **Interface:** WPF (Windows Presentation Foundation)
- **Banco de Dados:** MySQL (via XAMPP / phpMyAdmin)
- **Biblioteca para PDF:** PDFSharp
- **IDE Sugerida:** Visual Studio 2022

---

### 📂 Estrutura do Projeto

```
bin/                 # Arquivos compilados
obj/                 # Arquivos temporários
packages/            # Dependências do projeto
MainWindow.xaml      # Interface principal
MainWindow.xaml.cs   # Lógica do sistema
# Demais telas.
```
### 💡 Diferenciais

- Automatização completa do processo de orçamentos.
- Armazenamento seguro e confiável no banco de dados MySQL.

---

## 📦 Instalação e Configuração

## 🔧 1. Pré-requisitos

Instale:

- ✔️ XAMPP (para rodar o MySQL)  - https://www.apachefriends.org/pt_br/download.html

- ✔️ (Opcional) Visual Studio 2022 caso queira compilar

- ✔️ .NET 8 Runtime (se usar a versão compacta do ORCA) https://dotnet.microsoft.com/en-us/download/dotnet/8.0/runtime

##### Download do ORCA - Para utilização seguindo o modelo:

Na página do repositório você encontrará o projeto, basta fazer o download e receberá o conteudo zipado.

```
🔹 ORCA.zip # Exemplo
```

---

## 🗄️ 2. Instalação do Banco de Dados

##### 2.1 — Iniciar o MySQL

- Abra o XAMPP Control Panel

- Clique em Start em MySQL

#####2.2 — Abrir o phpMyAdmin

Acesse no navegador:

```
http://localhost/phpmyadmin
```

---

##### 2.3 — Criar o banco

1 - Clique em Novo

2 - Nomeie exatamente como:
```
banco # Caso queira alterar o nome, você deve alterar a conexão com o banco no arquivo inicial do projeto usando um compilador - MainWindow.xaml.cs
```

3 - Clique em Criar

---

##### 2.4 — Importar o banco

- Clique na database banco

- Vá em **Importar**

- Selecione o arquivo .sql enviado com o projeto

- Clique em Executar

---

## 👥 3. Criar Usuários no Banco

Execute no phpMyAdmin → Aba SQL:

Usuário ADM:
```
INSERT INTO usuario (id, nome_completo, email, telefone_celular, senha, permissao, cpf, rg)
VALUES (NULL, 'Administrador do Sistema', 'adm@gmail.com', '19900000000', 'adm123', 'adm', '00000000000', '0000000');
```

Usuário Comum:
```
INSERT INTO usuario (id, nome_completo, email, telefone_celular, senha, permissao, cpf, rg)
VALUES (NULL, 'Usuário Padrão', 'usuario@gmail.com', '19900000001', 'usr123', 'usr', '00000000000', '0000001');
```

---
## ▶️ 4. Como Rodar o ORCA

#### 💠 Método 1 — Versão Executável (Recomendado)

Acesse a pasta extraida: 

```
ORCA\ORCA\bin\Debug\net8.0-windows 
```

Execute: **ORCA.exe**

---

#### 💠 Método 2 — Rodar pelo Visual Studio

Abra o Visual Studio

Clique em Abrir Projeto/Solução

Selecione a pasta do ORCA

Pressione F5

---

## 🔐 5. Login

Use os usuários que você criou:

Administrador:
```
E-mail: adm@gmail.com
Senha: adm123
```

Usuário:
```
E-mail: usuario@gmail.com
Senha: usr123
```

---

## 📝 6. Como Utilizar o Sistema
- #### 🔸 Perfil Administrador:

- Criar modelos de orçamento

- Editar e excluir modelos

- Criar usuários

- Configurar fórmulas

- Acessar listas e cadastros

- #### Criando um Modelo

1 - Vá em Modelos

2 - Clique em Criar Modelo

3 - Configure:

- Nome

- Colunas

- Tipos

- Fórmulas

4 - Pressione ENTER após cada célula digitada

---

#### 🔸 Perfil Usuário

- Criar orçamentos baseados nos modelos

- Preencher valores

- Aplicar fórmulas automaticamente

- Gerar PDF profissional

- Salvar orçamento

- #### Gerar PDF

- Clique em Gerar PDF

- Adicione Cabeçalho e Rodapé

- Clique em Exportar

---

## ⚠️ 7. Informações Importantes

- O **ENTER** salva e executa fórmulas.

- Sem ENTER = nada funciona.

- Cabeçalho e rodapé são obrigatórios para gerar PDF.

- O banco deve estar iniciado no XAMPP.

---

## 📚 Créditos Acadêmicos

Projeto desenvolvido como **TCC** do curso **Técnico em Desenvolvimento de Sistemas**, orientado pelos professores **Maércio Girardi Bisco** e **Pedro Ramires da Silva Amalfi Costa**.
