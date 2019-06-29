package com.contoso.healthcare.fabric

import com.contoso.healthcare.fabric.HealthcareFabricConfig.chaincode
import com.contoso.healthcare.fabric.HealthcareFabricConfig.orderer
import com.contoso.healthcare.fabric.HealthcareFabricConfig.peers
import com.contoso.healthcare.fabric.HealthcareFabricConfig.user
import com.google.protobuf.GeneratedMessageV3
import com.google.protobuf.TextFormat
import io.grpc.stub.StreamObserver
import mu.KLogging
import org.bouncycastle.asn1.pkcs.PrivateKeyInfo
import org.bouncycastle.jce.provider.BouncyCastleProvider
import org.bouncycastle.openssl.PEMParser
import org.bouncycastle.openssl.jcajce.JcaPEMKeyConverter
import org.hyperledger.fabric.sdk.BlockEvent.TransactionEvent
import org.hyperledger.fabric.sdk.ChaincodeID
import org.hyperledger.fabric.sdk.Channel
import org.hyperledger.fabric.sdk.Enrollment
import org.hyperledger.fabric.sdk.HFClient
import org.hyperledger.fabric.sdk.ProposalResponse
import org.hyperledger.fabric.sdk.TransactionRequest
import org.hyperledger.fabric.sdk.TransactionRequest.Type
import org.hyperledger.fabric.sdk.exception.ProposalException
import org.hyperledger.fabric.sdk.identity.X509Enrollment
import org.hyperledger.fabric.sdk.security.CryptoSuite
import java.io.File
import java.io.FileReader
import java.nio.file.Files
import java.nio.file.Paths
import java.security.Security
import java.time.Duration
import java.util.ArrayList
import java.util.concurrent.CompletableFuture
import java.util.Properties
import java.util.concurrent.TimeUnit

@Suppress("MemberVisibilityCanBePrivate")
open class HealthcareFabricSupport(val config: HealthcareFabricConfig = HealthcareFabricConfig()) {

    private val converter = JcaPEMKeyConverter()

    private val channel: Channel
    private val client: HFClient

    init {
        logger.debug { "Initializing Fabric with configuration: $config" }

        Security.addProvider(BouncyCastleProvider())
        converter.setProvider(BouncyCastleProvider.PROVIDER_NAME)

        client = HFClient.createNewInstance().apply {
            cryptoSuite = CryptoSuite.Factory.getCryptoSuite()
            userContext = HealthcareFabricUser(
                _name = "Admin",
                _mspId = config[user.mspId],
                _affiliation = config[user.affiliation],
                _enrollment = enroll(config[user.keyStore], config[user.signedCert])
            )

            val peerProperties = Properties().apply {
                this["grpc.NettyChannelBuilderOption.keepAliveTime"] = arrayOf(5L, TimeUnit.MINUTES)
                this["grpc.NettyChannelBuilderOption.keepAliveTimeout"] = arrayOf(8L, TimeUnit.SECONDS)
                this["grpc.NettyChannelBuilderOption.keepAliveWithoutCalls"] = arrayOf(true)
            }
            val peers = listOf(
                newPeer(config[peers.org1.name], config[peers.org1.url].toString(), peerProperties)
            )

            channel = newChannel(config[HealthcareFabricConfig.channel.name]).apply {
                peers.forEach { addPeer(it) }
            }

            val ordererProperties = Properties().apply {
                this["grpc.NettyChannelBuilderOption.keepAliveTime"] = arrayOf(5L, TimeUnit.MINUTES)
                this["grpc.NettyChannelBuilderOption.keepAliveTimeout"] = arrayOf(8L, TimeUnit.SECONDS)
                this["grpc.NettyChannelBuilderOption.keepAliveWithoutCalls"] = arrayOf(true)
            }

            channel.addOrderer(newOrderer(config[orderer.name], config[orderer.url].toString(), ordererProperties))
            channel.initialize()
        }
    }

    fun install(
        name: String = config[chaincode.name],
        version: String = config[chaincode.version],
        waitTime: Duration = Duration.ofMinutes(5),
        sourceLocation: File = File(System.getProperty("user.dir"))
    ): Collection<ProposalResponse> {

        val installProposal = client.newInstallProposalRequest().apply {
            chaincodeLanguage = TransactionRequest.Type.JAVA
            chaincodeSourceLocation = sourceLocation
            proposalWaitTime = waitTime.toMillis()
            chaincodeID = ChaincodeID.newBuilder()
                .setName(name)
                .setVersion(version)
                .build()
        }

        logger.info {
            "Sending install proposal to peers ${channel.peers}: ${installProposal.chaincodeID}"
        }

        return client.sendInstallProposal(installProposal, channel.peers)
    }

    fun instantiate(
        name: String = config[chaincode.name],
        version: String = config[chaincode.version],
        message: GeneratedMessageV3? = null,
        waitTime: Duration = Duration.ofMinutes(5),
        onProposalResponse: ProposalResponse.() -> Unit = {}
    ): CompletableFuture<TransactionEvent> {

        val instantiationProposal = client.newInstantiationProposalRequest().apply {
            chaincodeLanguage = Type.JAVA
            argBytes = message.toArgBytes()
            args = arrayListOf()
            proposalWaitTime = waitTime.toMillis()
            chaincodeID = ChaincodeID.newBuilder()
                .setName(name)
                .setVersion(version)
                .build()
        }

        logger.info {
            "Sending instantiation proposal to peers ${channel.peers}: ${instantiationProposal.chaincodeID}"
        }
        message?.apply {
            logger.debug { "Message contents: ${TextFormat.shortDebugString(message)}" }
        }

        return channel.sendInstantiationProposal(instantiationProposal).onEach {
            onProposalResponse.invoke(it)
        }.let {
            sendTransaction(it)
        }
    }

