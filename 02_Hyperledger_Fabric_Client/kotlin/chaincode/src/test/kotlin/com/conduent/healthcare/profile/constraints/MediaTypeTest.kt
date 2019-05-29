package com.contoso.healthcare.profile.constraints

import org.assertj.core.api.Assertions.assertThat
import org.junit.jupiter.api.Test

class MediaTypeTest : AbstractConstraintTest() {

    private data class MediaTypeHolder(@field:MediaType val mediaType: String)

    @Test
    internal fun `abcd is not a valid media type`() {

        val holder = MediaTypeHolder("abcd")
        val constraints = validator.validate(holder)

        assertThat(constraints).isNotEmpty
    }

    @Test
    internal fun `an empty string is not a valid media type`() {

        val holder = MediaTypeHolder("")
        val constraints = validator.validate(holder)

        assertThat(constraints).isNotEmpty
    }

    @Test
    internal fun `application slash pdf is a valid media type`() {

        val holder = MediaTypeHolder("application/pdf")
        val constraints = validator.validate(holder)

        assertThat(constraints).isEmpty()
    }
}