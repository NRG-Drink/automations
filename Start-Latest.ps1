
param (
    [string]$dir = ".",
    [string]$done = ".\done"
)

$seriesPath = Resolve-Path -Path $dir
$donePath = Join-Path -Path $dir -ChildPath $done

cd $seriesPath

$first = Get-ChildItem -Filter "*.lnk" | Sort-Object -Property Name | Select-Object -First 1
echo $first

if ($first -ne $null -and $first -ne "") {
    Invoke-Item -Path $first.FullName
} else {
    echo "No item found."
}

cd $seriesPath