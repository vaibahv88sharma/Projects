#      c:\windows\System32\windowspowershell\v1.0\powershell.exe -file "C:\Users\svc_sp2016DB-prod\Documents\DevelopmentProj\BKIAuditing\BKIAuditing\PostDeploymentDeploySolution.ps1"
#      %SystemRoot%\System32\WindowsPowerShell\v1.0\PowerShell.exe "start-process powershell.exe -ArgumentList '-NoExit', '$(ProjectDir)PostDeploymentDeploySolution.ps1'"

#################   WORKING POST DEPLOYMENT SCRIPT    ###################

#      %SystemRoot%\sysnative\WindowsPowerShell\v1.0\powershell "start-process powershell.exe -ArgumentList '-NoExit', '$(ProjectDir)PostDeploymentDeploySolution.ps1'"

#		http://www.youvegotcode.com/2014/11/error-feature-with-id-is-not-installed.html




# This is a Post-Deployment PowerShell script to be called from Visual Studio to automate feature deployment

# Developed by: Vaibhav Sharma

# Feel free to adjust to your needs


# Parameters

$Url = "http://spwfe03p-bro/sites/staffportal/"

$UrlWebApplication = "http://spwfe03p-bro/"

$solutionName = "bkiauditing.wsp"

# $FeatureName is in the format: [ProjectName_FeatureName] and has nothing to do with feature title

$FeatureName = "BKIAuditing_Feature1"#"BKIAuditingFeature12"


write-host "Started.."



# When PowerShell version is 2 or more, create new thread for 1st invocation then reuses it

$ver = $host | select version.

if ($ver.Version.Major -gt 1) {

       $host.Runspace.ThreadOptions = "ReuseThread"

       write-host "ReuseThread"

} 



# Add SharePoint snap-in if needed

if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null) {

    Add-PSSnapin "Microsoft.SharePoint.PowerShell"

}

write-host "Microsoft.SharePoint.PowerShell snap-in is loaded."

# Install Solution

write-host "Installing Solution."
Install-SPSolution -Identity $solutionName -WebApplication $UrlWebApplication -GacDeployment

do
{
Write-Host "." -NoNewline -ForeGroundColor Green;
Start-Sleep -Seconds 5; 
try
{
write-host 'Verifying if the solution installation is installed'
$testsolution = Get-SPSolution -Identity $solutionName
}
catch
{}
}while(!$testsolution.Deployed);
#}while(!$testsolution.JobStatus -eq "Deployed");

# If feature already enabled then disables & retract it

$feature = Get-SPFeature -Site $Url | Where {$_.DisplayName -eq $FeatureName}

if ($feature -ne $null) {

       write-host "Feature found in target site " $Url

       Disable-SPFeature -Identity $FeatureName -Url $Url -confirm:$false

       write-host "Feature deactivated."

       Uninstall-SPFeature $FeatureName

       write-host "Feature uninstalled."

}

else{

       write-host "Feature was not found in target site: " $Url

}



# installing the feature

Install-SPFeature -Path $FeatureName

echo ""

write-host "Feature installed to 16 hive."



# If feature already enabled then disable it, because the enable is not complete (web parts are not copied


# to web part gallery in site collection)

# Note: feature will get automatically enabled when installed if scope is either 'SiteCollection' or 'Web'

$feature = Get-SPFeature -Site $Url | Where {$_.Displayname -like $FeatureName}

if ($feature -ne $null) {

       Write-Host "Feature is already activated at: " $Url

       Disable-SPFeature -Identity $FeatureName -Url $Url -confirm:$false

       write-host "Feature deactivated."

}

else {

       Write-Host "Feature is not activated at: " $Url

}



# Enable the feature

Enable-SPFeature -Identity $FeatureName -Url $Url

Write-Host "Feature got activated at: " $Url



write-host "Finished."


#Read-Host -Prompt 'Input your server  name'