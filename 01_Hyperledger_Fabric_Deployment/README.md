# Hyperledger Fabric Network
This folder contains resources, keys, and certificates to deploy a Hyperledger Fabric Development cluster using Kubernetes. To deploy this solution run the deployment script ./deploy.ps1.

Deploy this development network first, and then based on your needs modify for a production environment. Note do not use this in production without generating new cryptographic materials and using a proper CA.

## Prerequisites
1. [Kubectl](https://kubernetes.io/docs/tasks/tools/install-kubectl/)
2. [Helm](https://helm.sh/) The Package Mangager for Kubernetes
3. A Kubernetes cluster deployed in Azure

## Setup Options
- **Generate Keys and Certificates** Optional - To generate your own Keys and Certificates see the [Hyperledger Fabric Docs](https://hyperledger-fabric.readthedocs.io/en/release-1.4/) and This [Demos](https://github.com/aidtechnology/hgf-k8s-workshop/tree/master/dev_example) from the Hyperledger Fabric Global Workshop
- **Deployment Script** - run .\deploy.ps1

## Instructions
1. Update the variables in the deployment script to match the names of your resources from [01_Hyperleder_Fabric_Deployment](../00_Resource)
Once deployed save the external IP Addresses for the Hyperledger Fabric Orderer.

```
kubectl get services -n contoso
```

```
kubectl get services -n org1
```
## References
1. **Hyperledger Fabric** Optional - To generate your own Keys and Certificates, or use a CA see the [Hyperledger Fabric Docs](https://hyperledger-fabric.readthedocs.io/en/release-1.4/).
2. **Helm** - [Helm packages](https://helm.sh/) are kubernets packages used to easily deploy and manage your kubernets components. This [Demos](https://github.com/aidtechnology/hgf-k8s-workshop/tree/master/dev_example) from the Hyperledger Fabric Global Workshop walks through a similar Hyperledger Fabric network using Helm Packages.
