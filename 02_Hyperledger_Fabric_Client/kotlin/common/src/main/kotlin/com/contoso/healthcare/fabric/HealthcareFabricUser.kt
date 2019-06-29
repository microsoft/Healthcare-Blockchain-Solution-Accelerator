package com.contoso.healthcare.fabric

import org.hyperledger.fabric.sdk.Enrollment
import org.hyperledger.fabric.sdk.User

class HealthcareFabricUser(
    private val _name: String,
    private val _affiliation: String,
    private val _account: String? = null,
    private val _mspId: String,
    private val _roles: Set<String> = setOf(),
    private val _enrollment: Enrollment
) : User {
    override fun getEnrollment() = _enrollment
    override fun getName() = _name
    override fun getRoles() = _roles
    override fun getAffiliation() = _affiliation
    override fun getAccount() = _account
    override fun getMspId() = _mspId
}