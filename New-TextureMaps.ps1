<#
.SYNOPSIS
Creates black and red texture masks from images in a directory.
.DESCRIPTION
Processes PNG, JPEG, BMP, GIF, and TIFF files without recursing into subdirectories.
Only the first frame of multi-frame images is processed. Saves opaque PNGs in
the Maps subdirectory with an m suffix (north.png becomes Maps/northm.png).
Fully transparent pixels and exact grays below 125 become black. All other
pixels become RGB (252, 0, 0), including partially transparent colored pixels.
Existing outputs are skipped unless -Force is supplied. Requires Windows and
System.Drawing.
.EXAMPLE
.\New-TextureMaps.ps1 -Directory '.\Temp\Final'
.EXAMPLE
.\New-TextureMaps.ps1 -Directory '.\Temp\Final' -Force
#>
[CmdletBinding()]
param (
    [Parameter(Mandatory = $true, Position = 0)]
    [ValidateNotNullOrEmpty()]
    [string]$Directory,

    [switch]$Force
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

$sourceDirectory = Get-Item -LiteralPath $Directory
if (-not $sourceDirectory.PSIsContainer -or $sourceDirectory.PSProvider.Name -ne 'FileSystem') {
    throw "Not a filesystem directory: $Directory"
}

$supportedExtensions = @('.png', '.jpg', '.jpeg', '.bmp', '.gif', '.tif', '.tiff')
$sourceImages = @(Get-ChildItem -LiteralPath $sourceDirectory.FullName -File |
    Where-Object { $_.Extension -in $supportedExtensions })

if ($sourceImages.Count -eq 0) {
    Write-Warning "No supported images found in $($sourceDirectory.FullName)."
    return
}

$outputDirectory = Join-Path $sourceDirectory.FullName 'Maps'
[System.IO.Directory]::CreateDirectory($outputDirectory) | Out-Null
$black = [System.Drawing.Color]::FromArgb(255, 0, 0, 0)
$red = [System.Drawing.Color]::FromArgb(255, 252, 0, 0)

foreach ($sourceImage in $sourceImages) {
    $outputPath = Join-Path $outputDirectory ($sourceImage.BaseName + 'm.png')
    if ((Test-Path -LiteralPath $outputPath) -and -not $Force) {
        Write-Warning "Skipping existing output: $outputPath. Use -Force to replace it."
        continue
    }

    $sourceBitmap = $null
    $mapBitmap = $null
    try {
        $sourceBitmap = [System.Drawing.Bitmap]::new($sourceImage.FullName)
        $mapBitmap = [System.Drawing.Bitmap]::new(
            $sourceBitmap.Width,
            $sourceBitmap.Height,
            [System.Drawing.Imaging.PixelFormat]::Format24bppRgb
        )

        for ($row = 0; $row -lt $sourceBitmap.Height; $row++) {
            for ($column = 0; $column -lt $sourceBitmap.Width; $column++) {
                $pixel = $sourceBitmap.GetPixel($column, $row)
                $isDarkGray = $pixel.R -eq $pixel.G -and $pixel.G -eq $pixel.B -and $pixel.R -lt 125
                $mapColor = if ($pixel.A -eq 0 -or $isDarkGray) { $black } else { $red }
                $mapBitmap.SetPixel($column, $row, $mapColor)
            }
        }

        $mapBitmap.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)
        Write-Output $outputPath
    }
    finally {
        if ($null -ne $mapBitmap) { $mapBitmap.Dispose() }
        if ($null -ne $sourceBitmap) { $sourceBitmap.Dispose() }
    }
}
