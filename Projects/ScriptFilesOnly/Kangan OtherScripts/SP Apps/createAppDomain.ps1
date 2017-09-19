#Foundation Service 

$account = Get-SPManagedAccount KBTM\svc_sp2016DB
$appPoolSubSvc = New-SPServiceApplicationPool -Name MicrosoftSharePointFoundationSubscriptionServicePool -Account $account
$appSubSvc = New-SPSubscriptionSettingsServiceApplication -ApplicationPool $appPoolSubSvc -Name MicrosoftSharePointFoundationSubscriptionService -DatabaseName AppManagementKanganBroDB
#$appSubSvc = New-SPSubscriptionSettingsServiceApplication -ApplicationPool MicrosoftSharePointFoundationSubscriptionServicePool -Name MicrosoftSharePointFoundationSubscriptionService -DatabaseName AppManagementKanganBroDB
$proxySubSvc = New-SPSubscriptionSettingsServiceApplicationProxy -ServiceApplication $appSubSvc



#incorrect collation.  Rebuild the database with the Latin1_General_CI_AS_KS_WS collation or create a new database.
#KBTM\svc_sp2016DB






#App Service
$account = Get-SPManagedAccount KBTM\svc_sp2016DB
$appPoolAppSvc = New-SPServiceApplicationPool -Name AppManagementServiceAppPool -Account $account
$appAppSvc = New-SPAppManagementServiceApplication -ApplicationPool $appPoolAppSvc -Name AppManagementServiceApp -DatabaseName AppManagementServiceAppKanganBroDB
#$appAppSvc = New-SPAppManagementServiceApplication -ApplicationPool AppManagementServiceAppPool -Name AppManagementServiceApp -DatabaseName AppManagementServiceAppKanganBroDB
$proxyAppSvc = New-SPAppManagementServiceApplicationProxy -ServiceApplication $appAppSvc



#Access Service
$account = Get-SPManagedAccount KBTM\svc_sp2016DB
$accessAppPoolAppSvc = New-SPServiceApplicationPool -Name AccessServiceAppPool -Account $account
$accessAppAppSvc = New-SPAccessServiceApplication -ApplicationPool $accessAppPoolAppSvc -Name AccessServiceApp -DatabaseName AccessServiceAppKanganBroDB

$proxyAppSvc = New-SPAppManagementServiceApplicationProxy -ServiceApplication $accessAppAppSvc

