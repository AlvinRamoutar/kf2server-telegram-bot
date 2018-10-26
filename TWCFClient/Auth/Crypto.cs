using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace TWCFClient.Auth {
    class Crypto {


        /// <summary>
        /// Hashes provided data string using SHA256
        /// </summary>
        /// <param name="data">String to hash</param>
        /// <returns>Resultant hash</returns>
        public static string Hash(string data) {

            using (SHA256 sha256Hash = SHA256.Create()) {
                
                // Returns byte array (result of data hashed)
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));

                // Construct string object from byte array
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++) {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }

        }


        /// <summary>
        /// Serializes Users file using KeyManager (AESManaged)
        /// </summary>
        /// <param name="users">Users object</param>
        public static void EncryptalizeUsers(Users users) {

            using (FileStream fs = File.Open(Properties.Settings.Default.UsersRelFilePath, FileMode.Create)) {

                using (CryptoStream cs = new CryptoStream(fs, KeyManager.Instance.AES.CreateEncryptor(), CryptoStreamMode.Write)) {

                    XmlSerializer xmlser = new XmlSerializer(typeof(Users));
                    xmlser.Serialize(cs, users);
                }
            }
        }


        /// <summary>
        /// Deserializes Users file using KeyManager (AESManaged)
        /// </summary>
        /// <returns>Users object</returns>
        public static Users DecryptalizeUsers() {
            try {
                using (FileStream fs = File.Open(Properties.Settings.Default.UsersRelFilePath, FileMode.Open)) {

                    using (CryptoStream cs = new CryptoStream(fs, KeyManager.Instance.AES.CreateDecryptor(), CryptoStreamMode.Read)) {

                        XmlSerializer xmlser = new XmlSerializer(typeof(Users));
                        return (Users)xmlser.Deserialize(cs);
                    }
                }
            } catch(FileNotFoundException) {
                return new Auth.Users();
            }
        }




    }

    /// <summary>
    /// Singleton holding AesManaged object.
    /// Reads/Writes key files to disk.
    /// </summary>
    class KeyManager {

        /// <summary>
        /// Singleton design pattern logic.
        /// </summary>
        #region Singleton Pattern
        private static KeyManager instance = null;
        private static readonly object padlock = new object();

        public static KeyManager Instance {
            get {
                lock (padlock) {
                    if (instance == null) {
                        instance = new KeyManager();
                    }
                    return instance;
                }
            }
        }
        #endregion

        public AesManaged AES { get; set; }

        /// <summary>
        /// Constructs a new KeyManager object whose sole purpose is to populate AesManaged object.
        /// </summary>
        KeyManager() {

            AES = new AesManaged();

            // If both key and IV files exist in app dir
            if (File.Exists("Key.dll") && File.Exists("IV.dll")) {

                // Read bytes from key file into AesManaged Key property
                using (FileStream fs = File.Open("Key.dll", FileMode.Open)) {
                    byte[] tmpKey = new byte[32];
                    fs.Read(tmpKey, 0, 32);
                    AES.Key = tmpKey;
                    
                }

                // Read bytes from IV file into AesManaged IV property
                using (FileStream fs = File.Open("IV.dll", FileMode.Open)) {
                    fs.Read(AES.IV, 0, 16);
                }

            } else { // Otherwise, create these files

                // Writes bytes in AesManaged Key property to file
                using (FileStream fs = File.Open("Key.dll", FileMode.Create)) {
                    fs.Write(AES.Key, 0, 32);
                }

                // Writes bytes in AesManaged IV property to file
                using (FileStream fs = File.Open("IV.dll", FileMode.Create)) {
                    Console.WriteLine(AES.IV.Length);
                    fs.Write(AES.IV, 0, 16);
                }

            }

            AES.Padding = PaddingMode.PKCS7;
        }


    }
}
