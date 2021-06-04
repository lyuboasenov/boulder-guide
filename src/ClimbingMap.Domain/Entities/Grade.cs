using System;
using System.Collections.Generic;
using System.Linq;

namespace ClimbingMap.Domain.Entities {
   public class Grade {
      private static IDictionary<double, string> vGrade = new Dictionary<double, string>() {
         { 20, "VB" },
         { 25, "V0-" },
         { 35, "V0" },
         { 45, "V0+" },
         { 52, "V1" },
         { 55, "V2" },
         { 60, "V3" },
         { 65, "V3+" },
         { 70, "V4" },
         { 75, "V4+" },
         { 80, "V5" },
         { 85, "V6" },
         { 90, "V7" },
         { 100, "V8" },
         { 105, "V9" },
         { 110, "V10" },
         { 115, "V11" },
         { 120, "V12" },
         { 125, "V13" },
         { 130, "V14" },
         { 135, "V15" },
         { 140, "V16" },
         { 145, "V17" },
      };
      private static IDictionary<double, string> fontGrade = new Dictionary<double, string>() {
         { 20, "3" },
         { 25, "4-" },
         { 30, "4" },
         { 35, "4+" },
         { 40, "5-" },
         { 45, "5" },
         { 50, "5+" },
         { 55, "6A" },
         { 60, "6A+" },
         { 65, "6B" },
         { 70, "6B+" },
         { 75, "6C" },
         { 80, "6C+" },
         { 85, "7A" },
         { 90, "7A+" },
         { 95, "7B" },
         { 100, "7B+" },
         { 105, "7C" },
         { 110, "7C+" },
         { 115, "8A" },
         { 120, "8A+" },
         { 125, "8B" },
         { 130, "8B+" },
         { 135, "8C" },
         { 140, "8C+" },
         { 145, "9A" }
      };
      private readonly double difficulty;

      public Grade(double difficulty) {
         this.difficulty = difficulty;
      }

      public override string ToString() {
         return ToString("Font");
      }

      public string ToString(string format) {
         if (format.Equals("V", StringComparison.OrdinalIgnoreCase)) {
            return SelectGrade(vGrade);
         } else {
            return SelectGrade(fontGrade);
         }
      }


      private string SelectGrade(IDictionary<double, string> container) {
         //// TODO: bauble search for string
         //int index = container.Count / 2;
         //while (true) {
         //   if (container.Keys.ElementAt(index) > difficulty) {

         //   } else {

         //   }
         //}

         for (int i = container.Count - 1; i >= 0; i--) {
            var kv = container.ElementAt(i);
            if (kv.Key <= difficulty) {
               return kv.Value;
            }
         }

         throw new ArgumentException("Grade not found.");
      }
   }
}
