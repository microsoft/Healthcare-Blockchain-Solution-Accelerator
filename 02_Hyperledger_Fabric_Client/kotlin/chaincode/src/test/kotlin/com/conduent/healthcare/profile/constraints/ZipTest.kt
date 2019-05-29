package com.contoso.healthcare.profile.constraints

import com.contoso.healthcare.profile.State.NY
import org.assertj.core.api.Assertions.assertThat
import org.junit.jupiter.api.Test

class ZipTest : AbstractConstraintTest() {

    data class ZipCodeHolder(@field:Zip val zip: String)
    data class NewYorkZipCodeHolder(@field:Zip([NY]) val zip: String)

    @Test
    internal fun `an empty string is not a valid ZIP`() {

        val holder = ZipCodeHolder("")
        val constraints = validator.validate(holder)

        assertThat(constraints).isNotEmpty
        assertThat(constraints.first().message).isEqualTo("must be a ZIP code (valid states are [])")
    }

    @Test
    internal fun `the string ABCDE is not a valid ZIP`() {

        val holder = ZipCodeHolder("ABCDE")
        val constraints = validator.validate(holder)

        assertThat(constraints).isNotEmpty
        assertThat(constraints.first().message).isEqualTo("must be a ZIP code (valid states are [])")
    }

    @Test
    internal fun `the string 52801 is not a valid ZIP for New York`() {

        val holder = NewYorkZipCodeHolder("52801") // Iowa
        val constraints = validator.validate(holder)

        assertThat(constraints).isNotEmpty
        assertThat(constraints.first().message).isEqualTo("must be a ZIP code (valid states are [NY])")
    }

    @Test
    internal fun `the string 10269 is a valid ZIP for New York`() {

        val holder = NewYorkZipCodeHolder("10269") // New York
        val constraints = validator.validate(holder)

        assertThat(constraints).isEmpty()
    }

    @Test
    internal fun `the string 10022 is a valid ZIP for New York`() {

        val holder = NewYorkZipCodeHolder("10022") // New York
        val constraints = validator.validate(holder)

        assertThat(constraints).isEmpty()
    }

    @Test
    internal fun `the string 10005-1234 is a valid ZIP for New York`() {

        val holder = NewYorkZipCodeHolder("10005-1234") // New York
        val constraints = validator.validate(holder)

        assertThat(constraints).isEmpty()
    }

    @Test
    internal fun `the string 52801-1234 is a valid ZIP for some state`() {

        val holder = ZipCodeHolder("52801-1234") // Iowa
        val constraints = validator.validate(holder)

        assertThat(constraints).isEmpty()
    }
}