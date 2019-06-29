package com.contoso.healthcare.shim

import com.contoso.healthcare.profile.EnrollmentStatus.PENDING
import com.contoso.healthcare.profile.HealthcareStatus
import com.contoso.healthcare.profile.ProfileStatus
import com.contoso.healthcare.profile.messages.Address
import com.contoso.healthcare.profile.messages.ApplicationType
import com.contoso.healthcare.profile.messages.BasicProfileItem
import com.contoso.healthcare.profile.messages.DocProof
import com.contoso.healthcare.profile.messages.EnrollmentStatus
import com.contoso.healthcare.profile.messages.HealthcarePlan
import com.contoso.healthcare.profile.messages.HealthcarePlanItem
import com.contoso.healthcare.profile.messages.HealthcareStatus.ASSIGNED
import com.contoso.healthcare.profile.messages.Profile
import com.contoso.healthcare.profile.messages.ProfileStatus.COMPLETED
import com.contoso.healthcare.profile.messages.State.NY
import com.contoso.healthcare.service.ApproveHealthcarePlan
import com.contoso.healthcare.service.AssignHealthcarePlan
import com.contoso.healthcare.service.GetProfileInformation
import com.contoso.healthcare.service.SetEligibility
import com.contoso.healthcare.service.UpdateProfileProofDocument
import io.mockk.clearMocks
import io.mockk.every
import io.mockk.just
import io.mockk.mockk
import io.mockk.runs
import io.mockk.verify
import org.assertj.core.api.Assertions.assertThat
import org.hyperledger.fabric.shim.Chaincode.Response.Status.ERROR_THRESHOLD
import org.hyperledger.fabric.shim.Chaincode.Response.Status.SUCCESS
import org.hyperledger.fabric.shim.ChaincodeStub
import org.junit.jupiter.api.BeforeEach
import org.junit.jupiter.api.Test
import java.time.Clock
import java.time.Instant
import java.time.LocalDate
import java.time.LocalDateTime
import java.time.OffsetDateTime
import java.time.ZoneOffset

/**
 * TODO Add error test cases.
 */
class HealthcareChaincodeTest {

    private val testProfile = createTestProfile()
    private val chaincode = HealthcareChaincode()
    private val chaincodeStub = mockk<ChaincodeStub>(relaxed = true)

    @BeforeEach
    fun setUp() {
        clearMocks(chaincodeStub)
        val fixedTime = OffsetDateTime.of(LocalDateTime.of(2018, 1, 9, 0, 0, 0), ZoneOffset.UTC)
        HealthcareChaincode.clock = Clock.fixed(Instant.from(fixedTime), ZoneOffset.UTC)
    }

    @Test
    internal fun `when creating a profile with a valid message a JSON object is put in the chaincode state`() {

        chaincodeStub.apply {
            every { function } returns "createProfile"
            every { args } returns listOf("createProfile".toByteArray(), testProfile.toByteArray())
            every { parameters } returns listOf(String(testProfile.toByteArray()))
            every { putStringState(any(), any()) } just runs
        }

        val expected = testProfile.toEntity().run {
            copy(
                identifyProofs = null,
                status = ProfileStatus.CREATED,
                currentHealthcarePlan = currentHealthcarePlan.copy(
                    assignedDate = null,
                    approvedDate = null,
                    enrolledDate = null,
                    healthcareStatus = HealthcareStatus.NOT_ASSIGNED,
                    planApprover = null,
                    enrollmentStatus = PENDING,
                    approved = false,
                    plan = null
                )
            )
        }

        chaincode.invoke(chaincodeStub).apply {
            assertThat(status).isEqualTo(SUCCESS)
            assertThat(message).isNotNull()
            assertThat(Profile.parseFrom(payload)).isEqualTo(expected.toMessage())
        }

        verify {
            chaincodeStub.putStringState(testProfile.citizenIdentifier, expected.toJson())
        }
    }

    @Test
    internal fun `when creating a profile with an existing citizen ID an error is returned and state not modified`() {

        val profileBytes = Profile.getDefaultInstance().toByteArray()

        chaincodeStub.apply {
            every { function } returns "createProfile"
            every { args } returns listOf("createProfile".toByteArray(), profileBytes)
            every { parameters } returns listOf(String(profileBytes))
            every { getStringState(any()) } returns testProfile.toEntity().toJson()
        }

        chaincode.invoke(chaincodeStub).apply {
            assertThat(status).isEqualTo(ERROR_THRESHOLD)
            assertThat(message).isNotNull()
            assertThat(payload).isNull()
        }

        verify(exactly = 0) { chaincodeStub.putState(any(), any()) }
    }

    @Test
    internal fun `when creating a profile with an empty message an error is returned and state not modified`() {

        val profileBytes = Profile.getDefaultInstance().toByteArray()

        chaincodeStub.apply {
            every { function } returns "createProfile"
            every { args } returns listOf("createProfile".toByteArray(), profileBytes)
            every { parameters } returns listOf(String(profileBytes))
        }

        chaincode.invoke(chaincodeStub).apply {
            assertThat(status).isEqualTo(ERROR_THRESHOLD)
            assertThat(message).isNotNull()
            assertThat(payload).isNull()
        }

        verify(exactly = 0) { chaincodeStub.putState(any(), any()) }
    }

