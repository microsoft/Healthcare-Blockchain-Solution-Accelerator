package com.contoso.healthcare.shim

import com.contoso.healthcare.profile.Address
import com.contoso.healthcare.profile.ApplicationType
import com.contoso.healthcare.profile.BasicProfileItem
import com.contoso.healthcare.profile.DocProof
import com.contoso.healthcare.profile.EnrollmentStatus
import com.contoso.healthcare.profile.HealthcarePlan
import com.contoso.healthcare.profile.HealthcarePlanItem
import com.contoso.healthcare.profile.HealthcareStatus
import com.contoso.healthcare.profile.Profile
import com.contoso.healthcare.profile.ProfileStatus
import com.contoso.healthcare.profile.State
import com.contoso.healthcare.profile.messages.ApplicationType.FAMILY
import com.contoso.healthcare.profile.messages.ApplicationType.INDIVIDUAL
import com.contoso.healthcare.profile.messages.EnrollmentStatus.IN_ELIGIBLE
import com.contoso.healthcare.profile.messages.EnrollmentStatus.MANDATORY
import com.contoso.healthcare.profile.messages.EnrollmentStatus.PENDING
import com.contoso.healthcare.profile.messages.EnrollmentStatus.VOLUNTARY
import com.contoso.healthcare.profile.messages.HealthcareStatus.APPROVED
import com.contoso.healthcare.profile.messages.HealthcareStatus.ASSIGNED
import com.contoso.healthcare.profile.messages.HealthcareStatus.ENROLLED
import com.contoso.healthcare.profile.messages.HealthcareStatus.NOT_ASSIGNED
import com.contoso.healthcare.profile.messages.ProfileStatus.COMPLETED
import com.contoso.healthcare.profile.messages.ProfileStatus.CREATED
import com.contoso.healthcare.profile.messages.State.NJ
import com.contoso.healthcare.profile.messages.State.NY
import com.contoso.healthcare.shim.HealthcareChaincode.Companion.clock
import com.contoso.healthcare.shim.HealthcareChaincode.Companion.logger
import com.fasterxml.jackson.annotation.JsonInclude.Include.NON_EMPTY
import com.fasterxml.jackson.databind.DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES
import com.fasterxml.jackson.databind.DeserializationFeature.READ_UNKNOWN_ENUM_VALUES_AS_NULL
import com.fasterxml.jackson.databind.ObjectMapper
import com.fasterxml.jackson.databind.SerializationFeature.WRITE_DATES_AS_TIMESTAMPS
import com.fasterxml.jackson.datatype.jsr310.JavaTimeModule
import com.fasterxml.jackson.module.kotlin.jacksonObjectMapper
import com.fasterxml.jackson.module.kotlin.readValue
import com.google.protobuf.GeneratedMessageV3
import com.google.protobuf.Parser
import com.google.protobuf.Timestamp
import org.hyperledger.fabric.shim.ChaincodeStub
import java.time.Instant
import java.time.LocalDate
import java.time.LocalDateTime
import java.time.LocalTime
import java.time.ZoneOffset
import java.util.UUID
import javax.validation.ConstraintViolation

/*
 * Protobuf to data classes mapping extensions.
 *
 * An alternative could be to use [Jackson Binary Data Formats](https://github.com/FasterXML/jackson-dataformats-binary/tree/master/protobuf)
 */

/**
 * Extension method providing Protobuf messages parsing.
 *
 * @param index 0-based parameter index.
 * @param parser Protobuf parser for type [T].
 */
internal inline fun <reified T : GeneratedMessageV3> ChaincodeStub.getMessage(index: Int, parser: Parser<T>): T? {
    return try {
        parser.parseFrom(args[index + 1])
    } catch (e: Exception) {
        logger.warn(e) { "Error while parsing Protobuf message of type ${T::class.qualifiedName}" }
        null
    }
}

internal fun com.contoso.healthcare.profile.messages.Profile.toEntity() =
    Profile(
        transactionId.orRandomUuid(),
        transactedTime.toLocalDateTimeOrNull() ?: LocalDateTime.now(clock),
        description,
        enrolledHealthInsurance,
        citizenIdentifier.orRandomUuid(),
        basicProfile.toEntity(),
        identifyProofs.toEntityOrNull(),
        status.toEntityEnum(),
        activeState.toEntityEnum(),
        stateApprover,
        currentHealthcarePlan.toEntity(),
        preferredHealthcarePlan.toEntity()
    )

internal fun com.contoso.healthcare.profile.messages.BasicProfileItem.toEntity() =
    BasicProfileItem(
        name,
        fullAddress.toEntity(),
        dateOfBirth.toLocalDate(),
        citizenship,
        fedIncome,
        stateIncome,
        applicationType.toEntityEnum()
    )

internal fun com.contoso.healthcare.profile.messages.Address.toEntity() =
    Address(street, city, zip, state.toEntityEnum())

