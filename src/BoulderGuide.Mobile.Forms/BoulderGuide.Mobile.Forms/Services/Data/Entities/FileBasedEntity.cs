using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Services.Data {
   public abstract class FileBasedEntity : INotifyPropertyChanged {
      protected string relativePath;
      private readonly bool isPrivateUseKey;
      private static Tuple<byte[], byte[], byte[]> key;

      public event PropertyChangedEventHandler PropertyChanged;
      protected IDownloadService DownloadService { get; }

      protected FileBasedEntity(
         string relativePath,
         string remoteRootPath,
         string localRootPath,
         bool isPrivateUseKey) {

         DownloadService = (IDownloadService) Prism.PrismApplicationBase.Current.Container.CurrentScope.Resolve(typeof(IDownloadService));
         this.relativePath = relativePath ?? throw new ArgumentNullException(nameof(relativePath));
         RemoteRootPath = remoteRootPath ?? throw new ArgumentNullException(nameof(remoteRootPath));
         LocalRootPath = localRootPath ?? throw new ArgumentNullException(nameof(localRootPath));
         this.isPrivateUseKey = isPrivateUseKey;

         var dir = Directory.GetParent(LocalPath);
         if (!dir.Exists) {
            dir.Create();
         }

         if (isPrivateUseKey && key is null) {
            var preferences = (Preferences.IPreferences) Prism.PrismApplicationBase.Current.Container.CurrentScope.Resolve(typeof(Preferences.IPreferences));
            var parts = preferences.PrivateRegionsKey.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            key = Tuple.Create(
               StringToByteArray(parts[0]),
               StringToByteArray(parts[1]),
               StringToByteArray(parts[2]));
         }
      }

      public virtual string RemotePath => GetRemotePath(relativePath);

      public virtual string LocalPath => GetLocalPath(relativePath);

      public bool ExistsLocally => File.Exists(LocalPath);

      public string RemoteRootPath { get; }
      public string LocalRootPath { get; }

      public virtual Task DownloadAsync(bool force = false) {
         if (!ExistsLocally || force) {
            return DownloadService.DownloadFile(RemotePath, LocalPath);
         } else {
            return Task.CompletedTask;
         }
      }

      protected string GetRemotePath(string relativePath) {
         if (string.IsNullOrEmpty(relativePath)) {
            return string.Empty;
         }
         return RemoteRootPath.Trim('/') + "/" + relativePath.Trim('/');
      }

      protected string GetLocalPath(string relativePath) {
         if (string.IsNullOrEmpty(relativePath)) {
            return string.Empty;
         }
         return Path.Combine(LocalRootPath, relativePath.Trim('/'));
      }


      public string GetAllText() {
         if (isPrivateUseKey) {
            return DecryptAsText(File.ReadAllBytes(LocalPath));
         } else {
            return File.ReadAllText(LocalPath);
         }
      }

      protected byte[] GetAllBytes() {
         if (isPrivateUseKey) {
            return DecryptAsBytes(File.ReadAllBytes(LocalPath));
         } else {
            return File.ReadAllBytes(LocalPath);
         }
      }

      protected Stream GetStream() {
         if (isPrivateUseKey) {
            return DecryptStringFromBytesAes(
               File.ReadAllBytes(LocalPath),
               key.Item1,
               key.Item2,
               key.Item3);
         } else {
            return File.OpenRead(LocalPath);
         }
      }

      private byte[] DecryptAsBytes(byte[] cipher) {
         using (var stream =
            DecryptStringFromBytesAes(cipher, key.Item1, key.Item2, key.Item3)) {
            return stream.ToArray();
         }
      }

      private string DecryptAsText(byte[] cipher) {
         using (var stream =
            DecryptStringFromBytesAes(cipher, key.Item1, key.Item2, key.Item3)) {
            using (StreamReader sr = new StreamReader(stream)) {
               return sr.ReadToEnd();
            }
         }
      }

      private static MemoryStream DecryptStringFromBytesAes(byte[] cipherText, byte[] key, byte[] iv, byte[] salt) {
         // Check arguments.
         if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException("cipherText");
         if (key == null || key.Length <= 0)
            throw new ArgumentNullException("key");
         if (iv == null || iv.Length <= 0)
            throw new ArgumentNullException("iv");
         if (salt == null || salt.Length <= 0)
            throw new ArgumentNullException("salt");           // Declare the RijndaelManaged object
                                                                        // used to decrypt the data.
         RijndaelManaged aesAlg = null;                                 // Declare the string used to hold
                                                                        // the decrypted text.

         byte[] encryptedBytes = new byte[cipherText.Length - salt.Length - 8];
         Buffer.BlockCopy(cipherText, 8, salt, 0, salt.Length);
         Buffer.BlockCopy(cipherText, salt.Length + 8, encryptedBytes, 0, encryptedBytes.Length);

         MemoryStream result = new MemoryStream();
         try {
            // Create a RijndaelManaged object
            // with the specified key and IV.
            aesAlg = new RijndaelManaged { Mode = CipherMode.CBC, KeySize = 256, BlockSize = 128, Key = key, IV = iv };                // Create a decrytor to perform the stream transform.
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream(cipherText)) {
               using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                  csDecrypt.CopyTo(result);
               }
            }
         } finally {
            // Clear the RijndaelManaged object.
            if (aesAlg != null)
               aesAlg.Clear();
         }
         result.Seek(0, SeekOrigin.Begin);
         return result;
      }

      private  static byte[] StringToByteArray(String hex) {
         int NumberChars = hex.Length;
         byte[] bytes = new byte[NumberChars / 2];
         for (int i = 0; i < NumberChars; i += 2)
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
         return bytes;
      }
   }
}
