#Add SharePoint PowerShell SnapIn if not already added 
if ((Get-PSSnapin "Microsoft.SharePoint.PowerShell" -ErrorAction SilentlyContinue) -eq $null) { 
    Add-PSSnapin "Microsoft.SharePoint.PowerShell" 
} 

function Get-DocInventory() { 
  
 
	#$spWebApp = $Site.WebApplication
	$spWebApp = Get-SPWebApplication "http://SP01D-BRO"
	foreach($allSites in $spWebApp.Sites)
	{
		foreach($Web in $allSites.AllWebs)
		{
			foreach($list in $Web.Lists)
			{
				#if($list.BaseTemplate -eq "DocumentLibrary" -and $list.AllowContentTypes -eq $true)             
				#if(($list.BaseTemplate -eq "DocumentLibrary") -and ($list.NoCrawl -eq 0))
				if(($list.Title -ne "Pages") -and ($list.NoCrawl -eq 0)-and ($list.Title -ne "Site Pages"))
				{
					if(-not ($systemlibs -Contains $list.Title))
					{
						#if ($list.AllowContentTypes -eq $true)
						#{		
							foreach ($contenttype in $list.ContentTypes)
							{						
								$data = @{
									"WebTitle" = $web.Title
									"WebUrl" = $web.URL
									"ListTitle" = $list.Title
									"IsCrawlled" = $list.NoCrawl
									"ContentType" = $contenttype.Name
									#"UserName" = $profile["UserName"].Value
								}
										
								New-Object PSObject -Property $data | Select "WebTitle", "WebUrl" , "ListTitle", "IsCrawlled", "ContentType"
								#write-host "Profile for account ", $AccountName,"---",$profile.DisplayName,"---", $profile["UserName"].Value , " has been deleted" 				
								write-host $list.NoCrawl
								#Set Search visibility property to exclude the site from search
								$_.NoCrawl = $true    
								$_.Update()
								write-host $list.NoCrawl
							}
						#}
					}
				}
			}
		}
	} 


}

Get-DocInventory  | Export-Csv -NoTypeInformation -Path C:\IsDocSearchable.csv