$resourceGroupName='Hyperledger_Healthcare_EAP'
$location='SouthCentralUS'
$storageAccountName='patientdocuments' #needs to be lower case
$cosmosAccountName='patientdata'
$databaseName='ContractTransaction'
$subscriptionID='YOUR_SUBSCRIPTION_ID'
$clusterName='fabric-cluster'

az login

az account set --subscription $subscriptionID

az group create `
    --location $location `
    --name $resourceGroupName `
    --subscription $subscriptionID

# Create a MongoDB API Cosmos DB account with consistent prefix (Local) consistency and multi-master enabled
az cosmosdb create `
    --resource-group $resourceGroupName `
    --name $cosmosAccountName `
    --kind MongoDB `
    --locations "southcentralus=0" `
    --default-consistency-level "ConsistentPrefix" `
    --enable-multiple-write-locations false `
    --subscription $subscriptionID

# Create a database 
az cosmosdb database create `
    --resource-group-name $resourceGroupName `
    --db-name $databaseName `
    --name $cosmosAccountName

az cosmosdb collection create `
    --resource-group-name $resourceGroupName `
    --db-name $databaseName `
    --collection-name "ContractTransaction" `
    --name $cosmosAccountName

az storage account create `
    --location $location `
    --name $storageAccountName `
    --resource-group $resourceGroupName `
    --sku "Standard_LRS" `
    --subscription $subscriptionID

az aks create --name $clusterName `
    --resource-group $resourceGroupName `
    --location $location `
    --disable-rbac `
    --enable-addons http_application_routing