    fun upgrade(
        name: String = config[chaincode.name],
        version: String = config[chaincode.version],
        message: GeneratedMessageV3? = null,
        waitTime: Duration = Duration.ofMinutes(5),
        onProposalResponse: ProposalResponse.() -> Unit = {}
    ): CompletableFuture<TransactionEvent> {

        val upgradeProposal = client.newUpgradeProposalRequest().apply {
            chaincodeLanguage = Type.JAVA
            argBytes = message.toArgBytes()
            args = arrayListOf()
            proposalWaitTime = waitTime.toMillis()
            chaincodeID = ChaincodeID.newBuilder()
                .setName(name)
                .setVersion(version)
                .build()
        }

        logger.info {
            "Sending upgrade proposal to peers ${channel.peers}: ${upgradeProposal.chaincodeID}"
        }
        message?.apply {
            logger.debug { "Message contents: ${TextFormat.shortDebugString(message)}" }
        }

        return channel.sendUpgradeProposal(upgradeProposal).onEach {
            onProposalResponse.invoke(it)
        }.let {
            sendTransaction(it)
        }
    }

    fun transact(
        function: String,
        message: GeneratedMessageV3? = null,
        name: String = config[chaincode.name],
        version: String = config[chaincode.version],
        waitTime: Duration = Duration.ofSeconds(30),
        onProposalResponse: ProposalResponse.() -> Unit = {}
    ): CompletableFuture<TransactionEvent> {

        return transact(function, message, NoopStreamObserver, name, version, waitTime, onProposalResponse)
    }

    fun <V> transact(
        function: String,
        message: GeneratedMessageV3? = null,
        observer: StreamObserver<V>,
        name: String = config[chaincode.name],
        version: String = config[chaincode.version],
        waitTime: Duration = Duration.ofSeconds(30),
        onProposalResponse: ProposalResponse.() -> V
    ): CompletableFuture<TransactionEvent> {

        val transactionProposal = client.newTransactionProposalRequest().apply {
            argBytes = message.toArgBytes()
            chaincodeLanguage = Type.JAVA
            proposalWaitTime = waitTime.toMillis()
            chaincodeID = ChaincodeID.newBuilder()
                .setName(name)
                .setVersion(version)
                .build()
            fcn = function
        }

        logger.info {
            "Sending transaction proposal for function '$function' " +
                "to peers ${channel.peers}: ${transactionProposal.chaincodeID}"
        }
        message?.apply {
            logger.debug { "Message contents: ${TextFormat.shortDebugString(message)}" }
        }

        return channel.sendTransactionProposal(transactionProposal).onEach {
            logger.debug { "Received proposal response: ${it.proposalResponse.response.message}" }
            observer.onNext(onProposalResponse.invoke(it))
        }.let {
            sendTransaction(observer, it)
        }
    }

    fun query(
        function: String,
        message: GeneratedMessageV3,
        name: String = config[chaincode.name],
        version: String = config[chaincode.version],
        waitTime: Duration = Duration.ofSeconds(30)
    ): Collection<ProposalResponse> {

        val queryProposal = client.newQueryProposalRequest().apply {
            fcn = function
            chaincodeLanguage = Type.JAVA
            argBytes = message.toArgBytes()
            proposalWaitTime = waitTime.toMillis()
            chaincodeID = ChaincodeID.newBuilder()
                .setName(name)
                .setVersion(version)
                .build()
        }
        return channel.queryByChaincode(queryProposal)
    }

    private fun enroll(keyStore: String, signedCert: String): Enrollment {

        val keyFile = javaClass.classLoader.getResource(keyStore).file
        val pemObject = PEMParser(FileReader(keyFile)).readObject()
        val privateKey = converter.getPrivateKey(pemObject as PrivateKeyInfo)

        val certFile = javaClass.classLoader.getResource(signedCert).toURI()
        val signedCertData = Files.readString(Paths.get(certFile))

        return X509Enrollment(privateKey, signedCertData)
    }

    private fun sendTransaction(
        proposalResponses: Collection<ProposalResponse>
    ): CompletableFuture<TransactionEvent> {

        return if (proposalResponses.none { it.isInvalid }) {
            channel.sendTransaction(proposalResponses)
        } else {
            val message = "Failed proposals: " +
                proposalResponses.joinToString(",\n") {
                    it.proposalResponse.response.message
                }
            CompletableFuture.failedFuture(ProposalException(message))
        }
    }

    private fun <V> sendTransaction(
        observer: StreamObserver<V>,
        proposalResponses: Collection<ProposalResponse>
    ): CompletableFuture<TransactionEvent> {

        return if (proposalResponses.none { it.isInvalid }) {
            channel.sendTransaction(proposalResponses).thenApply {
                observer.onCompleted()
                it
            }.exceptionally {
                observer.onError(it.cause)
                null
            }
        } else {
            val exception = ProposalException("Failed proposals: " +
                proposalResponses.joinToString(",\n") {
                    it.proposalResponse.response.message
                })
            observer.onError(exception)
            CompletableFuture.failedFuture(exception)
        }
    }

    private object NoopStreamObserver : StreamObserver<Any> {
        override fun onNext(value: Any) {
        }

        override fun onError(t: Throwable) {
        }

        override fun onCompleted() {
        }
    }

    private fun GeneratedMessageV3?.toArgBytes(): ArrayList<ByteArray> =
        this?.run { arrayListOf(toByteArray()) } ?: arrayListOf()

    private companion object : KLogging()
}