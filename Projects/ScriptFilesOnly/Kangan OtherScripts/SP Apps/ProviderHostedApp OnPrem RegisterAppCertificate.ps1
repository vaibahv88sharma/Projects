$issuerID = "0a99d544-17e7-4d37-822a-7ec250c9b9bb" 
$publicCertPath = "C:\SPAppCert\17Feb\SP2016ProviderHostedApp.cer" 
$certificate = Get-PfxCertificate $publicCertPath 
$web = Get-SPWeb "http://sp01d-bro/intranet/" 
$realm = Get-SPAuthenticationRealm -ServiceContext $web.Site 
$fullAppIdentifier = $issuerId + ‘@’ + $realm 
New-SPTrustedSecurityTokenIssuer -Name "High Trust App" -Certificate $certificate -RegisteredIssuerName $fullAppIdentifier -IsTrustBroker 
iisreset