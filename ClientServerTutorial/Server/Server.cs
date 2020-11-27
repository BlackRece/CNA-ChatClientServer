using Packets;
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

            Packet receivedPacket;

            Client[] allClients;

            //client.Send("Connection Successful.");

            while ((receivedPacket = client.Read()) != null) {
                Console.Write("Receieved: ");

                allClients = _clients.ToArray();

                switch (receivedPacket._packetType) {
                    case Packet.PacketType.EMPTY:
                        Console.WriteLine("EMPTY");

                        /* do nothing */
                        break;
                    case Packet.PacketType.CHATMESSAGE:
                        Console.WriteLine("CHAT");

                        RespondToAll((ChatMessagePacket)receivedPacket, client);

                        break;
                    case Packet.PacketType.PRIVATEMESSAGE:
                        Console.WriteLine("PRIVATE");

                        RespondTo((PrivateMessagePacket)receivedPacket, client);

                        break;
                    case Packet.PacketType.SERVERMESSAGE:
                        Console.WriteLine("SERVER");

                        ServerMessagePacket servPacket = (ServerMessagePacket)receivedPacket;
                        servPacket._messageRecv = GetReturnMessage(servPacket._messageSent);
                        client.Send(servPacket);

                        break;
                    case Packet.PacketType.CLIENTNAME:
                        Console.WriteLine("NAME");

                        ClientNamePacket namePacket = (ClientNamePacket)receivedPacket;
                        client._name = namePacket._name;
                        namePacket._message = "Name changed.";
                        client.Send(namePacket);

                        break;
                    case Packet.PacketType.ERROR:
                        //should be for other things, in another place
                        Console.WriteLine("ERROR");

                        ErrorMessagePacket errPacket = (ErrorMessagePacket)receivedPacket;
                        client.Send(errPacket);
                        break;
                    default:
                        break;
                }

                //if (receivedMessage.ToLower() == "end") break;

                //broadcast to all clients
                /*
                if (receivedPacket.StartsWith("@")) {
                    Client[] allClients = _clients.ToArray();
                    for (int i = 0; i < _clients.Count; i++) {
                        if (client != allClients[i]) {
                            client.Send(
                                client._port + " said:" +
                                receivedPacket.Substring(1)
                            );
                        }
                    }
                } else {
                    string responseMessage = GetReturnMessage(receivedPacket);

                    client.Send(responseMessage);
                }
                */
            }

            client.Close();
            _clients.TryTake(out client);
        }

        void RejectClient(Client client) {
            Console.WriteLine("Socket to be refused: " + client._address + ":" + client._port);

            client.Send(new ErrorMessagePacket("ERROR\nToo many clients connected. Please try again."));

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

        void RespondToAll(ChatMessagePacket chatPacket, Client source) {
            Client[] allClients = _clients.ToArray();

            for (int i = 0; i < _clients.Count; i++)
                //if (source != allClients[i])
                    allClients[i].Send(chatPacket);

        }

        void RespondTo(PrivateMessagePacket privPacket, Client source) {
            Client[] allClients = _clients.ToArray();

            for (int i = 0; i < _clients.Count; i++) {
                if (allClients[i]._port.ToString() == privPacket._target) {
                    allClients[i].Send(privPacket);
                }
            }
        }
    }
}
