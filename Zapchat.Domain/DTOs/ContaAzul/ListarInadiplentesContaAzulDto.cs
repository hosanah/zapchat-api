using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Zapchat.Domain.DTOs.ContaAzul
{
    public class ListarInadiplentesContaAzulDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("version")]
        public int Version { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("note")]
        public string Note { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("dueDate")]
        public DateTime DueDate { get; set; }

        [JsonPropertyName("expectedPaymentDate")]
        public DateTime ExpectedPaymentDate { get; set; }

        [JsonPropertyName("lastAcquittanceDate")]
        public DateTime? LastAcquittanceDate { get; set; }

        [JsonPropertyName("unpaid")]
        public decimal Unpaid { get; set; }

        [JsonPropertyName("paid")]
        public decimal Paid { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("reference")]
        public string Reference { get; set; }

        [JsonPropertyName("conciliated")]
        public bool Conciliated { get; set; }

        [JsonPropertyName("totalNetValue")]
        public decimal TotalNetValue { get; set; }

        [JsonPropertyName("renegotiation")]
        public string Renegotiation { get; set; }

        [JsonPropertyName("loss")]
        public string Loss { get; set; }

        [JsonPropertyName("attachment")]
        public bool Attachment { get; set; }

        [JsonPropertyName("recurrent")]
        public bool Recurrent { get; set; }

        [JsonPropertyName("chargeRequest")]
        public ChargeRequest ChargeRequest { get; set; }

        [JsonPropertyName("valueComposition")]
        public ValueComposition ValueComposition { get; set; }

        [JsonPropertyName("financialAccount")]
        public FinancialAccount FinancialAccount { get; set; }

        [JsonPropertyName("financialEvent")]
        public FinancialEvent FinancialEvent { get; set; }

        [JsonPropertyName("hasDigitalReceipt")]
        public bool HasDigitalReceipt { get; set; }

        [JsonPropertyName("authorizedBankSlipId")]
        public string AuthorizedBankSlipId { get; set; }

        [JsonPropertyName("acquittanceScheduled")]
        public bool AcquittanceScheduled { get; set; }

        [JsonPropertyName("acquittances")]
        public object[] Acquittances { get; set; }
    }

    public class ChargeRequest
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("version")]
        public int Version { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("confirmedAt")]
        public DateTime ConfirmedAt { get; set; }

        [JsonPropertyName("sentAt")]
        public DateTime SentAt { get; set; }
    }

    public class ValueComposition
    {
        [JsonPropertyName("grossValue")]
        public decimal GrossValue { get; set; }

        [JsonPropertyName("interest")]
        public decimal Interest { get; set; }

        [JsonPropertyName("fine")]
        public decimal Fine { get; set; }

        [JsonPropertyName("netValue")]
        public decimal NetValue { get; set; }

        [JsonPropertyName("discount")]
        public decimal Discount { get; set; }

        [JsonPropertyName("fee")]
        public decimal Fee { get; set; }
    }

    public class FinancialAccount
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class FinancialEvent
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("competenceDate")]
        public DateTime CompetenceDate { get; set; }

        [JsonPropertyName("value")]
        public decimal Value { get; set; }

        [JsonPropertyName("negotiator")]
        public Negotiator Negotiator { get; set; }

        [JsonPropertyName("reference")]
        public Reference Reference { get; set; }
    }

    public class Negotiator
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class Reference
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("origin")]
        public string Origin { get; set; }
    }

}
