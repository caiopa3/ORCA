-- Reset banco
DROP DATABASE IF EXISTS orca;
CREATE DATABASE orca;
USE orca;

-- Usuários
CREATE TABLE usuario (
  id INT AUTO_INCREMENT PRIMARY KEY,
  email VARCHAR(255) DEFAULT NULL,
  senha VARCHAR(255) DEFAULT NULL,
  permissao VARCHAR(3) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO usuario (id, email, senha, permissao) VALUES
(1, 'yslan_adm@gmail.com', '123', 'adm'),
(2, 'yslan_usr@gmail.com', '123', 'usr'),
(3, 'yslan_ges@gmail.com', '123', 'ges');

-- Modelo Orçamento
CREATE TABLE modelo_orcamento (
  id INT AUTO_INCREMENT PRIMARY KEY,
  nome VARCHAR(255) NOT NULL,
  usr_criador_id INT NOT NULL,
  CONSTRAINT fk_modelo_usuario FOREIGN KEY (usr_criador_id) REFERENCES usuario (id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO modelo_orcamento (id, nome, usr_criador_id) VALUES
(1, 'teste', 1),
(2, 'Roberto', 1),
(3, 'teste 1', 1),
(4, 'Pedro lindo', 1),
(5, 'teste''); DROP TABLE usuario;', 1),
(6, 'DROP TABLE usuario;', 1);

-- Modelo Orçamento Dados
CREATE TABLE modelo_orcamento_dados (
  id INT AUTO_INCREMENT PRIMARY KEY,
  modelo_id INT NOT NULL,
  dados_json JSON NOT NULL,
  CONSTRAINT fk_dados_modelo FOREIGN KEY (modelo_id) REFERENCES modelo_orcamento (id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO modelo_orcamento_dados (id, modelo_id, dados_json) VALUES
(1, 1, '{ "Colunas": ["a (Texto)","b (Número)"], "Linhas": [{"a":"a","b":"b"}] }'),
(2, 2, '{ "Colunas": ["nome (Texto)","valor (Número)"], "Linhas": [{"nome":"Material","valor":1000},{"nome":"Mão de Obra","valor":500}] }'),
(3, 3, '{ "Colunas": ["produto (Texto)","qtd (Número)"], "Linhas": [{"produto":"Cimento","qtd":10},{"produto":"Areia","qtd":5}] }');

-- Modelo Orçamento Usuários
CREATE TABLE modelo_orcamento_usuarios (
  id INT AUTO_INCREMENT PRIMARY KEY,
  modelo_id INT NOT NULL,
  usuario_id INT NOT NULL,
  CONSTRAINT fk_mo_usuario FOREIGN KEY (usuario_id) REFERENCES usuario (id) ON DELETE CASCADE,
  CONSTRAINT fk_mo_modelo FOREIGN KEY (modelo_id) REFERENCES modelo_orcamento (id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO modelo_orcamento_usuarios (id, modelo_id, usuario_id) VALUES
(1, 1, 2),
(2, 1, 3),
(3, 2, 2),
(4, 3, 3);

-- Orçamento
CREATE TABLE orcamento (
  id INT AUTO_INCREMENT PRIMARY KEY,
  nome VARCHAR(255),
  usr_email VARCHAR(255)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO orcamento (id, nome, usr_email) VALUES
(1, 'Projeto Casa', 'yslan_usr@gmail.com'),
(2, 'Projeto Reforma', 'yslan_ges@gmail.com');
