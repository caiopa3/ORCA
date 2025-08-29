-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Tempo de geração: 29/08/2025 às 14:01
-- Versão do servidor: 10.4.32-MariaDB
-- Versão do PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Banco de dados: `banco`
--

-- --------------------------------------------------------

--
-- Estrutura para tabela `modelo_orcamento`
--

CREATE TABLE `modelo_orcamento` (
  `id` int(11) NOT NULL,
  `nome` varchar(255) NOT NULL,
  `usr_criador_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Despejando dados para a tabela `modelo_orcamento`
--

INSERT INTO `modelo_orcamento` (`id`, `nome`, `usr_criador_id`) VALUES
(13, 'Flores', 1);

-- --------------------------------------------------------

--
-- Estrutura para tabela `modelo_orcamento_dados`
--

CREATE TABLE `modelo_orcamento_dados` (
  `id` int(11) NOT NULL,
  `modelo_id` int(11) NOT NULL,
  `dados_json` longtext NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Despejando dados para a tabela `modelo_orcamento_dados`
--

INSERT INTO `modelo_orcamento_dados` (`id`, `modelo_id`, `dados_json`) VALUES
(13, 13, '{\"Colunas\":[\"Nome\",\"Quantidade\"],\"Linhas\":[{\"Nome\":\"Ortiga\",\"Quantidade\":\"3\"}]}');

-- --------------------------------------------------------

--
-- Estrutura para tabela `modelo_orcamento_usuarios`
--

CREATE TABLE `modelo_orcamento_usuarios` (
  `id` int(11) NOT NULL,
  `modelo_id` int(11) NOT NULL,
  `usuario_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Despejando dados para a tabela `modelo_orcamento_usuarios`
--

INSERT INTO `modelo_orcamento_usuarios` (`id`, `modelo_id`, `usuario_id`) VALUES
(7, 13, 1),
(8, 13, 2);

-- --------------------------------------------------------

--
-- Estrutura para tabela `orcamento`
--

CREATE TABLE `orcamento` (
  `id` int(11) NOT NULL,
  `nome` text DEFAULT NULL,
  `usr_email` text DEFAULT NULL,
  `modelo_id` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Despejando dados para a tabela `orcamento`
--

INSERT INTO `orcamento` (`id`, `nome`, `usr_email`, `modelo_id`) VALUES
(19, 'Flores', 'usr', 13);

-- --------------------------------------------------------

--
-- Estrutura para tabela `usuario`
--

CREATE TABLE `usuario` (
  `id` int(11) NOT NULL,
  `email` text DEFAULT NULL,
  `senha` varchar(255) DEFAULT NULL,
  `permissao` varchar(3) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Despejando dados para a tabela `usuario`
--

INSERT INTO `usuario` (`id`, `email`, `senha`, `permissao`) VALUES
(1, 'adm', '123', 'adm'),
(2, 'usr', '123', 'usr');

--
-- Índices para tabelas despejadas
--

--
-- Índices de tabela `modelo_orcamento`
--
ALTER TABLE `modelo_orcamento`
  ADD PRIMARY KEY (`id`),
  ADD KEY `usr_criador_id` (`usr_criador_id`);

--
-- Índices de tabela `modelo_orcamento_dados`
--
ALTER TABLE `modelo_orcamento_dados`
  ADD PRIMARY KEY (`id`),
  ADD KEY `modelo_id` (`modelo_id`);

--
-- Índices de tabela `modelo_orcamento_usuarios`
--
ALTER TABLE `modelo_orcamento_usuarios`
  ADD PRIMARY KEY (`id`),
  ADD KEY `modelo_id` (`modelo_id`),
  ADD KEY `usuario_id` (`usuario_id`);

--
-- Índices de tabela `orcamento`
--
ALTER TABLE `orcamento`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_orcamento_modelo` (`modelo_id`);

--
-- Índices de tabela `usuario`
--
ALTER TABLE `usuario`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT para tabelas despejadas
--

--
-- AUTO_INCREMENT de tabela `modelo_orcamento`
--
ALTER TABLE `modelo_orcamento`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- AUTO_INCREMENT de tabela `modelo_orcamento_dados`
--
ALTER TABLE `modelo_orcamento_dados`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- AUTO_INCREMENT de tabela `modelo_orcamento_usuarios`
--
ALTER TABLE `modelo_orcamento_usuarios`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT de tabela `orcamento`
--
ALTER TABLE `orcamento`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=20;

--
-- AUTO_INCREMENT de tabela `usuario`
--
ALTER TABLE `usuario`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- Restrições para tabelas despejadas
--

--
-- Restrições para tabelas `modelo_orcamento`
--
ALTER TABLE `modelo_orcamento`
  ADD CONSTRAINT `modelo_orcamento_ibfk_1` FOREIGN KEY (`usr_criador_id`) REFERENCES `usuario` (`id`) ON DELETE CASCADE;

--
-- Restrições para tabelas `modelo_orcamento_dados`
--
ALTER TABLE `modelo_orcamento_dados`
  ADD CONSTRAINT `modelo_orcamento_dados_ibfk_1` FOREIGN KEY (`modelo_id`) REFERENCES `modelo_orcamento` (`id`) ON DELETE CASCADE;

--
-- Restrições para tabelas `modelo_orcamento_usuarios`
--
ALTER TABLE `modelo_orcamento_usuarios`
  ADD CONSTRAINT `modelo_orcamento_usuarios_ibfk_1` FOREIGN KEY (`modelo_id`) REFERENCES `modelo_orcamento` (`id`) ON DELETE CASCADE,
  ADD CONSTRAINT `modelo_orcamento_usuarios_ibfk_2` FOREIGN KEY (`usuario_id`) REFERENCES `usuario` (`id`) ON DELETE CASCADE;

--
-- Restrições para tabelas `orcamento`
--
ALTER TABLE `orcamento`
  ADD CONSTRAINT `fk_orcamento_modelo` FOREIGN KEY (`modelo_id`) REFERENCES `modelo_orcamento` (`id`) ON DELETE SET NULL ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
