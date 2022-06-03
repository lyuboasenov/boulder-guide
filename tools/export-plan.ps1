param(
   [string]
   $rootPath
)

$index = Get-Content -Path (Join-Path $rootPath '.\index.json') -Raw | ConvertFrom-Json

$index.areas | % {
   $area = Get-Content -Path (Join-Path $rootPath $_.index) -Raw | ConvertFrom-Json

   $boulders = @{}
   $difficulties = [System.Collections.ArrayList]::new()

   foreach ($route in $_.routes) {
      $difficulties.Add($route.difficulty) | out-null
      $lat = [int]($route.location.Latitude * 1000000)
      $lon = [int]($route.location.Longitude * 1000000)

      $i = $lat * 100000000 + $lon

      if (!$boulders.ContainsKey($i) -or $boulders[$i].difficulty -lt $route.difficulty) {
         $boulders[$i] = $route
      }
   }

   Write-Host @"
- Сектор *"$($area.name)"*
  <u>Граници:</u>
"@
   $area.location | % {
      Write-Host "      GPS $($_.Latitude), $($_.Longitude)"
   }
   Write-Host @"
  <u>Брой маршрути</u>: $($difficulties.Count)
  <u>Брой боулдъри</u>: $($boulders.Count)
  <u>Структура</u>: гранит
"@
   $boulders.Keys | % {
      Write-Host @"
  - Боулдър *"$($boulders[$_].name)"*
  <u>Местоположение</u>: GPS $($boulders[$_].Location.Latitude), $($boulders[$_].Location.Longitude)
"@

   }

}