    @Test
    internal fun `when invoking getProfileInformation a profile by an existing citizen ID a profile is returned and state not modified`() {

        val request = GetProfileInformation.newBuilder()
            .setCitizenIdentifier(testProfile.citizenIdentifier)
            .build()

        chaincodeStub.apply {
            every { function } returns "getProfileInformation"

            every { args } returns listOf(
                "getProfileInformation".toByteArray(),
                request.toByteArray()
            )
            every { parameters } returns listOf(request.toString())

            every {
                getStringState(testProfile.citizenIdentifier)
            } returns testProfile.toEntity().toJson()
        }

        chaincode.invoke(chaincodeStub).apply {
            assertThat(Profile.parseFrom(payload)).isEqualTo(testProfile)
            assertThat(status).isEqualTo(SUCCESS)
            assertThat(message).isNull()
        }

        verify(exactly = 0) { chaincodeStub.putState(any(), any()) }
    }

    @Test
    internal fun `when invoking getProfileInformation by an non-existing citizen ID an error is returned and state not modified`() {

        val request = GetProfileInformation.newBuilder()
            .setCitizenIdentifier(testProfile.citizenIdentifier)
            .build().toByteArray()

        chaincodeStub.apply {
            every { function } returns "retrieveProfile"
            every { args } returns listOf("retrieveProfile".toByteArray(), request)
            every { parameters } returns listOf(String(request))
            every { getStringState(any()) } returns ""
        }

        chaincode.invoke(chaincodeStub).apply {
            assertThat(status).isEqualTo(ERROR_THRESHOLD)
            assertThat(message).isNotNull()
            assertThat(payload).isNull()
        }

        verify(exactly = 0) { chaincodeStub.putState(any(), any()) }
    }

    @Test
    internal fun `when updating a document proof with an existing ID a profile is updated in the chaincode state`() {

        val request = UpdateProfileProofDocument.newBuilder()
            .setCitizenIdentifier(testProfile.citizenIdentifier)
            .setDocProof(testProfile.identifyProofs)
            .build().toByteArray()

        val profile = testProfile.toBuilder()
            .setCurrentHealthcarePlan(
                testProfile.currentHealthcarePlan.toBuilder()
                    .setEligible(false)
                    .build()
            ).build()

        chaincodeStub.apply {
            every { function } returns "updateProfileProofDocument"
            every { args } returns listOf("updateProfileProofDocument".toByteArray(), request)
            every { parameters } returns listOf(String(request))
            every { getStringState(testProfile.citizenIdentifier) } returns profile.toEntity().toJson()
        }

        val expected = testProfile.toEntity().copy(
            identifyProofs = testProfile.identifyProofs.toEntityOrNull()
        )

        chaincode.invoke(chaincodeStub).apply {
            assertThat(status).isEqualTo(SUCCESS)
            assertThat(message).isNotNull()
            assertThat(Profile.parseFrom(payload)).isEqualTo(expected.toMessage())
        }

        verify {
            chaincodeStub.putStringState(testProfile.citizenIdentifier, expected.toJson())
        }
    }

    @Test
    internal fun `when updating a document proof with an empty message an error is returned and state not modified`() {

        val request = UpdateProfileProofDocument.getDefaultInstance().toByteArray()

        chaincodeStub.apply {
            every { function } returns "updateProfileProofDocument"
            every { args } returns listOf("updateProfileProofDocument".toByteArray(), request)
            every { parameters } returns listOf(String(request))
        }

        chaincode.invoke(chaincodeStub).apply {
            assertThat(status).isEqualTo(ERROR_THRESHOLD)
            assertThat(message).isNotNull()
            assertThat(payload).isNull()
        }

        verify(exactly = 0) { chaincodeStub.putState(any(), any()) }
    }

    @Test
    internal fun `when setting eligibility with an existing ID a profile is updated in the chaincode state`() {

        val request = SetEligibility.newBuilder()
            .setCitizenIdentifier(testProfile.citizenIdentifier)
            .setEligible(false)
            .build().toByteArray()

        val profile = testProfile.toEntity()

        chaincodeStub.apply {
            every { function } returns "setEligibility"
            every { args } returns listOf("setEligibility".toByteArray(), request)
            every { parameters } returns listOf(String(request))
            every { getStringState(testProfile.citizenIdentifier) } returns profile.toJson()
        }

        val expected = testProfile.toEntity().run {
            copy(currentHealthcarePlan = currentHealthcarePlan.copy(eligible = false))
        }

        chaincode.invoke(chaincodeStub).apply {
            assertThat(status).isEqualTo(SUCCESS)
            assertThat(message).isNotNull()
            assertThat(Profile.parseFrom(payload)).isEqualTo(expected.toMessage())
        }

        verify {
            chaincodeStub.putStringState(testProfile.citizenIdentifier, expected.toJson())
        }
    }

