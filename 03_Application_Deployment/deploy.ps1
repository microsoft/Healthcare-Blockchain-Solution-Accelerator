$resourceGroupName='Hyperledger_Healthcare_EAP'
$cosmosAccountName="patientdata"
$storageAccountName="patientdocuments"

$cosmosConnectionStrings = az cosmosdb list-connection-strings --name $cosmosAccountName --resource-group $resourceGroupName | ConvertFrom-JSON

$primaryCosmosString = $cosmosConnectionStrings.connectionStrings[0].connectionString

(Get-Content .\src\Healthcare.BC.Indexer\Healthcare.BC.Indexer.API\appsettings.json).replace('[REPLACE_OFFCHAIN_CONNECTION_STRING]', $primaryCosmosString) | Set-Content .\src\Healthcare.BC.Indexer\Healthcare.BC.Indexer.API\appsettings.json

(Get-Content .\src\Healthcare.BC.Tracker\Healthcare.BC.Tracker.API\appsettings.json).replace('[REPLACE_OFFCHAIN_CONNECTION_STRING]', $primaryCosmosString) | Set-Content .\src\Healthcare.BC.Tracker\Healthcare.BC.Tracker.API\appsettings.json

$proofConnectionStrings = az storage account show-connection-string --name $storageAccountName | ConvertFrom-Json

$primaryStorageString = $proofConnectionStrings.connectionString

(Get-Content .\src\Healthcare.Proofing\Healthcare.Proofing.Service\appsettings.json).replace('[REPLACE_PROOFVAULT_CONNECTION_STRING]', $primaryStorageString) | Set-Content .\src\Healthcare.Proofing\Healthcare.Proofing.Service\appsettings.json

#Set execution policy for SF
Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Force -Scope CurrentUser


