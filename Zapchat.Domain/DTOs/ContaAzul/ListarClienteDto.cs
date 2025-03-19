using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zapchat.Domain.DTOs.ContaAzul
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class ListarClienteDto
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("document")]
        public string Document { get; set; }

        [JsonPropertyName("lateReceipts")]
        public int LateReceipts { get; set; }

        [JsonPropertyName("currentMonthReceipts")]
        public int CurrentMonthReceipts { get; set; }

        [JsonPropertyName("latePayments")]
        public int LatePayments { get; set; }

        [JsonPropertyName("currentMonthPayments")]
        public int CurrentMonthPayments { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("businessPhone")]
        public string BusinessPhone { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("profiles")]
        public List<string> Profiles { get; set; }

        [JsonPropertyName("personsLegacy")]
        public List<PersonLegacyDto> PersonsLegacy { get; set; }

        [JsonPropertyName("address")]
        public AddressDto Address { get; set; }

        [JsonPropertyName("nationalSimpleOptant")]
        public bool NationalSimpleOptant { get; set; }

        [JsonPropertyName("publicAgency")]
        public bool PublicAgency { get; set; }

        [JsonPropertyName("companyName")]
        public string CompanyName { get; set; }

        [JsonPropertyName("personType")]
        public string PersonType { get; set; }

        [JsonPropertyName("openPaymentsMessage")]
        public OpenPaymentsMessageDto OpenPaymentsMessage { get; set; }

        [JsonPropertyName("additionalInformations")]
        public List<AdditionalInformationDto> AdditionalInformations { get; set; }

        [JsonPropertyName("otherContacts")]
        public List<OtherContactDto> OtherContacts { get; set; }

        [JsonPropertyName("taxInformations")]
        public List<TaxInformationDto> TaxInformations { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }

        [JsonPropertyName("expirationReminder")]
        public ExpirationReminderDto ExpirationReminder { get; set; }
    }

    public class PersonLegacyDto
    {
        [JsonPropertyName("personLegacyId")]
        public int PersonLegacyId { get; set; }

        [JsonPropertyName("personLegacyUUID")]
        public string PersonLegacyUUID { get; set; }

        [JsonPropertyName("profile")]
        public string Profile { get; set; }
    }

    public class AddressDto
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("zipcode")]
        public string Zipcode { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("numberAddress")]
        public string NumberAddress { get; set; }

        [JsonPropertyName("complement")]
        public string Complement { get; set; }

        [JsonPropertyName("neighborhood")]
        public string Neighborhood { get; set; }

        [JsonPropertyName("idCity")]
        public int IdCity { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("resumeAddress")]
        public string ResumeAddress { get; set; }
    }

    public class OpenPaymentsMessageDto
    {
        [JsonPropertyName("number")]
        public int Number { get; set; }
    }

    public class AdditionalInformationDto
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class OtherContactDto
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }

    public class TaxInformationDto
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }
    }

    public class ExpirationReminderDto
    {
        [JsonPropertyName("reminderEmail")]
        public string ReminderEmail { get; set; }

        [JsonPropertyName("status")]
        public bool Status { get; set; }
    }
}