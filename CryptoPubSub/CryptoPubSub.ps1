# NuGet路徑
$nugetUrl = "http://localhost:5555/v3/index.json"
$specName = $MyInvocation.MyCommand.Name.Substring(0, $MyInvocation.MyCommand.Name.indexof(".ps1")) + ".csproj"

Write-host $specName

$paths=@(
    "bin",
    "obj",
    "*.nupkg"
)

foreach($path in $paths){
    if(Test-Path $path){
        Remove-Item $path -Recurse
        Write-host Remove $path
    }
}

$spec = (Get-Content $specName)

# 取版本號 (ex: 1.0.0 要符合此命名規範)
$version = $spec[4].Substring($spec[4].indexof("<version>") + 9)
$version = $version.Substring(0, $version.indexof("</version>")).split(".")

if([int]$version[2] -eq 99)
{
	#版本號99進位+1
	$version[1] = [int]$version[1] + 1
	$version[2] = 0
}
else
{
	# 版本號+1
	$version[2] = [int]$version[2] + 1
}

$ofs = "."
$spec[4] = "<version>$version</version>"

# 回寫版本號
Set-Content $specName -Value $spec

#編譯
dotnet build -c release
#打包
dotnet pack -c release -o .
#上傳nuget
dotnet nuget push -s $nugetUrl *.nupkg

read-host