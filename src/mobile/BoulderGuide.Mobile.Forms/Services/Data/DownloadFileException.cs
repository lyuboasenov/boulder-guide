using System;

namespace BoulderGuide.Mobile.Forms.Services.Data {
   public class DownloadFileException : Exception {

      public DownloadFileException() : base() {
      }

      public DownloadFileException(string message) : base(message) {
      }

      public DownloadFileException(string message, Exception innerException) : base(message, innerException) { }
      public DownloadFileException(string remote, string local, Exception innerException) : base($"{nameof(remote)}: '{remote}'. {nameof(local)}: '{local}'", innerException) {
      }
   }
}
