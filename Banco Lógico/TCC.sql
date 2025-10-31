-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Tempo de geração: 17/10/2025 às 13:59
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

DELIMITER $$
--
-- Procedimentos
--
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_atualizar_usuario_completo` (IN `p_email` VARCHAR(255), IN `p_nome_completo` VARCHAR(255), IN `p_telefone` VARCHAR(20), IN `p_cpf` VARCHAR(14), IN `p_rg` VARCHAR(20), IN `p_permissao` VARCHAR(3))   BEGIN
    UPDATE usuario
    SET nome_completo = p_nome_completo,
        telefone_celular = p_telefone,
        cpf = p_cpf,
        rg = p_rg,
        permissao = p_permissao
    WHERE email = AES_ENCRYPT(p_email, UNHEX('e37ad45fb981c26e90fe4b13a92d7ec34f88a1b2c7d95a33f06d9e12ac4788d2'));
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_registrar_log_autom` (IN `p_tabela` VARCHAR(100), IN `p_operacao` VARCHAR(20), IN `p_registro_id` INT, IN `p_detalhes` TEXT)   BEGIN
    DECLARE chave VARBINARY(32);
    SET chave = UNHEX('e37ad45fb981c26e90fe4b13a92d7ec34f88a1b2c7d95a33f06d9e12ac4788d2');

    INSERT INTO log_atividades (tabela_afetada, operacao, registro_id, detalhes)
    VALUES (
        p_tabela,
        p_operacao,
        p_registro_id,
        AES_ENCRYPT(p_detalhes, chave)
    );
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Estrutura para tabela `log_atividades`
--

