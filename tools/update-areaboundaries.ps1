param(
   [string]
   $rootPath
)

function cross ($o, $a, $b) {
   return ($a.X - $o.X) * ($b.Y - $o.Y) * ($b.X - $o.X)
}

function getBoundaries ([System.Collections.ArrayList] $points) {

   if (!$points) {
      return $null
   }

   if ($points.Count -le 1) {
      return $points
   }

   $n = $points.Count
   $k = 0
   $hull = [Object[]]::new(2 * $n)

   $points = @($points | Sort-Object -Property X, Y)

   # Build lower hull
   for ($i = 0; $i -lt $n; $i++) {
      while ($k -ge 2 -and (cross $hull[$k - 2] $hull[$k - 1] points[$i]) -le 0) {
         $k--
      }
      $hull[$k++] = $points[$i]
   }

   $i = $n - 2
   $t = $k + 1
   for (; $i -ge 0; $i--) {
      while ($k -ge $t -and (cross $hull[$k - 2] $hull[$k - 1] $points[$i]) -le 0) {
         $k--
      }
      $hull[$k++] = $points[$i]
   }

   return [System.Linq.Enumerable]::ToArray([System.Linq.Enumerable]::Take($hull, $k - 1))
}

Get-ChildItem -Path $rootPath -Filter 'area.json' -Recurse | % {
   $points = [System.Collections.ArrayList]::new()
   Get-ChildItem -Path $_.Directory  -file -Filter '*.json' -Recurse -Exclude 'area.json', 'index.json' | % {
      $routeObj = Get-Content -Path $_.FullName -Raw | ConvertFrom-Json
      $offset = 0.005
      $points.Add([PSCustomObject]@{
         X = $routeObj.Location.Longitude - $offset
         Y = $routeObj.Location.Latitude - $offset
      }) | Out-Null
      $points.Add([PSCustomObject]@{
         X = $routeObj.Location.Longitude + $offset
         Y = $routeObj.Location.Latitude - $offset
      }) | Out-Null
      $points.Add([PSCustomObject]@{
         X = $routeObj.Location.Longitude + $offset
         Y = $routeObj.Location.Latitude + $offset
      }) | Out-Null
      $points.Add([PSCustomObject]@{
         X = $routeObj.Location.Longitude - $offset
         Y = $routeObj.Location.Latitude + $offset
      }) | Out-Null
   }

   Write-Host $_.FullName

   $bounds = getBoundaries $points

   Write-Host $points.Capacity
   Write-Host $bounds.Length

   $bounds | % {
      $_ | Write-Host
   }

   $areaObj = Get-Content -Path $_.FullName -Raw | ConvertFrom-Json

   $areaObj.Location = @($bounds | % { [PSCustomObject]@{
      Latitude = $_.Y
      Longitude = $_.X
   } })

   $areaObj | ConvertTo-Json | Set-Content -Path $_.FullName -Force
}