
param (
    # start, finish, revoke
    [string]$type = "revoke",
    [string]$dir = ".",
    [string]$relativeDone = ".\done",
    [string]$fileEndling = "*.lnk"
)

$seriesPath = Resolve-Path -Path $dir
$donePath = Join-Path -Path $seriesPath -ChildPath $relativeDone


if ($type -eq "revoke") {
    Set-Location $donePath
    $first = Get-ChildItem -Filter $fileEndling | Sort-Object -Property Name -Descending | Select-Object -First 1
} else {
    Set-Location $seriesPath
    $first = Get-ChildItem -Filter $fileEndling | Sort-Object -Property Name | Select-Object -First 1
}

Write-Output $first

if ($null -ne $first -and $first -ne "") {
    if ($type -eq "finish") {
        Move-Item -Path $first.FullName -Destination $donePath
    } elseif ($type -eq "revoke") {
        Move-Item -Path $first.FullName -Destination $seriesPath
    } else {
        Invoke-Item -Path $first.FullName
    }
} else {
    Write-Output "No item found."
}

Set-Location $seriesPath