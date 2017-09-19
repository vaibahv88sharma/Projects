#Param ([string]$webUrl)

# load the SP snapin if it's not already loaded

if((Get-PSSnapin Microsoft.Sharepoint.Powershell -ErrorAction SilentlyContinue) -eq $null)

{

    Add-PSSnapin Microsoft.SharePoint.Powershell;

}

$webUrl = "http://staffportal.myselfserve.com.au/"

$web = get-spweb $webUrl

# publish the files for the design package

function PublishFilesInFolder($folder) {

    $folder.Files | ?{ $_.Item -ne $null -and $_.Item.Properties["HtmlDesignLockedFile"] -eq $null -and $_.MinorVersion -ne 0 } | %{

        Write-Host ("Publishing {0}" -f $_.Url);

        $_.Publish("design package deployment");

    }

    $folder.SubFolders | %{ PublishFilesInFolder $_; }

}

PublishFilesInFolder $web.GetFolder("_catalogs/masterpage/Kangan")