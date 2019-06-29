package com.contoso.healthcare.shim

import com.contoso.healthcare.fabric.HealthcareFabricSupport
import com.contoso.healthcare.profile.messages.Address
import com.contoso.healthcare.profile.messages.ApplicationType
import com.contoso.healthcare.profile.messages.BasicProfileItem
import com.contoso.healthcare.profile.messages.DocProof
import com.contoso.healthcare.profile.messages.EnrollmentStatus
import com.contoso.healthcare.profile.messages.HealthcarePlan
import com.contoso.healthcare.profile.messages.HealthcarePlanItem
import com.contoso.healthcare.profile.messages.HealthcareStatus
import com.contoso.healthcare.profile.messages.Profile
import com.contoso.healthcare.profile.messages.ProfileStatus
import com.contoso.healthcare.profile.messages.State.NY
import com.contoso.healthcare.service.ApproveHealthcarePlan
import com.contoso.healthcare.service.AssignHealthcarePlan
import com.contoso.healthcare.service.GetProfileInformation
import com.contoso.healthcare.service.SetEligibility
import com.contoso.healthcare.service.UpdateProfileProofDocument
import com.google.protobuf.Timestamp
import mu.KLogging
import org.assertj.core.api.Assertions
import org.assertj.core.api.Assertions.assertThat
import org.assertj.core.api.Assertions.within
import org.hyperledger.fabric.sdk.BlockEvent.TransactionEvent
import org.junit.jupiter.api.Test
import java.time.Clock
import java.time.Instant
import java.time.LocalDateTime
import java.time.ZoneOffset
import java.time.temporal.ChronoUnit.MINUTES
import java.util.UUID
import java.util.concurrent.CompletableFuture

class HealthcareChaincodeAzureTest : HealthcareFabricSupport() {

    private val testProfile = buildTestProfile()

    private val chaincodeVersion = "1.0.0"
    private val chaincodeName = "healthcare-chaincode-test-${System.currentTimeMillis()}"

    @Test
    internal fun `end to end test`() {

        install(chaincodeName, chaincodeVersion).forEach {
            Assertions.assertThat(it.isInvalid).isFalse()
        }

        val expected = testProfile.toBuilder()
            .setCitizenIdentifier(UUID.randomUUID().toString())
            .build()

        val actual = instantiate(chaincodeName, chaincodeVersion).thenCompose {
            assertThat(it.isValid).isTrue()
            createProfile(expected)
        }.thenCompose {
            assertThat(it.isValid).isTrue()
            updateProfileProofDocument(expected.citizenIdentifier)
        }.thenCompose {
            assertThat(it.isValid).isTrue()
            setEligibility(expected.citizenIdentifier)
        }.thenCompose {
            assertThat(it.isValid).isTrue()
            assignHealthcarePlan(expected.citizenIdentifier)
        }.thenCompose {
            assertThat(it.isValid).isTrue()
            approveHealthcarePlan(expected.citizenIdentifier)
        }.thenApply {
            assertThat(it.isValid).isTrue()
            getProfileInformation(expected.citizenIdentifier)
        }.get()

        Assertions.setAllowComparingPrivateFields(false)

        assertThat(actual).isEqualToIgnoringGivenFields(
            expected, "currentHealthcarePlan"
        )

        assertThat(actual.currentHealthcarePlan).isEqualToIgnoringGivenFields(
            expected.currentHealthcarePlan, "assignedDate", "approvedDate", "enrolledDate"
        )

        assertThat(actual.currentHealthcarePlan.assignedDate.toLocalDateTime())
            .isCloseTo(LocalDateTime.now(Clock.systemUTC()), within(1, MINUTES))

        assertThat(actual.currentHealthcarePlan.approvedDate.toLocalDateTime())
            .isCloseTo(LocalDateTime.now(Clock.systemUTC()), within(1, MINUTES))

//        FIXME When enrolled date is set? Post MVP...
//        assertThat(actual.currentHealthcarePlan.enrolledDate.toLocalDateTime())
//            .isCloseTo(LocalDateTime.now(), within(2, ChronoUnit.MINUTES))
    }

    private fun createProfile(profile: Profile): CompletableFuture<TransactionEvent> {
        return transact("createProfile", profile, chaincodeName, chaincodeVersion) {
            assertThat(isInvalid).isFalse()
        }
    }

    private fun updateProfileProofDocument(citizenId: String): CompletableFuture<TransactionEvent> {

        val request = UpdateProfileProofDocument.newBuilder()
            .setCitizenIdentifier(citizenId)
            .setDocProof(testProfile.identifyProofs)
            .build()

        return transact("updateProfileProofDocument", request, chaincodeName, chaincodeVersion) {
            assertThat(isInvalid).isFalse()
        }
    }

    private fun setEligibility(citizenId: String): CompletableFuture<TransactionEvent> {

        val request = SetEligibility.newBuilder()
            .setCitizenIdentifier(citizenId)
            .setEligible(true)
            .build()

        return transact("setEligibility", request, chaincodeName, chaincodeVersion) {
            assertThat(isInvalid).isFalse()
        }
    }

    private fun assignHealthcarePlan(citizenId: String): CompletableFuture<TransactionEvent> {

        val request = AssignHealthcarePlan.newBuilder()
            .setCitizenIdentifier(citizenId)
            .setPlan(testProfile.currentHealthcarePlan.plan)
            .build()

        return transact("assignHealthcarePlan", request, chaincodeName, chaincodeVersion) {
            assertThat(isInvalid).isFalse()
        }
    }

    private fun approveHealthcarePlan(citizenId: String): CompletableFuture<TransactionEvent> {

        val request = ApproveHealthcarePlan.newBuilder()
            .setCitizenIdentifier(citizenId)
            .setPlanApprover(testProfile.currentHealthcarePlan.planApprover)
            .build()

        return transact("approveHealthcarePlan", request, chaincodeName, chaincodeVersion) {
            assertThat(isInvalid).isFalse()
        }
    }

    private fun getProfileInformation(citizenId: String): Profile {

        val request = GetProfileInformation.newBuilder()
            .setCitizenIdentifier(citizenId)
            .build()

        return query("getProfileInformation", request, chaincodeName, chaincodeVersion).first().run {
            assertThat(isInvalid).isFalse()
            Profile.parseFrom(chaincodeActionResponsePayload)
        }
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
                    .setState(NY)
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
            .setEnrollmentBrokerAssignedState(NY)
            .setHealthcareStatus(HealthcareStatus.APPROVED)
            .setEnrollmentStatus(EnrollmentStatus.VOLUNTARY)
            .setPlanApprover("Test Approver")
            .setOwnerState(NY)
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
            .setActiveState(NY)
            .setCurrentHealthcarePlan(plan)
            .setDescription("Test profile")
            .setEnrolledHealthInsurance(false)
            .setStateApprover("State approver")
            .setIdentifyProofs(proof)
            .build()
    }

    companion object : KLogging()
}

fun Int.toTimestamp(): Timestamp =
    Timestamp.newBuilder().setSeconds(this.toLong()).build()!!

fun Timestamp.toLocalDateTime(): LocalDateTime =
    LocalDateTime.ofInstant(Instant.ofEpochSecond(this.seconds), ZoneOffset.UTC)
