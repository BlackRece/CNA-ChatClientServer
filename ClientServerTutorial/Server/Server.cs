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

        ConcurrentDictionary<int, Client> _clients;
        int _maxClients = 4;

        public Server(string ipAddress, int port) {
            _tcpListener = new TcpListener(IPAddress.Parse(ipAddress), port);
        }

        public void Start() {
            int clientIndex = 0;
            _clients = new ConcurrentDictionary<int, Client>();

            _tcpListener.Start();
            Console.WriteLine("Starting server...");

            while (true) {
                int index = clientIndex;
                clientIndex++;

                Client newClient = new Client(_tcpListener.AcceptSocket());
                Thread thread;

                if (_clients.Count >= _maxClients) {
                    Console.WriteLine("Too many clients!");

                    thread = new Thread(() => { RejectClient(newClient); });
                } else {
                    _clients.TryAdd(index, newClient);

                    //thread = new Thread(() => { ClientMethod(newClient); });
                    thread = new Thread(() => { ClientMethod(index); });
                }

                thread.Start();
            }
        }

        public void Stop() {
            _tcpListener.Stop();
        }

        void ClientMethod(int index) {
            Console.WriteLine("Socket opened. (" + _tcpListener.ToString() + ")");

            Packet receivedPacket;
            Client client = _clients[index];
            List<string> usernames = new List<string>();

            try {
                while ((receivedPacket = client.Read()) != null) {
                    Console.Write("Receieved: ");

                    // collect user names
                    foreach (KeyValuePair<int, Client> c in _clients)
                        usernames.Add(c.Value._name);

                    // act on packet type
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
                            // create message
                            namePacket._message = client._name + " changed name to " + namePacket._name;

                            // change name
                            client._name = namePacket._name;

                            // update client
                            client.Send(namePacket);

                            // notify all clients
                            RespondToAll(new ChatMessagePacket(namePacket._message), client);

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
                }
            } catch (Exception e) {
                Console.WriteLine("Error: " + e.Message);
                if (client != null)
                    Console.WriteLine("Client: " + client._name);
            }

            client.Close();
            _clients.TryRemove(index, out client);
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
            string msg = source._name + ": " + chatPacket._message;

            foreach(KeyValuePair<int, Client> pair in _clients) { 
                if(source != pair.Value) {
                    chatPacket._message = source._name + ": " + chatPacket._message;
                    pair.Value.Send(chatPacket);
                } else {
                    chatPacket._message = "me: " + chatPacket._message;
                    pair.Value.Send(chatPacket);
                }
            }
        }

        void RespondTo(PrivateMessagePacket privPacket, Client source) {
            foreach (KeyValuePair<int, Client> pair in _clients) {
                if (pair.Value._name == privPacket._target) {
                    privPacket._message = source._name + ": " + privPacket._message;
                    pair.Value.Send(privPacket);
                } 
            }
        }
    }
}
