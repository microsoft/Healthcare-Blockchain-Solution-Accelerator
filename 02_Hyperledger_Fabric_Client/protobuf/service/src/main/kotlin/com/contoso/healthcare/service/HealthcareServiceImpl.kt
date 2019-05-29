package com.contoso.healthcare.service

import com.contoso.healthcare.fabric.HealthcareFabricSupport
import com.contoso.healthcare.profile.messages.Profile
import com.contoso.healthcare.service.HealthcareServiceGrpc.HealthcareServiceImplBase
import io.grpc.Server
import io.grpc.ServerBuilder
import io.grpc.stub.StreamObserver
import mu.KLogging
import java.util.concurrent.Executors
import java.util.concurrent.TimeUnit
import java.io.File

class HealthcareServiceImpl(private val port: Int) : HealthcareServiceImplBase(),
    Runnable, AutoCloseable {

    private val server: Server
        get() = ServerBuilder.forPort(port)
            .addService(this)
            .build()

    private val support = HealthcareFabricSupport()

    private var running = false

    init {
        val path = File("/app/chaincode")
        val chaincodeName = "healthcare"
        val chaincodeVersion = "1.0.0"

        support.install(name = chaincodeName, version = chaincodeVersion, sourceLocation = path)
        support.instantiate(name = chaincodeName, version = chaincodeVersion)
    }

    override fun createProfile(
        request: Profile,
        responseObserver: StreamObserver<Profile>
    ) {
        support.transact("createProfile", request, responseObserver) {
            Profile.parseFrom(proposalResponse.response.payload)
        }.get()
    }

    override fun updateProfileProofDocument(
        request: UpdateProfileProofDocument,
        responseObserver: StreamObserver<Profile>
    ) {
        support.transact("updateProfileProofDocument", request, responseObserver) {
            Profile.parseFrom(proposalResponse.response.payload)
        }.get()
    }

    override fun getProfileInformation(
        request: GetProfileInformation,
        responseObserver: StreamObserver<Profile>
    ) {
        support.transact("getProfileInformation", request, responseObserver) {
            Profile.parseFrom(proposalResponse.response.payload)
        }.get()
    }

    override fun assignHealthcarePlan(
        request: AssignHealthcarePlan,
        responseObserver: StreamObserver<Profile>
    ) {
        support.transact("assignHealthcarePlan", request, responseObserver) {
            Profile.parseFrom(proposalResponse.response.payload)
        }.get()
    }

    override fun approveHealthcarePlan(
        request: ApproveHealthcarePlan,
        responseObserver: StreamObserver<Profile>
    ) {
        support.transact("approveHealthcarePlan", request, responseObserver) {
            Profile.parseFrom(proposalResponse.response.payload)
        }.get()
    }

    override fun changeActiveState(
        request: ChangeActiveState,
        responseObserver: StreamObserver<Profile>
    ) {
        support.transact("changeActiveState", request, responseObserver) {
            Profile.parseFrom(proposalResponse.response.payload)
        }.get()
    }

    override fun run() {
        check(!running) { "Service is already running!" }

        server.start()
        running = true

        kLog.logger.info { "Server started, listening on $port" }
        server.awaitTermination()
    }

    override fun close() {
        check(running) { "Service is not running!" }

        if (!server.isShutdown) {
            server.shutdown()
            running = false
        }
    }

    companion object {
        val kLog = KLogging()
        private val service = HealthcareServiceImpl(9090)

        @JvmStatic
        fun main(args: Array<String>) {
            Executors.newSingleThreadExecutor().submit(service)
            TimeUnit.SECONDS.sleep(3) // Sleep until service is started
        }
    }
}