CREATE TABLE `log_atividades` (
  `id` int(11) NOT NULL,
  `tabela_afetada` varchar(100) NOT NULL,
  `operacao` varchar(20) NOT NULL,
  `registro_id` int(11) DEFAULT NULL,
  `usuario_email` varbinary(255) DEFAULT NULL,
  `detalhes` varbinary(500) DEFAULT NULL,
  `data_hora` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Despejando dados para a tabela `log_atividades`
--

INSERT INTO `log_atividades` (`id`, `tabela_afetada`, `operacao`, `registro_id`, `usuario_email`, `detalhes`, `data_hora`) VALUES
(21, 'usuario', 'INSERT', 23, NULL, 0x4096ebd61c4496b1fc6760c568e7902af2dfece9102549984326eb576c2cd5f7a9b8d621c0eca6bb8fa726d0756e5140dcf7ab66d7c5f6d9e438d5e9ef87683e, '2025-10-17 08:59:09'),
(22, 'usuario', 'INSERT', 24, NULL, 0x4096ebd61c4496b1fc6760c568e7902af2dfece9102549984326eb576c2cd5f7a9b8d621c0eca6bb8fa726d0756e5140ab7ce1cdc21b83f941c703f16e77c372, '2025-10-17 08:59:09');

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
-- Acionadores `modelo_orcamento`
--
DELIMITER $$
CREATE TRIGGER `trg_log_modelo_orcamento_delete` AFTER DELETE ON `modelo_orcamento` FOR EACH ROW BEGIN
    CALL sp_registrar_log_autom('modelo_orcamento', 'DELETE', OLD.id,
        CONCAT('Modelo "', OLD.nome, '" removido.'));
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_log_modelo_orcamento_insert` AFTER INSERT ON `modelo_orcamento` FOR EACH ROW BEGIN
    CALL sp_registrar_log_autom('modelo_orcamento', 'INSERT', NEW.id,
        CONCAT('Modelo criado: ', NEW.nome));
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_log_modelo_orcamento_update` AFTER UPDATE ON `modelo_orcamento` FOR EACH ROW BEGIN
    CALL sp_registrar_log_autom('modelo_orcamento', 'UPDATE', NEW.id,
        CONCAT('Modelo atualizado de "', OLD.nome, '" para "', NEW.nome, '"'));
END
$$
DELIMITER ;

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
-- Acionadores `modelo_orcamento_dados`
--
DELIMITER $$
CREATE TRIGGER `trg_log_modelo_orcamento_dados_delete` AFTER DELETE ON `modelo_orcamento_dados` FOR EACH ROW BEGIN
    CALL sp_registrar_log_autom('modelo_orcamento_dados', 'DELETE', OLD.id,
        CONCAT('Dados do modelo ', OLD.modelo_id, ' removidos.'));
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_log_modelo_orcamento_dados_insert` AFTER INSERT ON `modelo_orcamento_dados` FOR EACH ROW BEGIN
    CALL sp_registrar_log_autom('modelo_orcamento_dados', 'INSERT', NEW.id,
        CONCAT('Novo dado inserido no modelo ID ', NEW.modelo_id));
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_log_modelo_orcamento_dados_update` AFTER UPDATE ON `modelo_orcamento_dados` FOR EACH ROW BEGIN
    CALL sp_registrar_log_autom('modelo_orcamento_dados', 'UPDATE', NEW.id,
        CONCAT('Dados do modelo ', NEW.modelo_id, ' atualizados.'));
END
$$
DELIMITER ;

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
-- Acionadores `modelo_orcamento_usuarios`
--
DELIMITER $$
CREATE TRIGGER `trg_log_modelo_orcamento_usuarios_delete` AFTER DELETE ON `modelo_orcamento_usuarios` FOR EACH ROW BEGIN
    CALL sp_registrar_log_autom('modelo_orcamento_usuarios', 'DELETE', OLD.id,
        CONCAT('Usuário ', OLD.usuario_id, ' removido do modelo ', OLD.modelo_id));
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_log_modelo_orcamento_usuarios_insert` AFTER INSERT ON `modelo_orcamento_usuarios` FOR EACH ROW BEGIN
    CALL sp_registrar_log_autom('modelo_orcamento_usuarios', 'INSERT', NEW.id,
        CONCAT('Usuário ', NEW.usuario_id, ' adicionado ao modelo ', NEW.modelo_id));
END
$$
DELIMITER ;

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
(21, 'Flores', 'yslan_usr@gmail.com', NULL),
(22, 'Carro 1', 'caio_usr@gmail.com', NULL),
(23, 'Carro 1', 'yslan_usr@gmail.com', NULL),
(24, 'Novo Orçamento', 'yslan_usr@gmail.com', NULL);

--
-- Acionadores `orcamento`
--
DELIMITER $$
CREATE TRIGGER `trg_log_orcamento_delete` AFTER DELETE ON `orcamento` FOR EACH ROW BEGIN
    CALL sp_registrar_log_autom('orcamento', 'DELETE', OLD.id,
        CONCAT('Orçamento "', OLD.nome, '" removido.'));
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_log_orcamento_insert` AFTER INSERT ON `orcamento` FOR EACH ROW BEGIN
    CALL sp_registrar_log_autom('orcamento', 'INSERT', NEW.id,
        CONCAT('Novo orçamento "', NEW.nome, '" criado.'));
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_log_orcamento_update` AFTER UPDATE ON `orcamento` FOR EACH ROW BEGIN
    CALL sp_registrar_log_autom('orcamento', 'UPDATE', NEW.id,
        CONCAT('Orçamento atualizado: "', OLD.nome, '" → "', NEW.nome, '"'));
END
$$
DELIMITER ;

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
-- Acionadores `orcamento_dados`
--
DELIMITER $$
CREATE TRIGGER `trg_log_orcamento_dados_delete` AFTER DELETE ON `orcamento_dados` FOR EACH ROW BEGIN
    CALL sp_registrar_log_autom('orcamento_dados', 'DELETE', OLD.id,
        CONCAT('Dados removidos do orçamento ', OLD.orcamento_id));
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_log_orcamento_dados_insert` AFTER INSERT ON `orcamento_dados` FOR EACH ROW BEGIN
    CALL sp_registrar_log_autom('orcamento_dados', 'INSERT', NEW.id,
        CONCAT('Novo dado adicionado ao orçamento ID ', NEW.orcamento_id));
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_log_orcamento_dados_update` AFTER UPDATE ON `orcamento_dados` FOR EACH ROW BEGIN
    CALL sp_registrar_log_autom('orcamento_dados', 'UPDATE', NEW.id,
        CONCAT('Dados atualizados no orçamento ', NEW.orcamento_id));
END
$$
DELIMITER ;

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
(23, 0x1f24738c1c0a43283eef965b26f4d8db, 0x43f0eb618a6c6a74b0e0ec0fecf4eb30fd5fc242252cdf5abba3327f07aa67d7, 0xba05f4d7c3406db134bdbe2efb75f115, 0x34c58a79aba7badd6472cc2c53d583e8, 'usr', 0x783fb3488fc1dc57248bfba0ec353464, 0x24da2d764472ccc7d43cdf5816c815e8),
(24, 0xa0445e313bae0ddd629e71c3591254d3, 0xcefe67d83f4dabc03986ab450b017337fd5fc242252cdf5abba3327f07aa67d7, 0x92912d1a919e062f4e53af3047388ce5, 0x34c58a79aba7badd6472cc2c53d583e8, 'adm', 0x7d684896d25448f6eb5d87d9c06c2686, 0xfc59e74c65cff48d2d790f5da40b7712);

--
-- Acionadores `usuario`
--
DELIMITER $$
CREATE TRIGGER `trg_log_usuario_delete` AFTER DELETE ON `usuario` FOR EACH ROW BEGIN
    CALL sp_registrar_log_autom('usuario', 'DELETE', OLD.id, 'Usuário removido.');
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_log_usuario_insert` AFTER INSERT ON `usuario` FOR EACH ROW BEGIN
    CALL sp_registrar_log_autom('usuario', 'INSERT', NEW.id,
        CONCAT('Novo usuário inserido. Dados (parciais): permissao=', NEW.permissao));
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_log_usuario_update` AFTER UPDATE ON `usuario` FOR EACH ROW BEGIN
    CALL sp_registrar_log_autom('usuario', 'UPDATE', NEW.id,
        CONCAT('Usuário atualizado. permissao de ', OLD.permissao, ' para ', NEW.permissao));
END
$$
DELIMITER ;
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
-- Estrutura stand-in para view `view_logs_autom`
-- (Veja abaixo para a visão atual)
--
CREATE TABLE `view_logs_autom` (
`id` int(11)
,`tabela_afetada` varchar(100)
,`operacao` varchar(20)
,`registro_id` int(11)
,`detalhes` text
,`data_hora` datetime
);

-- --------------------------------------------------------

--
-- Estrutura para view `view_decripto`
--
DROP TABLE IF EXISTS `view_decripto`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `view_decripto`  AS SELECT `usuario`.`id` AS `id`, cast(aes_decrypt(`usuario`.`nome_completo`,unhex('e37ad45fb981c26e90fe4b13a92d7ec34f88a1b2c7d95a33f06d9e12ac4788d2')) as char(255) charset utf8mb4) AS `nome_completo`, cast(aes_decrypt(`usuario`.`email`,unhex('e37ad45fb981c26e90fe4b13a92d7ec34f88a1b2c7d95a33f06d9e12ac4788d2')) as char(255) charset utf8mb4) AS `email`, cast(aes_decrypt(`usuario`.`telefone_celular`,unhex('e37ad45fb981c26e90fe4b13a92d7ec34f88a1b2c7d95a33f06d9e12ac4788d2')) as char(20) charset utf8mb4) AS `telefone_celular`, cast(aes_decrypt(`usuario`.`senha`,unhex('e37ad45fb981c26e90fe4b13a92d7ec34f88a1b2c7d95a33f06d9e12ac4788d2')) as char(255) charset utf8mb4) AS `senha`, `usuario`.`permissao` AS `permissao`, cast(aes_decrypt(`usuario`.`cpf`,unhex('e37ad45fb981c26e90fe4b13a92d7ec34f88a1b2c7d95a33f06d9e12ac4788d2')) as char(14) charset utf8mb4) AS `cpf`, cast(aes_decrypt(`usuario`.`rg`,unhex('e37ad45fb981c26e90fe4b13a92d7ec34f88a1b2c7d95a33f06d9e12ac4788d2')) as char(20) charset utf8mb4) AS `rg` FROM `usuario` ;

-- --------------------------------------------------------

--
-- Estrutura para view `view_logs_autom`
--
DROP TABLE IF EXISTS `view_logs_autom`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `view_logs_autom`  AS SELECT `l`.`id` AS `id`, `l`.`tabela_afetada` AS `tabela_afetada`, `l`.`operacao` AS `operacao`, `l`.`registro_id` AS `registro_id`, cast(aes_decrypt(`l`.`detalhes`,unhex('e37ad45fb981c26e90fe4b13a92d7ec34f88a1b2c7d95a33f06d9e12ac4788d2')) as char(1000) charset utf8mb4) AS `detalhes`, `l`.`data_hora` AS `data_hora` FROM `log_atividades` AS `l` ORDER BY `l`.`data_hora` DESC ;

--
-- Índices para tabelas despejadas
--

--
-- Índices de tabela `log_atividades`
--
ALTER TABLE `log_atividades`
  ADD PRIMARY KEY (`id`);

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
-- AUTO_INCREMENT de tabela `log_atividades`
--
ALTER TABLE `log_atividades`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=23;

--
-- AUTO_INCREMENT de tabela `modelo_orcamento`
--
ALTER TABLE `modelo_orcamento`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;

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
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=26;

--
-- AUTO_INCREMENT de tabela `orcamento_dados`
--
ALTER TABLE `orcamento_dados`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

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
