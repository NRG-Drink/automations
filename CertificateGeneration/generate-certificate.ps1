. ".\password-utils.ps1"
. ".\certificate-utils.ps1"

# Ask for certificate name.
$name = Read-Host "Enter certifiacte name"
Write-Host "-> $name"
$exportDirPath = ".secrets.$name"
$certStore = "cert:\CurrentUser\My"

#region Checks
# Check existing output folder.
if (Test-Path $exportDirPath) {
    Write-Warning ("The outputfolder does already exit (.\$exportDirPath). " +
        "The program will not overwrite existing data. " +
        "Please rename, move or delete the existing folder.")
    return
}

$certificates = Get-CertificatesByName -Name $name -Store $certStore
if ($certificates) {
    # Print duplicate certificates.
    $certificates | Format-List

    # Write warning.
    Write-Warning ("A certificate with the same name is already installed. (Name: $name, Store: $certStore)")
    Write-Host "Certificates with the same name are confusing but works for the computer."
    
    # Ask proceed.
    $duplicateResponse = Read-Host "Do you want to proceed? (y|n) "
    if ($duplicateResponse -eq 'Y' -or $duplicateResponse -eq 'y') {
        Write-Host "-> yes proceeding with the operation."
    } else {
        Write-Host "Operation canceled."
        return 
    }
} 
#endregion

# Program variables.
$endDate = (Get-Date).AddDays(15000) # today + ~41.096 years
$pw = Start-GeneratePassword -Length 30
$password = ConvertTo-SecureString -String $pw -AsPlainText -Force
$certFilePathPart = "$exportDirPath\$name"

# Create export folder.
New-Item $exportDirPath -ItemType Directory | Out-Null
Write-Host "Export folder created '$exportDirPath'\"

# Create and export certificate.
$cert = Start-CreateAndInstallCertificate -Name $name -EndDate $endDate -CertStore $certStore

Start-ExportPrivateKey -CertFilePathPart $certFilePathPart -Certificate $cert -Password $password
Start-ExportPublicKey -CertFilePathPart $certFilePathPart -Certificate $cert
Start-ExportCertificateProperties -CertFilePathPart $certFilePathPart -Certificate $cert -Password $password

# Ask deinstall certificate.
$deinstallResponse = Read-Host "Do you want to deinstall the certificate? (y|n) "
if ($deinstallResponse -eq 'Y' -or $deinstallResponse -eq 'y') {
    Write-Host "-> yes (deinstall certificate)"
    Remove-Certificate -Thumbprint $cert.Thumbprint -Store $certStore
}

# Ask open export folder.
$openFolderResponse = Read-Host "Do you want to open the export folder? (y|n) "
if ($openFolderResponse -eq 'Y' -or $openFolderResponse -eq 'y') {
    Write-Host "-> yes (open export folder)"
    Invoke-Item $exportDirPath
}