using Packets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server {
    class Server {
        TcpListener _tcpListener;
        UdpClient _udpListener;

        ConcurrentDictionary<int, Client> _clients;
        int _maxClients = 4;

        public Server(string ipAddress, int port) {
            _tcpListener = new TcpListener(IPAddress.Parse(ipAddress), port);
            _udpListener = new UdpClient(port);
            _udpListener.Connect(IPAddress.Parse(ipAddress), port);
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
                Thread tcpThread = null, udpThread = null;

                if (_clients.Count >= _maxClients) {
                    Console.WriteLine("Too many clients!");

                    //tcpThread = new Thread(() => { TcpRejectClient(newClient); });
                    udpThread = new Thread(() => { UdpRejectClient(newClient); });
                } else {
                    newClient._endPoint = (IPEndPoint)_udpListener.Client.RemoteEndPoint;
                    _clients.TryAdd(index, newClient);

                    //thread = new Thread(() => { ClientMethod(newClient); });
                    //tcpThread = new Thread(() => { TcpClientMethod(index); });
                    udpThread = new Thread(() => { UdpListen(); });
                }

                if(tcpThread != null) tcpThread.Start();
                if(udpThread != null) udpThread.Start();
            }
        }

        public void Stop() {
            _tcpListener.Stop();
        }

        void TcpClientMethod(int index) {
            Console.WriteLine("Socket opened. (" + _tcpListener.ToString() + ")");

            Packet receivedPacket;
            Client client = _clients[index];
            List<string> usernames = new List<string>();

            try {
                while ((receivedPacket = client.Read()) != null) {
                    Console.Write("TCP Receieved: ");

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

                        case Packet.PacketType.LOGIN:
                            LoginPacket loginPacket = (LoginPacket)receivedPacket;
                            client._endPoint = loginPacket._endPoint;

                            RespondToAll(new ChatMessagePacket("User [" + client._name + "] just logged in."), client);
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

        void TcpRejectClient(Client client) {
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

        Packet UdpGetPacket(byte[] buffer) {
            BinaryFormatter _formatter = new BinaryFormatter();
            MemoryStream memStream = new MemoryStream(buffer);
            return _formatter.Deserialize(memStream) as Packet;
        }

        void UdpListen() {
            try {
                IPEndPoint endPoint = (IPEndPoint)_udpListener.Client.LocalEndPoint;

                while (true) {
                    byte[] buffer = _udpListener.Receive(ref endPoint);
                    Packet packet = UdpGetPacket(buffer);

                    foreach (Client c in _clients.Values) {
                        if (endPoint.ToString() == c._endPoint.ToString()) {
                            Console.Write("UDP Receieved: ");

                            switch (packet._packetType) {
                                case Packet.PacketType.EMPTY:
                                    /* do nothing */
                                    break;
                                case Packet.PacketType.CHATMESSAGE:
                                    UdpSendToAll((ChatMessagePacket)packet);
                                    break;
                                case Packet.PacketType.PRIVATEMESSAGE:
                                    break;
                                case Packet.PacketType.SERVERMESSAGE:
                                    break;
                                case Packet.PacketType.CLIENTNAME:
                                    break;
                                case Packet.PacketType.LOGIN:
                                    LoginPacket logPacket = (LoginPacket)packet;

                                    Console.WriteLine(
                                        "Connection received. ID: " + c._name +
                                        "\nIP: " + logPacket._endPoint.Address.ToString() +
                                        "\nPort: " + logPacket._endPoint.Port.ToString()
                                    );
                                    UdpSendToAll(new ChatMessagePacket(
                                        "Client [" + c._name + "] has joined."
                                    ));
                                    break;
                                case Packet.PacketType.USERLIST:
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            } catch (SocketException e) {
                Console.WriteLine("Server UDP Read Method Exception: " + e.Message);
            }
        }

        void UdpRejectClient(Client client) {
            Console.WriteLine("Socket to be refused: " + client._address + ":" + client._port);

            UdpSendPacket(
                new ErrorMessagePacket("ERROR\nToo many clients connected. Please try again."),
                (IPEndPoint)_udpListener.Client.RemoteEndPoint
            );


            //client.Send(new ErrorMessagePacket("ERROR\nToo many clients connected. Please try again."));

            client.Close();
        }

        void UdpSendToAll(Packet packet) {
            foreach (Client c in _clients.Values) {
                UdpSendPacket(packet, c._endPoint);
            }
        }

        void UdpSendPacket(Packet packet, IPEndPoint endPoint) {
            MemoryStream memStream = new MemoryStream();
            
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memStream, packet);
            
            byte[] buffer = memStream.GetBuffer();

            _udpListener.Send(buffer, buffer.Length, endPoint);
        }
    }
}
