using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Zapchat.Domain.DTOs.ContaAzul
{
    public class CadastroContaAzulDto
    {
        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("archived")]
        public bool Archived { get; set; }

        [JsonPropertyName("complement")]
        public string Complement { get; set; }

        [JsonPropertyName("cpfCnpj")]
        public string CpfCnpj { get; set; }

        [JsonPropertyName("createdToday")]
        public bool? CreatedToday { get; set; }

        [JsonPropertyName("customerId")]
        public string CustomerId { get; set; }

        [JsonPropertyName("customerType")]
        public string CustomerType { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("dueDate")]
        public DateTime DueDate { get; set; }

        [JsonPropertyName("indicationId")]
        public string IndicationId { get; set; }

        [JsonPropertyName("indicationStatus")]
        public string IndicationStatus { get; set; }

        [JsonPropertyName("indicationUnavailabilityReason")]
        public string IndicationUnavailabilityReason { get; set; }

        [JsonPropertyName("isDominioIntegrationActive")]
        public bool IsDominioIntegrationActive { get; set; }

        [JsonPropertyName("lastAccess")]
        public DateTime? LastAccess { get; set; }

        [JsonPropertyName("lastBankConciliation")]
        public DateTime? LastBankConciliation { get; set; }

        [JsonPropertyName("lastExportation")]
        public LastExportationDto LastExportation { get; set; }

        [JsonPropertyName("licenseHistory")]
        public string LicenseHistory { get; set; }

        [JsonPropertyName("multipleConnections")]
        public bool MultipleConnections { get; set; }

        [JsonPropertyName("neighborhood")]
        public string Neighborhood { get; set; }

        [JsonPropertyName("number")]
        public string Number { get; set; }

        [JsonPropertyName("paymentStatus")]
        public string PaymentStatus { get; set; }

        [JsonPropertyName("postalCode")]
        public string PostalCode { get; set; }

        [JsonPropertyName("primaryCnae")]
        public string PrimaryCnae { get; set; }

        [JsonPropertyName("recommendation")]
        public string Recommendation { get; set; }

        [JsonPropertyName("relationId")]
        public int RelationId { get; set; }

        [JsonPropertyName("subscriptionType")]
        public string SubscriptionType { get; set; }

        [JsonPropertyName("tenantId")]
        public int TenantId { get; set; }
    }

    public class LastExportationDto
    {
        [JsonPropertyName("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime? StartDate { get; set; }
    }
}