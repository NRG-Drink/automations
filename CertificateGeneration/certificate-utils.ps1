
function Get-CertificatesByName {
    param (
        [string]$Name,
        [string]$Store = "cert:\CurrentUser\My"
    )
    
    Get-ChildItem $Store | Where-Object { $_.FriendlyName -eq $Name -or $_.Name -eq $Name }
}

function Start-CreateAndInstallCertificate {
    [OutputType([System.Security.Cryptography.X509Certificates.X509Certificate2])]
    param (
        [string]$Name,
        [datetime]$EndDate,
        [string]$CertStore = "cert:\CurrentUser\My",
        [int]$KeyLength = 3072
    )
    
    try {
        $cert = New-SelfSignedCertificate `
            -Type Custom `
            -DnsName $Name `
            -CertStoreLocation $CertStore `
            -NotAfter $EndDate `
            -KeySpec KeyExchange `
            -KeyExportPolicy Exportable `
            -KeyLength $KeyLength `
            -Subject $Name `
            -FriendlyName $Name `
            -ErrorAction Stop
    
        Write-Host "  Certificate is created and installed at '$CertStore'"
        return $cert
    }
    catch {
        Write-Error "Error in crete certificate: $_"
    }

    return $null;
}

function Start-ExportPublicKey {
    param (
        [string]$CertFilePathPart,
        [System.Security.Cryptography.X509Certificates.X509Certificate2]$Certificate
    )

    $file = "$CertFilePathPart.cer"
    
    try {
        $Certificate 
            | Export-Certificate `
                -FilePath $file `
                -ErrorAction Stop `
            | Out-Null
    
        Write-Host "  Certificate public-key exported at '$file'"
    }
    catch {
        Write-Error "Error in export public-key: $_"
    }
}

function Start-ExportPrivateKey {
    param (
        [string]$CertFilePathPart,
        [System.Security.Cryptography.X509Certificates.X509Certificate2]$Certificate,
        [securestring]$Password
    )

    $file = "$CertFilePathPart.pfx"

    try {
        $Certificate `
            | Export-PfxCertificate `
                -FilePath $file `
                -Password $Password `
                -ErrorAction Stop `
            | Out-Null
            
        Write-Host "  Certificate private-key exported at '$file'"
    }
    catch {
        Write-Error "Error in export private-key: $_"
    }
}

function Start-ExportCertificateProperties {
    param (
        [string]$CertFilePathPart,
        [System.Security.Cryptography.X509Certificates.X509Certificate2]$Certificate,
        [securestring]$Password
    )

    $file = "$CertFilePathPart.props.txt"
    
    try {
        # Select certificate properties for export.
        $props = @{
            Name = $Certificate.FriendlyName
            Thumbprint = $Certificate.Thumbprint 
            StartDate = $Certificate.NotBefore
            EndDate = $Certificate.NotAfter
            PublishDate = Get-Date
            Publisher = $Env:ComputerName
            Password = [System.Net.NetworkCredential]::new("", $Password).password
        }
    
        # Export certificate properties to file (formatted).
        New-Object PSObject -Property $props `
            | Select-Object Name, Thumbprint, StartDate, EndDate, PublishDate, Publisher, Password `
            > $file
    
        Write-Host "  Certificate properties exported at '$file'"
    }
    catch {
        Write-Error "Error in cert-property export: $_"
    }
}

function Remove-Certificate {
    param (
        [string]$Thumbprint,
        [string]$Store = "cert:\CurrentUser\My"
    )

    $path = "$Store\$Thumbprint"

    try {
        Remove-Item -Path $path -ErrorAction Stop

        Write-Host "  Certificate removed '$path'"
    }
    catch {
        Write-Error "Error in remove certificate: $_"
    }
}