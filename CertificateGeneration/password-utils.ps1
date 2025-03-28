function Start-GeneratePassword {
    [OutputType([securestring])]
    param (
        [int]$length = 30
    )

    # Define the character sets to use
    $upper = 65..90 | ForEach-Object {[char]$_}  # A-Z
    $lower = 97..122 | ForEach-Object {[char]$_} # a-z
    $digits = 48..57 | ForEach-Object {[char]$_} # 0-9
    $special = "!?%&#$+-/*=_.:;'^()[]".ToCharArray()

    # Combine all characters
    $allChars = $upper + $lower + $digits + $special

    # Generate the password
    $password = -join ((1..$length) | ForEach-Object { Get-Random -InputObject $allChars })

    # Ensure the password meets complexity requirements
    if (($password -match '[A-Z]') -and
        ($password -match '[a-z]') -and
        ($password -match '[0-9]') -and
        ($password -match "[$special]")) 
    {
        return Start-GeneratePassword -length $length  # Regenerate if it doesn't meet requirements
    }

    return $password
}