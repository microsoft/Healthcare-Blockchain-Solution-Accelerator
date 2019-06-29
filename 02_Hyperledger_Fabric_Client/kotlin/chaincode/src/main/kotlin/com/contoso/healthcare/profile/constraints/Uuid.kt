package com.contoso.healthcare.profile.constraints

import java.util.UUID
import javax.validation.Constraint
import javax.validation.ConstraintValidator
import javax.validation.ConstraintValidatorContext
import javax.validation.ReportAsSingleViolation
import javax.validation.constraints.NotEmpty
import kotlin.annotation.AnnotationTarget.ANNOTATION_CLASS
import kotlin.annotation.AnnotationTarget.FIELD
import kotlin.annotation.AnnotationTarget.FUNCTION
import kotlin.annotation.AnnotationTarget.PROPERTY_GETTER
import kotlin.annotation.AnnotationTarget.VALUE_PARAMETER
import kotlin.reflect.KClass

@NotEmpty
@MustBeDocumented
@ReportAsSingleViolation
@Retention(AnnotationRetention.RUNTIME)
@Constraint(validatedBy = [Uuid.UuidValidator::class])
@Target(FUNCTION, FIELD, ANNOTATION_CLASS, PROPERTY_GETTER, VALUE_PARAMETER)
annotation class Uuid(
    val message: String = "{com.contoso.healthcare.profile.constraints.Uuid.message}",
    val groups: Array<KClass<out Any>> = [],
    val payload: Array<KClass<out Any>> = []
) {
    class UuidValidator : ConstraintValidator<Uuid, String> {
        override fun isValid(value: String?, context: ConstraintValidatorContext): Boolean {
            return value?.run {
                try {
                    UUID.fromString(this)
                    true
                } catch (e: IllegalArgumentException) {
                    false
                }
            } ?: false
        }
    }
}
