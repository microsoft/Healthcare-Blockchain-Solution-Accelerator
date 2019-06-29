package com.contoso.healthcare.profile

import com.contoso.healthcare.profile.constraints.MediaType
import javax.validation.constraints.NotEmpty

data class DocProof(
    /**
     * File Name.
     */
    @field:NotEmpty val fileName: String,
    /**
     * File Hash for validating.
     */
    @field:NotEmpty val hash: String,
    /**
     * File content type.
     */
    @field:MediaType val contentType: String,
    /**
     * File location to be stored.
     */
    @field:NotEmpty val container: String,
    /**
     * File Storage location to be stored.... (for massive storage).
     */
    @field:NotEmpty val storageSharding: String
)