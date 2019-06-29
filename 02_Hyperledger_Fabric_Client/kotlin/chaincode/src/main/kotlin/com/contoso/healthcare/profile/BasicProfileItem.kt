package com.contoso.healthcare.profile

import com.contoso.healthcare.profile.constraints.MonetaryAmount
import java.time.LocalDate
import javax.validation.Valid
import javax.validation.constraints.NotEmpty
import javax.validation.constraints.Past

/**
 * Basic Identifier Entities. Should be encrypted and only seen by active state.
 */
data class BasicProfileItem(
    @field:NotEmpty val name: String,
    @field:Valid val fullAddress: Address,
    @field:Past val dateOfBirth: LocalDate,
    /**
     * US Citizen, Not US Citizen.
     */
    val citizenship: Boolean,
    /**
     * Income (Federal tax returns).
     */
    @field:MonetaryAmount val fedIncome: Double,
    /**
     * Income (State tax returns).
     */
    @field:MonetaryAmount val stateIncome: Double,
    /**
     * Application Type (Individual or Family).
     */
    val applicationType: ApplicationType
)
