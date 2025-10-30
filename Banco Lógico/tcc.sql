-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Tempo de geração: 30/10/2025 às 15:07
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
(15, 'Teste 1', 22);

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
(15, 15, '{\"Colunas\":[\"Nome\",\"Valor Fixo\",\"Valor Variável\",\"Total\"],\"Linhas\":[{\"Nome\":\"\",\"Valor Fixo\":\"200\",\"Valor Variável\":\"\",\"Total\":\"= Valor FIxo + Valor Variável\"}],\"FixedValues\":{\"Valor Fixo\":200.0,\"Total\":\"= Valor FIxo + Valor Variável\"}}');

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
(13, 15, 24);

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
(25, 'Novo Orçamento', 'usr@orcamento.com', 15),
(26, 'Novo Orçamento 2', 'usr@orcamento.com', 15);

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
(5, 25, 24, '{\"Colunas\":[\"Nome\",\"Valor Fixo\",\"Valor Variável\",\"Total\"],\"Linhas\":[{\"Nome\":\"Urtigas\",\"Valor Fixo\":\"200\",\"Valor Variável\":\"5\",\"Total\":\"205\"},{\"Nome\":\"Rosas\",\"Valor Fixo\":\"200\",\"Valor Variável\":\"20\",\"Total\":\"220\"},{\"Nome\":\"Margaridas\",\"Valor Fixo\":\"200\",\"Valor Variável\":\"7\",\"Total\":\"207\"},{\"Nome\":\"Total\",\"Valor Fixo\":\"\",\"Valor Variável\":null,\"Total\":\"632\"}],\"Formulas\":{\"3:Total\":\"= [1]Total + [2]Total + [3]Total\",\"2:Total\":\"= Valor FIxo + Valor Variável\",\"1:Total\":\"= Valor FIxo + Valor Variável\",\"0:Total\":\"= Valor FIxo + Valor Variável\"}}');

-- --------------------------------------------------------

--
-- Estrutura para tabela `usuario`
--

