#Add SharePoint PowerShell SnapIn if not already added 
if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null) { 
    Add-PSSnapin "Microsoft.SharePoint.PowerShell" 
} 

function Get-DocInventory() { 
 
#$site = new-object Microsoft.SharePoint.SPSite("http://SP01D-BRO/");  
$site = new-object Microsoft.SharePoint.SPSite("http://sbmysite");  
#$site = new-object Microsoft.SharePoint.SPSite("http://sp01d-bro:3047");  
$ServiceContext = [Microsoft.SharePoint.SPServiceContext]::GetContext($site);  
 
#Get UserProfileManager from the My Site Host Site context 
$ProfileManager = new-object Microsoft.Office.Server.UserProfiles.UserProfileManager($ServiceContext)    
$AllProfiles = $ProfileManager.GetEnumerator()  
 
foreach($profile in $AllProfiles)  
{  
    $DisplayName = $profile.DisplayName  
    $AccountName = $profile[[Microsoft.Office.Server.UserProfiles.PropertyConstants]::AccountName].Value  
 
 
    #Do not delete setup (admin) account from user profiles. Please enter the account name below 
	if (($AccountName -ne "kbtm\vsharma.adm") -and ($AccountName -ne "kbtm\vsharma") -and ($AccountName -ne "kbtm\svc_sp2016DB-prod") -and ($AccountName -ne "kbtm\svc_sp2016Mgr-prod") -and ($AccountName -ne "kbtm\svc_sp2016DB") -and ($AccountName -ne "sp01d-bro\administrator")) 
    { 
        $ProfileManager.RemoveUserProfile($AccountName); 
        $data = @{
			"AccountName" = $AccountName
			"DisplayName" = $profile.DisplayName
			"UserName" = $profile["UserName"].Value
        }
				
        New-Object PSObject -Property $data | Select "AccountName", "DisplayName" , "UserName"# , "CourseCode"
		
		write-host "Profile for account ", $AccountName,"---",$profile.DisplayName,"---", $profile["UserName"].Value , " has been deleted" 
    } 
 
}  
write-host "Finished." 
$site.Dispose()

}

Get-DocInventory  | Export-Csv -NoTypeInformation -Path C:\NewOutputMySite.csv
