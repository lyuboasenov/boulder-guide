using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Services.Data {
   public abstract class FileBasedEntity : INotifyPropertyChanged {
      protected string relativePath;
      private readonly bool isPrivateUseKey;
      private static Tuple<string, string> key;

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
            if (preferences.ShowPrivateRegions) {
               var parts = preferences.PrivateRegionsKey.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
               if (parts.Length > 1) {
                  key = Tuple.Create(
                     parts[0],
                     parts[1]);
               } else {
                  preferences.PrivateRegionsKey = string.Empty;
                  preferences.ShowPrivateRegions = false;
               }

            }
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
            return DecryptAsText(File.OpenRead(LocalPath));
         } else {
            return File.ReadAllText(LocalPath);
         }
      }

      protected Stream GetStream() {
         if (isPrivateUseKey) {
            return new DecryptingStream(
               File.OpenRead(LocalPath),
               key.Item1,
               key.Item2);
         } else {
            return File.OpenRead(LocalPath);
         }
      }

      private string DecryptAsText(Stream source) {
         using (var stream = new DecryptingStream(source, key.Item1, key.Item2)) {
            using (StreamReader sr = new StreamReader(stream)) {
               return sr.ReadToEnd();
            }
         }
      }

      private class DecryptingStream : Stream {

         private Stream stream;
         private bool disposedValue;
         private readonly Stream cypherStream;
         private RijndaelManaged aesAlg;

         public DecryptingStream(Stream cypherStream, string key, string iv) {
            if (string.IsNullOrEmpty(key)) {
               throw new ArgumentException($"'{nameof(key)}' cannot be null or empty.", nameof(key));
            }

            if (string.IsNullOrEmpty(iv)) {
               throw new ArgumentException($"'{nameof(iv)}' cannot be null or empty.", nameof(iv));
            }

            this.cypherStream = cypherStream ?? throw new ArgumentNullException(nameof(cypherStream));

            aesAlg = new RijndaelManaged {
               Mode = CipherMode.CBC,
               KeySize = 256,
               BlockSize = 128,
               Key = HexStringToBytes(key),
               IV = HexStringToBytes(iv)
            };
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);


            stream = new CryptoStream(cypherStream, decryptor, CryptoStreamMode.Read);
         }

         public override void Flush() {
            stream.Flush();
         }

         public override int Read(byte[] buffer, int offset, int count) {
            return stream.Read(buffer, offset, count);
         }

         public override long Seek(long offset, SeekOrigin origin) {
            return stream.Seek(offset, origin);
         }

         public override void SetLength(long value) {
            stream.SetLength(value);
         }

         public override void Write(byte[] buffer, int offset, int count) {
            stream.Write(buffer, offset, count);
         }

         public override bool CanRead => stream.CanRead;
         public override bool CanSeek => stream.CanSeek;
         public override bool CanWrite => stream.CanWrite;
         public override long Length => stream.Length;
         public override long Position { get { return stream.Position; } set { stream.Position = value; } }


         protected override void Dispose(bool disposing) {
            base.Dispose(disposing);

            if (!disposedValue) {
               if (disposing) {
                  aesAlg.Dispose();
                  stream.Dispose();
                  cypherStream.Dispose();
               }

               disposedValue = true;
            }
         }

         private static byte[] HexStringToBytes(String hex) {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
               bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
         }
      }
   }
}
