using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;

/// <summary>
/// KF2 Telegram Bot
/// An experiment in automating KF2 server webmin actions with Selenium, triggered via Telegram's Bot API
/// Copyright (c) 2018-2019 Alvin Ramoutar https://alvinr.ca/ 
/// </summary>
namespace kf2server_tbot.Security {

    /// <summary>
    /// Handles encryption/decryption of XML Users data object
    /// </summary>
    class Crypto {

        /// <summary>
        /// Serializes and encrypts Users file using KeyManager (AESManaged), and writes to file.
        /// </summary>
        /// <param name="users">Users object</param>
        public static void EncryptalizeUsers(Users users) {

            using (FileStream fs = File.Open(Properties.Settings.Default.UsersRelFilePath, FileMode.Create)) {

                using (CryptoStream cs = new CryptoStream(fs, KeyManager.Instance.AES.CreateEncryptor(), CryptoStreamMode.Write)) {

                    /// Writes encrypted, serialized Users object to file
                    XmlSerializer xmlser = new XmlSerializer(typeof(Users));
                    xmlser.Serialize(cs, users);
                }
            }
        }


        /// <summary>
        /// Decrypts and Deserializes Users file using KeyManager (AESManaged) from file.
        /// </summary>
        /// <returns>Users object</returns>
        public static Users DecryptalizeUsers() {
            try {
                using (FileStream fs = File.Open(Properties.Settings.Default.UsersRelFilePath, FileMode.Open)) {

                    using (CryptoStream cs = new CryptoStream(fs, KeyManager.Instance.AES.CreateDecryptor(), CryptoStreamMode.Read)) {

                        XmlSerializer xmlser = new XmlSerializer(typeof(Users));

                        Users tmpUsers = (Users)xmlser.Deserialize(cs);

                        return tmpUsers ?? new Users();
                    }
                }
            } catch(FileNotFoundException) {

                Users tmpUsers = new Users();
                tmpUsers.Accounts = new System.Collections.Generic.List<Account>();
                return tmpUsers;
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

            /// If both key and IV files exist in app dir
            if (File.Exists("Key.dll") && File.Exists("IV.dll")) {

                /// Read bytes from key file into AesManaged Key property
                using (FileStream fs = File.Open("Key.dll", FileMode.Open)) {
                    byte[] tmpKey = new byte[32];
                    fs.Read(tmpKey, 0, 32);
                    AES.Key = tmpKey;
                }

                /// Read bytes from IV file into AesManaged IV property
                using (FileStream fs = File.Open("IV.dll", FileMode.Open)) {
                    byte[] tmpIV = new byte[16];
                    fs.Read(tmpIV, 0, 16);
                    AES.IV = tmpIV;
                }

            } else { /// Otherwise, create these files

                /// Writes bytes in AesManaged Key property to file
                using (FileStream fs = File.Open("Key.dll", FileMode.Create)) {
                    fs.Write(AES.Key, 0, 32);
                }

                /// Writes bytes in AesManaged IV property to file
                using (FileStream fs = File.Open("IV.dll", FileMode.Create)) {
                    fs.Write(AES.IV, 0, 16);
                }

            }

            AES.Padding = PaddingMode.PKCS7;
        }


    }
}
