-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Tempo de geração: 15/10/2025 às 03:03
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
(12, 'Carro', 13),
(13, 'fsafsafsa', 1);

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
(12, 12, '{\"Colunas\":[\"Nome\",\"Valor\"],\"Linhas\":[{\"Nome\":\"\",\"Valor\":\"\"}]}'),
(13, 13, '{\"Colunas\":[\"dsadsa\",\"fsafsa\"],\"Linhas\":[{\"dsadsa\":\"12\",\"fsafsa\":52.0}],\"FixedValues\":{\"dsadsa\":\"12\",\"fsafsa\":52.0}}');

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
(8, 12, 2),
(9, 13, 2),
(10, 13, 1),
(12, 13, 13);

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
(23, 'Carro 1', 'yslan_usr@gmail.com', 12),
(24, 'Novo Orçamento', 'yslan_usr@gmail.com', 11);

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
(3, 23, 2, '{\"Colunas\":[\"Nome\",\"Valor\"],\"Linhas\":[{\"Nome\":\"Toyota\",\"Valor\":\"130000\"}]}'),
(4, 24, 2, '{\"Colunas\":[\"Nome\",\"Valor\"],\"Linhas\":[{\"Nome\":\"fsafsa\",\"Valor\":\"fsafsa\"},{\"Nome\":\"fsafsa\",\"Valor\":\"fsafsaaffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffD\"}],\"Formulas\":{}}');

-- --------------------------------------------------------

--
-- Estrutura para tabela `usuario`
--

CREATE TABLE `usuario` (
  `id` int(11) NOT NULL,
  `nome_completo` varchar(255) DEFAULT NULL,
  `email` text DEFAULT NULL,
  `telefone_celular` varchar(20) DEFAULT NULL,
  `senha` varchar(255) DEFAULT NULL,
  `permissao` varchar(3) DEFAULT NULL,
  `cpf` varchar(14) DEFAULT NULL,
  `rg` varchar(20) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Despejando dados para a tabela `usuario`
--

INSERT INTO `usuario` (`id`, `nome_completo`, `email`, `telefone_celular`, `senha`, `permissao`, `cpf`, `rg`) VALUES
(1, NULL, 'yslan_adm2@gmail.com', NULL, '123', 'adm', NULL, NULL),
(2, NULL, 'yslan_usr@gmail.com', NULL, '123', 'usr', NULL, NULL),
(13, NULL, 'caio_adm@gmail.com', NULL, '123', 'adm', NULL, NULL),
(14, 'fsaf', 'ffasfsa@gmail.com', '(12) 32312-3564', '123', 'usr', '123,156,464-76', '15,641,564-4'),
(15, 'Yslan De Jesus Santos Da Costa', 'zezinho@gmail.com', '(11) 11111-1111', '1234', 'Adm', '111,111,111-11', '11,111,111-1');

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
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT de tabela `orcamento`
--
ALTER TABLE `orcamento`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=25;

--
-- AUTO_INCREMENT de tabela `orcamento_dados`
--
ALTER TABLE `orcamento_dados`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de tabela `usuario`
--
ALTER TABLE `usuario`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

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
