package com.contoso.healthcare.shim

import com.contoso.healthcare.shim.ChaincodeDsl.Companion.logger
import com.google.protobuf.GeneratedMessageV3
import mu.KLogging
import org.hyperledger.fabric.shim.Chaincode
import org.hyperledger.fabric.shim.Chaincode.Response
import org.hyperledger.fabric.shim.Chaincode.Response.Status.ERROR_THRESHOLD
import org.hyperledger.fabric.shim.Chaincode.Response.Status.INTERNAL_SERVER_ERROR
import org.hyperledger.fabric.shim.Chaincode.Response.Status.SUCCESS
import org.hyperledger.fabric.shim.ChaincodeBase
import org.hyperledger.fabric.shim.ChaincodeStub
import java.math.BigDecimal
import java.math.BigInteger

/**
 * Kotlin DSL for writing Fabric Chaincode using the Shim API.
 *
 * Simply declare a `chaincode` block to create a [ChaincodeBase] instance:
 * ```
 *  val myChaincode = chaincode {
 *      ...
 *  }
 * ```
 * You can then use it to declare a class implementing [Chaincode] by delegation:
 *  ```
 *  class MyChaincode: Chaincode by myChaincode {
 *      ...
 *  }
 * ```
 */
@ChaincodeMarker
class ChaincodeDsl : ChaincodeBase() {

    /**
     * The default empty chaincode initialization function.
     */
    private var initFunction = ChaincodeFunction(
        "init",
        0
    ) { success() }

    /**
     * The mapping between function name and chaincode function to invoke.
     */
    private val invokeFunctions = mutableMapOf<String, ChaincodeFunction>()

    override fun init(stub: ChaincodeStub) = invokeInternal(initFunction, stub)

    override fun invoke(stub: ChaincodeStub): Response {
        val function: ChaincodeFunction = invokeFunctions[stub.function]
            ?: return badRequest("No function '${stub.function}' defined")

        return invokeInternal(function, stub)
    }

    /**
     * Starts an `init` handler block allowing nested `function` blocks.
     *
     * @param numParams Numbers of parameters this function should expect.
     * @param function Function code to be executed upon invocation.
     * @receiver [ChaincodeInvocationHandler]
     * @see [Chaincode.init]
     */
    fun init(numParams: Int = 0, function: ChaincodeStub.() -> Response) {
        initFunction = ChaincodeFunction("init", numParams, function)
    }

    /**
     * Starts an `invoke` handler block allowing nested `function` blocks.
     *
     * @receiver [ChaincodeInvocationHandler]
     * @see [Chaincode.invoke]
     */
    inline fun invoke(invoke: ChaincodeInvocationHandler.() -> Unit) {
        val handler = ChaincodeInvocationHandler()
        invoke.invoke(handler)
    }

    private fun invokeInternal(function: ChaincodeFunction, stub: ChaincodeStub): Response {
        return if (stub.parameters.size != function.numParams) {

            val message = "Incorrect number of parameters for function '${function.name}' " +
                "(expecting ${function.numParams} and was ${stub.parameters.size})"

            logger.warn { message }
            badRequest(message)
        } else {
            logger.debug { "Invoking function ${function.name} with ${function.numParams} parameters." }
            return try {
                function.function.invoke(stub)
            } catch (e: Exception) {
                logger.warn(e) { "Exception while invoking function ${function.name}: ${e.message}" }
                throw e // Rethrow and let Fabric handle the exception
            }
        }
    }

    companion object : KLogging() {
        /**
         * Starts a `chaincode` block allowing nested `init` and `invoke` calls.
         * ```
         * chaincode {
         *      init {
         *              ... // this is a ChaincodeStub
         *      }
         *      invoke {
         *          function("myInvokeFunction") {
         *              ... // this is a ChaincodeStub
         *          }
         *      }
         * }
         * ```
         *
         * @param mapper Jackson object mapper to manage chaincode state.
         * @receiver [ChaincodeDsl]
         */
        inline fun chaincode(chaincode: ChaincodeDsl.() -> Unit): ChaincodeBase {
            val dsl = ChaincodeDsl()
            chaincode.invoke(dsl)
            return dsl
        }

        /**
         * Returns a Chaincode [SUCCESS] response with the given message and result.
         */
        fun success(message: String? = null, result: Any? = null): Response {
            return result.toPayload().run {
                newSuccessResponse(message, this)
            }
        }

        /**
         * Returns a Chaincode [INTERNAL_SERVER_ERROR] response with the given message and result.
         */
        fun internalServerError(message: String? = null, result: Any? = null): Response {
            return result.toPayload().run {
                newErrorResponse(message, this)
            }
        }

        /**
         * Returns a Chaincode [INTERNAL_SERVER_ERROR] response with the given throwable.
         */
        fun internalServerError(throwable: Throwable): Response {
            return newErrorResponse(throwable)
        }

        /**
         * Returns a Chaincode [ERROR_THRESHOLD] response with the given message and result.
         */
        fun badRequest(message: String? = null, result: Any? = null): Response {
            return result.toPayload().run {
                Response(ERROR_THRESHOLD, message, this)
            }
        }

        private fun Any?.toPayload(): ByteArray? {
            return when (this) {
                is ByteArray -> this
                is GeneratedMessageV3 -> this.toByteArray()
                else -> this?.toString()?.toByteArray()
            }
        }
    }

