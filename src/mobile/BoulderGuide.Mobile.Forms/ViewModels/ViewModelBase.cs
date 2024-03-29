﻿using BoulderGuide.Mobile.Forms.Services.Errors;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
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
      private INavigationService NavigationService { get; }
      private IDialogService DialogService { get; }

      public ICommand GoBackCommand { get; }

      public ViewModelBase() {
         var container = Prism.PrismApplicationBase.Current.Container.CurrentScope;
         NavigationService = (INavigationService) container.Resolve(typeof(INavigationService));
         DialogService = (IDialogService) container.Resolve(typeof(IDialogService));
         errorService = (IErrorService) container.Resolve(typeof(IErrorService));
         mainThread = (IMainThread) container.Resolve(typeof(IMainThread));

         GoBackCommand = new AsyncCommand(GoBackAsync);
      }

      public virtual void OnNavigatedFrom(INavigationParameters parameters) {

      }

      public virtual void OnNavigatedTo(INavigationParameters parameters) {

      }

      public virtual void Destroy() {

      }

      public Task NavigateAsync(string title, string path, INavigationParameters parameters = null, string glyph = "") {
         var target = new Breadcrumbs.Item() {
            Glyph = glyph,
            Title = title,
            Path = path,
            Parameters = parameters
         };

         return NavigateAsync(target);
      }

      public async Task NavigateAsync(Breadcrumbs.Item target) {

         if (Breadcrumbs.Items.Any(i => i.Equals(target))) {
            // backtrack
            try {
               for (int i = Breadcrumbs.Items.Count - 1; i >= 0; i--) {
                  var current = Breadcrumbs.Items[i];
                  Breadcrumbs.Items.RemoveAt(i);
                  if (current.Equals(target)) {
                     await NavigateAsync(current.Title, current.Path, current.Parameters, current.Glyph);
                     break;
                  }
               }
            } catch (Exception ex) {
               await HandleOperationExceptionAsync(ex, string.Format(Strings.UnableToNavigateFormat, target?.Title));
            }
         } else {
            Breadcrumbs.Items.Add(target);
         }

         await RunOnMainThreadAsync(async () => {
            var result = await NavigationService.NavigateAsync(target.Path, target.Parameters);
            if (!result.Success) {
               await HandleOperationExceptionAsync(result.Exception, string.Format(Strings.UnableToNavigateFormat, target.Title));
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

      public async Task ShowDialogAsync(string path, IDialogParameters parameters = null) {
         var result = await DialogService.ShowDialogAsync(
               path,
               parameters).
               ConfigureAwait(false);

         if (result.Exception != null) {
            await HandleOperationExceptionAsync(result.Exception, Strings.UnableToOpenDialog);
         }
      }

      public Task HandleExceptionAsync(Exception ex) {
         return errorService.HandleErrorAsync(ex);
      }

      public Task HandleExceptionAsync(Exception ex, string message) {
         return errorService.HandleErrorAsync(ex, message);
      }

      public Task HandleOperationExceptionAsync(Exception ex, string operation) {
         return HandleExceptionAsync(ex, string.Format(Strings.OperationExceptionFormat, operation));
      }

      public void HandleOperationException(Exception ex, string operation) {
         errorService.HandleError(ex, string.Format(Strings.OperationExceptionFormat, operation));
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
