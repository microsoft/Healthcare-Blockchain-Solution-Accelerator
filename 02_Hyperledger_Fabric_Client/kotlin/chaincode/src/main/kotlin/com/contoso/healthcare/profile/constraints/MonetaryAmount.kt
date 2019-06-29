package com.contoso.healthcare.profile.constraints

import javax.validation.Constraint
import javax.validation.ReportAsSingleViolation
import javax.validation.constraints.PositiveOrZero
import kotlin.annotation.AnnotationTarget.ANNOTATION_CLASS
import kotlin.annotation.AnnotationTarget.FIELD
import kotlin.annotation.AnnotationTarget.FUNCTION
import kotlin.annotation.AnnotationTarget.PROPERTY_GETTER
import kotlin.annotation.AnnotationTarget.VALUE_PARAMETER
import kotlin.reflect.KClass

@PositiveOrZero
@MustBeDocumented
@ReportAsSingleViolation
@Constraint(validatedBy = [])
@Retention(AnnotationRetention.RUNTIME)
@Target(FUNCTION, FIELD, ANNOTATION_CLASS, PROPERTY_GETTER, VALUE_PARAMETER)
annotation class MonetaryAmount(
    val message: String = "{com.contoso.healthcare.profile.constraints.MonetaryAmount.message}",
    val groups: Array<KClass<out Any>> = [],
    val payload: Array<KClass<out Any>> = []
)
