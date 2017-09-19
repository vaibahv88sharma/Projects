#
# UninstallRemoveApp.ps1
#


# Gets all apps installed to the subsite you specify.
#$instances = Get-SPAppInstance -Web http://staffportal.myselfserve.com.au/sites/StaffPortalApp
$instances = Get-SPAppInstance -Web http://staffportal.myselfserve.com.au/sites/StaffPortal

# Sets the $instance variable to the app with the title you supply.
$instance = $instances | where {$_.Title -eq 'MobileInfo'}

# Uninstalls the app from the subsite.
Uninstall-SPAppInstance -Identity $instance
