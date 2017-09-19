#
# WFE_WebFrontEnd_as_WFmanager.ps1
#


Register-SPWorkflowService –SPSite "http://sp01d-bro/" –WorkflowHostUri "http://sp01d-bro:12290" –AllowOAuthHttp -force
Register-SPWorkflowService -SPSite "http://sp01d-bro/" -WorkflowHostUri "http://sp01d-bro:12291"

Register-SPWorkflowService -SPSite "http://SPWFE03P-BRO/" -WorkflowHostUri "http://SPWFE03P-BRO:12291" -AllowOAuthHttp -force


Register-SPWorkflowService -SPSite "http://spwfe03p-bro/sites/StaffPortal/" -WorkflowHostUri "http://spwfe03p-bro:12291" -AllowOAuthHttp -force

Register-SPWorkflowService -SPSite "http://staffportal.myselfserve.com.au/sites/StaffPortal/" -WorkflowHostUri "http://spwfe03p-bro:12291" -AllowOAuthHttp -force



IPCONFIG /FLUSHDNS