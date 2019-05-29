package com.contoso.healthcare.profile.constraints

import javax.validation.Constraint
import javax.validation.constraints.NotEmpty
import javax.validation.constraints.Pattern
import kotlin.annotation.AnnotationTarget.ANNOTATION_CLASS
import kotlin.annotation.AnnotationTarget.FIELD
import kotlin.annotation.AnnotationTarget.FUNCTION
import kotlin.annotation.AnnotationTarget.PROPERTY_GETTER
import kotlin.annotation.AnnotationTarget.VALUE_PARAMETER
import kotlin.reflect.KClass

@NotEmpty
@MustBeDocumented
@Constraint(validatedBy = [])
@Pattern(regexp = MediaType.REGEXP)
@Retention(AnnotationRetention.RUNTIME)
@Target(FUNCTION, FIELD, ANNOTATION_CLASS, PROPERTY_GETTER, VALUE_PARAMETER)
annotation class MediaType(
    val message: String = "{com.contoso.healthcare.profile.constraints.MediaType.message}",
    val groups: Array<KClass<out Any>> = [],
    val payload: Array<KClass<out Any>> = []
) {
    companion object {
        const val REGEXP = "(application|audio|font|example|image|message|model|multipart|text|video|x-" +
            "(?:[0-9A-Za-z!#\$%&'*+.^_`|~-]+))/([0-9A-Za-z!#\$%&'*+.^_`|~-]+)"
    }
}
