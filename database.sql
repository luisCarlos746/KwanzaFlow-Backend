CREATE DATABASE IF NOT EXISTS KwanzaFlow;
USE KwanzaFlow;

CREATE TABLE IF NOT EXISTS Clientes (
    id_cliente INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    telefone VARCHAR(20) UNIQUE NOT NULL, 
    pin_hash VARCHAR(255) NOT NULL,      
    saldo DECIMAL(15, 2) DEFAULT 0.00    
);

CREATE TABLE IF NOT EXISTS Transacoes (
    id_transacao INT AUTO_INCREMENT PRIMARY KEY,
    id_origem INT,
    id_destino INT,
    valor DECIMAL(15, 2) NOT NULL,
    data_hora DATETIME DEFAULT CURRENT_TIMESTAMP,
    estado VARCHAR(50) NOT NULL,          
    FOREIGN KEY (id_origem) REFERENCES Clientes(id_cliente),
    FOREIGN KEY (id_destino) REFERENCES Clientes(id_cliente)
);

INSERT INTO Clientes (nome, telefone, pin_hash, saldo) VALUES 
('Analtino Silva', '923000001', '1234', 50000.00), 
('Maria Dombasi', '923000002', '4321', 1500.00);
