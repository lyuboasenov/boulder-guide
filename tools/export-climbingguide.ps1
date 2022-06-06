param(
   [string]
   $rootPath
)

function GetGrade() {
   param(
      $value
   )

   if ($value -le -1) { return "Проект"; }
   elseif ($value -le 20) { return "3"; }
   elseif ($value -le 25) { return "4-"; }
   elseif ($value -le 30) { return "4"; }
   elseif ($value -le 35) { return "4+"; }
   elseif ($value -le 40) { return "5-"; }
   elseif ($value -le 45) { return "5"; }
   elseif ($value -le 50) { return "5+"; }
   elseif ($value -le 55) { return "6A"; }
   elseif ($value -le 60) { return "6A+"; }
   elseif ($value -le 65) { return "6B"; }
   elseif ($value -le 70) { return "6B+"; }
   elseif ($value -le 75) { return "6C"; }
   elseif ($value -le 80) { return "6C+"; }
   elseif ($value -le 85) { return "7A"; }
   elseif ($value -le 90) { return "7A+"; }
   elseif ($value -le 95) { return "7B"; }
   elseif ($value -le 100) { return "7B+"; }
   elseif ($value -le 105) { return "7C"; }
   elseif ($value -le 110) { return "7C+"; }
   elseif ($value -le 115) { return "8A"; }
   elseif ($value -le 120) { return "8A+"; }
   elseif ($value -le 125) { return "8B"; }
   elseif ($value -le 130) { return "8B+"; }
   elseif ($value -le 135) { return "8C"; }
   elseif ($value -le 140) { return "8C+"; }
   else { return "9A"; }
}

function GetCenter() {
   param(
      $locations
   )

   $count = 0
   $lat = 0
   $lon = 0

   foreach ($loc in $locations) {
      $count += 1
      $lat += $loc.Latitude
      $lon += $loc.Longitude
   }

   return [PSCustomObject]@{
      Latitude = $lat / $count;
      Longitude = $lon / $count;
   }
}


$index = Get-Content -Path (Join-Path $rootPath '.\index.json') -Raw | ConvertFrom-Json

$routeCounter = 0
$routes = [System.Collections.ArrayList]::new()
$areaCounter = 0
$areas = [System.Collections.ArrayList]::new()

$index.areas | % {
   $area = Get-Content -Path (Join-Path $rootPath $_.index) -Raw | ConvertFrom-Json
   $areaCounter += 1

   $areas.Add(
      [PSCustomObject]@{
         Id = $area.id;
         Number = $areaCounter;
         Name = $area.name;
         Parent = "Бели Искър";
         Length = '';
         Kind_alp = 0;
         Kind_trad = 0;
         Kind_sport = 0;
         Kind_boulder = $_.routes.Count;
         Kind_dws = 0;
         Kind_ice = 0;
         RockKind = "гранит";
         Latitude = (GetCenter $area.location).Latitude;
         Longitude = (GetCenter $area.location).Longitude;
         Approach = $area.approach;
         Descend = '';
         Description = $area.info;
         Source = 'https://lyuboasenov.github.io/boulder-guide/view/beli-iskyr' + $area.id;
         Remarks = ''
      }
   ) | Out-Null

   foreach ($route in $_.routes) {
      $routeCounter += 1

      $routeInfo = Get-Content -Path (Join-Path $rootPath $route.index) -Raw | ConvertFrom-Json

      $routes.Add(
         [PSCustomObject]@{
            Id = $route.id;
            Number = $routeCounter;
            Name = $route.name;
            Area = $area.name;
            Difficulty = (GetGrade $route.difficulty);
            Length = '';
            Incline = '';
            Popularity = '';
            Rain = '';
            Shade = '';
            Kind_alp = 0;
            Kind_trad = 0;
            Kind_sport = 0;
            Kind_boulder = 1;
            Kind_dws = 0;
            Kind_ice = 0;
            Bolts = 0;
            NewBolts = 0;
            Pitons = 0;
            Friends = 0;
            Ekipiran = '';
            Inventar = '';
            Latitude = $route.location.Latitude;
            Longitude = $route.location.Longitude;
            Start = '';
            Descend = '';
            Crux = '';
            FA = '';
            FA_by = '';
            History = '';
            Description = $routeInfo.info;
            Source = 'https://lyuboasenov.github.io/boulder-guide/view/beli-iskyr' + $area.id + '/' + $route.Id;
            Remarks = $routeInfo.EightALink;
         }
      ) | Out-Null
   }
}

Write-host
Write-host "Areas"
Write-host

$areas | ConvertTo-Csv

Write-host
Write-host "Routes"
Write-host

$routes | ConvertTo-Csv