internal fun com.contoso.healthcare.profile.messages.DocProof.toEntityOrNull() =
    if (this == com.contoso.healthcare.profile.messages.DocProof.getDefaultInstance()) null
    else DocProof(fileName, hash, contentType, container, storageSharding)

internal fun com.contoso.healthcare.profile.messages.HealthcarePlan.toEntity() =
    HealthcarePlan(
        healthcareStatus.toEntityEnum(),
        plan.toEntityOrNull(),
        ownerState.toEntityEnum(),
        enrollmentBrokerAssignedState.toEntityEnum(),
        enrollmentStatus.toEntityEnum(),
        if (planApprover.isBlank()) null else planApprover,
        approved,
        eligible,
        assignedDate.toLocalDateTimeOrNull(),
        enrolledDate.toLocalDateTimeOrNull(),
        approvedDate.toLocalDateTimeOrNull()
    )

internal fun com.contoso.healthcare.profile.messages.HealthcarePlanItem.toEntityOrNull() =
    if (this == com.contoso.healthcare.profile.messages.HealthcarePlanItem.getDefaultInstance()) null
    else toEntity()

internal fun com.contoso.healthcare.profile.messages.HealthcarePlanItem.toEntity() =
    HealthcarePlanItem(id, name)

internal fun com.contoso.healthcare.profile.messages.State.toEntityEnum() = when (this) {
    NY -> State.NY
    NJ -> State.NJ
    com.contoso.healthcare.profile.messages.State.UNRECOGNIZED ->
        State.NY // FIXME Default value?
}

internal fun com.contoso.healthcare.profile.messages.ApplicationType.toEntityEnum() = when (this) {
    INDIVIDUAL -> ApplicationType.INDIVIDUAL
    FAMILY -> ApplicationType.FAMILY
    com.contoso.healthcare.profile.messages.ApplicationType.UNRECOGNIZED ->
        ApplicationType.INDIVIDUAL // FIXME Default value?
}

internal fun com.contoso.healthcare.profile.messages.ProfileStatus.toEntityEnum() = when (this) {
    COMPLETED -> ProfileStatus.COMPLETED
    CREATED -> ProfileStatus.CREATED
    com.contoso.healthcare.profile.messages.ProfileStatus.UNRECOGNIZED ->
        ProfileStatus.CREATED // FIXME Default value?
}

internal fun com.contoso.healthcare.profile.messages.EnrollmentStatus.toEntityEnum() = when (this) {
    PENDING -> EnrollmentStatus.PENDING
    MANDATORY -> EnrollmentStatus.MANDATORY
    VOLUNTARY -> EnrollmentStatus.VOLUNTARY
    IN_ELIGIBLE -> EnrollmentStatus.IN_ELIGIBLE
    com.contoso.healthcare.profile.messages.EnrollmentStatus.UNRECOGNIZED ->
        EnrollmentStatus.IN_ELIGIBLE // FIXME Default value?
}

internal fun com.contoso.healthcare.profile.messages.HealthcareStatus.toEntityEnum() = when (this) {
    NOT_ASSIGNED -> HealthcareStatus.NOT_ASSIGNED
    ASSIGNED -> HealthcareStatus.ASSIGNED
    APPROVED -> HealthcareStatus.APPROVED
    ENROLLED -> HealthcareStatus.ENROLLED
    com.contoso.healthcare.profile.messages.HealthcareStatus.UNRECOGNIZED ->
        HealthcareStatus.NOT_ASSIGNED // FIXME Default value?
}

internal fun Profile.toMessage() =
    com.contoso.healthcare.profile.messages.Profile.newBuilder()
        .setTransactionId(transactionId)
        .setTransactedTime(transactedTime.toTimestamp())
        .setDescription(description)
        .setEnrolledHealthInsurance(enrolledHealthInsurance)
        .setCitizenIdentifier(citizenIdentifier)
        .setBasicProfile(basicProfile.toMessage())
        .setStatus(status.toMessageEnum())
        .setActiveState(activeState.toMessageEnum())
        .setStateApprover(stateApprover)
        .setCurrentHealthcarePlan(currentHealthcarePlan.toMessage())
        .setPreferredHealthcarePlan(preferredHealthcarePlan.toMessage())
        .also {
            identifyProofs?.run { it.setIdentifyProofs(toMessage()) }
        }
        .build()

internal fun BasicProfileItem.toMessage() =
    com.contoso.healthcare.profile.messages.BasicProfileItem.newBuilder()
        .setName(name)
        .setFullAddress(fullAddress.toMessage())
        .setDateOfBirth(dateOfBirth.toTimestamp())
        .setCitizenship(citizenship)
        .setFedIncome(fedIncome)
        .setStateIncome(stateIncome)
        .setApplicationType(applicationType.toMessageEnum())
        .build()

internal fun Address.toMessage() =
    com.contoso.healthcare.profile.messages.Address.newBuilder()
        .setStreet(street)
        .setCity(city)
        .setZip(zip)
        .setState(state.toMessageEnum())
        .build()

