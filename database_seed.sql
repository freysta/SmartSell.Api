-- Script para popular o banco de dados InterUniBus com dados de teste

USE InterUniBus;

-- Limpar dados existentes (cuidado em produção!)
SET FOREIGN_KEY_CHECKS = 0;
TRUNCATE TABLE Notificacao;
TRUNCATE TABLE Presenca;
TRUNCATE TABLE Pagamento;
TRUNCATE TABLE RotaAluno;
TRUNCATE TABLE Rota;
TRUNCATE TABLE PontoEmbarque;
TRUNCATE TABLE Aluno;
TRUNCATE TABLE Usuario;
SET FOREIGN_KEY_CHECKS = 1;

-- Inserir usuários (Admin e Motoristas)
INSERT INTO Usuario (nome, email, senha, tipo) VALUES
('Admin Galdino', 'admin@test.com', '$2a$11$8YvF8qZ9X.rQ7K3mN2pL4eJ5H6G9I0K1L2M3N4O5P6Q7R8S9T0U1V2', 'Admin'),
('Carlos Santos Silva', 'motorista@test.com', '$2a$11$8YvF8qZ9X.rQ7K3mN2pL4eJ5H6G9I0K1L2M3N4O5P6Q7R8S9T0U1V2', 'Motorista'),
('Maria Oliveira Costa', 'maria.motorista@email.com', '$2a$11$8YvF8qZ9X.rQ7K3mN2pL4eJ5H6G9I0K1L2M3N4O5P6Q7R8S9T0U1V2', 'Motorista'),
('João Pereira Lima', 'joao.motorista@email.com', '$2a$11$8YvF8qZ9X.rQ7K3mN2pL4eJ5H6G9I0K1L2M3N4O5P6Q7R8S9T0U1V2', 'Motorista');

-- Inserir alunos
INSERT INTO Aluno (nome, telefone, email, cpf) VALUES
('Ana Silva Santos', '(11) 99999-1111', 'aluno@test.com', '123.456.789-01'),
('João Pedro Oliveira', '(11) 99999-2222', 'joao.pedro@email.com', '987.654.321-00'),
('Mariana Costa Lima', '(11) 99999-3333', 'mariana.costa@email.com', '456.789.012-34'),
('Pedro Henrique Souza', '(11) 99999-4444', 'pedro.souza@email.com', '789.012.345-67'),
('Juliana Ferreira Santos', '(11) 99999-5555', 'juliana.ferreira@email.com', '012.345.678-90'),
('Lucas Rodrigues Silva', '(11) 99999-6666', 'lucas.rodrigues@email.com', '345.678.901-23'),
('Camila Alves Costa', '(11) 99999-7777', 'camila.alves@email.com', '678.901.234-56'),
('Rafael Santos Oliveira', '(11) 99999-8888', 'rafael.santos@email.com', '901.234.567-89'),
('Beatriz Lima Ferreira', '(11) 99999-9999', 'beatriz.lima@email.com', '234.567.890-12'),
('Gabriel Costa Santos', '(11) 99999-0000', 'gabriel.costa@email.com', '567.890.123-45');

-- Inserir pontos de embarque
INSERT INTO PontoEmbarque (nome, rua, bairro, cidade, ponto_referencia) VALUES
('Terminal Central', 'Av. Principal, 123', 'Centro', 'São Paulo', 'Próximo ao shopping'),
('Estação Metro Norte', 'Rua das Flores, 456', 'Vila Norte', 'São Paulo', 'Saída A do metrô'),
('Praça da Liberdade', 'Rua da Liberdade, 789', 'Liberdade', 'São Paulo', 'Em frente ao mercado'),
('Terminal Rodoviário Sul', 'Av. Sul, 321', 'Vila Sul', 'São Paulo', 'Plataforma 3'),
('Estação Trem Oeste', 'Rua Oeste, 654', 'Zona Oeste', 'São Paulo', 'Portão principal'),
('Centro Comercial Leste', 'Av. Leste, 987', 'Zona Leste', 'São Paulo', 'Entrada principal');

-- Inserir rotas
INSERT INTO Rota (data_rota, destino, horario_saida, status, fk_id_motorista) VALUES
('2024-01-15', 'Campus Norte - Universidade ABC', '07:00:00', 'Planejada', 2),
('2024-01-15', 'Campus Sul - Universidade XYZ', '07:30:00', 'Planejada', 3),
('2024-01-15', 'Campus Centro - Faculdade DEF', '08:00:00', 'Planejada', 4),
('2024-01-16', 'Campus Norte - Universidade ABC', '07:00:00', 'Planejada', 2),
('2024-01-16', 'Campus Sul - Universidade XYZ', '07:30:00', 'Planejada', 3),
('2024-01-14', 'Campus Norte - Universidade ABC', '07:00:00', 'Concluída', 2),
('2024-01-14', 'Campus Sul - Universidade XYZ', '07:30:00', 'Concluída', 3),
('2024-01-13', 'Campus Centro - Faculdade DEF', '08:00:00', 'Concluída', 4);

-- Inserir relação rota-aluno
INSERT INTO RotaAluno (fk_id_rota, fk_id_aluno, fk_id_ponto, confirmado) VALUES
-- Rota 1 (Campus Norte - 15/01)
(1, 1, 1, 'Sim'),
(1, 2, 2, 'Sim'),
(1, 3, 1, 'Não'),
-- Rota 2 (Campus Sul - 15/01)
(2, 4, 3, 'Sim'),
(2, 5, 4, 'Sim'),
(2, 6, 3, 'Não'),
-- Rota 3 (Campus Centro - 15/01)
(3, 7, 5, 'Sim'),
(3, 8, 6, 'Sim'),
-- Rota 4 (Campus Norte - 16/01)
(4, 1, 1, 'Não'),
(4, 2, 2, 'Não'),
(4, 9, 1, 'Não'),
-- Rota 5 (Campus Sul - 16/01)
(5, 4, 3, 'Não'),
(5, 5, 4, 'Não'),
(5, 10, 4, 'Não');

