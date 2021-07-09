using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Services.Data {
   public interface IDownloadService {
      Task DownloadFile(string remotePath, string localPath);
   }
}
