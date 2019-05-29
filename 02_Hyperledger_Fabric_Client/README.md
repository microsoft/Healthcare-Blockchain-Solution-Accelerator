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
2. Copy these IP addresses into the [deploy.ps1](./deploy.ps1) script
3. Run the script. This script will pull the fabric-server image and target your network
4. You can either run locally with your application which we will configure next, or host in Kubernetes

## Kotlin
To explore this solution download the [IntelliJ CE IDE](https://www.jetbrains.com/idea/download/#section=windows). Open the Kotlin or protobuf projects and explore. Here you can run associated tasks yourself and modify as needed. The Kotlin directory contains chaincode, supporting artifacts, and tests. The [chaincode](./kotlin/chaincode/src/main/kotlin/com/contoso/healthcare/shim/HealthcareChaincode.kt) is installed on your Hypeledger Fabric network.

## Protobuf
This folder contains the protobuf files and Java generated code to send messages between the .net application and kotlin server. The [service](./protobuf/service/src/main/kotlin/com/contoso/healthcare/service/HealthcareServiceImpl.kt) installs and instantiates the chaincode. Once installed and instantiated, the service serves gRPC requests and can execute the corresponding function.

The [protobuf definition](./protobuf/service/src/main/proto/contoso/healthcare/service.proto) shows the requests that can be made against the service. These requests also correspond to the calls that can be made against the Chaincode as well.