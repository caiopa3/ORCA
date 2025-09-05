-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Tempo de geração: 05/09/2025 às 19:32
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
(11, 'Flores', 1),
(12, 'Carro', 13);

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
(11, 11, '{\"Colunas\":[\"Nome\",\"Valor\"],\"Linhas\":[{\"Nome\":\"\",\"Valor\":\"\"}]}'),
(12, 12, '{\"Colunas\":[\"Nome\",\"Valor\"],\"Linhas\":[{\"Nome\":\"\",\"Valor\":\"\"}]}');

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
(6, 11, 2),
(7, 12, 12),
(8, 12, 2);

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
(21, 'Flores', 'yslan_usr@gmail.com', 11),
(22, 'Carro 1', 'caio_usr@gmail.com', 12),
(23, 'Carro 1', 'yslan_usr@gmail.com', 12);

-- --------------------------------------------------------

--
-- Estrutura para tabela `orcamento_dados`
--

CREATE TABLE `orcamento_dados` (
  `id` int(11) NOT NULL,
  `orcamento_id` int(11) NOT NULL,
  `usuario_id` int(11) NOT NULL,
  `dados_json` longtext NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Despejando dados para a tabela `orcamento_dados`
--

INSERT INTO `orcamento_dados` (`id`, `orcamento_id`, `usuario_id`, `dados_json`) VALUES
(1, 21, 2, '{\"Colunas\":[\"Nome\",\"Valor\"],\"Linhas\":[{\"Nome\":\"Ortiga\",\"Valor\":\"123\"}]}'),
(2, 22, 12, '{\"Colunas\":[\"Nome\",\"Valor\"],\"Linhas\":[{\"Nome\":\"BYD\",\"Valor\":\"130000\"},{\"Nome\":\"Civic\",\"Valor\":\"100000\"}]}'),
(3, 23, 2, '{\"Colunas\":[\"Nome\",\"Valor\"],\"Linhas\":[{\"Nome\":\"Toyota\",\"Valor\":\"130000\"}]}');

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
(1, 'yslan_adm@gmail.com', '123', 'adm'),
(2, 'yslan_usr@gmail.com', '123', 'usr'),
(12, 'caio_usr@gmail.com', '123', 'usr'),
(13, 'caio_adm@gmail.com', '123', 'adm');

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
-- Índices de tabela `orcamento_dados`
--
ALTER TABLE `orcamento_dados`
  ADD PRIMARY KEY (`id`),
  ADD KEY `orcamento_id` (`orcamento_id`),
  ADD KEY `usuario_id` (`usuario_id`);

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
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT de tabela `modelo_orcamento_dados`
--
ALTER TABLE `modelo_orcamento_dados`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT de tabela `modelo_orcamento_usuarios`
--
ALTER TABLE `modelo_orcamento_usuarios`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT de tabela `orcamento`
--
ALTER TABLE `orcamento`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=24;

--
-- AUTO_INCREMENT de tabela `orcamento_dados`
--
ALTER TABLE `orcamento_dados`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de tabela `usuario`
--
ALTER TABLE `usuario`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

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

--
-- Restrições para tabelas `orcamento_dados`
--
ALTER TABLE `orcamento_dados`
  ADD CONSTRAINT `orcamento_dados_ibfk_1` FOREIGN KEY (`orcamento_id`) REFERENCES `orcamento` (`id`) ON DELETE CASCADE,
  ADD CONSTRAINT `orcamento_dados_ibfk_2` FOREIGN KEY (`usuario_id`) REFERENCES `usuario` (`id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
