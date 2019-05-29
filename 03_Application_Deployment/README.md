# Application Deployment
After following all previous steps, you will have a resource group containing CosmosDB, a Storage Account, and a Kubernetes cluster. The Kubernetes cluster will be hosting a Hyperledger Fabric blockchain network, and the Healthcare chaincode will be installed while the fabric-server image is serving requests locally to your Fabric network.

There are a few modifications to the [Healthcare Solution](./src/Healthcare.sln) that need to be made to work with your infrastructure. The connection strings need to be updated for the Proof Document Storage service and the Tracker/Indexer services.

To update the necessary appsettings.json connection strings, run the deploy.ps1 script.

## Prerequisites
1. [Service Fabric](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-get-started)
2. [Visual Studio](https://visualstudio.microsoft.com/)
3. Updated connection strings

## Options
You can either run the application locally or hosted using Service Fabric in your resource group. To run in Azure follow the guid to [deploying a service fabric .net application](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-tutorial-deploy-app-to-party-cluster)

# Steps
1. Open [Healthcare.sln](./src/Healthcare.sln) in Visual Studio (as Admin)
2. Build Project
3. Run the Service Fabric Apphosting project
4. Go to http://localhost:8085 to use the application

## Components
This project contains a number of components described below.

##### Tracker Service
 The tracker service retrieves information from the MongoDB database in your CosmosDB subscription.

##### Indexer Service
 The indexer service stores each blockchain transaction in the MongoDB store.

##### Fabric Client
 The Fabric Client sends gRPC requests to the Koltin Server which serves requests to your blockchain network and chaincode.

##### Proof Document Service
 The Proof Document service stores images

##### API Service
 The API consolidates all requests to a single interface for the UI.

##### Webapp.UI
 This project hosts the Angular application that uses the API to execute on all the previous services.

 ## Using the Application

 ### Create a Profile

To create a profile, click on the Case Worker persona, and then click on the create profile link. Pictured below. Be sure to fill all information to a NY address, and add a picture for license validation.

 ![Main Page](../References/main_page.JPG)
 ![Search](../References/case_worker.JPG)
 ![Create Profile](../References/create_profile.JPG)
 
### Assign a Plan

To assign a plan, select the Enrollment broker, select a profile, and then select a plan from the drop down.

![Select Prifle](../References/select_citizen.JPG)

### Approve a Plan

To approve a plan, select the State Approver, select a profile, and click approve.

![Select Prifle](../References/select_citizen.JPG)