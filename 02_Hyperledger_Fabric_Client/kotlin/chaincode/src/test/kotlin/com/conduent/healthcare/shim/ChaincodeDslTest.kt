package com.contoso.healthcare.shim

import com.contoso.healthcare.shim.ChaincodeDsl.Companion.badRequest
import com.contoso.healthcare.shim.ChaincodeDsl.Companion.chaincode
import com.contoso.healthcare.shim.ChaincodeDsl.Companion.internalServerError
import com.contoso.healthcare.shim.ChaincodeDsl.Companion.success
import io.mockk.clearMocks
import io.mockk.every
import io.mockk.mockk
import org.assertj.core.api.Assertions.assertThat
import org.assertj.core.api.Assertions.assertThatIllegalStateException
import org.hyperledger.fabric.shim.Chaincode.Response.Status.ERROR_THRESHOLD
import org.hyperledger.fabric.shim.Chaincode.Response.Status.INTERNAL_SERVER_ERROR
import org.hyperledger.fabric.shim.Chaincode.Response.Status.SUCCESS
import org.hyperledger.fabric.shim.ChaincodeStub
import org.junit.jupiter.api.BeforeEach
import org.junit.jupiter.api.Nested
import org.junit.jupiter.api.Test

class ChaincodeDslTest {

    private val chaincodeStub = mockk<ChaincodeStub>(relaxed = true)

    @BeforeEach
    fun setUp() {
        clearMocks(chaincodeStub)
    }

    @Nested
    internal inner class EmptyChaincode {

        @Test
        internal fun `An empty chaincode init method is successful and message is null`() {
            val chaincode = chaincode {}
            val response = chaincode.init(chaincodeStub)
            assertThat(response.status).isEqualTo(SUCCESS)
            assertThat(response.message).isNull()
        }

        @Test
        internal fun `Any empty chaincode invoke function is unsuccessful`() {
            val chaincode = chaincode {}

            every { chaincodeStub.function } returns "abc"

            val response = chaincode.invoke(chaincodeStub)
            assertThat(response.status).isEqualTo(ERROR_THRESHOLD)
            assertThat(response.message).isEqualTo("No function 'abc' defined")
        }
    }

    @Nested
    internal inner class ParameterCheck {

        @Test
        internal fun `An empty init block with numParams checks the actual number of parameters`() {
            val chaincode = chaincode {
                init(1) { success() }
            }

            every { chaincodeStub.parameters } returns listOf()

            val response = chaincode.init(chaincodeStub)
            assertThat(response.status).isEqualTo(ERROR_THRESHOLD)
            assertThat(response.message).isEqualTo("Incorrect number of parameters for function 'init' (expecting 1 and was 0)")
        }

        @Test
        internal fun `An invoke function with numParams checks the actual number of parameters`() {
            val chaincode = chaincode {
                invoke {
                    function("abc", 1) { success() }
                }
            }

            every { chaincodeStub.parameters } returns listOf()
            every { chaincodeStub.function } returns "abc"

            val response = chaincode.invoke(chaincodeStub)
            assertThat(response.status).isEqualTo(ERROR_THRESHOLD)
            assertThat(response.message).isEqualTo("Incorrect number of parameters for function 'abc' (expecting 1 and was 0)")
        }
    }

    @Nested
    internal inner class ErrorResponses {

        @Test
        internal fun `An invoke function returning an error response contains a valid status and message`() {
            val chaincode = chaincode {
                invoke {
                    function("internalServerError") {
                        badRequest("message")
                    }
                }
            }

            every { chaincodeStub.function } returns "internalServerError"

            val response = chaincode.invoke(chaincodeStub)
            assertThat(response.status).isEqualTo(ERROR_THRESHOLD)
            assertThat(response.message).isEqualTo("message")
        }

        @Test
        internal fun `An invoke function returning an exception response contains a valid status and message`() {
            val chaincode = chaincode {
                invoke {
                    function("throwable") {
                        internalServerError(Exception("message"))
                    }
                }
            }

            every { chaincodeStub.function } returns "throwable"

            val response = chaincode.invoke(chaincodeStub)
            assertThat(response.status).isEqualTo(INTERNAL_SERVER_ERROR)
            assertThat(response.message).isEqualTo("message")
        }

        @Test
        internal fun `An invoke function throwing an exception actually throws the exception`() {
            val chaincode = chaincode {
                invoke {
                    function("exception") {
                        throw IllegalStateException("message")
                    }
                }
            }

            every { chaincodeStub.function } returns "exception"

            assertThatIllegalStateException().isThrownBy {
                chaincode.invoke(chaincodeStub)
            }
        }
    }

