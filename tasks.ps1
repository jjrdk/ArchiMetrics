properties {
	$configuration = "Release"
	$platform = "Any CPU"
	$folderPath = ".\"
	$cleanPackages = $false
	$oldEnvPath = ""
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
	Exec { .\.nuget\nuget.exe pack ArchiMetrics.Analysis.nuspec }
}

task Test -depends Compile, Clean {
	'Running Tests'
	Exec { .\packages\NUnit.Runners.2.6.2\tools\nunit-console.exe .\ArchiMetrics.Analysis.Tests\bin\$configuration\ArchiMetrics.Analysis.Tests.dll }
	Exec { .\packages\NUnit.Runners.2.6.2\tools\nunit-console.exe .\ArchiMetrics.CodeReview.Tests\bin\$configuration\ArchiMetrics.CodeReview.Tests.dll }
	Exec { .\packages\NUnit.Runners.2.6.2\tools\nunit-console.exe .\ArchiMetrics.Common.Tests\bin\$configuration\ArchiMetrics.Common.Tests.dll }
}

task Compile -depends UpdatePackages { 
	$options = "/p:configuration=$configuration;platform=$platform"
	Exec { C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe "ArchiMetrics.sln" $options }
	'Executed Compile!'
}

task UpdatePackages -depends Clean {
	$packageConfigs = Get-ChildItem -Path .\ -Include "packages.config" -Recurse
	foreach($config in $packageConfigs){
        #Write-Host $config.DirectoryName
		.\.nuget\nuget.exe i $config.FullName -o packages -source https://nuget.org/api/v2/
	}
}

task Clean -depends CheckMsBuildPath { 
	Get-ChildItem $folderPath -include bin,obj -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }
	if($cleanPackages -eq $true){
		if(Test-Path "$folderPath\packages"){
			Get-ChildItem "$folderPath\packages" -Recurse | Where { $_.PSIsContainer } | foreach ($_) { Write-Host $_.fullname; remove-item $_.fullname -Force -Recurse }
		}
	}
	
	if(Test-Path "$folderPath\BuildOutput"){
		Get-ChildItem "$folderPath\BuildOutput" -Recurse | foreach ($_) { Write-Host $_.fullname; remove-item $_.fullname -Force -Recurse }
	}
}

task CheckMsBuildPath {
	$envPath = $Env:Path
	if($envPath.Contains("C:\Windows\Microsoft.NET\Framework\v4.0") -eq $false)
	{
		if(Test-Path "C:\Windows\Microsoft.NET\Framework\v4.0.30319")
		{
			$oldEnvPath = $envPath
			$Env:Path = $envPath + ";C:\Windows\Microsoft.NET\Framework\v4.0.30319"
		}
		else
		{
			throw "Could not determine path to MSBuild. Make sure you have .NET 4.0.30319 installed"
		}
	}
}

task ? -Description "Helper to display task info" {
	Write-Documentation
}
