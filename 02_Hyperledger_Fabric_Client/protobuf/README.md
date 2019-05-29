Contoso Healthcare Protobuf
============================

This project contains the required Protobuf messages and the tools to generate the Java and .Net code.

It is organised in two submodules:

  * [Cryptlet Protos](./cryptlet): Contains the messages received by the ESC components. 
  * [Profile Protos](./profile): Contains the messages received by the Fabric chaincode.
  
Basically, the ESC layer unwraps the message headers and sends the payload to the chaincode.

To generate the Java code run the command:
```
./gradlew clean build
```
