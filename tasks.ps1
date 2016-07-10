properties {
	$configuration = "Release"
	$platform = "Any CPU"
	$folderPath = ".\"
	$cleanPackages = $false
	$oldEnvPath = ""
	$buildOutput = "artifacts"
	$fwkVersions = "4.5.2"
}

task default -depends CleanUpMsBuildPath

task CleanUpMsBuildPath -depends BuildPackages {
	if($oldEnvPath -ne "")
	{
		Write-Host "Reverting Path variable"
		$Env:Path = $oldEnvPath
	}
}

task BuildPackages -depends Test {
	Exec { dnu pack ** --configuration $configuration}
#	Exec { .\.nuget\nuget.exe pack ArchiMetrics.Analysis.nuspec }
#	Exec { .\.nuget\nuget.exe pack ArchiMetrics.Analysis.symbols.nuspec -Symbols }
}

task Test -depends Compile, Clean {
	'Running Tests'
	foreach($fwk in $fwkVersions) {
		Write-Host "Building v. $fwk"
		$output = ".\$buildOutput\$fwk\$configuration"
		$analysis = Resolve-Path "$output\ArchiMetrics.Analysis.Tests.dll"
		$codereview = Resolve-Path "$output\ArchiMetrics.CodeReview.Rules.Tests.dll"
		
		Exec { .\packages\xunit.runner.console\2.1.0\tools\xunit-console.exe $analysis }
		Exec { .\packages\xunit.runner.console\2.1.0\tools\xunit-console.exe $codereview }
	}
}

task Compile -depends UpdatePackages {
	Exec { dnu build ** --configuration $configuration }
#	$msbuild = Resolve-Path "${Env:ProgramFiles(x86)}\MSBuild\14.0\Bin\MSBuild.exe"
#	foreach($fwk in $fwkVersions) {
#		$output = "..\$buildOutput\$fwk\$configuration"
#		$options = "/p:configuration=$configuration;platform=$platform;TargetFrameworkVersion=v$fwk;OutputPath=$output"
#		Exec { & $msbuild ArchiMetrics.sln $options }
#	}
	'Executed Compile!'
}

task UpdatePackages -depends Clean {
	Exec { & dnu restore}
#	$packageConfigs = Get-ChildItem -Path .\ -Include "packages.config" -Recurse
#	foreach($config in $packageConfigs){
#		#Write-Host $config.DirectoryName
#		Exec { .\.nuget\nuget.exe i $config.FullName -o packages -source https://nuget.org/api/v2/ }
#	}
}

task Clean { #-depends CheckMsBuildPath { 
	Get-ChildItem $folderPath -include bin,obj -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }
	if($cleanPackages -eq $true){
		if(Test-Path "$folderPath\packages"){
			Get-ChildItem "$folderPath\packages" -Recurse | Where { $_.PSIsContainer } | foreach ($_) { Write-Host $_.fullname; remove-item $_.fullname -Force -Recurse }
		}
	}
	
	if(Test-Path "$folderPath\$buildOutput"){
		Get-ChildItem "$folderPath\$buildOutput" -Recurse | foreach ($_) { Write-Host $_.fullname; remove-item $_.fullname -Force -Recurse }
	}
}

#task CheckMsBuildPath {
#	$envPath = $Env:Path
#	if($envPath.Contains("C:\Windows\Microsoft.NET\Framework\v4.0") -eq $false)
#	{
#		if(Test-Path "C:\Windows\Microsoft.NET\Framework\v4.0.30319")
#		{
#			$oldEnvPath = $envPath
#			$Env:Path = $envPath + ";C:\Windows\Microsoft.NET\Framework\v4.0.30319"
#		}
#		else
#		{
#			throw "Could not determine path to MSBuild. Make sure you have .NET 4.0.30319 installed"
#		}
#	}
#}

task ? -Description "Helper to display task info" {
	Write-Documentation
}
