/* Lógico_TCC: */

CREATE TABLE orcamento (
    id INTEGER AUTO_INCREMENT NOT NULL PRIMARY KEY,
    nome TEXT,
    usr_email TEXT
);

CREATE TABLE usuario (
    id INTEGER AUTO_INCREMENT NOT NULL PRIMARY KEY,
    email TEXT,
    senha VARCHAR(255),
    permissao VARCHAR(3)
);