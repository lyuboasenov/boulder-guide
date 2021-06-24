using BoulderGuide.Mobile.Forms.Services.Errors;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Services.Data {
   public class DownloadService : IDownloadService {

      private readonly IErrorService errorService;
      private static readonly HttpClient httpClient = new HttpClient();

      public DownloadService(IErrorService errorService) {
         this.errorService = errorService;
      }

      public async Task DownloadFile(string remotePath, string localPath) {
         try {
            using (var response = await httpClient.GetAsync(remotePath).ConfigureAwait(false)) {
               response.EnsureSuccessStatusCode();

               using (var stream = File.Open(localPath, FileMode.Create)) {
                  await response.Content.CopyToAsync(stream).ConfigureAwait(false);
               }
            }
         } catch (Exception ex) {
            await errorService.HandleErrorAsync(new DownloadFileException("", ex) {
               Address = remotePath
            });
         }

      }
   }
}
