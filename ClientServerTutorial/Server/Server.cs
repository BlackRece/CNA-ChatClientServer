using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server {
    class Server {

        TcpListener _tcpListener;

        ConcurrentBag<Client> _clients;
        int _maxClients = 4;

        public Server(string ipAddress, int port) {
            _tcpListener = new TcpListener(IPAddress.Parse(ipAddress), port);
        }

        public void Start() {
            _clients = new ConcurrentBag<Client>();

            _tcpListener.Start();
            Console.WriteLine("Starting server...");

            while (true) {
                Client newClient = new Client(_tcpListener.AcceptSocket());
                Thread thread;

                if (_clients.Count >= _maxClients) {
                    Console.WriteLine("Too many clients!");

                    thread = new Thread(() => { RejectClient(newClient); });
                } else {
                    _clients.Add(newClient);

                    thread = new Thread(() => { ClientMethod(newClient); });
                }

                thread.Start();
            }
        }

        public void Stop() {
            _tcpListener.Stop();
        }

        void ClientMethod(Client client) {
            Console.WriteLine("Socket opened. (" + _tcpListener.ToString() + ")");

            string receivedMessage;

            client.Send("Connection Successful.");

            while ((receivedMessage = client.Read()) != null) {
                Console.WriteLine("Receieved: " + receivedMessage);

                if (receivedMessage.ToLower() == "end") break;

                //broadcast to all clients
                if(receivedMessage.StartsWith("@")) {
                    Client[] allClients = _clients.ToArray();
                    for(int i = 0; i < _clients.Count; i++) {
                        if(client != allClients[i]) {
                            client.Send(
                                client._port + " said:" +
                                receivedMessage.Substring(1)
                            );
                        }
                    }
                } else {
                string responseMessage = GetReturnMessage(receivedMessage);

                client.Send(responseMessage);
                }
            }

            client.Close();
            _clients.TryTake(out client);
        }

        void RejectClient(Client client) {
            Console.WriteLine("Socket to be refused: " + client._address + ":" + client._port);

            client.Send("ERROR\nToo many clients connected. Please try again.");

            client.Close();
        }

        string GetReturnMessage(string code) {
            switch (code) {
                case "hi": return "Hello.";
                case "nani": return "What!?";
                case "?": return "commands: hi, nani, ?, end";
                default: return "I have no response to that message.";
            }
        }
    }
}
