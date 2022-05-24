#Quick script to select an output csv and open articles less than X days old
[cmdletbinding()]
	param()

Add-Type -AssemblyName System.Windows.Forms

Write-Output "Select Files to process for report."

$FileBrowser = New-Object System.Windows.Forms.OpenFileDialog -Property @{ 
InitialDirectory = $PSScriptRoot
}

$null = $FileBrowser.ShowDialog()
if($FileBrowser.FileNames.Length -ne 0)
{
    [int]$age = Read-Host "How many days back to be displayed?"
    $age = $age*-1

    $csv = Import-Csv -Path $FileBrowser.FileName 

    $chromekey = 'HKLM:\SOFTWARE\Classes\ChromeHTML\shell\open\command'
    $chromepath = (Get-Item -Path $chromekey).GetValue("") -replace " *--.*", ""
    foreach ($item in $csv)
    {
        if (([datetime]::Parse($item.LastUpdate)).date -gt [datetime]::Now.AddDays($age))
        {
            Write-Output "opening article:"
            Write-Output $item.Title
            Write-Output $item.Url
            Start-Process -FilePath $chromepath -ArgumentList $item.Url
        }
    }
}
else
{
    Write-Output "No file selected!"
}
Write-Host -NoNewLine 'Press any key to continue...'
$null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown')