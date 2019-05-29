package com.contoso.healthcare.profile

import com.contoso.healthcare.profile.constraints.Uuid
import java.time.LocalDateTime
import javax.validation.Valid
import javax.validation.constraints.NotEmpty
import javax.validation.constraints.Past

/**
 * Basic profile information will be just property but need to be encrypted.
 */
data class Profile(
    @field:Uuid val transactionId: String,

    @field:Past val transactedTime: LocalDateTime,

    @field:NotEmpty val description: String,

    /**
     * Enrollment in Health Insurance from another source.
     */
    val enrolledHealthInsurance: Boolean,

    /**
     * Unique key for citizen. Should be encrypted and only seen by active state.
     */
    @field:Uuid val citizenIdentifier: String,

    @field:Valid val basicProfile: BasicProfileItem,

    /**
     *  Identity Proof information.
     */
    @field:Valid val identifyProofs: DocProof?,

    /**
     *  Created, completed.
     */
    val status: ProfileStatus,

    /**
     *  Profile created Caseworker state, only ActiveState can access DocProofs.
     */
    val activeState: State,

    /**
     *  State approver for eligibility.
     */
    @field:NotEmpty val stateApprover: String,

    /**
     *  Assigned Healthcare Plan.
     */
    @field:Valid val currentHealthcarePlan: HealthcarePlan,

    /**
     *  Preferred Healthcare Plan.
     */
    @field:Valid val preferredHealthcarePlan: HealthcarePlanItem
)