using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace BoulderGuide.Wpf.ViewModels {
   public abstract class BaseViewModel<T> : INotifyPropertyChanged where T : FrameworkElement, new() {
      private T view;

      public bool? Result { get; private set; }

      public T GetView() {
         view = GetViewInternal();
         view.DataContext = this;
         view.Unloaded += View_Unloaded;

         Initialize();

         return view;
      }

      private void View_Unloaded(object sender, RoutedEventArgs e) {
         (this as IDisposable)?.Dispose();
      }

      protected virtual T GetViewInternal() {
         return new T();
      }

      public virtual void Initialize() {
      }

      public virtual void Close(bool? result = null) {
         if (view is Window w) {
            w.Close();
            Result = result;
         }
      }

      protected void HandleError(Exception ex) {
         MessageBox.Show(ex.Message);
      }

      protected string SelectFile(string fileFilter) {
         // Create OpenFileDialog
         Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
         openFileDlg.CheckFileExists = true;
         openFileDlg.Filter = fileFilter;

         // Launch OpenFileDialog by calling ShowDialog method
         Nullable<bool> result = openFileDlg.ShowDialog();
         if (result == true) {
            return openFileDlg.FileName;
         } else {
            return string.Empty;
         }
      }

      public event PropertyChangedEventHandler PropertyChanged;

      protected void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? "something changed"));
      }
   }
}