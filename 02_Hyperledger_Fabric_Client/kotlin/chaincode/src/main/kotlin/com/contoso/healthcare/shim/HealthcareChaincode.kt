package com.contoso.healthcare.shim

import com.contoso.healthcare.profile.EnrollmentStatus
import com.contoso.healthcare.profile.HealthcareStatus
import com.contoso.healthcare.profile.Profile
import com.contoso.healthcare.profile.ProfileStatus
import com.contoso.healthcare.service.ApproveHealthcarePlan
import com.contoso.healthcare.service.AssignHealthcarePlan
import com.contoso.healthcare.service.ChangeActiveState
import com.contoso.healthcare.service.GetProfileInformation
import com.contoso.healthcare.service.SetEligibility
import com.contoso.healthcare.service.UpdateProfileProofDocument
import com.contoso.healthcare.shim.ChaincodeDsl.Companion.badRequest
import com.contoso.healthcare.shim.ChaincodeDsl.Companion.chaincode
import com.contoso.healthcare.shim.ChaincodeDsl.Companion.success
import com.contoso.healthcare.shim.HealthcareChaincode.Companion.clock
import com.contoso.healthcare.shim.HealthcareChaincode.Companion.logger
import com.contoso.healthcare.shim.HealthcareChaincode.Companion.validator
import com.google.protobuf.TextFormat
import io.netty.handler.ssl.OpenSsl
import mu.KLogging
import org.hyperledger.fabric.shim.Chaincode
import java.time.Clock
import java.time.LocalDateTime
import javax.validation.Validation

class HealthcareChaincode : Chaincode by dsl {
    companion object : KLogging() {
        /**
         * Internal clock for testing purposes.
         */
        internal var clock = Clock.systemUTC()
        /**
         * Java Bean annotation-based validator.
         */
        internal val validator = Validation.buildDefaultValidatorFactory().validator

        @JvmStatic
        fun main(args: Array<String>) {
            logger.info { "OpenSSL available: ${OpenSsl.isAvailable()}" }
            dsl.start(args)
        }
    }
}

