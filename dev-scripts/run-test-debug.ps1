param(
    [Parameter(Mandatory=$true)]
    [string]$TestName
)

try {
    # Store the original value of PWDEBUG if it exists
    $originalPWDEBUG = $env:PWDEBUG

    # Set PWDEBUG for the duration of this script
    $env:PWDEBUG = "1"

    # Run the specified test using the workspace root path
    dotnet test "$env:WORKSPACE_ROOT/tests/ctf-sandbox.tests" --filter "FullyQualifiedName=$TestName"
}
finally {
    # Restore the original PWDEBUG value (or remove it if it wasn't set)
    if ($null -eq $originalPWDEBUG) {
        Remove-Item Env:PWDEBUG -ErrorAction SilentlyContinue
    } else {
        $env:PWDEBUG = $originalPWDEBUG
    }
}