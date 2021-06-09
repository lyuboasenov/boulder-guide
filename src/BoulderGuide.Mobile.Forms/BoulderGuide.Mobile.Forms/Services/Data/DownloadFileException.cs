using System;
using System.Collections.Generic;
using System.Text;

namespace BoulderGuide.Mobile.Forms.Services.Data {
   public class DownloadFileException : Exception {
      public string Address { get; set; }

      public DownloadFileException() : base() {
      }

      public DownloadFileException(string message) : base(message) {
      }

      public DownloadFileException(string message, Exception innerException) : base(message, innerException) {
      }
   }
}
