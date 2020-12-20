using Packets;
using Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace CNA_Client {
    class NetworkManager {
        private const bool DEBUG = true;

        IPEndPoint _tcpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        IPEndPoint _udpEndPoint = new IPEndPoint(IPAddress.Any, 0);

        UdpClient _udpClient;
        TcpClient _tcpClient;

        NetworkStream _stream;
        BinaryFormatter _formatter;

        BinaryReader _reader;
        BinaryWriter _writer;

        private Secure _crypt;

        public bool IsTcpConnected { get { return _tcpClient.Connected; } }
        public bool IsUdpConnected { get { return _udpClient.Client.Connected; } }

        public NetworkManager(ref string name) {
            _crypt = new Secure();
            name = _crypt.PublicKey.GetHashCode().ToString();
        }

        ~NetworkManager() {
            Close();
        }

        public  void Close() {
            // close local connections
            _tcpClient.Close();
            _udpClient.Close();
        }

        #region TCP related code

        public bool TcpConnect(string ipAddress, int port) {
            try {
                _tcpClient = new TcpClient(ipAddress, port);
                _udpClient = new UdpClient(ipAddress, port);

                _stream = _tcpClient.GetStream();
                _formatter = new BinaryFormatter();

                _reader = new BinaryReader(_stream);
                _writer = new BinaryWriter(_stream);

                return true;
            } catch (Exception e) {
                Console.WriteLine("NetworkManager Exception: " + e.Message);
                return false;
            }
        }

        public void TcpBeginLogin(string src) {
            Console.WriteLine("Logging in to server...");

            _tcpEndPoint = (IPEndPoint)_tcpClient.Client.LocalEndPoint;

            LoginPacket logPacket =
                //new LoginPacket((IPEndPoint)_tcpClient.Client.LocalEndPoint) { 
                new LoginPacket(_tcpEndPoint) {
                    _packetSrc = src, _clientKey = _crypt.PublicKey
                };
            
            TcpSendPacket(logPacket);
        }

        public void TcpFinishLogin(LoginPacket logPacket) {
            _crypt.ExternalKey = logPacket._serverKey;
        }

        public Packet TcpReadPacket() {
            int numberOfBytes;
            Packet packet = null;

            try {
                // check reader and store return val
                if ((numberOfBytes = _reader.ReadInt32()) > 0) {
                    packet = Serialiser.Deserialise(_reader.ReadBytes(numberOfBytes));
                }
            } catch (System.Threading.ThreadAbortException) {
                // expected exception since app is being closed
            } catch (Exception e) {
                Console.WriteLine("TcpReadPacket Error: " + e.Message);
            }

            return packet;
        }

        public bool TcpSendPacket(Packet packet) {
            bool result = false;
            if (_writer.BaseStream.CanWrite) {
                byte[] buffer = Serialiser.Serialise(packet);
                
                _writer.Write(buffer.Length);
                _writer.Write(buffer);
                _writer.Flush();

                result = true;
            }
            return result;
        }

        public bool TcpSendSecurePacket(string message, string src) {
            SecurePacket securePacket = new SecurePacket(_crypt.EncryptString(message)) {
                _packetSrc = src
            };

            return TcpSendPacket(securePacket);
        }

        #endregion

        #region UDP related methods

        public Packet UdpReadPacket() {
            Packet result = null;
            try { 
                result = Serialiser.Deserialise(_udpClient.Receive(ref _udpEndPoint));
            } catch (System.Threading.ThreadAbortException) {
                // expected exception since app is being closed
            } catch (Exception e) {
                Debug("UdpReadPacket Error: " + e.Message);
            }
            return result;
        }

        public bool UdpSendPacket(Packet packet) {
            byte[] buffer = Serialiser.Serialise(packet);

            int result = _udpClient.Send(buffer, buffer.Length);

            return result > 0;
        }

        public bool UdpSendSecurePacket(string message, string src) {
            SecurePacket securePacket = new SecurePacket(_crypt.EncryptString(message)) {
                _packetSrc = src
            };

            return UdpSendPacket(securePacket);
        }

        #endregion

        #region RSA Encryption and Decryption Methods

        public string DecryptString(byte[] message) {
            byte[] data = _crypt.Decrypt(message);
            string result = Encoding.UTF8.GetString(data);
            return result;
        }

        public byte[] EncryptString(string message) {
            byte[] data = Encoding.UTF8.GetBytes(message);
            return _crypt.Encrypt(data);
        }

        #endregion
        private void Debug(string msg) {
            if (DEBUG) Console.WriteLine("Client - " + msg);
        }
    }
}
