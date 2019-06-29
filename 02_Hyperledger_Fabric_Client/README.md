# Hyperledger Fabric Kotlin Client
Hyperledger Fabric supports a number of clients and chaincode languages including Node, Java, and Go. Since our application is an .net core application, we must utilize a service written in one of these languages to execute transactions against Hyperledger Fabric until .net is a supported client langauage.

## Prerequesites

1. Docker Installed
2. Kubectl installed
3. IPs for Peer and Orderer

## Instructions

1. Get IP addresses for Orderer and Peer nodes by running and copying the external IPs for peer-service and ord-service
```
kubectl get services --all-namespaces
```
2. Copy these IP addresses into the [deploy.ps1](./deploy.ps1) or (./deploy_azure.ps1) script

### Run Locally
3. Run the [deploy.ps1](./deploy.ps1) script. This script will pull the fabric-server image and run in docker on your host machine

### Run in Kubernetes
4. Run the [deploy_azure.ps1](./deploy_azure.ps1) script. This script will pull the fabric-server image and deploy the container to your K8s cluster
5. After running, get the External IP address for the fabric-server. Using
```
kubectl get services --all-namespaces
```
6. Update the endpoint in your [Fabric Client](.\src\Healthcare.BC.Chain\Healthcare.BC.Chain.Client\Healthcare.BC.Chain.Client\FabricClient.cs)

## Kotlin
To explore this solution download the [IntelliJ CE IDE](https://www.jetbrains.com/idea/download/#section=windows). Open the Kotlin or protobuf projects and explore. Here you can run associated tasks yourself and modify as needed. The Kotlin directory contains chaincode, supporting artifacts, and tests. The [chaincode](./kotlin/chaincode/src/main/kotlin/com/contoso/healthcare/shim/HealthcareChaincode.kt) is installed on your Hypeledger Fabric network.

## Protobuf
This folder contains the protobuf files and Java generated code to send messages between the .net application and kotlin server. The [service](./protobuf/service/src/main/kotlin/com/contoso/healthcare/service/HealthcareServiceImpl.kt) installs and instantiates the chaincode. Once installed and instantiated, the service serves gRPC requests and can execute the corresponding function.

The [protobuf definition](./protobuf/service/src/main/proto/contoso/healthcare/service.proto) shows the requests that can be made against the service. These requests also correspond to the calls that can be made against the Chaincode as well.