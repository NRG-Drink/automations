# Create a link for all files in the given directory.
# NOTE: This will also flatten the structure.

# Fill in your directory where you want to copy the files as links from (will be flattened).
$source = Resolve-Path "F:\1Serien\Mannezimmer"
# When having two series' in one folder, the sorting can be wrong. Use the prefix to fix this.
$prefix = "1_"
$target = Resolve-Path "C:\Users\beatd\ZwoDrive\Desktop\series\"

Write-Host "Start creating links"

$files = Get-ChildItem -Path $source -Recurse -File -Filter "*.mp4"
$created = 0
$skipped = 0
foreach ($file in $files) {
    $fileName = "$($prefix)$($file.BaseName).lnk"
    $targetFile = $target.Path + $fileName
    if (Test-Path $targetFile) {
        $skipped++
        Write-Host "File with name already exists '$fileName'"
        Write-Host "-> skip"
        continue
    }
    
    $created++
    $shell = New-Object -ComObject WScript.Shell
    $shortcut = $shell.CreateShortcut($targetFile)
    $shortcut.TargetPath = $file.FullName
    $shortcut.Save()
}

Write-Host "Finish creating links for $created files, $skipped skipped ($($created + $skipped) total)"
