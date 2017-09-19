# Add the PowerShell Snapin      
$snapin = Get-PSSnapin | Where-Object {$_.Name -eq 'Microsoft.SharePoint.Powershell'}      
if ($snapin -eq $null)       
{       
   Add-PSSnapin "Microsoft.SharePoint.Powershell"      
}    
# Get the siteURL    
function Get-DocInventory() {

$spWeb = Get-SPWeb http://spwfe1:8080/Courses/
# Enter the pages library name     
$listName = "Pages"    
 $list = $spWeb.Lists |? {$_.Title -eq $listName}  
 
  foreach ($item in $list.Items)     
  {    
    $itemFile = $item.File    
  
    if( $list.EnableVersioning -and $list.EnableMinorVersions )    
    {	
		write-host "Automatically published: "$item.Title" by Powershell"
		
		
		
        $data = @{
			"Title" = $item.Title
			"URL" = $item.URL
			"CourseCode" = $item["CourseCode"]
        }
		
        New-Object PSObject -Property $data | Select "Title", "URL" , "CourseCode"
		
		$itemFile.Publish("Automatically published by Powershell");		
		
		
    } 
  } 

  
$spWeb.Dispose()

}

Get-DocInventory  | Export-Csv -NoTypeInformation -Path C:\NewOutput.csv
