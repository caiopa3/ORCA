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

Pequenas e médias empresas que precisam **automatizar** a emissão de **orçamentos**.

Empreendedores e gestores que buscam **economia de tempo e redução de erros manuais.**

---

## 🛠️ Tecnologias Utilizadas

- **Linguagem:** C#
- **Interface:** WPF (Windows Presentation Foundation)
- **Banco de Dados:** MySQL

---

## ⚙️ Funcionalidades

- Cadastro de produtos, serviços e clientes.
- Criação automática de orçamentos.
- Atualização e exclusão de orçamentos.
- Interface amigável em WPF para desktop.

---

### 📂 Estrutura do Projeto

```
bin/                 # Arquivos compilados
obj/                 # Arquivos temporários
packages/            # Dependências do projeto
MainWindow.xaml      # Interface principal
MainWindow.xaml.cs   # Lógica do sistema
```
### 💡 Diferenciais

- Automatização completa do processo de orçamentos.
- Armazenamento seguro e confiável no banco de dados MySQL.

---

## 📦 Downloads do ORCA - Para utilização seguindo o modelo:

Na página de Releases você encontrará dois pacotes do ORCA:
```
🔹 ORCA-win-x64-versão.zip
```
Menor tamanho.
**Requer** que o usuário tenha o .NET 8 Runtime instalado no Windows.
Ideal para quem já possui outras aplicações .NET 8.
**Baixar o runtime oficial (caso não tenha):**
👉[Download .NET 8 Runtime (Microsoft)]([https://nodejs.org/](https://dotnet.microsoft.com/en-us/download/dotnet/8.0/runtime))

```
🔹 ORCA-win-x64-selfcontained-versão.zip
```
Maior tamanho (inclui o runtime).
**Não precisa** instalar nada extra.
Recomendado para a maioria dos usuários que só querem baixar e usar.

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
