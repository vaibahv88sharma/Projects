############  HTTP only, not for HTTPS


#Office Online Server:
New-OfficeWebAppsFarm -InternalURL "http://docs.myselfserve.com.au/" -AllowHttp -EditingEnabled


#SharePoint Server

#FQDN:
New-SPWOPIBinding -ServerName docs.myselfserve.com.au -AllowHTTP
#New-SPWOPIBinding -ServerName docs.myselfserve.com.au.kbtm.kangan.edu.au -AllowHTTP

Get-SPWOPIZone
Set-SPWOPIZone -zone "internal-http"

(Get-SPSecurityTokenServiceConfig).AllowOAuthOverHttp
$config = (Get-SPSecurityTokenServiceConfig)
$config.AllowOAuthOverHttp = $true
$config.Update()

$Farm = Get-SPFarm
$Farm.Properties.Add("WopiLegacySoapSupport", "http://docs.myselfserve.com.au/x/_vti_bin/ExcelServiceInternal.asmx");
$Farm.Update();


#$Farm = Get-SPFarm
#$Farm.Properties["WopiLegacySoapSupport"]= "http://docs.myselfserve.com.au/x/_vti_bin/ExcelServiceInternal.asmx"
#$Farm.Update();

$Farm = Get-SPFarm
$Farm.Properties["WopiLegacySoapSupport"]