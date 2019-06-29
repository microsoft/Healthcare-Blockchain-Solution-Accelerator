package com.contoso.healthcare.profile

import javax.validation.constraints.NotEmpty

data class HealthcarePlanItem(
    @field:NotEmpty val id: String,
    @field:NotEmpty val name: String
)
