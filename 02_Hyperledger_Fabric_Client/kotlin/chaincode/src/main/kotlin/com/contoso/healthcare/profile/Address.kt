package com.contoso.healthcare.profile

import com.contoso.healthcare.profile.State.NJ
import com.contoso.healthcare.profile.State.NY
import com.contoso.healthcare.profile.constraints.Zip
import javax.validation.constraints.NotEmpty

data class Address(
    @field:NotEmpty val street: String,
    @field:NotEmpty val city: String,
    @field:Zip([NY, NJ]) val zip: String,
    val state: State
)