internal fun DocProof.toMessage() = com.contoso.healthcare.profile.messages.DocProof.newBuilder()
    .setFileName(fileName)
    .setHash(hash)
    .setContentType(contentType)
    .setContainer(container)
    .setStorageSharding(storageSharding)
    .build()

internal fun HealthcarePlan.toMessage() = com.contoso.healthcare.profile.messages.HealthcarePlan.newBuilder()
    .setHealthcareStatus(healthcareStatus.toMessageEnum())
    .setOwnerState(ownerState.toMessageEnum())
    .setEnrollmentBrokerAssignedState(enrollmentBrokerAssignedState.toMessageEnum())
    .setEnrollmentStatus(enrollmentStatus.toMessageEnum())
    .setApproved(approved)
    .setEligible(eligible)
    .also {
        assignedDate?.run { it.setAssignedDate(toTimestamp()) }
        approvedDate?.run { it.setApprovedDate(toTimestamp()) }
        enrolledDate?.run { it.setEnrolledDate(toTimestamp()) }
        planApprover?.run { it.setPlanApprover(this) }
        plan?.run { it.setPlan(toMessage()) }
    }
    .build()

internal fun HealthcarePlanItem.toMessage() = com.contoso.healthcare.profile.messages.HealthcarePlanItem.newBuilder()
    .setId(id)
    .setName(name)
    .build()

internal fun com.contoso.healthcare.profile.State.toMessageEnum() = when (this) {
    com.contoso.healthcare.profile.State.NY -> NY
    com.contoso.healthcare.profile.State.NJ -> NJ
    else -> com.contoso.healthcare.profile.messages.State.UNRECOGNIZED
}

internal fun ApplicationType.toMessageEnum() = when (this) {
    com.contoso.healthcare.profile.ApplicationType.INDIVIDUAL -> INDIVIDUAL
    com.contoso.healthcare.profile.ApplicationType.FAMILY -> FAMILY
}

internal fun ProfileStatus.toMessageEnum() = when (this) {
    com.contoso.healthcare.profile.ProfileStatus.COMPLETED -> COMPLETED
    com.contoso.healthcare.profile.ProfileStatus.CREATED -> CREATED
}

internal fun EnrollmentStatus.toMessageEnum() = when (this) {
    com.contoso.healthcare.profile.EnrollmentStatus.PENDING -> PENDING
    com.contoso.healthcare.profile.EnrollmentStatus.MANDATORY -> MANDATORY
    com.contoso.healthcare.profile.EnrollmentStatus.VOLUNTARY -> VOLUNTARY
    com.contoso.healthcare.profile.EnrollmentStatus.IN_ELIGIBLE -> IN_ELIGIBLE
}

internal fun HealthcareStatus.toMessageEnum() = when (this) {
    com.contoso.healthcare.profile.HealthcareStatus.NOT_ASSIGNED -> NOT_ASSIGNED
    com.contoso.healthcare.profile.HealthcareStatus.ASSIGNED -> ASSIGNED
    com.contoso.healthcare.profile.HealthcareStatus.APPROVED -> APPROVED
    com.contoso.healthcare.profile.HealthcareStatus.ENROLLED -> ENROLLED
}

internal fun Timestamp.toLocalDate(): LocalDate =
    Instant.ofEpochSecond(this.seconds).atZone(ZoneOffset.UTC).toLocalDate()

internal fun Timestamp.toLocalDateTime(): LocalDateTime =
    LocalDateTime.ofInstant(Instant.ofEpochSecond(this.seconds), ZoneOffset.UTC)!!

internal fun Timestamp.toLocalDateTimeOrNull(): LocalDateTime? =
    this.run { if (this != Timestamp.getDefaultInstance()) toLocalDateTime() else null }

internal fun LocalDate.toTimestamp(): Timestamp =
    LocalDateTime.of(this, LocalTime.MIN).toTimestamp()

internal fun LocalDateTime.toTimestamp(): Timestamp =
    Timestamp.newBuilder().setSeconds(toEpochSecond(ZoneOffset.UTC)).build()

internal fun Any.toJson(): String = mapper.writeValueAsString(this)!!

internal inline fun <reified T> String.toEntity(): T = mapper.readValue(this)

internal fun <T> ConstraintViolation<T>.formattedMessage(): String = "$propertyPath $message"

internal fun String.orRandomUuid(): String = if (isBlank()) UUID.randomUUID().toString() else this

@PublishedApi
internal val mapper: ObjectMapper by lazy {
    jacksonObjectMapper()
        .registerModule(JavaTimeModule())
        .enable(READ_UNKNOWN_ENUM_VALUES_AS_NULL)
        .disable(FAIL_ON_UNKNOWN_PROPERTIES)
        .disable(WRITE_DATES_AS_TIMESTAMPS)
        .setSerializationInclusion(NON_EMPTY)
}