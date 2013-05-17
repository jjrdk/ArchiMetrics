param (
	[string]$settings = ".\settings.xml",
	[string]$tfexe = "c:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE"
)

$tf = Resolve-Path $tfexe
$currentLocation = Get-Location

function Get-SpecificVersion([string]$location, [string]$version) {
    Set-Location $tf
	& .\tf.exe get $location /version:$version /recursive /force
	Set-Location $currentLocation
}

function Get-Dependencies([string] $folder){
	$files = Get-ChildItem -Path $folder -Include GetDependencies.*,GetComponents.*,BuildLocalDeployment.* -Recurse | where { ! $_.PSIsContainer }
	if($files -ne $null){
		foreach($f in $files){
			Write-Host $f.FullName
			$fileName = $f.FullName
			Set-Location $f.Directory
			& cmd /c "$fileName"
		}
	}
	Set-Location $currentLocation
}

function UpdateFiles(){
	[xml]$s = Get-Content $settings
	$root = $s.ReportConfig
	foreach($pc in $root.Project){
		$revision = $pc.Revision
		foreach($repo in $pc.Repo){
			Get-SpecificVersion $repo.Source $revision
			Get-Dependencies $repo.Source
		}
	}
	Set-Location $currentLocation
}

function CollectMetrics(){
	& .\ArchiMeter.DataLoader.exe $settings
	Set-Location $currentLocation
}

cls
UpdateFiles
CollectMetrics