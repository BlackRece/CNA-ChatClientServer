using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client {
    class Client {
        TcpClient _tcpClient;

        NetworkStream _stream;

        StreamWriter _writer;
        StreamReader _reader;

        public Client() {
            _tcpClient = new TcpClient();
        }

        public bool Connect(string ipAddress, int port) {
            try {
                _tcpClient.Connect(ipAddress, port);

                _stream = _tcpClient.GetStream();
                _reader = new StreamReader(_stream, Encoding.UTF8);
                _writer = new StreamWriter(_stream, Encoding.UTF8);

                return true;

            } catch (Exception e) {
                Console.WriteLine("Exception: " + e.Message);
                return false;
            }
        }

        public void Run() {
            string userInput = default(string);

            if (ProcessServerResponse()) {

                while ((userInput = Console.ReadLine()) != null) {
                    _writer.WriteLine(userInput);
                    _writer.Flush();

                    if (userInput.ToLower() == "end") break;

                    if (!ProcessServerResponse()) break;
                }
            }

            _tcpClient.Close();
        }

        bool ProcessServerResponse() {
            bool result = true;
            string serverMessage = _reader.ReadLine();

            if (serverMessage == "ERROR") {
                serverMessage = _reader.ReadLine();
                result = false;
            }

            Console.WriteLine("Server says: " + serverMessage);
            Console.WriteLine();

            if (!result) _ = Console.ReadLine();

            return result;
        }
    }
}