    @Test
    internal fun `when assigning a healthcare plan with an existing ID a profile is updated in the chaincode state`() {

        val request = AssignHealthcarePlan.newBuilder()
            .setCitizenIdentifier(testProfile.citizenIdentifier)
            .setPlan(testProfile.currentHealthcarePlan.plan)
            .build().toByteArray()

        val profile = testProfile.toEntity()

        chaincodeStub.apply {
            every { function } returns "assignHealthcarePlan"
            every { args } returns listOf("assignHealthcarePlan".toByteArray(), request)
            every { parameters } returns listOf(String(request))
            every { getStringState(testProfile.citizenIdentifier) } returns profile.toJson()
        }

        val expected = testProfile.toEntity().run {
            copy(
                currentHealthcarePlan = currentHealthcarePlan.copy(
                    healthcareStatus = HealthcareStatus.ASSIGNED,
                    assignedDate = LocalDateTime.now(HealthcareChaincode.clock),
                    plan = testProfile.currentHealthcarePlan.plan.toEntityOrNull()
                )
            )
        }

        chaincode.invoke(chaincodeStub).apply {
            assertThat(status).isEqualTo(SUCCESS)
            assertThat(message).isNotNull()
            assertThat(Profile.parseFrom(payload)).isEqualTo(expected.toMessage())
        }

        verify {
            chaincodeStub.putStringState(testProfile.citizenIdentifier, expected.toJson())
        }
    }

    @Test
    internal fun `when approving a healthcare plan with an existing ID a profile is updated in the chaincode state`() {

        val request = ApproveHealthcarePlan.newBuilder()
            .setCitizenIdentifier(testProfile.citizenIdentifier)
            .setPlanApprover(testProfile.currentHealthcarePlan.planApprover)
            .build().toByteArray()

        chaincodeStub.apply {
            every { function } returns "approveHealthcarePlan"
            every { args } returns listOf("approveHealthcarePlan".toByteArray(), request)
            every { parameters } returns listOf(String(request))
            every { getStringState(testProfile.citizenIdentifier) } returns testProfile.toEntity().toJson()
        }

        val expected = testProfile.toEntity().run {
            copy(
                currentHealthcarePlan = currentHealthcarePlan.copy(
                    healthcareStatus = HealthcareStatus.APPROVED,
                    approvedDate = LocalDateTime.now(HealthcareChaincode.clock),
                    planApprover = testProfile.currentHealthcarePlan.planApprover,
                    approved = true
                )
            )
        }

        chaincode.invoke(chaincodeStub).apply {
            assertThat(status).isEqualTo(SUCCESS)
            assertThat(message).isNotNull()
            assertThat(Profile.parseFrom(payload)).isEqualTo(expected.toMessage())
        }

        verify {
            chaincodeStub.putStringState(testProfile.citizenIdentifier, expected.toJson())
        }
    }

    private fun createTestProfile(): Profile {
        val basicProfile = BasicProfileItem.newBuilder()
            .setApplicationType(ApplicationType.INDIVIDUAL)
            .setDateOfBirth(LocalDate.of(1979, 12, 9).toTimestamp())
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
            .setName("Test Healthcare Plan")
            .setId("THP")
            .build()

        val plan = HealthcarePlan.newBuilder()
            .setPlan(planItem)
            .setHealthcareStatus(ASSIGNED)
            .setApprovedDate(LocalDate.now(HealthcareChaincode.clock).toTimestamp())
            .setAssignedDate(LocalDate.now(HealthcareChaincode.clock).toTimestamp())
            .setEnrolledDate(LocalDate.now(HealthcareChaincode.clock).toTimestamp())
            .setEnrollmentBrokerAssignedState(NY)
            .setEnrollmentStatus(EnrollmentStatus.VOLUNTARY)
            .setPlanApprover("Test Approver")
            .setOwnerState(NY)
            .setApproved(true)
            .setEligible(false)
            .build()

        val proof = DocProof.newBuilder()
            .setContainer("Test proof container")
            .setContentType("application/pdf")
            .setFileName("test-drive-license.pdf")
            .setHash("test-drive-license.pdf".hashCode().toString())
            .setStorageSharding("Test sharding")
            .build()

        return Profile.newBuilder()
            .setStatus(COMPLETED)
            .setTransactionId("51486e77-c4e2-4396-81ee-af64f583d874")
            .setTransactedTime(LocalDate.of(2019, 1, 9).toTimestamp())
            .setCitizenIdentifier("acbb0316-c5f4-4726-9941-f5deae5cc11a")
            .setBasicProfile(basicProfile)
            .setActiveState(NY)
            .setCurrentHealthcarePlan(plan)
            .setPreferredHealthcarePlan(planItem)
            .setDescription("Test profile")
            .setEnrolledHealthInsurance(false)
            .setStateApprover("State approver")
            .setIdentifyProofs(proof)
            .build()
    }
}
