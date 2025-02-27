﻿using Packets;
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
using System.Threading.Tasks;

namespace CNA_Server {
    public class Client {
        Socket _socket;
        public string _name;

        public IPEndPoint _endPoint;
        public string _address {
            get { return _endPoint.Address.ToString(); }
        }
        public int _port {
            get { return _endPoint.Port; }
        }

        NetworkStream _stream;

        BinaryReader _reader;
        BinaryWriter _writer;

        object _readLock;
        object _writeLock;

        private Secure _crypt;
        
        public Client(Socket socket) {
            _crypt = new Secure();

            _readLock = new object();
            _writeLock = new object();

            _socket = socket;

            _stream = new NetworkStream(_socket);

            _reader = new BinaryReader(_stream);
            _writer = new BinaryWriter(_stream);
        }

        ~Client() {
            Close();
        }

        public void Close() {
            _stream.Close();
            if(_reader != null) _reader.Close();
            if(_writer != null) _writer.Close();
            _socket.Close();

            _reader = null;
            _writer = null;
        }

        public Packet TcpRead() {
            lock (_readLock) {
                int numberOfBytes;
                Packet packet = null;

                if (_reader == null)
                    return packet;

                // check reader and store returned val
                if ((numberOfBytes = _reader.ReadInt32()) != -1) {
                    packet = Serialiser.Deserialise(_reader.ReadBytes(numberOfBytes));
                }

                return packet;
            }
        }

        public void TcpSend(Packet packet) {
            lock(_writeLock) {
                byte[] buffer = Serialiser.Serialise(packet);

                //4 write length
                _writer.Write(buffer.Length);

                //5 write buffer
                _writer.Write(buffer);

                //6 flush
                _writer.Flush();
            }
        }

        public void UdpSend(Packet packet, ref UdpClient udpClient) {
            byte[] buffer = Serialiser.Serialise(packet);

            udpClient.Send(buffer, buffer.Length, _endPoint);
        }

        public RSAParameters UpdateRSA(LoginPacket logPacket) {
            _crypt.ExternalKey = logPacket._clientKey;
            return _crypt.PublicKey;
        }

        public string GetSecureMessage(SecurePacket safePacket) {
            return _crypt.DecryptString(safePacket._data);
        }

        public byte[] SetSecureMessage(string message) {
            return _crypt.EncryptString(message);
        }
    }
}
