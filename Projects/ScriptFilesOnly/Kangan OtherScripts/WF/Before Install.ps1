# To be run in Workflow Manager PowerShell console that has both Workflow Manager and Service Bus installed.

# Create new SB Farm
$SBCertificateAutoGenerationKey = ConvertTo-SecureString -AsPlainText  -Force  -String '***** Replace with Service Bus Certificate Auto-generation key ******' -Verbose;


New-SBFarm -SBFarmDBConnectionString 'Data Source=SPAPP03P-BRO;Initial Catalog=SbManagementDB;Integrated Security=True;Encrypt=False' -InternalPortRangeStart 9000 -TcpPort 9354 -MessageBrokerPort 9356 -RunAsAccount 'kbtm\svc_sp2016DB-prod' -AdminGroup 'BUILTIN\Administrators' -GatewayDBConnectionString 'Data Source=SPAPP03P-BRO;Initial Catalog=SbGatewayDatabase;Integrated Security=True;Encrypt=False' -CertificateAutoGenerationKey $SBCertificateAutoGenerationKey -MessageContainerDBConnectionString 'Data Source=SPAPP03P-BRO;Initial Catalog=SBMessageContainer01;Integrated Security=True;Encrypt=False' -Verbose;

# To be run in Workflow Manager PowerShell console that has both Workflow Manager and Service Bus installed.

# Create new WF Farm
$WFCertAutoGenerationKey = ConvertTo-SecureString -AsPlainText  -Force  -String '***** Replace with Workflow Manager Certificate Auto-generation key ******' -Verbose;


New-WFFarm -WFFarmDBConnectionString 'Data Source=SPAPP03P-BRO;Initial Catalog=WFManagementDB;Integrated Security=True;Encrypt=False' -RunAsAccount 'kbtm\svc_sp2016DB-prod' -AdminGroup 'BUILTIN\Administrators' -HttpsPort 12290 -HttpPort 12291 -InstanceDBConnectionString 'Data Source=SPAPP03P-BRO;Initial Catalog=WFInstanceManagementDB;Integrated Security=True;Encrypt=False' -ResourceDBConnectionString 'Data Source=SPAPP03P-BRO;Initial Catalog=WFResourceManagementDB;Integrated Security=True;Encrypt=False' -CertificateAutoGenerationKey $WFCertAutoGenerationKey -Verbose;

# Add SB Host
$SBRunAsPassword = ConvertTo-SecureString -AsPlainText  -Force  -String '***** Replace with RunAs Password for Service Bus ******' -Verbose;


Add-SBHost -SBFarmDBConnectionString 'Data Source=SPAPP03P-BRO;Initial Catalog=SbManagementDB;Integrated Security=True;Encrypt=False' -RunAsPassword $SBRunAsPassword -EnableFirewallRules $true -CertificateAutoGenerationKey $SBCertificateAutoGenerationKey -Verbose;

Try
{
    # Create new SB Namespace
    New-SBNamespace -Name 'WorkflowDefaultNamespace' -AddressingScheme 'Path' -ManageUsers 'kbtm\svc_sp2016DB-prod','svc_sp2016DB-prod@KBTM' -Verbose;

    Start-Sleep -s 90
}
Catch [system.InvalidOperationException]
{
}

# Get SB Client Configuration
$SBClientConfiguration = Get-SBClientConfiguration -Namespaces 'WorkflowDefaultNamespace' -Verbose;

# Add WF Host
$WFRunAsPassword = ConvertTo-SecureString -AsPlainText  -Force  -String '***** Replace with RunAs Password for Workflow Manager ******' -Verbose;


Add-WFHost -WFFarmDBConnectionString 'Data Source=SPAPP03P-BRO;Initial Catalog=WFManagementDB;Integrated Security=True;Encrypt=False' -RunAsPassword $WFRunAsPassword -EnableFirewallRules $true -SBClientConfiguration $SBClientConfiguration -EnableHttpPort  -CertificateAutoGenerationKey $WFCertAutoGenerationKey -Verbose;
