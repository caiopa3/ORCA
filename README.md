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

- ✔️ XAMPP (para rodar o MySQL) [Download XAMPP)](https://www.apachefriends.org/pt_br/download.html) https://www.apachefriends.org/pt_br/download.html

- ✔️ Projeto ORCA (.zip) – baixado nas Releases

- ✔️ (Opcional) Visual Studio 2022 caso queira compilar

- ✔️ .NET 8 Runtime (se usar a versão compacta do ORCA) https://dotnet.microsoft.com/en-us/download/dotnet/8.0/runtime

##### Download do ORCA - Para utilização seguindo o modelo:

Na página do repositório você encontrará o projeto, basta baixado e recebera todo o conteudo zipado.
```
🔹 ORCA.zip # Exemplo
```
**Requer** que o usuário tenha o .NET 8 Runtime instalado no Windows.
**Baixar o runtime oficial (caso não tenha):**
👉[Download .NET 8 Runtime (Microsoft)]([https://nodejs.org/](https://dotnet.microsoft.com/en-us/download/dotnet/8.0/runtime))

## 🚀 Como usar:

```
- Baixe o arquivo .zip desejado.
- Extraia o conteúdo em uma pasta no seu PC.
- Execute o arquivo ORCA.exe para iniciar o aplicativo.
- (Opcional) Crie um atalho na área de trabalho para acesso rápido.
```

---

## 📚 Créditos Acadêmicos

Projeto desenvolvido como **TCC** do curso **Técnico em Desenvolvimento de Sistemas**, orientado pelos professores **Maércio Girardi Bisco** e **Pedro Ramires da Silva Amalfi Costa**.