    @ChaincodeMarker
    inner class ChaincodeInvocationHandler {
        /**
         * Starts a function declaration inside of a handler block.
         *
         * @receiver [ChaincodeInvocationHandler]
         * @receiving [ChaincodeStub]
         */
        fun function(
            name: String,
            numParams: Int = 0,
//            vararg params: Pair<String, Class<*>> = arrayOf(),
            function: ChaincodeStub.() -> Response
        ) {
            requireNotNull(name) { "Function name cannot be null" }
            require(name.isNotBlank()) { "Function name cannot be blank" }
            require(0 <= numParams) { "Function parameters have to be zero or more" }
            require(!this@ChaincodeDsl.invokeFunctions.containsKey(name)) {
                "Function '$name' already defined"
            }

            this@ChaincodeDsl.invokeFunctions[name] =
                ChaincodeFunction(name, numParams, function)
        }
    }

    private data class ChaincodeFunction(
        internal val name: String,
        internal val numParams: Int,
        internal val function: ChaincodeStub.() -> Response
    )
}

/**
 * Extension method providing argument type conversion,
 * to [ChaincodeStub.getArgs], e.g. `val arg0 = getArg<Int>(0)`.
 *
 * @receiver [ChaincodeStub]
 * @see [ChaincodeStub.getStringArgs]
 */
inline fun <reified T> ChaincodeStub.getParameter(index: Int): T {
    val value = parameters[index]
    return when (T::class) {
        String::class -> value
        Byte::class -> value.toByte()
        ByteArray::class -> value.toByteArray()
        Int::class -> value.toInt()
        Long::class -> value.toLong()
        BigInteger::class -> value.toBigInteger()
        Float::class -> value.toFloat()
        Double::class -> value.toDouble()
        BigDecimal::class -> value.toBigDecimal()
        Boolean::class -> value?.toBoolean()
        else -> throw IllegalArgumentException(
            "No parameter conversion for ${T::class} and index $index"
        )
    } as T
}

/**
 * Extension method providing argument type conversion,
 * to [ChaincodeStub.getParameters], e.g. `val arg0 = getParameter<Int>(0)`.
 *
 * @receiver [ChaincodeStub]
 * @see [ChaincodeStub.getParameters]
 */
inline fun <reified T> ChaincodeStub.getArg(index: Int): T = getParameter(index - 1)

@Suppress("EXTENSION_SHADOWED_BY_MEMBER")
inline fun <reified T> ChaincodeStub.getState(key: String): T? {
    val state = getStringState(key)
    return if (!state.isNullOrEmpty()) {
        logger.debug { "Getting chaincode state: $key -> $state" }
        when (T::class) {
            String::class -> state
            Byte::class -> state.toByte()
            ByteArray::class -> state.toByteArray()
            Int::class -> state.toInt()
            Long::class -> state.toLong()
            BigInteger::class -> state.toBigInteger()
            Float::class -> state.toFloat()
            Double::class -> state.toDouble()
            BigDecimal::class -> state.toBigDecimal()
            Boolean::class -> state.toBoolean()
            else -> {
                logger.warn { "No state conversion: $key -> ${T::class}" }
                null
            }
        } as T?
    } else {
        logger.warn { "Chaincode state not found for key: $key" }
        null
    }
}

fun ChaincodeStub.putState(key: String, value: Any) {
    if (!value.toString().isEmpty()) {
        val stringValue = value.toString()
        logger.info { "Putting chaincode state: $key -> $stringValue" }
        putStringState(key, stringValue)
    } else {
        logger.warn { "Ignore putting chaincode empty state for key: $key" }
    }
}

@DslMarker
private annotation class ChaincodeMarker
