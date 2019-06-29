#Update connection strings, create container, and run
$ordererIP = "YOUR_ORDERER_IP"
$peerIP = "YOUR_PEER_IP"
$peerURL = "PEERS_ORG1_URL=" + "grpc://" +  $peerIP + ":7051"
$ordererURL = "ORDERER_URL=" + "grpc://" + $ordererIP + ":7050"
$chaincodeName = "CHAINCODE_NAME=" + "healthcare"

docker pull edsgoode/fabric-server:1.0.4

kubectl run fabric-server --image=edsgoode/fabric-server:1.0.4 --env=$chaincodeName --env="CHANNEL_NAME=statechannel" --env=$peerURL --env=$ordererURL --port=9090

start-sleep -s 60

kubectl expose deployment fabric-server --type=LoadBalancer --name=fabric-service