    @Nested
    internal inner class TypeConversion {

        @Test
        internal fun `An invoke function with a boolean parameter can access it type-safely`() {
            val chaincode = chaincode {
                invoke {
                    function("negate", 1) {
                        success("OK", getParameter<Boolean>(0).not())
                    }
                }
            }

            every { chaincodeStub.parameters } returns listOf("true")
            every { chaincodeStub.function } returns "negate"

            val response = chaincode.invoke(chaincodeStub)
            assertThat(response.status).isEqualTo(SUCCESS)
            assertThat(response.message).isEqualTo("OK")
            assertThat(response.payload).isEqualTo(false.toString().toByteArray())
        }

        @Test
        internal fun `An invoke function with numeric parameters can access them type-safely`() {
            val chaincode = chaincode {
                invoke {
                    function("sum", 3) {
                        val sum = parameters.mapIndexed { i, _ -> getParameter<Int>(i) }
                            .reduce { acc, value -> acc + value }

                        success("OK", sum)
                    }
                }
            }

            every { chaincodeStub.function } returns "sum"
            every { chaincodeStub.parameters } returns listOf(1, 2, 3).map { it.toString() }

            val response = chaincode.invoke(chaincodeStub)
            assertThat(response.status).isEqualTo(SUCCESS)
            assertThat(response.message).isEqualTo("OK")
            assertThat(String(response.payload).toInt()).isEqualTo(6)
        }
    }

    /*@Nested
    internal inner class StateManagement {

        @Test
        internal fun `An invoke function putting a state object will be stored in the chain as JSON`() {

            val state = slot<String>()
            val date = LocalDateTime.of(1, 1, 1, 1, 1)

            every { chaincodeStub.function } returns "putState"
            every { chaincodeStub.putStringState("state", capture(state)) } just runs

            val chaincode = chaincode {
                invoke {
                    function("putState") {
                        putState("state", State(1.0, InnerState(date)))
                        success()
                    }
                }
            }

            val response = chaincode.invoke(chaincodeStub)
            assertThat(response.status).isEqualTo(SUCCESS)
            assertThat(response.message).isNull()

            verify {
                chaincodeStub.putStringState(
                    "state",
                    "{\"number\":1.0,\"inner\":{\"date\":\"0001-01-01T01:01:00\"}}"
                )
            }
        }

        @Test
        internal fun `An invoke function getting a state object will be retrieved from the chain as JSON`() {

            val date = LocalDateTime.of(1, 1, 1, 1, 1)

            every { chaincodeStub.function } returns "putState"
            every { chaincodeStub.getStringState("state") } returns
                    "{\"number\":1.0,\"inner\":{\"date\":\"0001-01-01T01:01:00\"}}"

            var state: State? = null
            val chaincode = chaincode {
                invoke {
                    function("putState") {
                        state = getState<State>("state")
                        success()
                    }
                }
            }

            val response = chaincode.invoke(chaincodeStub)
            assertThat(state).isEqualTo(State(1.0, InnerState(date)))
            assertThat(response.status).isEqualTo(SUCCESS)
            assertThat(response.message).isNull()

            verify { chaincodeStub.getStringState("state") }
        }
    }

    data class InnerState(val date: LocalDateTime)
    data class State(val number: Double, val inner: InnerState)*/
}