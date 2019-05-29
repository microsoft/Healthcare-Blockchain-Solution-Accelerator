package com.contoso.healthcare.profile

import com.contoso.healthcare.profile.constraints.Zip
import java.util.regex.Pattern

/**
 * List of states with ZIP codes taken from [https://en.wikipedia.org/wiki/List_of_ZIP_Code_prefixes].
 */
enum class State(
    val label: String,
    private val zips: Set<IntRange> = setOf()
) {
    AA("United States Armed Forces", setOf(34001..34999)),
    AL("Alabama", setOf(35001..35299, 35401..36999)),
    AE("Armed Forces", setOf(9000..9999)),
    AK("Alaska", setOf(99501..99999)),
    AP("United States Armed Forces", setOf(96201..96699)),
    AZ("Arizona", setOf(85001..85399, 85501..85799, 85901..86099, 86301..86599)),
    AR("Arkansas", setOf(71601..72999)),
    CA("California", setOf(90001..90899, 91001..92899, 93001..96199)),
    CO("Colorado", setOf(80001..81699)),
    CT("Connecticut", setOf(6001..6999)),
    DE("Delaware", setOf(19701..19999)),
    DC("District of Columbia", setOf(20001..20099, 20201..20599, 56901..56999)),
    FL("Florida", setOf(32001..33999, 34101..34299, 34401..34499, 34601..34799, 34901..34999)),
    GA("Georgia", setOf(30001..31999, 39901..39999)),
    HI("Hawaii", setOf(96701..96899)),
    ID("Idaho", setOf(83201..83899)),
    IL("Illinois", setOf(60001..62099, 62201..62999)),
    IN("Indiana", setOf(46001..47999)),
    IA("Iowa", setOf(50001..51699, 52001..52899)),
    KS("Kansas", setOf(66001..66299, 66401..67999)),
    KY("Kentucky", setOf(40001..41899, 42001..42799)),
    LA("Louisiana", setOf(70001..70199, 70301..70899, 71001..71499)),
    ME("Maine", setOf(3900..4999)),
    MD("Maryland", setOf(20601..21299, 21401..21999)),
    MA("Massachusetts", setOf(1100..2799)),
    MI("Michigan", setOf(48001..49999)),
    MN("Minnesota", setOf(55001..55100, 55301..56799)),
    MS("Mississippi", setOf(38601..39799)),
    MO("Missouri", setOf(63001..63199, 63301..64199, 64401..65899)),
    MT("Montana", setOf(59001..59999)),
    NE("Nebraska", setOf(68001..68199, 68301..69399)),
    NV("Nevada", setOf(88901..89199, 89301..89599, 89701..89899)),
    NH("New Hampshire", setOf(3000..3899)),
    NJ("New Jersey", setOf(7001..8999)),
    NM("New Mexico", setOf(87001..87199, 87301..87599, 87701..88499)),
    NY("New York", setOf(5..5, 10001..14999)),
    NC("North Carolina", setOf(27001..28999)),
    ND("North Dakota", setOf(58001..58899)),
    OH("Ohio", setOf(43001..45999)),
    OK("Oklahoma", setOf(73001..73199, 73401..74199, 74301..74999)),
    OR("Oregon", setOf(97001..97999)),
    PA("Pennsylvania", setOf(15001..19699)),
    RI("Rhode Island", setOf(2801..2999)),
    SC("South Carolina", setOf(29001..29999)),
    SD("South Dakota", setOf(57001..57799)),
    TN("Tennessee", setOf(37001..38599)),
    TX("Texas", setOf(73301..73399, 75001..77099, 77201..79999, 88501..88599)),
    UT("Utah", setOf(84001..84799)),
    VT("Vermont", setOf(5001..5999)),
    VA("Virginia", setOf(22001..24699)),
    WA("Washington", setOf(98001..99499)),
    WV("West Virginia", setOf(24701..26899)),
    WI("Wisconsin", setOf(53001..53299, 53401..53599, 53701..54999)),
    WY("Wyoming", setOf(82001..83199)),
    GU("Guam", setOf(96901..96999)),
    PR("Puerto Rico", setOf(601..799, 901..999)),
    VI("Virgin Islands", setOf(801..899));

    companion object {
        private val pattern = Pattern.compile(Zip.REGEXP)

        fun forZipCode(@Zip zip: String): State? {

            if (!pattern.matcher(zip).matches()) {
                return null
            }

            val fiveDigitCode = if (zip.contains("-")) {
                zip.split("-")[0]
            } else {
                zip.substring(0, 5)
            }.toInt()

            val allStates = values()
            return allStates.find { state ->
                state.zips.any { it.contains(fiveDigitCode) }
            }
        }
    }
}