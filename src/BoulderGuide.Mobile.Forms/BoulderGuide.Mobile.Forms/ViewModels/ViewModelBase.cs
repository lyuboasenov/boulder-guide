using BoulderGuide.Mobile.Forms.Services.Data;
using BoulderGuide.Mobile.Forms.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible {
      protected INavigationService NavigationService { get; }
      protected IDialogService DialogService { get; }

      private string _title;
      public string Title {
         get { return _title; }
         set { SetProperty(ref _title, value); }
      }

      public ViewModelBase(INavigationService navigationService, IDialogService dialogService) {
         NavigationService = navigationService;
         DialogService = dialogService;
      }

      public virtual void Initialize(INavigationParameters parameters) {

      }

      public virtual void OnNavigatedFrom(INavigationParameters parameters) {

      }

      public virtual void OnNavigatedTo(INavigationParameters parameters) {

      }

      public virtual void Destroy() {

      }

      public Task<INavigationResult> NavigateAsync(string title, string path, INavigationParameters parameters = null) {
         var lastItem = Breadcrumbs.Items.LastOrDefault();
         if (lastItem is null ||
            lastItem.Name != title ||
            lastItem.Path != path ||
            lastItem.Parameters != parameters) {
            Breadcrumbs.Items.Add(new Breadcrumbs.Item() {
               Name = title,
               Path = path,
               Parameters = parameters
            });
         }

         return NavigationService.NavigateAsync(path, parameters);
      }

      public Task HandleExceptionAsync(Exception ex) {
         var dialogParams = new DialogParameters();
         dialogParams.Add(
            DialogPageViewModel.ParameterKeys.Message, ex.Message);
         dialogParams.Add(DialogPageViewModel.ParameterKeys.Severity, DialogPageViewModel.Severity.Error);

         Device.BeginInvokeOnMainThread(async () => { await DialogService.ShowDialogAsync(nameof(DialogPage), dialogParams); });

         return Task.CompletedTask;
      }

      protected static NavigationParameters InitializeParameters(string name, object value) {
         return InitializeParameters(new KeyValuePair<string, object>(name, value ));
      }

      protected static NavigationParameters InitializeParameters(params KeyValuePair<string, object>[] nameValues) {
         NavigationParameters parameters = new NavigationParameters();
         foreach (var kv in nameValues) {
            parameters.Add(kv.Key, kv.Value);
         }
         return parameters;
      }
   }
}
