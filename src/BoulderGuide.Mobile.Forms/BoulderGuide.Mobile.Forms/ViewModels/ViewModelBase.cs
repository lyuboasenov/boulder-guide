using BoulderGuide.Mobile.Forms.Services.Errors;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials.Interfaces;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class ViewModelBase :
      BindableBase,
      IInitialize,
      IInitializeAsync,
      INavigationAware,
      IDestructible,
      IConfirmNavigation,
      IConfirmNavigationAsync {
      private IErrorService errorService;
      private IMainThread mainThread;
      protected INavigationService NavigationService { get; }
      protected IDialogService DialogService { get; }

      private string _title;
      public string Title {
         get { return _title; }
         set { SetProperty(ref _title, value); }
      }

      public ViewModelBase() {
         var container = Prism.PrismApplicationBase.Current.Container.CurrentScope;
         NavigationService = (INavigationService) container.Resolve(typeof(INavigationService));
         DialogService = (IDialogService) container.Resolve(typeof(IDialogService));
         errorService = (IErrorService) container.Resolve(typeof(IErrorService));
         mainThread = (IMainThread) container.Resolve(typeof(IMainThread));
      }

      public virtual void OnNavigatedFrom(INavigationParameters parameters) {

      }

      public virtual void OnNavigatedTo(INavigationParameters parameters) {

      }

      public virtual void Destroy() {

      }

      public Task NavigateAsync(string title, string path, INavigationParameters parameters = null, string glyph = "") {
         var lastItem = Breadcrumbs.Items.LastOrDefault();
         if (lastItem is null ||
            lastItem.Glyph != glyph ||
            lastItem.Title != title ||
            lastItem.Path != path ||
            lastItem.Parameters != parameters) {
            Breadcrumbs.Items.Add(new Breadcrumbs.Item() {
               Glyph = glyph,
               Title = title,
               Path = path,
               Parameters = parameters
            });
         }

         return RunOnMainThreadAsync(async () => {
            var result = await NavigationService.NavigateAsync(path, parameters);
            if (!result.Success) {
               HandleException(result.Exception);
            }
         });
      }

      public Task GoBackAsync() {
         var lastItem = Breadcrumbs.Items.LastOrDefault();
         if (lastItem != null) {
            Breadcrumbs.Items.Remove(lastItem);
            var secondToLast = Breadcrumbs.Items.LastOrDefault();
            if (secondToLast != null) {
               Breadcrumbs.Items.Remove(secondToLast);
               lastItem = secondToLast;
            }
         }

         return NavigateAsync(lastItem.Title, lastItem.Path, lastItem.Parameters, lastItem.Glyph);
      }

      public async Task ShowDialogAsync(string path, IDialogParameters parameters) {
         var result = await DialogService.ShowDialogAsync(
               path,
               parameters).
               ConfigureAwait(false);

         if (result.Exception != null) {
            HandleException(result.Exception);
         }
      }

      public void HandleException(Exception ex) {
         errorService.HandleError(ex);
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

      public virtual Task InitializeAsync(INavigationParameters parameters) {
         return Task.CompletedTask;
      }

      public virtual void Initialize(INavigationParameters parameters) {

      }

      protected Task RunOnMainThreadAsync(Action action) {
         return mainThread.InvokeOnMainThreadAsync(action);
      }

      protected Task RunOnMainThreadAsync(Func<Task> func) {
         return mainThread.InvokeOnMainThreadAsync(func);
      }

      protected Task RunAsync(Func<Task> func) {
         return Task.Run(func);
      }

      public virtual bool CanNavigate(INavigationParameters parameters) {
         return true;
      }

      public virtual Task<bool> CanNavigateAsync(INavigationParameters parameters) {
         return Task.FromResult(true);
      }
   }
}
