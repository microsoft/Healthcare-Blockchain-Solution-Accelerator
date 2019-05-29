package com.contoso.healthcare.profile.constraints

import org.assertj.core.api.Assertions.assertThat
import org.junit.jupiter.api.Test

class MonetaryAmountTest : AbstractConstraintTest() {

    private data class MonetaryAmountHolder(@field:MonetaryAmount val amount: Double)

    @Test
    internal fun `the number -1 is not a valid monetary amount`() {

        val holder = MonetaryAmountHolder(-1.0)
        val constraints = validator.validate(holder)

        assertThat(constraints).isNotEmpty
        assertThat(constraints.first().message).isEqualTo("must be a monetary amount")
    }

    @Test
    internal fun `the number 0 is a valid monetary amount`() {

        val holder = MonetaryAmountHolder(0.0)
        val constraints = validator.validate(holder)

        assertThat(constraints).isEmpty()
    }

    @Test
    internal fun `the number 10000 is a valid monetary amount`() {

        val holder = MonetaryAmountHolder(10000.0)
        val constraints = validator.validate(holder)

        assertThat(constraints).isEmpty()
    }
}