-- Inserir pagamentos
INSERT INTO Pagamento (fk_id_aluno, valor, data_pagamento, referencia_mes, status, forma_pagamento) VALUES
-- Janeiro 2024
(1, 150.00, '2024-01-05', '01/2024', 'Pago', 'PIX'),
(2, 150.00, '2024-01-03', '01/2024', 'Pago', 'Cartão'),
(3, 150.00, '2024-01-08', '01/2024', 'Pago', 'Transferência'),
(4, 150.00, '2024-01-10', '01/2024', 'Pendente', NULL),
(5, 150.00, '2024-01-02', '01/2024', 'Pago', 'PIX'),
(6, 150.00, '2024-01-15', '01/2024', 'Atrasado', NULL),
(7, 150.00, '2024-01-04', '01/2024', 'Pago', 'Dinheiro'),
(8, 150.00, '2024-01-12', '01/2024', 'Pendente', NULL),
(9, 150.00, '2024-01-06', '01/2024', 'Pago', 'Cartão'),
(10, 150.00, '2024-01-20', '01/2024', 'Atrasado', NULL),

-- Dezembro 2023
(1, 150.00, '2023-12-05', '12/2023', 'Pago', 'PIX'),
(2, 150.00, '2023-12-03', '12/2023', 'Pago', 'Cartão'),
(3, 150.00, '2023-12-08', '12/2023', 'Pago', 'Transferência'),
(4, 150.00, '2023-12-10', '12/2023', 'Pago', 'PIX'),
(5, 150.00, '2023-12-02', '12/2023', 'Pago', 'PIX');

-- Inserir presenças (para rotas concluídas)
INSERT INTO Presenca (fk_id_rota, fk_id_aluno, presente, observacao) VALUES
-- Rota 6 (Campus Norte - 14/01 - Concluída)
(6, 1, 'Sim', 'Embarcou no horário'),
(6, 2, 'Sim', 'Embarcou no horário'),
(6, 3, 'Não', 'Faltou - avisou por WhatsApp'),
-- Rota 7 (Campus Sul - 14/01 - Concluída)
(7, 4, 'Sim', 'Embarcou no horário'),
(7, 5, 'Sim', 'Embarcou no horário'),
(7, 6, 'Sim', 'Embarcou no horário'),
-- Rota 8 (Campus Centro - 13/01 - Concluída)
(8, 7, 'Sim', 'Embarcou no horário'),
(8, 8, 'Não', 'Faltou sem avisar');

-- Inserir notificações
INSERT INTO Notificacao (titulo, mensagem, data_envio, fk_id_aluno) VALUES
('Pagamento em atraso', 'Sua mensalidade de janeiro está em atraso. Por favor, regularize sua situação.', '2024-01-16 10:00:00', 6),
('Pagamento em atraso', 'Sua mensalidade de janeiro está em atraso. Por favor, regularize sua situação.', '2024-01-21 10:00:00', 10),
('Rota cancelada', 'A rota de hoje (16/01) para o Campus Centro foi cancelada devido ao trânsito.', '2024-01-16 06:30:00', NULL),
('Novo horário', 'A partir de segunda-feira (22/01), o horário da rota Campus Norte será às 06:45.', '2024-01-19 15:00:00', NULL),
('Pagamento confirmado', 'Seu pagamento de janeiro foi confirmado. Obrigado!', '2024-01-05 14:30:00', 1),
('Pagamento confirmado', 'Seu pagamento de janeiro foi confirmado. Obrigado!', '2024-01-03 16:45:00', 2),
('Lembrete de pagamento', 'Sua mensalidade vence em 3 dias. Não esqueça de efetuar o pagamento.', '2024-01-07 09:00:00', 4),
('Lembrete de pagamento', 'Sua mensalidade vence em 3 dias. Não esqueça de efetuar o pagamento.', '2024-01-09 09:00:00', 8);

-- Verificar dados inseridos
SELECT 'Usuários' as Tabela, COUNT(*) as Total FROM Usuario
UNION ALL
SELECT 'Alunos', COUNT(*) FROM Aluno
UNION ALL
SELECT 'Pontos de Embarque', COUNT(*) FROM PontoEmbarque
UNION ALL
SELECT 'Rotas', COUNT(*) FROM Rota
UNION ALL
SELECT 'Rota-Aluno', COUNT(*) FROM RotaAluno
UNION ALL
SELECT 'Pagamentos', COUNT(*) FROM Pagamento
UNION ALL
SELECT 'Presenças', COUNT(*) FROM Presenca
UNION ALL
SELECT 'Notificações', COUNT(*) FROM Notificacao;

-- Mostrar estatísticas
SELECT 
    'Dashboard Stats' as Info,
    (SELECT COUNT(*) FROM Aluno) as Total_Alunos,
    (SELECT COUNT(*) FROM Usuario WHERE tipo = 'Motorista') as Total_Motoristas,
    (SELECT COUNT(*) FROM Rota) as Total_Rotas,
    (SELECT COUNT(*) FROM Pagamento WHERE status IN ('Pendente', 'Atrasado')) as Pagamentos_Pendentes,
    (SELECT SUM(valor) FROM Pagamento WHERE status = 'Pago' AND MONTH(data_pagamento) = 1 AND YEAR(data_pagamento) = 2024) as Receita_Janeiro;

COMMIT;
