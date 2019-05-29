package com.contoso.healthcare.profile

import org.assertj.core.api.Assertions
import org.junit.jupiter.params.ParameterizedTest
import org.junit.jupiter.params.provider.Arguments.arguments
import org.junit.jupiter.params.provider.MethodSource
import java.util.stream.Stream

class StateTest {

    @ParameterizedTest
    @MethodSource("zipCodeForStateProvider")
    internal fun `state is valid for each ZIP code`(zipCode: String, state: State) {
        Assertions.assertThat(State.forZipCode(zipCode)).isEqualTo(state)
    }

    companion object {
        @JvmStatic
        fun zipCodeForStateProvider() = Stream.of(
            arguments("07039-1111", State.NJ),
            arguments("10001", State.NY),
            arguments("10048", State.NY)
        )
    }
}