private val dsl = chaincode {
    invoke {

        function("createProfile", 1) {
            getMessage(0, com.contoso.healthcare.profile.messages.Profile.parser())
                ?.also { logger.debug { "Creating profile with message: ${TextFormat.shortDebugString(it)}" } }
                ?.toEntity()?.let { profile ->

                    // Check if the profile already exists
                    if (getState<String>(profile.citizenIdentifier) != null) {
                        logger.warn { "Attempt to create a profile with existing citizen ID: ${profile.citizenIdentifier}" }
                        badRequest("A profile exists with citizen ID: ${profile.citizenIdentifier}")
                    } else {
                        logger.info { "Profile not exists with citizen ID: ${profile.citizenIdentifier}" }
                        validator.validate(profile).run {
                            if (isEmpty()) {
                                logger.info { "Creating profile with citizen ID: ${profile.citizenIdentifier}" }
                                val createdProfile = profile.copy(
                                    // Override status and doc proof
                                    status = ProfileStatus.CREATED,
                                    identifyProofs = null,
                                    // Override healthcare plan and statuses
                                    currentHealthcarePlan = profile.currentHealthcarePlan.copy(
                                        healthcareStatus = HealthcareStatus.NOT_ASSIGNED,
                                        enrollmentStatus = EnrollmentStatus.PENDING,
                                        planApprover = null,
                                        assignedDate = null,
                                        approvedDate = null,
                                        enrolledDate = null,
                                        approved = false,
                                        plan = null
                                    )
                                )
                                putState(
                                    profile.citizenIdentifier, createdProfile.toJson()
                                )
                                success(
                                    "Profile created for citizen ID: ${profile.citizenIdentifier}",
                                    createdProfile.toMessage()
                                )
                            } else {
                                logger.warn { "Validation failed for profile with citizen ID: ${profile.citizenIdentifier}" }

                                forEach { logger.warn { it.formattedMessage() } }
                                badRequest("Invalid profile: ${joinToString { it.formattedMessage() }}")
                            }
                        }
                    }
                } ?: badRequest(
                "Invalid message of type " +
                    com.contoso.healthcare.profile.messages.Profile.getDescriptor().fullName
            )
        }

        function("updateProfileProofDocument", 1) {
            getMessage(0, UpdateProfileProofDocument.parser())?.run {
                if (citizenIdentifier.isBlank()) {
                    logger.warn { "Attempt to update a profile with an empty citizen ID" }
                    badRequest("Citizen ID parameter cannot be empty")
                } else {
                    docProof.toEntityOrNull()?.let { docProof ->
                        validator.validate(docProof).run {
                            if (isEmpty()) {
                                logger.info { "Updating profile state with document proof: ${docProof.fileName}" }
                                // Obtain the profile from citizen ID
                                getState<String>(citizenIdentifier)?.toEntity<Profile>()?.run {

                                    val updatedDocProfile = copy(
                                        // Override status and doc proof
                                        status = ProfileStatus.COMPLETED,
                                        identifyProofs = docProof
                                    )
                                    putState(citizenIdentifier, updatedDocProfile.toJson())
                                    success(
                                        "Profile updated with document proof for citizen ID: $citizenIdentifier",
                                        updatedDocProfile.toMessage()
                                    )
                                } ?: badRequest("Profile not found: $citizenIdentifier")
                            } else {
                                forEach { logger.warn { it.formattedMessage() } }
                                badRequest("Invalid document proof: ${joinToString { "${it.formattedMessage()}\n" }}")
                            }
                        }
                    } ?: badRequest("Document proof is empty")
                }
            } ?: badRequest("Invalid message of type ${UpdateProfileProofDocument.getDescriptor().fullName}")
        }

        function("setEligibility", 1) {
            getMessage(0, SetEligibility.parser())?.run {
                if (citizenIdentifier.isBlank()) {
                    logger.warn { "Attempt to set eligibility with an empty citizen ID" }
                    badRequest("Citizen ID parameter cannot be empty")
                } else {
                    getState<String>(citizenIdentifier)?.toEntity<Profile>()?.run {
                        logger.info { "Setting eligibility to $eligible for profile with citizen ID: $citizenIdentifier" }
                        val updatedProfile =
                            copy(currentHealthcarePlan = currentHealthcarePlan.copy(eligible = eligible))
                        putState(
                            citizenIdentifier,
                            updatedProfile.toJson()
                        )
                        success(
                            message = "Profile set to ${
                            if (!eligible) "not " else ""
                            }eligible for citizen ID: $citizenIdentifier",
                            result = updatedProfile.toMessage()
                        )
                    } ?: badRequest("Profile not found by ID: $citizenIdentifier")
                }
            } ?: badRequest("Invalid message of type ${SetEligibility.getDescriptor().fullName}")
        }

        function("getProfileInformation", 1) {
            getMessage(0, GetProfileInformation.parser())?.run {

                if (citizenIdentifier.isBlank()) {
                    logger.warn { "Attempt to retrieve a profile with an empty citizen ID" }
                    badRequest("Citizen ID parameter cannot be empty")
                } else {
                    logger.info { "Retrieving profile with citizen ID: $citizenIdentifier" }
                    getState<String>(citizenIdentifier)?.run {

                        val result = toEntity<com.contoso.healthcare.profile.Profile>().toMessage()
                        success(message = null, result = result)
                    } ?: badRequest("Profile not found by ID: $citizenIdentifier")
                }
            } ?: badRequest("Invalid message of type ${GetProfileInformation.getDescriptor().fullName}")
        }

        function("assignHealthcarePlan", 1) {
            getMessage(0, AssignHealthcarePlan.parser())?.run {

                if (citizenIdentifier.isBlank()) {
                    logger.warn { "Attempt to assign a plan with an empty citizen ID" }
                    badRequest("Citizen ID parameter cannot be empty")
                } else {
                    plan.toEntityOrNull()?.let { plan ->
                        validator.validate(plan).run {
                            if (isEmpty()) {
                                getState<String>(citizenIdentifier)?.toEntity<Profile>()?.run {
                                    logger.info { "Assigning healthcare plan to profile with citizen ID: $citizenIdentifier" }

                                    // Override healthcare status and plan
                                    val healthcarePlan = currentHealthcarePlan.copy(
                                        healthcareStatus = HealthcareStatus.ASSIGNED,
                                        assignedDate = LocalDateTime.now(clock),
                                        plan = plan
                                    )
                                    val updatedProfile = copy(currentHealthcarePlan = healthcarePlan)
                                    putState(citizenIdentifier, updatedProfile.toJson())
                                    success(
                                        message = "Profile updated with healthcare plan for citizen ID: $citizenIdentifier",
                                        result = updatedProfile.toMessage()
                                    )
                                } ?: badRequest("Profile not found by ID: $citizenIdentifier")
                            } else {
                                forEach { logger.warn { it.formattedMessage() } }
                                badRequest("Invalid healthcare plan: ${joinToString { "${it.formattedMessage()}\n" }}")
                            }
                        }
                    } ?: badRequest("Healthcare plan item is empty")
                }
            } ?: badRequest("Invalid message of type ${AssignHealthcarePlan.getDescriptor().fullName}")
        }

        function("approveHealthcarePlan", 1) {
            getMessage(0, ApproveHealthcarePlan.parser())?.run {
                if (citizenIdentifier.isBlank()) {
                    logger.warn { "Attempt to approve a plan with an empty citizen ID" }
                    badRequest("Citizen ID parameter cannot be empty")
                } else {
                    getState<String>(citizenIdentifier)?.toEntity<Profile>()?.run {
                        logger.info { "Approving healthcare plan to profile with citizen ID: $citizenIdentifier" }

                        // Override healthcare status and plan status
                        val updatedProfile = copy(
                            currentHealthcarePlan = currentHealthcarePlan.copy(
                                healthcareStatus = HealthcareStatus.APPROVED,
                                approvedDate = LocalDateTime.now(clock),
                                planApprover = planApprover,
                                approved = true
                            )
                        )
                        putState(
                            citizenIdentifier, updatedProfile.toJson()
                        )
                        success(
                            message = "Profile approved with healthcare plan for citizen ID: $citizenIdentifier",
                            result = updatedProfile.toMessage()
                        )
                    } ?: badRequest("Profile not found by ID: $citizenIdentifier")
                }
            } ?: badRequest("Invalid message of type ${ApproveHealthcarePlan.getDescriptor().fullName}")
        }

        function("changeActiveState", 1) {
            getMessage(0, ChangeActiveState.parser())?.run {
                if (citizenIdentifier.isBlank()) {
                    logger.warn { "Attempt to change active state with an empty citizen ID" }
                    badRequest("Citizen ID parameter cannot be empty")
                } else {
                    val activeState = activeState.toEntityEnum()

                    getState<String>(citizenIdentifier)?.toEntity<Profile>()?.run {
                        logger.info { "Changing active state to ${activeState.label} for citizen ID: $citizenIdentifier" }
                        val updatedProfile = copy(
                            // Override status and active state
                            status = ProfileStatus.CREATED,
                            activeState = activeState,
                            currentHealthcarePlan = currentHealthcarePlan.copy(
                                assignedDate = null,
                                approvedDate = null,
                                enrolledDate = null
                            )
                        )
                        putState(
                            citizenIdentifier, updatedProfile.toJson()
                        )
                        success(
                            message = "Active state changed for citizen ID: $citizenIdentifier",
                            result = updatedProfile.toMessage()
                        )
                    } ?: badRequest("Profile not found by ID: $citizenIdentifier")
                }
            } ?: badRequest("Invalid message of type ${ChangeActiveState.getDescriptor().fullName}")
        }
    }
}
