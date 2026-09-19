param(
    [Parameter(Mandatory=$true)]
    [string[]]$TestName
)

try {
    # Store the original value of PWDEBUG if it exists
    $originalPWDEBUG = $env:PWDEBUG
    $originalTESTINGPLATFORM_DEBUG = $env:TESTINGPLATFORM_DEBUG
    $originalVSTEST_HOST_DEBUG = $env:VSTEST_HOST_DEBUG


    # Set PWDEBUG for the duration of this script
    $env:PWDEBUG = "1"
    $env:TESTINGPLATFORM_DEBUG = "1"
    $env:VSTEST_HOST_DEBUG = "1"

    # Build filter expression joining multiple test names with |
    $filter = ($TestName | ForEach-Object { "FullyQualifiedName=$_" }) -join "|"

    # Run the specified test(s) using the workspace root path
    dotnet test "$env:WORKSPACE_ROOT/tests/ctf-sandbox.tests" --filter $filter
}
finally {
    # Restore the original PWDEBUG value (or remove it if it wasn't set)
    if ($null -eq $originalPWDEBUG) {
        Remove-Item Env:PWDEBUG -ErrorAction SilentlyContinue
        Remove-Item Env:TESTINGPLATFORM_DEBUG -ErrorAction SilentlyContinue
        Remove-Item Env:VSTEST_HOST_DEBUG -ErrorAction SilentlyContinue
    } else {
        $env:PWDEBUG = $originalPWDEBUG
        $env:TESTINGPLATFORM_DEBUG = $originalTESTINGPLATFORM_DEBUG
        $env:VSTEST_HOST_DEBUG = $originalVSTEST_HOST_DEBUG
    }
}