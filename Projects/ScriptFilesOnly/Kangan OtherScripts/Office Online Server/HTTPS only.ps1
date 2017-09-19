#
# HTTPS_only.ps1
#

# HTTPS certificat enot added to OOS but the OOS itself was redirected to load balancer so skipped the following step

#### Skipped :  #New-OfficeWebAppsFarm -InternalUrl "https://server.contoso.com" -ExternalUrl "https://wacweb01.contoso.com" -CertificateName "OfficeWebApps Certificate" -EditingEnabled

New-OfficeWebAppsFarm -InternalUrl "http://docs.myselfserve.com.au/" -ExternalUrl "https://docs.myselfserve.com.au/" -SSLOffloaded -EditingEnabled
#New-OfficeWebAppsFarm -InternalUrl "https://docs.myselfserve.com.au/" -ExternalUrl "https://docs.myselfserve.com.au/" -SSLOffloaded -EditingEnabled


## Test following to check if its working
###https://docs.myselfserve.com.au/hosting/discovery


########### SharePoint Server

#FQDN:
New-SPWOPIBinding -ServerName docs.myselfserve.com.au
#New-SPWOPIBinding -ServerName spapp04p-bro.kbtm.kangan.edu.au


Set-SPWOPIZone -zone "external-https"
Get-SPWOPIZone

$Farm = Get-SPFarm
$Farm.Properties.Add("WopiLegacySoapSupport", "https://docs.myselfserve.com.au/x/_vti_bin/ExcelServiceInternal.asmx");
$Farm.Update();

Set-SPWOPIZone -zone "internal-http"
#Set-SPWOPIZone -zone "external-https"
Get-SPWOPIZone
$Farm = Get-SPFarm
$Farm.Properties["WopiLegacySoapSupport"]= "https://docs.myselfserve.com.au/x/_vti_bin/ExcelServiceInternal.asmx"
$Farm.Update();

New-OfficeWebAppsHost -Domain staffportal.myselfserve.com.au
New-OfficeWebAppsHost -Domain spwfe03p-bro
New-OfficeWebAppsHost -Domain spwfe04p-bro


Update-SPWOPIProofKey -ServerName "docs.myselfserve.com.au"