# Resource Deployment

This folder contains a PowerShell script that can be used to provision the Azure resources required to build your Blockchain solution.  You may skip this folder if you prefer to provision your Azure resources via the Azure Portal.  The PowerShell script will provision the following resources to your Azure subscription:

 
| Resource              | Usage                                                                                     |
|-----------------------|-------------------------------------------------------------------------------------------|
| [Azure Cosmos DB](https://azure.microsoft.com/en-us/services/cosmos-db/)  | The patient information stored as a document          |
|[Azure Storage Account](https://azure.microsoft.com/en-us/services/storage/?v=18.24) | Data source where photo IDs are stored|                                                     |
| [Kubernetes](https://azure.microsoft.com/en-us/services/kubernetes-service/)               | The Blockchain Network                                                    |

## Prerequisites
1. Access to an Azure Subscription
2. Azure CLI Installed

## Deploy via Azure Portal
As an alternative to running the PowerShell script, you can deploy the resources manually via the Azure Portal or click the button below to deploy the resources:

<a href="https://azuredeploy.net/?repository=https:" target="_blank">
    <img src="http://azuredeploy.net/deploybutton.png"/>
</a> 

## Steps for Resource Deployment via PowerShell

To run the [PowerShell script](./deploy.ps1):

1. Modify the parameters at the top of **deploy.ps1** to configure the names of your resources and other settings.   
2. Run the [PowerShell script](./deploy.ps1). If you have PowerShell opened to this folder run the command:
`./deploy.ps1`
3. You will then be prompted to login and provide additional information.
