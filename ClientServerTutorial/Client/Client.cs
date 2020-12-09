using Packets;
using Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
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

            public string[] UserList {
                set {
                    if (value.Length < 1)
                        return;

                    if (_isWPF)
                        _wpf.UpdateUserList(value);
                    else
                        _win.UpdateUserList(value);
                }
            }

            public bool IsReady {
                get {
                    bool result = false;
                    if (_isWPF) {
                        try {
                            result = _wpf.IsInitialized;
                        } catch { }
                    } else {
                        try {
                            result = _win.Visible;
                        } catch { }
                    }
                    return result;
                }
            }

            private Client_WinForm _win;
            private Client_WPFForm _wpf;

            private Game_WinForm _winGame;
            public Game_WinForm gameForm {
                get { return _winGame; }
                set { _winGame = value; }
            }

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

        private Secure _crypt;

        public Client() {
            _crypt = new Secure();
        }

        public void Close() {
            // close local connections
            _tcpClient.Close();
            _udpClient.Close();
        }

        public Packet TcpReadPacket() {
            int numberOfBytes;
            Packet packet = new Packet();

            try {
                // check reader and store return val
                if ((numberOfBytes = _reader.ReadInt32()) > 0) {
                    byte[] buffer = _reader.ReadBytes(numberOfBytes);
                    MemoryStream memStream = new MemoryStream(buffer);
                    packet = _formatter.Deserialize(memStream) as Packet;
                }
            } catch (Exception e) {
                Console.WriteLine("Error: " + e.Message);
            }

            return packet;
        }
               
        public void Run() {
            // create window container
            _win = new WinClient();

            // select window type
            Console.WriteLine(
                "Select client type:\n" +
                "1:\tWinForm Client\n" +
                "2:\tWPF Client\n"
            );

            string choice = string.Empty;
            while (choice != "1" && choice != "2")
                choice = Console.ReadLine();

            // start network worker threads
            Thread tcpThread = new Thread(() => TcpProcessServerPacket());
            tcpThread.Start();

            Thread udpThread = new Thread(() => UdpProcessServerPacket());
            udpThread.Start();

            // login to server
            TcpLogin();

            // launch selected window
            _win.NewWin(choice, this);
            // this blocks the thread until window is closed

            // clean up connections before exit
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

        public void TcpLogin() {
            Console.WriteLine("Logging in to server...");

            LoginPacket logPacket = new LoginPacket((IPEndPoint)_udpClient.Client.LocalEndPoint);
            logPacket._clientKey = _crypt.PublicKey;
            _nick = _crypt.PublicKey.ToString().Substring(0, 4);
            TcpSendPacket(logPacket);
        }

        private void TcpProcessServerPacket() {
            Packet packet = new Packet();

            while (_tcpClient.Connected) {
                if(_reader != null) {
                    packet = TcpReadPacket();

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
                        case Packet.PacketType.LOGIN:
                            Console.WriteLine("Logged in to server.");

                            LoginPacket loginPacket = (LoginPacket)packet;
                            _crypt.ExternalKey = loginPacket._serverKey;

                            break;
                        case Packet.PacketType.SECUREMESSAGE:
                            SecurePacket safePacket = (SecurePacket)packet;
                            _win.UpdateChat(
                                "Secure Message Received [" + safePacket._author + "]: " +
                                _crypt.DecryptString(safePacket._data)
                                );

                            break;
                        case Packet.PacketType.ENDSESSION:
                            Console.WriteLine("Disconnecting from server.");

                            _tcpClient.Close();
                            break;
                        case Packet.PacketType.JOINGAME:
                            _win.gameForm = new Game_WinForm(this);
                            _win.gameForm.ShowDialog();
                            break;
                        case Packet.PacketType.USERLIST:
                            UserListPacket userList = (UserListPacket)packet;
                            _win.UserList = userList._users;
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

        public bool TcpSendSecurePacket(string message) {
            SecurePacket securePacket = new SecurePacket(_nick, _crypt.EncryptString(message));

            return TcpSendPacket(securePacket);
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
                            ChatMessagePacket chatPacket = (ChatMessagePacket)packet;
                            _win.UpdateChat(chatPacket._message);
                            break;
                        case Packet.PacketType.PRIVATEMESSAGE:
                            PrivateMessagePacket privPacket = (PrivateMessagePacket)packet;
                            _win.UpdateChat(privPacket._packetSrc + ": " + privPacket._message);
                            break;
                        case Packet.PacketType.SERVERMESSAGE:
                            break;
                        case Packet.PacketType.CLIENTNAME:
                            ClientNamePacket namePacket = (ClientNamePacket)packet;

                            _win.name = namePacket._name;
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
                        case Packet.PacketType.ENDSESSION:
                            Console.WriteLine("Disconnecting from server.");

                            _udpClient.Close();
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


        public bool UdpSendPacket(Packet packet) {
            //1 create obj
            MemoryStream memStream = new MemoryStream();

            //2 serialise packet and store in memoryStream
            _formatter.Serialize(memStream, packet);

            //3 get byte array
            byte[] buffer = memStream.GetBuffer();

            //4 send packet
            int result = _udpClient.Send(buffer, buffer.Length);

            return result > 0;
        }
    }
}
