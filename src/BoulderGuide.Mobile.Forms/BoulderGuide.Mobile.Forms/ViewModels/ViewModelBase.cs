using BoulderGuide.Mobile.Forms.Services.Data;
using BoulderGuide.Mobile.Forms.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
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

      protected INavigationParameters BackParameters { get; private set; }
      protected INavigationParameters InputParameters { get; private set; }


      public ViewModelBase(INavigationService navigationService, IDialogService dialogService) {
         NavigationService = navigationService;
         DialogService = dialogService;
      }

      public virtual void Initialize(INavigationParameters parameters) {

      }

      public virtual void OnNavigatedFrom(INavigationParameters parameters) {
         parameters?.Add(nameof(BackParameters), InputParameters);
      }

      public virtual void OnNavigatedTo(INavigationParameters parameters) {
         if (parameters.TryGetValue(nameof(BackParameters), out INavigationParameters backParameters)) {
            BackParameters = backParameters;
         }
         InputParameters = parameters;
      }

      public virtual void Destroy() {

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
