param(
   [Parameter(Mandatory = $true, Position = 0)]
   [int]
   $Major,

   [Parameter(Mandatory = $true, Position = 1)]
   [int]
   $Minor,

   [Parameter(Mandatory = $true, Position = 2)]
   [int]
   $Revision
)

# $PSScriptRoot
$manifestPath = Join-Path $PSScriptRoot '..\src\mobile\BoulderGuide.Mobile.Forms.Android\Properties\AndroidManifest.xml'
# Read droid manifest
$xml = [xml](Get-Content $manifestPath)

$version = [int] $xml.manifest.versionCode
$version++

$xml.manifest.versionCode = $version
$xml.manifest.versionName = "$Major.$Minor.$Revision ($version)"

$xml.Save($manifestPath)

$infoPlistPath = Join-Path $PSScriptRoot '..\src\mobile\BoulderGuide.Mobile.Forms.iOS\Info.plist'
$xml = [System.Xml.XmlDocument]::new()
$xml.Load($infoPlistPath)

for ($i = 0; $i -lt $xml.DocumentElement.ChildNodes[0].ChildNodes.Count; $i++) {
   $node = $xml.DocumentElement.ChildNodes[0].ChildNodes[$i]
   if ($node.Name -eq 'key' -and $node.InnerText -eq 'CFBundleVersion') {
      $i++
      $xml.DocumentElement.ChildNodes[0].ChildNodes[$i].InnerText = "$Major.$Minor.$Revision.$version"
   }

   if ($node.Name -eq 'key' -and $node.InnerText -eq 'CFBundleShortVersionString') {
      $i++
      $xml.DocumentElement.ChildNodes[0].ChildNodes[$i].InnerText = "$Major.$Minor.$Revision"
   }
}

$xml.Save($infoPlistPath)

$fixAutoClose = Get-Content $infoPlistPath -Raw
$fixAutoClose.Replace(' />', '/>').Replace('[]>', '>') | Set-Content $infoPlistPath

git commit -a -m "Bump version to $Major.$Minor.$Revision ($version)"
git push
git tag "v$Major.$Minor.$Revision" -a -m "Release $Major.$Minor.$Revision ($version)"
git push origin --tags