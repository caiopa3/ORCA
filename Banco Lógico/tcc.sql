-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Tempo de geração: 15/10/2025 às 14:28
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
-- Banco de dados: `tcc`
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
(1, NULL, 0x79736c616e5f61646d3240676d61696c2e636f6d, NULL, 0x313233, 'adm', NULL, NULL),
(2, NULL, 0x79736c616e5f75737240676d61696c2e636f6d, NULL, 0x313233, 'usr', NULL, NULL),
(13, NULL, 0x6361696f5f61646d40676d61696c2e636f6d, NULL, 0x313233, 'adm', NULL, NULL),
(14, 0x66736166, 0x6666617366736140676d61696c2e636f6d, 0x283132292033323331322d33353634, 0x313233, 'usr', 0x3132332c3135362c3436342d3736, 0x31352c3634312c3536342d34),
(15, 0x59736c616e204465204a657375732053616e746f7320446120436f737461, 0x7a657a696e686f40676d61696c2e636f6d, 0x283131292031313131312d31313131, 0x31323334, 'Adm', 0x3131312c3131312c3131312d3131, 0x31312c3131312c3131312d31),
(19, 0xa85db156ab53c52a2de5b88cb15ee36a, 0xf44db837763a9f0f644f1a50fd5b05f5, 0x72cef7d88d360a243fa7b7109cf1dfe5, 0x09d97527ba89ef9bd814e63bffc822e4, 'usr', 0x696b7c65b65a8dd88fac08a0f4749a19, 0x327fc1cc5f6fc5e1212e52c6b9533c2d);

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
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=20;

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
