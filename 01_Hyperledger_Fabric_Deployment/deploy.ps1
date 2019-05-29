$resourceGroupName='Hyperledger_Healthcare_EAP'
$location='SouthCentralUS'
$clusterName='fabric-cluster'

az aks get-credentials -n $clusterName --resource-group $resourceGroupName

helm init

#Wait for helm to initialize
Start-Sleep -s 60

kubectl create ns contoso
kubectl create ns org1

#Create Peers and Orderers
$MSP_DIR=".\src\config\crypto-config\ordererOrganizations\contoso.svc.cluster.local\users\Admin@contoso.svc.cluster.local\msp"
$ORG_CERT=$MSP_DIR + "\admincerts\Admin@Contoso.svc.cluster.local-cert.pem"

kubectl create secret generic -n contoso hlf--ord-admincert --from-file=cert.pem=$ORG_CERT

$CA_CERT=$MSP_DIR + "\cacerts\ca.Contoso.svc.cluster.local-cert.pem"

kubectl create secret generic -n contoso hlf--ord-cacert --from-file=cacert.pem=$CA_CERT

$MSP_DIR=".\src\config\crypto-config\peerOrganizations\Org1.svc.cluster.local\users\Admin@Org1.svc.cluster.local\msp"
$ORG_CERT=$MSP_DIR + "\admincerts\Admin@Org1.svc.cluster.local-cert.pem"

kubectl create secret generic -n org1 hlf--peer-admincert --from-file=cert.pem=$ORG_CERT

$ORG_KEY=$MSP_DIR + "\keystore\993d97d0c9e5cd28e79c4fa793803e13f198e07a027fdc5c0c9658e2c9fa5ab3_sk"

kubectl create secret generic -n org1 hlf--peer-adminkey --from-file=key.pem=$ORG_KEY

$CA_CERT=$MSP_DIR + "\cacerts\ca.Org1.svc.cluster.local-cert.pem"

kubectl create secret generic -n org1 hlf--peer-cacert --from-file=cacert.pem=$CA_CERT

$BLOCK=".\src\config\genesis.block"
kubectl create secret generic -n contoso hlf--genesis --from-file=genesis.block=$BLOCK

$TX=".\src\config\statechannel.tx"
kubectl create secret generic -n org1 hlf--channel --from-file=statechannel.tx=$TX

$MSP_DIR=".\src\config\crypto-config\ordererOrganizations\Contoso.svc.cluster.local\orderers\Contosoord-hlf-ord.contoso.svc.cluster.local\msp"
$NODE_CERT=$MSP_DIR + "\signcerts\Contosoord-hlf-ord.Contoso.svc.cluster.local-cert.pem"

kubectl create secret generic -n contoso hlf--ord1-idcert --from-file=cert.pem=$NODE_CERT

$NODE_KEY=$(ls ${MSP_DIR}/keystore/*_sk)

kubectl create secret generic -n contoso hlf--ord1-idkey --from-file=key.pem=$NODE_KEY

helm install .\src\helm_charts\hlf-ord -n contosoord --namespace contoso -f .\src\helm_values\contoso-orderer.yaml

Write-Output "Starting Orderer"

#Wait for Orderer to initialize
Start-Sleep -s 60

$ORD_POD=kubectl get pods -n contoso -l "app=hlf-ord" -o jsonpath="{.items[0].metadata.name}"

$MSP_DIR=".\src\config\crypto-config\peerOrganizations\Org1.svc.cluster.local\peers\Org1peer-hlf-peer.Org1.svc.cluster.local\msp"
$NODE_CERT=$(ls ${MSP_DIR}\signcerts\*.pem)

kubectl create secret generic -n org1 hlf--peer1-idcert --from-file=cert.pem=$NODE_CERT

$NODE_KEY=$(ls ${MSP_DIR}\keystore\*_sk)

kubectl create secret generic -n org1 hlf--peer1-idkey --from-file=key.pem=$NODE_KEY

kubectl -n org1 apply -f .\src\docker\did\docker-volume.yaml

kubectl -n org1 apply -f .\src\docker\did\docker.yaml

Write-Output "Starting Docker in Docker"

#Wait for docker in docker to initialize
Start-Sleep -s 60

helm install .\src\helm_charts\hlf-peer -n org1peer --namespace org1 -f .\src\helm_values\org1-peer.yaml

Write-Output "Starting Peer"

#Wait for Peer to initialize
Start-Sleep -s 60


kubectl expose deployment org1peer-hlf-peer --type=LoadBalancer --name=peer-service

kubectl expose deployment contosoord-hlf-ord --type=LoadBalancer --name=orderer-service

$PEER_POD=$(kubectl get pods -n org1 -l "app=hlf-peer" -o jsonpath="{.items[0].metadata.name}")

kubectl exec -n org1 $PEER_POD -- peer channel create -o contosoord-hlf-ord.contoso.svc.cluster.local:7050 -c statechannel -f /hl_config/channel/statechannel.tx

kubectl exec -n org1 $PEER_POD -- peer channel fetch config /var/hyperledger/statechannel.block -c statechannel -o contosoord-hlf-ord.contoso.svc.cluster.local:7050

kubectl exec -n org1 $PEER_POD -- bash -c 'CORE_PEER_MSPCONFIGPATH=$ADMIN_MSP_PATH peer channel join -b /var/hyperledger/statechannel.block'

kubectl -n org1 expose deployment org1peer-hlf-peer --type=LoadBalancer --name=peer-service

kubectl -n contoso expose deployment contosoord-hlf-ord --type=LoadBalancer --name=ord-service

Write-Output "Exposing Services"

Start-Sleep -s 60

# TODO - Add get IPs in script