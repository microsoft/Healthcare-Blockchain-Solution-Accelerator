package com.contoso.healthcare.service

/* ktlint-disable no-wildcard-imports */
import com.contoso.healthcare.profile.messages.*
/* ktlint-enable no-wildcard-imports */
import io.grpc.ManagedChannelBuilder
import org.assertj.core.api.Assertions.assertThat
import org.junit.jupiter.api.Test
import org.junit.jupiter.api.TestInstance
import org.junit.jupiter.api.TestInstance.Lifecycle
import com.google.protobuf.Timestamp
import java.time.Instant
import java.time.LocalDateTime
import java.time.ZoneOffset
import java.util.UUID

@TestInstance(Lifecycle.PER_CLASS)
class HealthcareServiceImplTest {

    private val testProfile = buildTestProfile()

    private val client = HealthcareServiceGrpc.newBlockingStub(
        ManagedChannelBuilder.forAddress("localhost", 9090)
            .usePlaintext()
            .build()
    )

    @Test
    internal fun `end to end test`() {
        val expected = testProfile.toBuilder()
            .setCitizenIdentifier(UUID.randomUUID().toString())
            .build()

        val response = client.createProfile(expected)
        assertThat(response).isEqualTo(expected) // Validation should fail
    }

    private fun buildTestProfile(): Profile {
        val basicProfile = BasicProfileItem.newBuilder()
            .setApplicationType(ApplicationType.INDIVIDUAL)
            .setDateOfBirth(1548201600.toTimestamp())
            .setFedIncome(50000.0)
            .setStateIncome(50000.0)
            .setName("Pamela Chu")
            .setFullAddress(
                Address.newBuilder()
                    .setCity("New York")
                    .setState(State.NY)
                    .setStreet("Test Street")
                    .setZip("10005-1234")
                    .build()
            )
            .setCitizenship(true)
            .build()

        val planItem = HealthcarePlanItem.newBuilder()
            .setId("THP")
            .setName("Test Healthcare Plan")
            .build()

        val plan = HealthcarePlan.newBuilder()
            .setPlan(planItem)
            .setApprovedDate(1548287501.toTimestamp())
            .setAssignedDate(1548287501.toTimestamp())
            .setEnrolledDate(1548287501.toTimestamp())
            .setEnrollmentBrokerAssignedState(State.NY)
            .setHealthcareStatus(HealthcareStatus.APPROVED)
            .setEnrollmentStatus(EnrollmentStatus.VOLUNTARY)
            .setPlanApprover("Test Approver")
            .setOwnerState(State.NY)
            .setApproved(true)
            .setEligible(true)
            .build()

        val proof = DocProof.newBuilder()
            .setContainer("Test proof container")
            .setContentType("application/pdf")
            .setFileName("test-drive-license.pdf")
            .setHash("test-drive-license.pdf".hashCode().toString())
            .setStorageSharding("Test sharding")
            .build()

        return Profile.newBuilder()
            .setStatus(ProfileStatus.COMPLETED)
            .setTransactionId("320dbe65-64bc-4e1b-b4e3-fe7535381838")
            .setTransactedTime(1548287501.toTimestamp())
            .setCitizenIdentifier("320dbe65-64bc-4e1b-b4e3-fe7535381838")
            .setPreferredHealthcarePlan(planItem)
            .setBasicProfile(basicProfile)
            .setActiveState(State.NY)
            .setCurrentHealthcarePlan(plan)
            .setDescription("Test profile")
            .setEnrolledHealthInsurance(false)
            .setStateApprover("State approver")
            .setIdentifyProofs(proof)
            .build()
    }
}

fun Int.toTimestamp(): Timestamp = Timestamp.newBuilder().setSeconds(this.toLong()).build()!!

fun Timestamp.toLocalDateTime(): LocalDateTime = LocalDateTime.ofInstant(Instant.ofEpochSecond(this.seconds), ZoneOffset.UTC)
