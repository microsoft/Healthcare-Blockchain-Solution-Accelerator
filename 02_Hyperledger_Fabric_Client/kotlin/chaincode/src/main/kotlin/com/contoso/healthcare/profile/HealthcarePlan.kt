package com.contoso.healthcare.profile

import java.time.LocalDateTime
import javax.validation.constraints.NotEmpty
import javax.validation.constraints.Past

data class HealthcarePlan(
    val healthcareStatus: HealthcareStatus,
    val plan: HealthcarePlanItem?,
    val ownerState: State,
    val enrollmentBrokerAssignedState: State,
    val enrollmentStatus: EnrollmentStatus,
    @field:NotEmpty val planApprover: String?,
    val approved: Boolean,
    val eligible: Boolean,
    @field:Past val assignedDate: LocalDateTime?,
    @field:Past val enrolledDate: LocalDateTime?,
    @field:Past val approvedDate: LocalDateTime?
)
