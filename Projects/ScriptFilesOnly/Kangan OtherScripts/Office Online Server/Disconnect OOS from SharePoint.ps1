#
# Disconnect_OOS_from_SharePoint.ps1
#


### Run  on SharePoint farm

Remove-SPWOPIBinding -All:$true
New-SPWOPIBinding -ServerName docs.myselfserve.com.au -AllowHTTP