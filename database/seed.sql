CREATE DATABASE IF NOT EXISTS banco;
USE banco;

CREATE TABLE `usuario` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `email` text DEFAULT NULL,
  `senha` varchar(255) DEFAULT NULL,
  `permissao` varchar(3) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO `usuario` (`id`, `email`, `senha`, `permissao`) VALUES
(1, 'yslan_adm@gmail.com', '123', 'adm'),
(2, 'yslan_usr@gmail.com', '123', 'usr'),
(3, 'yslan_ges@gmail.com', '123', 'ges');

CREATE TABLE `modelo_orcamento` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `nome` varchar(255) NOT NULL,
  `usr_criador_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `usr_criador_id` (`usr_criador_id`),
  CONSTRAINT `modelo_orcamento_ibfk_1` FOREIGN KEY (`usr_criador_id`) REFERENCES `usuario` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO `modelo_orcamento` (`id`, `nome`, `usr_criador_id`) VALUES
(1, 'teste', 1),
(2, 'Roberto', 1);

CREATE TABLE `modelo_orcamento_dados` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `modelo_id` int(11) NOT NULL,
  `dados_json` longtext NOT NULL,
  PRIMARY KEY (`id`),
  KEY `modelo_id` (`modelo_id`),
  CONSTRAINT `modelo_orcamento_dados_ibfk_1` FOREIGN KEY (`modelo_id`) REFERENCES `modelo_orcamento` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `modelo_orcamento_usuarios` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `modelo_id` int(11) NOT NULL,
  `usuario_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `modelo_id` (`modelo_id`),
  KEY `usuario_id` (`usuario_id`),
  CONSTRAINT `modelo_orcamento_usuarios_ibfk_1` FOREIGN KEY (`modelo_id`) REFERENCES `modelo_orcamento` (`id`) ON DELETE CASCADE,
  CONSTRAINT `modelo_orcamento_usuarios_ibfk_2` FOREIGN KEY (`usuario_id`) REFERENCES `usuario` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `orcamento` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `nome` text DEFAULT NULL,
  `usr_email` text DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO `orcamento` (`id`, `nome`, `usr_email`) VALUES
(1, 'Projeto Casa', 'yslan_usr@gmail.com');
