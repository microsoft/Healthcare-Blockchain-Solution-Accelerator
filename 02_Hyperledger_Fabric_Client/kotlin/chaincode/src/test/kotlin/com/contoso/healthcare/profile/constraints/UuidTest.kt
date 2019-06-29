package com.contoso.healthcare.profile.constraints

import org.assertj.core.api.Assertions.assertThat
import org.junit.jupiter.api.Test

class UuidTest : AbstractConstraintTest() {

    private data class UuidHolder(@field:Uuid val uuid: String)

    @Test
    internal fun `an empty string is not a valid UUID`() {

        val holder = UuidHolder("")
        val constraints = validator.validate(holder)

        assertThat(constraints).isNotEmpty
    }

    @Test
    internal fun `the string ABCDE is not a valid UUID`() {

        val holder = UuidHolder("ABCDE")
        val constraints = validator.validate(holder)

        assertThat(constraints).isNotEmpty
    }

    @Test
    internal fun `the string 2ab1ff06-9680-4c24-9cd7-bb98730dcbd9 is a valid UUID`() {

        val holder = UuidHolder("2ab1ff06-9680-4c24-9cd7-bb98730dcbd9")
        val constraints = validator.validate(holder)

        assertThat(constraints).isEmpty()
    }
}