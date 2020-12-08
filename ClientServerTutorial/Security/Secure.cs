using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    public class Secure
    {
        // lock objects
        private object _encrypt;
        private object _decrypt;

        RSACryptoServiceProvider _RSAProvider;
        private RSAParameters _privateKey;
        private RSAParameters _publicKey;
        public RSAParameters PublicKey { get { return _publicKey; } }
        public RSAParameters ExternalKey;

        public Secure() {
            _RSAProvider = new RSACryptoServiceProvider(2048);
            _publicKey = _RSAProvider.ExportParameters(false);
            _privateKey = _RSAProvider.ExportParameters(true);

            _encrypt = new object();
            _decrypt = new object();
        }

        public byte[] Decrypt(byte[] data) {
            lock (_decrypt) {
                _RSAProvider.ImportParameters(_privateKey);
                return _RSAProvider.Decrypt(data, true);
            }
        }

        public string DecryptString(byte[] message) {
            byte[] data = Decrypt(message);
            string result = Encoding.UTF8.GetString(data);
            return result;
        }
        public byte[] Encrypt(byte[] data) {
            lock (_encrypt) {
                try {
                    _RSAProvider.ImportParameters(ExternalKey);
                    return _RSAProvider.Encrypt(data, true);
                } catch (Exception e) {
                    Console.WriteLine("Encrypt Error: " + e.Message);
                    Console.WriteLine("ExternalKey: " + ExternalKey);
                    return null;
                }
            }
        }

        public byte[] EncryptString(string message) {
            byte[] data = Encoding.UTF8.GetBytes(message);
            return Encrypt(data);
        }
    }
}