CREATE TABLE `usuario` (
  `id` int(11) NOT NULL,
  `nome_completo` blob DEFAULT NULL,
  `email` blob DEFAULT NULL,
  `telefone_celular` blob DEFAULT NULL,
  `senha` blob DEFAULT NULL,
  `permissao` varchar(3) DEFAULT NULL,
  `cpf` blob DEFAULT NULL,
  `rg` blob DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Despejando dados para a tabela `usuario`
--

INSERT INTO `usuario` (`id`, `nome_completo`, `email`, `telefone_celular`, `senha`, `permissao`, `cpf`, `rg`) VALUES
(22, 0x019e69786e145654dd9143a634fc7f4daae7d438cc23ee0445cf66f248024d9b, 0x6d944b3437da1cc3f147a6be15b43d67bae8c42f94c2ec35862df3f2521972c2, 0x13a9c55e4dd9315f7771c84c83e51f81, 0x34c58a79aba7badd6472cc2c53d583e8, 'adm', 0xfc3ed9f3fd493026fef72ce593b71c2f, 0xb5a23f7765344fd90c8216e5ff9181fb),
(23, 0x0ebfef58b2d631320e52bcedb7368977, 0xbdcc74aaf1b204529c9c2167a12d6c9a5aea88f1978e4eb46a5945fc3d03c57f, 0x6b7af9393047fcd828ac04a235e1acf0, 0x34c58a79aba7badd6472cc2c53d583e8, 'adm', 0x7b1f227f4aef8a79500401d51b005620, 0xc4f0851f5cbd3ee9141ad67b24a5e868),
(24, 0x00c1ad4b7e2a30d7916425e0003b00aa, 0x659b08564ed2b54a77adf33a98f24c73bae8c42f94c2ec35862df3f2521972c2, 0x13a9c55e4dd9315f7771c84c83e51f81, 0x34c58a79aba7badd6472cc2c53d583e8, 'usr', 0xfc3ed9f3fd493026fef72ce593b71c2f, 0xb5a23f7765344fd90c8216e5ff9181fb);

--
-- Acionadores `usuario`
--
DELIMITER $$
CREATE TRIGGER `trg_usuario_encrypt_insert` BEFORE INSERT ON `usuario` FOR EACH ROW BEGIN
    -- converte o HEX da chave para binário
    DECLARE chave VARBINARY(32);
    SET chave = UNHEX('e37ad45fb981c26e90fe4b13a92d7ec34f88a1b2c7d95a33f06d9e12ac4788d2');

    IF NEW.nome_completo IS NOT NULL THEN
        SET NEW.nome_completo = AES_ENCRYPT(NEW.nome_completo, chave);
    END IF;

    IF NEW.email IS NOT NULL THEN
        SET NEW.email = AES_ENCRYPT(NEW.email, chave);
    END IF;

    IF NEW.telefone_celular IS NOT NULL THEN
        SET NEW.telefone_celular = AES_ENCRYPT(NEW.telefone_celular, chave);
    END IF;

    IF NEW.senha IS NOT NULL THEN
        SET NEW.senha = AES_ENCRYPT(NEW.senha, chave);
    END IF;

    IF NEW.cpf IS NOT NULL THEN
        SET NEW.cpf = AES_ENCRYPT(NEW.cpf, chave);
    END IF;

    IF NEW.rg IS NOT NULL THEN
        SET NEW.rg = AES_ENCRYPT(NEW.rg, chave);
    END IF;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_usuario_encrypt_update` BEFORE UPDATE ON `usuario` FOR EACH ROW BEGIN
    DECLARE chave VARBINARY(32);
  
    SET chave = UNHEX('e37ad45fb981c26e90fe4b13a92d7ec34f88a1b2c7d95a33f06d9e12ac4788d2');

    IF NEW.nome_completo <> OLD.nome_completo THEN
        SET NEW.nome_completo = AES_ENCRYPT(NEW.nome_completo, chave);
    END IF;

    IF NEW.email <> OLD.email THEN
        SET NEW.email = AES_ENCRYPT(NEW.email, chave);
    END IF;

    IF NEW.telefone_celular <> OLD.telefone_celular THEN
        SET NEW.telefone_celular = AES_ENCRYPT(NEW.telefone_celular, chave);
    END IF;

    IF NEW.senha <> OLD.senha THEN
        SET NEW.senha = AES_ENCRYPT(NEW.senha, chave);
    END IF;

    IF NEW.cpf <> OLD.cpf THEN
        SET NEW.cpf = AES_ENCRYPT(NEW.cpf, chave);
    END IF;

    IF NEW.rg <> OLD.rg THEN
        SET NEW.rg = AES_ENCRYPT(NEW.rg, chave);
    END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Estrutura stand-in para view `view_decripto`
-- (Veja abaixo para a visão atual)
--
CREATE TABLE `view_decripto` (
`id` int(11)
,`nome_completo` varchar(255)
,`email` varchar(255)
,`telefone_celular` varchar(20)
,`senha` varchar(255)
,`permissao` varchar(3)
,`cpf` varchar(14)
,`rg` varchar(20)
);

-- --------------------------------------------------------

--
-- Estrutura para view `view_decripto`
--
DROP TABLE IF EXISTS `view_decripto`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `view_decripto`  AS SELECT `usuario`.`id` AS `id`, cast(aes_decrypt(`usuario`.`nome_completo`,unhex('e37ad45fb981c26e90fe4b13a92d7ec34f88a1b2c7d95a33f06d9e12ac4788d2')) as char(255) charset utf8mb4) AS `nome_completo`, cast(aes_decrypt(`usuario`.`email`,unhex('e37ad45fb981c26e90fe4b13a92d7ec34f88a1b2c7d95a33f06d9e12ac4788d2')) as char(255) charset utf8mb4) AS `email`, cast(aes_decrypt(`usuario`.`telefone_celular`,unhex('e37ad45fb981c26e90fe4b13a92d7ec34f88a1b2c7d95a33f06d9e12ac4788d2')) as char(20) charset utf8mb4) AS `telefone_celular`, cast(aes_decrypt(`usuario`.`senha`,unhex('e37ad45fb981c26e90fe4b13a92d7ec34f88a1b2c7d95a33f06d9e12ac4788d2')) as char(255) charset utf8mb4) AS `senha`, `usuario`.`permissao` AS `permissao`, cast(aes_decrypt(`usuario`.`cpf`,unhex('e37ad45fb981c26e90fe4b13a92d7ec34f88a1b2c7d95a33f06d9e12ac4788d2')) as char(14) charset utf8mb4) AS `cpf`, cast(aes_decrypt(`usuario`.`rg`,unhex('e37ad45fb981c26e90fe4b13a92d7ec34f88a1b2c7d95a33f06d9e12ac4788d2')) as char(20) charset utf8mb4) AS `rg` FROM `usuario` ;

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
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- AUTO_INCREMENT de tabela `modelo_orcamento_dados`
--
ALTER TABLE `modelo_orcamento_dados`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- AUTO_INCREMENT de tabela `modelo_orcamento_usuarios`
--
ALTER TABLE `modelo_orcamento_usuarios`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- AUTO_INCREMENT de tabela `orcamento`
--
ALTER TABLE `orcamento`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=27;

--
-- AUTO_INCREMENT de tabela `orcamento_dados`
--
ALTER TABLE `orcamento_dados`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT de tabela `usuario`
--
ALTER TABLE `usuario`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=25;

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
