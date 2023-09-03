param(
   [string]
   $rootPath
)

function getArea ($path, $date) {

   Write-Host "Getting area from '$path'"

   if (-Not (test-path (Join-Path $path 'area.json'))) {
      throw "'$path' does not contain area.json - invalid container/area."
   }

   $areas = [System.Collections.ArrayList]::new()
   $routes = [System.Collections.ArrayList]::new()

   Get-ChildItem -Path $path | % {
      if ($_.PSIsContainer -and !$_.name.StartsWith('.')) {
         $area = getArea ($_.FullName) $date
         $areas.Add($area) | Out-Null
      } elseif ($_.Name -eq 'area.json') {
         # added by default
      } elseif ($_.Extension -eq '.json') {
         $routeObject = [PSCustomObject]@{
            id = (getId $path ($_.Name));
            name = (getName $path ($_.Name));
            index = (getRelativePath $path ($_.Name));
            images = [string[]](getImages $path ($_.Name));
            difficulty = (getDifficulty $path ($_.Name));
            location = (getLocation $path ($_.Name))
         }

         $routeObject.images | % {
            $imageLocalPath = Join-Path $rootPath $_

            if (-Not (test-path $imageLocalPath)) {
               throw "Image '$imageLocalPath' does not exist - route '$($routeObject.Name)' is invalid."
            }
         }
         $routes.Add($routeObject) | Out-Null
      }
   }

   if ($areas.Count -eq 0) {
      $areas = $null
   }

   if ($routes.Count -eq 0) {
      $routes = $null
   }

   return [PSCustomObject]@{
      id = (getId $path 'area.json');
      name = (getName $path 'area.json');
      index = (getRelativePath $path 'area.json');
      areas = $areas;
      routes = $routes;
      map = '';
      images = @();
      date = $date
   }
}

function getRelativePath ($path, $file) {
   $filePath = Join-Path $path $file
   $rel = $rootUri.MakeRelativeUri([Uri]::new($filePath)).OriginalString
   return "/$rel"
}

function getPropertyValue($path, $file, $property) {
   Get-Content (Join-Path $path $file) | `
   ConvertFrom-Json | `
   Select-Object -ExpandProperty $property | `
   Write-Output
}

function getId ($path, $file) {
   (getPropertyValue $path $file Id) | Write-Output
}

function getName ($path, $file) {
   (getPropertyValue $path $file Name) | Write-Output
}

function getDifficulty ($path, $file) {
   (getPropertyValue $path $file Difficulty) | Write-Output
}

function getLocation ($path, $file) {
   (getPropertyValue $path $file Location) | Write-Output
}

function getImages ($path, $file) {
   Get-Content (Join-Path $path $file) | `
   ConvertFrom-Json | %{
      $_.Schemas | %{
         $imagePath = Join-Path $path $_.Id
         . (Join-Path $PSScriptRoot 'update-images.ps1') $imagePath
         $size = Get-Item $imagePath | Select-Object -ExpandProperty Length
         if ($size -gt 410000) {
            Write-Error "Image '$(Join-Path $path $_.Id)' is larger than 400KB ($size)."
         }
         getRelativePath $path ($_.Id)
      }
   } | `
   Write-Output
}

$indexLocation = (Join-Path $rootPath 'index.json')
$rootUri = [System.Uri]::new("$rootPath/")
Remove-Item -Path $indexLocation -ErrorAction:SilentlyContinue

$area = getArea $rootPath ([DateTime]::Now.ToFileTimeUtc())
$area.images = [string[]]@(Get-ChildItem -Path $rootPath -Filter '*.jpg' | %{ "/$($_.Name)" })
$area.map = '/map.mbtiles'

$area | ConvertTo-Json -depth 100 | Set-Content -Path $indexLocation -Force
