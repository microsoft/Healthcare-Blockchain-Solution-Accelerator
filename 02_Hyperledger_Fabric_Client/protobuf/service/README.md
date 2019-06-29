Contoso Healthcare gRPC Service
================================

This project contains the Protobuf gRPC definition and server for the Healthcare service.

Configuration
-------------

The server can be configured with Java system properties, environment variables and a 
`service.properties` file, in this order of precedence. Environment variables are:

 * `CHANNEL_NAME`: Chaincode channel name.
 * `CHAINCODE_NAME`: Name of the chaincode to run.
 * `CHAINCODE_VERSION`: Version of the chaincode to run.
 * `ORDERER_NAME`: Name of the orderer. 
 * `ORDERER_URL`: URL of the orderer.
 * `PEERS_NY_NAME`: New York peer node name.
 * `PEERS_NY_URL`: New York peer node URL.
 * `PEERS_NJ_NAME`: New Jersey peer node name.
 * `PEERS_NJ_URL`: New Jersey peer node URL.


Running the server
------------------

To run the server you can either use the Gradle command:

```
../gradlew run --args=<PORT>
```

or build the project and run the distribution ZIP or TAR file:

```
../gradlew build
cd build/distributions
unzip contoso-healthcare-service-grpc-shadow-<VERSION>.zip
./contoso-healthcare-service-grpc-shadow-<VERSION>/bin/contoso-healthcare-service-grpc <PORT>
```
