@echo off

echo ***********   Generate API Proxy
start nswag swagger2csclient /input:http://localhost:8080/swagger/v1/swagger.json /classname:ApplicationProxy /namespace:HealthcareBC.Application /output:ApplicationProxy.cs

echo ***********   Generate ESC Proxy
start nswag swagger2csclient /input:http://localhost:8081/swagger/v1/swagger.json /classname:FabricProxy /namespace:HealthcareBC.Fabric /output:FabricProxy.cs

echo ***********   Generate Indexer Proxy
start nswag swagger2csclient /input:http://localhost:8082/swagger/v1/swagger.json /classname:IndexerProxy /namespace:HealthcareBC.Indexer /output:IndexerProxy.cs

echo ***********   Generate ProofDoc Proxy
start nswag swagger2csclient /input:http://localhost:8084/swagger/v1/swagger.json /classname:ProofDocProxy /namespace:HealthcareBC.ProofDocSvc /output:ProofDocProxy.cs

echo ***********   Generate Tracker Proxy
start nswag swagger2csclient /input:http://localhost:8083/swagger/v1/swagger.json /classname:TrackerProxy /namespace:HealthcareBC.Tracker /output:TrackerProxy.cs

echo ========>>> after generating all proxy classes, try compile it as Proxylib
