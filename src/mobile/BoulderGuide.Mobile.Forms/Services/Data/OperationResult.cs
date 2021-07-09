using System;
using System.Collections.Generic;
using System.Linq;

namespace BoulderGuide.Mobile.Forms.Services.Data {
   public class OperationResult<T> {
      public T Result { get; }
      public Exception Exception { get; }

      public OperationResult(T result, IEnumerable<Exception> ex) {
         Result = result;
         Exception = ex?.Any() == true ? new AggregateException(ex) : null;
      }

      public OperationResult(T result, Exception ex) {
         Result = result;
         Exception = ex;
      }

      public void EnsureSuccessful() {
         if (null != Exception) {
            throw Exception;
         }
      }
   }
}
