package com.contoso.healthcare.profile.constraints

import javax.validation.Validation
import javax.validation.Validator

abstract class AbstractConstraintTest {
    companion object {
        val validator: Validator by lazy {
            Validation.buildDefaultValidatorFactory().validator
        }
    }
}