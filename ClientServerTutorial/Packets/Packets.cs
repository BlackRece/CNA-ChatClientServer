using System;
using System.Net;

namespace Packets {
    [Serializable]
    public class Packet {
        public enum PacketType {
            ERROR = -1,
            EMPTY,
            CHATMESSAGE,
            PRIVATEMESSAGE,
            SERVERMESSAGE,
            CLIENTNAME,
            LOGIN,
            USERLIST
        }

        public PacketType _packetType { get; protected set; }
        public string _packetSrc { get; set; }
    }

    [Serializable]
    public class ChatMessagePacket : Packet {
        public string _message;

        public ChatMessagePacket(string message) {
            _message = message;

            _packetType = PacketType.CHATMESSAGE;
        }
    }

    [Serializable]
    public class PrivateMessagePacket : Packet {
        public string _message;
        public string _target;

        public PrivateMessagePacket(string target, string message) {
            _message = message;
            _target = target;

            _packetType = PacketType.PRIVATEMESSAGE;
        }
    }

    [Serializable]
    public class ClientNamePacket : Packet {
        public string _name;
        public string _message;

        public ClientNamePacket(string name) {
            _name = name;

            _packetType = PacketType.CLIENTNAME;
        }
    }

    [Serializable]
    public class ErrorMessagePacket : Packet {
        public string _message;

        public ErrorMessagePacket(string message) {
            _message = message;

            _packetType = PacketType.ERROR;
        }
    }

    [Serializable]
    public class ServerMessagePacket : Packet {
        public string _messageSent;
        public string _messageRecv;

        public ServerMessagePacket(string messageSent, string messageReceived) {
            _messageSent = messageSent;
            _messageRecv = messageReceived;

            _packetType = PacketType.SERVERMESSAGE;
        }
    }

    [Serializable]
    public class UserListPacket : Packet {
        public string[] _users;

        public UserListPacket(string[] users) {
            _users = users;
        }
    }

    [Serializable]
    public class LoginPacket : Packet {
        public IPEndPoint _endPoint;

        public LoginPacket(IPEndPoint endPoint) {
            _endPoint = endPoint;

            _packetType = PacketType.LOGIN;
        }
    }
}
