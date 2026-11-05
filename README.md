# 💸 KwanzaFlow - Motor de Transações Financeiras via USSD

O **KwanzaFlow** é um protótipo de sistema de pagamentos instantâneos (estilo PIX/Kwik) desenvolvido para simular transações financeiras seguras através de redes móveis de baixa largura de banda (tecnologia USSD). 

O projeto foi desenhado especificamente para responder a dois grandes desafios estruturais de mercados informais em mercados emergentes (como a Praça dos Congolenses, em Luanda): a **exclusão digital** (alto uso de *feature phones* ou telefones de botão) e a **insegurança física** (risco de assaltos ao expor smartphones em público).

---

## 🛠️ Arquitetura e Solução Tecnológica

O sistema utiliza um backend robusto para processar requisições de texto simples que simulam o protocolo USSD, garantindo que qualquer cidadão com um telefone antigo possa efetuar pagamentos discretos e imediatos.

### Tecnologias Utilizadas:
*   **C# e .NET (Console Application):** Responsável pelas regras de negócio, métodos automatizados e validações de segurança.
*   **MySQL Server:** Base de dados relacional que garante a persistência estável dos saldos e do histórico de transações.
*   **Dapper / MySql.Data:** Conectores utilizados para estabelecer uma comunicação segura e eficiente entre a aplicação e a base de dados.

---

## 🛡️ Camadas de Segurança e Regras Implementadas

Como se trata de um sistema financeiro simulado, o código foi construído seguindo padrões estritos de integridade de dados:

1.  **Integridade Referencial (Chaves Estrangeiras):** O banco de dados bloqueia automaticamente qualquer tentativa de transação que envolva contas inexistentes (Error Code 1452).
2.  **Validação de Saldo Prévio:** O motor em C# verifica e barra na hora transferências superiores ao saldo disponível, registando a tentativa falhada para auditoria de segurança.
3.  **Restrição UNIQUE no SQL:** Garante que cada número de telefone atue como um identificador único de conta, impedindo duplicidade ou clonagem de acessos no sistema informático.
4.  **Parâmetros Seguros contra SQL Injection:** Toda a comunicação com o banco de dados é feita através de parâmetros (`Parameters.AddWithValue`), blindando o backend contra ataques maliciosos de injeção de código.

---

## 📊 Estrutura do Banco de Dados (SQL)

O ecossistema é sustentado por duas tabelas principais geridas de forma estrita:
*   `Clientes`: Armazena o ID, Nome, Número de Telefone (Chave USSD), Hash de Segurança e Saldo Preciso (`DECIMAL`).
*   `Transacoes`: Regista de forma cronológica e imutável o histórico de transferências (`id_origem`, `id_destino`, `valor`, `data_hora` e `estado`).

---

## 🚀 Como Executar o Projeto

1. Execute o script contido em `database.sql` no seu servidor MySQL local para criar o esquema `KwanzaFlow`.
2. Abra a solução no **Visual Studio**, ajuste a string de conexão no `Program.cs` com as suas credenciais locais.
3. Execute o projeto (`F5`) para simular o processamento automático da transferência entre as contas de teste.

---
💻 Projeto desenvolvido por um estudante de 16 anos focado em Engenharia de Software e Desenvolvimento Backend de alto impacto social.
