function WaitForJobToFinish([string]$SolutionFileName)
{ 
    $JobName = "*solution-deployment*$SolutionFileName*"
    $job = Get-SPTimerJob | ?{ $_.Name -like $JobName }
    if ($job -eq $null) 
    {
        Write-Host 'Timer job not found'
    }
    else
    {
        $JobFullName = $job.Name
        Write-Host -NoNewLine "Waiting to finish job $JobFullName"
        
        while ((Get-SPTimerJob $JobFullName) -ne $null) 
        {
            Write-Host -NoNewLine .
            Start-Sleep -Seconds 2
        }
        Write-Host  "Finished waiting for job.."
    }
}



Add-PsSnapin Microsoft.SharePoint.PowerShell
 
$CurrentDir=$args[0]
$solutionName="HVE.SharePoint.wsp"
$SolutionPath=$CurrentDir + "\"+$solutionName 

Write-Host 'Going to uninstall solution'
Uninstall-SPSolution -identity $solutionName  -WebApplication http://aespaspsas/ -confirm:$false 

Write-Host 'Waiting for the job to finish'
WaitForJobToFinish

Write-Host 'Going to remove solution'
Remove-SPSolution –Identity $solutionName -confirm:$false
 
Write-Host 'Going to add solution'
Add-SPSolution $SolutionPath
 
Write-Host 'Going to install solution'
Install-SPSolution –Identity  $solutionName  -WebApplication http://aespaspsas/  –GACDeployment

Write-Host 'Waiting for the job to finish' 
WaitForJobToFinish


Remove-PsSnapin Microsoft.SharePoint.PowerShell
