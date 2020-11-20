using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    class Client {
        Socket _socket;
        IPEndPoint _ipEndPoint;
        public string _address {
            get { return _ipEndPoint.Address.ToString(); }
        }
        public int _port {
            get { return _ipEndPoint.Port; }
        }

        NetworkStream _stream;
        StreamReader _reader;
        StreamWriter _writer;

        object _readLock;
        object _writeLock;

        public Client(Socket socket) {
            _readLock = new object();
            _writeLock = new object();

            _socket = socket;

            _stream = new NetworkStream(_socket);
            _reader = new StreamReader(_stream, Encoding.UTF8);
            _writer = new StreamWriter(_stream, Encoding.UTF8);

            _ipEndPoint = _socket.RemoteEndPoint as IPEndPoint;
        }

        public void Close() {
            _stream.Close();
            _reader.Close();
            _writer.Close();
            _socket.Close();
        }

        public string Read() {
            lock (_readLock) {
                return _reader.ReadLine();
            }
        }

        public void Send(string message) {
            lock(_writeLock) {
                _writer.WriteLine(message);
                _writer.Flush();
            }
        }
    }
}
