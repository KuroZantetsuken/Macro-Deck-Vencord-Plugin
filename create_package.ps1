$source = $PSScriptRoot
$build = "$source/bin/Release/net8.0-windows10.0.22000.0"
$dist = "$source/Dist"

Write-Host "Cleaning dist..."
if (Test-Path $dist) { Remove-Item $dist -Recurse -Force; Start-Sleep -Seconds 1 }
New-Item -ItemType Directory -Path $dist | Out-Null

Write-Host "Copying manifest and icon..."
Copy-Item "$source/ExtensionManifest.json" -Destination $dist
Copy-Item "$source/ExtensionIcon.png" -Destination $dist

Write-Host "Copying binaries..."
Copy-Item "$build/*" -Destination $dist -Recurse

Start-Sleep -Seconds 5

Write-Host "Creating archive..."
$zipPath = "$source/VencordPlugin.zip"
$finalPath = "$source/VencordPlugin.macroDeckPlugin"
if (Test-Path $zipPath) { Remove-Item $zipPath }
if (Test-Path $finalPath) { Remove-Item $finalPath }

Compress-Archive -Path "$dist/*" -DestinationPath $zipPath -Force
Move-Item $zipPath $finalPath -Force

Write-Host "Done! Created: $finalPath"
