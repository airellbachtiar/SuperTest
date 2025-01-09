# Set the directory paths
$localGitFolder = "C:\Dev\GitLocalFolderTest"
$repoClonePath = "C:\Dev\SuperRequirementsStorage"

# File paths to copy
$file1 = "Test1.reqif"
$file2 = "Test2.reqif"

# Ensure the local folder exists
if (!(Test-Path -Path $localGitFolder)) {
    Write-Host "Creating directory: $localGitFolder"
    New-Item -ItemType Directory -Path $localGitFolder
} else {
    Write-Host "Directory already exists: $localGitFolder"
}

# Clone the repository
$repositoryUrl = "https://supermodels.visualstudio.com/Requirements/_git/SuperRequirementsStorage"
if (!(Test-Path -Path $repoClonePath)) {
    Write-Host "Cloning repository from $repositoryUrl to $repoClonePath"
    git clone $repositoryUrl $repoClonePath
} else {
    Write-Host "Repository folder already exists: $repoClonePath"
}

# Copy the files to the local Git folder
if (Test-Path -Path $file1 -PathType Leaf) {
    Write-Host "Copying $file1 to $localGitFolder"
    Copy-Item -Path $file1 -Destination $localGitFolder
} else {
    Write-Host "File not found: $file1"
}

if (Test-Path -Path $file2 -PathType Leaf) {
    Write-Host "Copying $file2 to $localGitFolder"
    Copy-Item -Path $file2 -Destination $localGitFolder
} else {
    Write-Host "File not found: $file2"
}

Write-Host "Script completed."
