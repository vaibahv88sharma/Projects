#https://technet.microsoft.com/en-us/library/dn197169.aspx
#https://blogs.msdn.microsoft.com/spses/2013/10/22/office-365-configure-hybrid-search-with-directory-synchronization-password-sync/

Add-PSSnapin Microsoft.SharePoint.PowerShell

#Variables
#SharePoint 2013 Management Shell
$stscertpfx="c:\cert\stscertO365.pfx"
$stscertcer="c:\cert\stscertO365.cer"
$stscertpassword="LS1setup!"
$spcn="*.kbtm.kangan.edu.au" # replace yourdomainname with your onpremise domain that you added to Office 365
#kangan.edu.au
#$spsite="http://staffportal.myselfserve.com.au/"
$spsite="http://spwfe03p-bro/"
$spoappid="00000003-0000-0ff1-ce00-000000000000"


#Update the Certificate on the STS
$pfxCertificate=New-Object System.Security.Cryptography.X509Certificates.X509Certificate2 $stscertpfx, $stscertpassword, 20
Set-SPSecurityTokenServiceConfig -ImportSigningCertificate $pfxCertificate
# Type Yes when prompted with the following message.


#compare the below mentioned values from each row, they must match
$pfxCertificate
(Get-SPSecurityTokenServiceConfig).LocalLoginProvider.SigningCertificate




#Restart IIS so STS Picks up the New Certificate
& iisreset
& net stop SPTimerV4
& net start SPTimerV4


#Do Some Conversions With the Certificates to Base64
$pfxCertificate = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2 -ArgumentList $stscertpfx,$stscertpassword
$pfxCertificateBin = $pfxCertificate.GetRawCertData()

$cerCertificate = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2
$cerCertificate.Import($stscertcer)

$cerCertificateBin = $cerCertificate.GetRawCertData()
$credValue = [System.Convert]::ToBase64String($cerCertificateBin)


#Establish Remote Windows PowerShell Connection with Office 365
enable-psremoting
#When prompted with Are you sure you want to perform this action? type Yes for all of the actions.
new-pssession

Import-Module MSOnline -force –verbose 
Import-Module MSOnlineExtended -force –verbose


#Log on as a Global Administrator for Office 365
Connect-MsolService


##### DO NOT USE BELOW LINES
## Check if the server certificates already exists
$msp11 = Get-MsolServicePrincipal -AppPrincipalId $spoappid
$spns11 = $msp11.ServicePrincipalNames
$spns11

## Remove the previous server certificates
$msp1111 = Get-MsolServicePrincipal -AppPrincipalId $spoappid
$spns1111 = $msp1111.ServicePrincipalNames
$spns1111.Remove("$spoappid/$spcn")
Set-MsolServicePrincipal -AppPrincipalId $spoappid -ServicePrincipalNames $spns1111

# Remove-MsolServicePrincipalCredential 
######


#Register the On-Premise STS as Service Principal in Office 365
New-MsolServicePrincipalCredential -AppPrincipalId $spoappid -Type asymmetric -Usage Verify -Value $credValue

$SharePoint = Get-MsolServicePrincipal -AppPrincipalId $spoappid

$spns = $SharePoint.ServicePrincipalNames

$spns.Add("$spoappid/$spcn")

Set-MsolServicePrincipal -AppPrincipalId $spoappid -ServicePrincipalNames $spns

$spocontextID = (Get-MsolCompanyInformation).ObjectID

$spoappprincipalID = (Get-MsolServicePrincipal -ServicePrincipalName $spoappid).ObjectID

$sponameidentifier = "$spoappprincipalID@$spocontextID"


#Finally Establish in the On-Premise Farm a Trust with the ACS
$site=Get-Spsite "$spsite"

$appPrincipal = Register-SPAppPrincipal -site $site.rootweb -nameIdentifier $sponameidentifier -displayName "SharePoint Online"

Set-SPAuthenticationRealm -realm $spocontextID

New-SPAzureAccessControlServiceApplicationProxy -Name "ACS 2" -MetadataServiceEndpointUri "https://accounts.accesscontrol.windows.net/metadata/json/1/" -DefaultProxyGroup

New-SPTrustedSecurityTokenIssuer -MetadataEndpoint "https://accounts.accesscontrol.windows.net/metadata/json/1/" -IsTrustBroker -Name "ACS 2"

