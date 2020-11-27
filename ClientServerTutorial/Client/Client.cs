using Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client {
    public class Client {
        struct WinClient {
            bool IsWPF { get; }
            private bool _isWPF;

            private string _name;
            public string name {
                get { return _name; }
                set {
                    if (value.Length > 0) {
                        _name = value;
                        if (_isWPF)
                            _wpf.UpdateNickName(value);
                        else
                            _win.UpdateNickName(value);
                    }
                }
            }

            private Client_WinForm _win;
            private Client_WPFForm _wpf;

            public void NewWin(string choice, Client client) {
                if (choice == "1") {
                    _isWPF = false;
                    _win = new Client_WinForm(client);
                    _win.ShowDialog();
                } else {
                    _isWPF = true;
                    _wpf = new Client_WPFForm(client);
                    _wpf.ShowDialog();
                }
            }

            public void UpdateChat(string message) {
                if (_isWPF)
                    _wpf.UpdateChatWindow(message);
                else
                    _win.UpdateChatWindow(message);
            }

        }
        
        TcpClient _tcpClient;
        UdpClient _udpClient;

        NetworkStream _stream;
        BinaryFormatter _formatter;

        BinaryReader _reader;
        BinaryWriter _writer;

        WinClient _win;

        public string _nick;


        public void Close() {
            _tcpClient.Close();
            _udpClient.Close();
        }

        public void Login() {
            UdpSendPacket(new LoginPacket((IPEndPoint)_udpClient.Client.LocalEndPoint));
        }

        public Packet ReadPacket() {
            int numberOfBytes;
            Packet packet = new Packet();

            // check reader and store return val
            if ((numberOfBytes = _reader.ReadInt32()) > 0) {
                byte[] buffer = _reader.ReadBytes(numberOfBytes);
                MemoryStream memStream = new MemoryStream(buffer);
                packet = _formatter.Deserialize(memStream) as Packet;
            }

            return packet;
        }
               
        public void Run() {
            _win = new WinClient();

            //Thread thread = new Thread(() => ProcessServerResponse());
            Thread tcpThread = new Thread(() => TcpProcessServerPacket());
            tcpThread.Start();

            Thread udpThread = new Thread(() => UdpProcessServerPacket());
            udpThread.Start();

            Login();

            Console.WriteLine(
                "Select client type:\n" +
                "1:\tWinForm Client\n" +
                "2:\tWPF Client\n"
            );

            string choice = string.Empty;
            while (choice != "1" && choice != "2")
                choice = Console.ReadLine();

            _win.NewWin(choice, this);

            Close();
        }

        public bool TcpConnect(string ipAddress, int port) {
            try {
                _tcpClient = new TcpClient(ipAddress, port);
                _udpClient = new UdpClient(ipAddress, port);

                _nick = port.ToString();

                _stream = _tcpClient.GetStream();
                _formatter = new BinaryFormatter();

                _reader = new BinaryReader(_stream);
                _writer = new BinaryWriter(_stream);

                return true;
            } catch (Exception e) {
                Console.WriteLine("Exception: " + e.Message);
                return false;
            }
        }

        private void TcpProcessServerPacket() {
            Packet packet = new Packet();

            while (_tcpClient.Connected) {
                if(_reader != null) {
                //if(_reader.BaseStream.CanSeek) { 
                    packet = ReadPacket();

                    switch (packet._packetType) {
                        case Packet.PacketType.EMPTY:
                            /* do nothing */
                            break;
                        case Packet.PacketType.CLIENTNAME:
                            ClientNamePacket namePacket = (ClientNamePacket)packet;

                            _win.name = namePacket._name;
                            break;
                        case Packet.PacketType.CHATMESSAGE:
                            ChatMessagePacket chatPacket = (ChatMessagePacket)packet;
                            _win.UpdateChat(chatPacket._message);

                            break;
                        case Packet.PacketType.PRIVATEMESSAGE:
                            PrivateMessagePacket privPacket = (PrivateMessagePacket)packet;
                            _win.UpdateChat(privPacket._packetSrc + ": " + privPacket._message);

                            break;
                    }
                }
            }
        }

        public bool TcpSendPacket(Packet packet) {
            bool result = false;
            //if (_writer.BaseStream.CanSeek) {
            if (_writer != null) {
                //1 create obj
                MemoryStream memStream = new MemoryStream();

                //2 serialise packet and store in memoryStream
                _formatter.Serialize(memStream, packet);

                //3 get byte array
                byte[] buffer = memStream.GetBuffer();

                //4 write length
                _writer.Write(buffer.Length);

                //5 write buffer
                _writer.Write(buffer);

                //6 flush
                _writer.Flush();

                result = true;
            }
            return result;
        }

        private void UdpProcessServerPacket() {
            try {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);

                while(_udpClient.Client.Connected) {
                    byte[] bytes = _udpClient.Receive(ref endPoint);
                    Packet packet = UdpReadPacket(bytes);

                    switch (packet._packetType) {
                        case Packet.PacketType.ERROR:
                            break;
                        case Packet.PacketType.EMPTY:
                            break;
                        case Packet.PacketType.CHATMESSAGE:
                            break;
                        case Packet.PacketType.PRIVATEMESSAGE:
                            break;
                        case Packet.PacketType.SERVERMESSAGE:
                            break;
                        case Packet.PacketType.CLIENTNAME:
                            break;
                        case Packet.PacketType.LOGIN:
                            LoginPacket logPacket = (LoginPacket)packet;

                            _win.UpdateChat(
                                "Logged in on IP: " + logPacket._endPoint.Address.ToString() +
                                " on Port: " + logPacket._endPoint.Port.ToString()
                            );
                            break;
                        case Packet.PacketType.USERLIST:
                            break;
                    }
                }
            } catch (SocketException e) {
                Console.WriteLine("Client UDP Read Method Exception: " + e.Message);
            }
        }

        public Packet UdpReadPacket(byte[] buffer) {
            
            MemoryStream memStream = new MemoryStream(buffer);
            return _formatter.Deserialize(memStream) as Packet;
        }


        public void UdpSendPacket(Packet packet) {
            //1 create obj
            MemoryStream memStream = new MemoryStream();

            //2 serialise packet and store in memoryStream
            _formatter.Serialize(memStream, packet);

            //3 get byte array
            byte[] buffer = memStream.GetBuffer();

            //4 send packet
            _udpClient.Send(buffer, buffer.Length);
        }
    }
}
