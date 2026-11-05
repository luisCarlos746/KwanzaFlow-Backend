using MySql.Data.MySqlClient;
using System;

namespace KwanzaFlow_Projeto
{
    internal class Program
    {
        // NOTA DE SEGURANÇA: Senha omitida para publicação no GitHub. 
        // Substitua 'A_SUA_SENHA_AQUI' pela sua credencial local ao executar.
        static string dadosConexao = "server=localhost;database=KwanzaFlow;uid=root;password=A_SUA_SENHA_AQUI;port=3306;";

        static void Main(string[] args)
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("===   KWANZAFLOW - SIMULADOR DE MOTOR USSD     ===");
            Console.WriteLine("==================================================\n");

            // MENU INTERATIVO: Captura os dados inseridos pelo utilizador
            try
            {
                Console.Write("Digite o ID da conta de ORIGEM (Ex: 1 para Analtino): ");
                int idOrigem = Convert.ToInt32(Console.ReadLine());

                Console.Write("Digite o ID da conta de DESTINO (Ex: 2 para Maria): ");
                int idDestino = Convert.ToInt32(Console.ReadLine());

                Console.Write("Digite o VALOR em Kwanzas a transferir (Ex: 500,00): ");
                decimal valorTransferir = Convert.ToDecimal(Console.ReadLine());

                Console.WriteLine("\n[Processando requisicao de rede simulada via USSD...]");
                
                // Dispara o motor de transferencia com os dados digitados
                EfetuarTransferencia(idOrigem, idDestino, valorTransferir);
            }
            catch (FormatException)
            {
                Console.WriteLine("\nErro: Por favor, introduza valores numericos validos.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nErro inesperado no fluxo do menu: {ex.Message}");
            }

            Console.WriteLine("\nPrime Enter para fechar o simulador...");
            Console.ReadLine();
        }

        // MOTOR FINANCEIRO: Valida regras de negocio e executa as queries SQL
        static void EfetuarTransferencia(int origem, int destino, decimal valor)
        {
            using (MySqlConnection conexao = new MySqlConnection(dadosConexao))
            { 
                try
                {
                    conexao.Open();

                    // PASSO 1: Cláusula de Guarda - Verificar o saldo previo de quem envia
                    string querySaldo = "SELECT saldo FROM Clientes WHERE id_cliente = @origem";
                    decimal saldoDisponivel = 0;

                    using (MySqlCommand cmdSaldo = new MySqlCommand(querySaldo, conexao))
                    {
                        cmdSaldo.Parameters.AddWithValue("@origem", origem);
                        
                        var resultado = cmdSaldo.ExecuteScalar();
                        if (resultado == null)
                        {
                            Console.WriteLine("Erro: Conta de origem nao foi encontrada no sistema.");
                            return;
                        }
                        saldoDisponivel = Convert.ToDecimal(resultado);
                    }

                    // Se o saldo for insuficiente, barra a transacao imediatamente
                    if (saldoDisponivel < valor)
                    {
                        Console.WriteLine("\nTRANSACAO RECUSADA: Saldo Insuficiente para concluir a operacao!");
                        RegistarHistorico(origem, destino, valor, "Falha - Sem Saldo", conexao);
                        return;
                    }

                    // PASSO 2: Deduzir o valor do saldo do Cliente de Origem
                    string querySubtrair = "UPDATE Clientes SET saldo = saldo - @valor WHERE id_cliente = @origem";
                    using (MySqlCommand cmdSubtrair = new MySqlCommand(querySubtrair, conexao))
                    {
                        cmdSubtrair.Parameters.AddWithValue("@valor", valor);
                        cmdSubtrair.Parameters.AddWithValue("@origem", origem);
                        cmdSubtrair.ExecuteNonQuery();
                    }

                    // PASSO 3: Incrementar o valor no saldo do Cliente de Destino
                    string querySomar = "UPDATE Clientes SET saldo = saldo + @valor WHERE id_cliente = @destino";
                    using (MySqlCommand cmdSomar = new MySqlCommand(querySomar, conexao))
                    {
                        cmdSomar.Parameters.AddWithValue("@valor", valor);
                        cmdSomar.Parameters.AddWithValue("@destino", destino);
                        
                        int linhasAfetadas = cmdSomar.ExecuteNonQuery();
                        
                        if (linhasAfetadas == 0)
                        {
                            Console.WriteLine("Erro Critico: Conta de destino invalida.");
                            return;
                        }
                    }

                    // PASSO 4: Registar o sucesso de forma imutavel no historico (Extrato)
                    RegistarHistorico(origem, destino, valor, "Sucesso", conexao);

                    Console.WriteLine("\nSUCESSO: Transacao processada e registada com sucesso na rede interbancaria!");
                }
                catch (MySqlException ex) when (ex.Number == 1452)
                {
                    // Captura especificamente o erro de chave estrangeira (Integridade Referencial)
                    Console.WriteLine("\nERRO DE FRAUDE: Operacao abortada. A conta de destino nao existe no sistema.");
                }
                catch (Exception erro)
                {
                    Console.WriteLine("\nErro critico no motor de processamento: " + erro.Message);
                }
            }
        } 

        // HISTORICO: Insere as linhas de auditoria na tabela de transacoes
        static void RegistarHistorico(int origem, int destino, decimal valor, string estado, MySqlConnection conexao)
        {
            string queryHistorico = "INSERT INTO Transacoes (id_origem, id_destino, valor, estado) VALUES (@origem, @destino, @valor, @estado)";
            using (MySqlCommand cmd = new MySqlCommand(queryHistorico, conexao))
            {
                cmd.Parameters.AddWithValue("@origem", origem);
                cmd.Parameters.AddWithValue("@destino", destino);
                cmd.Parameters.AddWithValue("@valor", valor);
                cmd.Parameters.AddWithValue("@estado", estado);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
