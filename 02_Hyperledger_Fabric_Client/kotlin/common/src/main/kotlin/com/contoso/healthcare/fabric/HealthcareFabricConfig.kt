package com.contoso.healthcare.fabric

import com.natpryce.konfig.Configuration
import com.natpryce.konfig.ConfigurationProperties.Companion.fromResource
import com.natpryce.konfig.ConfigurationProperties.Companion.systemProperties
import com.natpryce.konfig.EnvironmentVariables
import com.natpryce.konfig.PropertyGroup
import com.natpryce.konfig.getValue
import com.natpryce.konfig.overriding
import com.natpryce.konfig.stringType
import com.natpryce.konfig.uriType

@Suppress("ClassName")
class HealthcareFabricConfig : Configuration by systemProperties() overriding
    EnvironmentVariables() overriding fromResource("fabric.properties") {

    open class Node : PropertyGroup() {
        val name by stringType
        val url by uriType
    }

    object user : PropertyGroup() {
        val name by stringType
        val mspId by stringType
        val affiliation by stringType
        val keyStore by stringType
        val signedCert by stringType
    }

    object channel : PropertyGroup() {
        val name by stringType
    }

    object chaincode : PropertyGroup() {
        val name by stringType
        val version by stringType
    }

    object orderer : Node()

    object peers : PropertyGroup() {
        object org1 : Node()
    }
}
