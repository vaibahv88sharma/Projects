# This is a Post-Deployment PowerShell script to be called from Visual Studio to automate feature deployment

# Developed by: Ibraheem A. Ibraheem

# Feel free to adjust to your needs




# Parameters

$Url = "http://aespaspsas/sites/HVEDevSite/"

# $FeatureName is in the format: [ProjectName_FeatureName] and has nothing to do with feature title

$FeatureName = "HVEMasterPagesFeature"

#"PSproj_Feature1"
#"MasterProjOneFeature1"




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

write-host "Feature installed to 15 hive."



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


