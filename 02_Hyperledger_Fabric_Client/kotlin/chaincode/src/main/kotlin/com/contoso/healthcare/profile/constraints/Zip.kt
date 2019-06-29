package com.contoso.healthcare.profile.constraints

import com.contoso.healthcare.profile.State
import javax.validation.Constraint
import javax.validation.ConstraintValidator
import javax.validation.ConstraintValidatorContext
import javax.validation.ReportAsSingleViolation
import javax.validation.constraints.Pattern
import kotlin.annotation.AnnotationTarget.ANNOTATION_CLASS
import kotlin.annotation.AnnotationTarget.FIELD
import kotlin.annotation.AnnotationTarget.FUNCTION
import kotlin.annotation.AnnotationTarget.PROPERTY_GETTER
import kotlin.annotation.AnnotationTarget.VALUE_PARAMETER
import kotlin.reflect.KClass

@MustBeDocumented
@ReportAsSingleViolation
@Pattern(regexp = Zip.REGEXP)
@Retention(AnnotationRetention.RUNTIME)
@Constraint(validatedBy = [Zip.ZipValidator::class])
@Target(FUNCTION, FIELD, ANNOTATION_CLASS, PROPERTY_GETTER, VALUE_PARAMETER)
annotation class Zip(
    val value: Array<State> = [], // If empty, all states are valid
    val message: String = "{com.contoso.healthcare.profile.constraints.Zip.message}",
    val groups: Array<KClass<out Any>> = [],
    val payload: Array<KClass<out Any>> = []
) {
    class ZipValidator : ConstraintValidator<Zip, String> {
        private lateinit var zip: Zip

        override fun initialize(zip: Zip) {
            super.initialize(zip)
            this.zip = zip
        }

        override fun isValid(value: String?, context: ConstraintValidatorContext): Boolean {

            val validStates = if (zip.value.isEmpty()) {
                State.values()
            } else {
                zip.value
            }

            return value?.run { validStates.contains(State.forZipCode(this)) } ?: false
        }
    }

    companion object {
        const val REGEXP = "^\\d{5}(?:[-\\s]\\d{4})?\$"
    }
}
