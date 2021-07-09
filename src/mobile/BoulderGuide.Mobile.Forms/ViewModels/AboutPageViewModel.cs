using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class AboutPageViewModel : ViewModelBase {

      public ICommand WebSiteCommand { get; }
      public ICommand ContactsCommand { get; }
      public ICommand OnErrorCommand { get; }

      private readonly IBrowser browser;
      private readonly IEmail email;

      public AboutPageViewModel(IBrowser browser, IEmail email) {
         this.browser = browser;
         this.email = email;

         WebSiteCommand = new AsyncCommand(WebSite);
         ContactsCommand = new AsyncCommand(Contacts);
         OnErrorCommand = new AsyncCommand(OnError);
      }

      private Task OnError() {
         return OpenBrowserToAsync("https://github.com/lyuboasenov/boulder-guide/issues");
      }

      private async Task Contacts() {
         try {
            var message = new EmailMessage {
               Subject = Strings.DefaultEmailSubject,
               Body = Strings.DefaultEmailBody,
               To = new List<string>(new[] { "boulder.guide.app@gmail.com" })
            };
            await email.ComposeAsync(message);
         } catch (Exception ex) {
            await HandleOperationExceptionAsync(ex, Strings.UnableToOpenEmailClient);
         }
      }

      private Task WebSite() {
         return OpenBrowserToAsync("https://github.com/lyuboasenov/boulder-guide");
      }

      private Task OpenBrowserToAsync(string url) {
         try {
            return browser.OpenAsync(url);
         } catch (Exception ex) {
            return HandleOperationExceptionAsync(ex, string.Format(Strings.UnableToOpenBrowserFormat, url));
         }
      }
   }
}
