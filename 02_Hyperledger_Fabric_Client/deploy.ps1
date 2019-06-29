#Update connection strings, create container, and run
$ordererIP = "40.124.25.101"
$peerIP = "13.84.147.175"
$peerURL = "grpc://" +  $peerIP + ":7051"
$ordererURL = "grpc://" + $ordererIP + ":7050"
$chaincodeName = "healthcare"
$channelName = "statechannel"

docker pull edsgoode/fabric-server:1.0.4
docker run -it -e CHANNEL_NAME=$channelName -e PEERS_ORG1_URL=$peerURL -e ORDERER_URL=$ordererURL -p 9090:9090 edsgoode/fabric-server:1.0.4