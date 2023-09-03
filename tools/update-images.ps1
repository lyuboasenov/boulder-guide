param(
   [Parameter(Position = 0, Mandatory = $true)]
   [string]
   $imageFilePath,

   [Parameter(Position = 1)]
   [int]
   $sizeInBytes = 409600,

   [Parameter(Position = 2)]
   [int]
   $width = 1920,

   [Parameter(Position = 3)]
   [int]
   $height = 1920
)

function _getEncoderInfo($mimeType) {
   $j = 0
   $encoders = [System.Drawing.Imaging.ImageCodecInfo]::GetImageEncoders()
   for ($j = 0; $j -lt $encoders.Length; $j++) {
      if ($encoders[$j].MimeType -eq $mimeType) {
         return $encoders[$j]
      }
   }
   return $null;
}

function _downscaleImage($image) {
   $resizedPhotoStream = [System.IO.MemoryStream]::new();

   $resizedSize = 0
   [long] $quality = 93

   $eps = [System.Drawing.Imaging.EncoderParameters]::new(1)
   $ici = _getEncoderInfo("image/jpeg")

   do {

      $eps.Param[0] = [System.Drawing.Imaging.EncoderParameter]::new([System.Drawing.Imaging.Encoder]::Quality, $quality)
      $null = $resizedPhotoStream.SetLength(0)

      $null = $image.Save($resizedPhotoStream, $ici, $eps)
      $resizedSize = $resizedPhotoStream.Position

      $quality -= 1
   } while ($resizedSize -gt $sizeInBytes)

   $null = $resizedPhotoStream.Seek(0, [System.IO.SeekOrigin]::Begin)

   return $resizedPhotoStream
}

function _resizeImage($image) {
   $widthRatio = $image.Width / $width
   $heightRatio = $image.Height / $height

   $ratio = $heightRatio
   if ($widthRatio -gt $heightRatio) {
      $ratio = $widthRatio
   }

   $newWidth = $image.Width / $ratio
   $newHeight = $image.Height / $ratio

   $resizedImage = [System.Drawing.Bitmap]::new([int]$newWidth, [int]$newHeight)

   # Create a graphics object from the new bitmap
   $graphics = [System.Drawing.Graphics]::FromImage($resizedImage)

   # Set the interpolation mode for better resizing quality
   $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic

   # Draw the original image onto the new bitmap with resizing
   $graphics.DrawImage($image, 0, 0, $newWidth, $newHeight)

   # Dispose the objects to free up resources
   $graphics.Dispose()
   $image.Dispose()

   return $resizedImage
}

$imageFile = Get-Item $imageFilePath
if ($imageFile.Length -gt $sizeInBytes) {
   $image = [System.Drawing.Image]::FromFile($imageFile.FullName)
   $image = _resizeImage($image)
   $stream = _downscaleImage($image)

   $image.Dispose()

   Remove-Item $imageFile.FullName -Confirm:$false -Force

   $file = [System.IO.File]::Create($imageFile.FullName)
   $stream.CopyTo($file);

   $file.Dispose()
   $stream.Dispose()
}