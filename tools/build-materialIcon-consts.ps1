param(
   [string]
   $codepointsFilePath
)

write-host @"

namespace BoulderGuide.Mobile.Forms.Icons {
   internal static class MaterialIconFont {
#pragma warning disable IDE1006 // Naming Styles

"

get-content $codepointsFilePath | % {
   $cur = $_.Split(' ')
   $nameParts = $cur[0].Split('_')
   $name = ''
   $nameParts | % {
      $name += $_.Substring(0, 1).ToUpper() + $_.Substring(1)
   }
   $value = $cur[1]
   Write-host "      public const string $name = ""\u$value"";"
}

write-host @"

#pragma warning restore IDE1006 // Naming Styles
   }
}

"