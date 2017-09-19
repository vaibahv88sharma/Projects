#
# SubscriptionServiceApplication.ps1
#
#Provide the service account as per your configuration 

$account=Get-SPManagedAccount -Identity kbtm\svc_sp2016Mgr-prod

#configure Subscription Service Application for Sharepoint

$appPoolSubsvc=New-SPServiceApplicationPool -Name "SubscriptionServericeAppPoolBKI" -Account $account



#New-SPSubscriptionSettingsServiceApplication : The specified database has an incorrect collation.  
#Rebuild the database with the Latin1_General_CI_AS_KS_WS collation or create a new database.
#At line:1 char:12
#+ $appSubSvc=New-SPSubscriptionSettingsServiceApplication -ApplicationPool
#$appPoo ...
#+ ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
#~~~
#    + CategoryInfo          : InvalidData: (Microsoft.Share...viceApplication:
#   SPCmdletNewSubs...viceApplication) [New-SPSubscript...viceApplication], SP
#  InvalidCollationException
#    + FullyQualifiedErrorId : Microsoft.SharePoint.PowerShell.SPCmdletNewSubsc
#   riptionSettingsServiceApplication




$appSubSvc=New-SPSubscriptionSettingsServiceApplication -ApplicationPool $appPoolSubsvc -Name "Subscription Service Application BKI" -DatabaseName SubscriptonServiceDbBKI
$proxySubSvc=New-SPSubscriptionSettingsServiceApplicationProxy -ServiceApplication $appSubSvc
Write-Host "Subscirption Service Application Created"