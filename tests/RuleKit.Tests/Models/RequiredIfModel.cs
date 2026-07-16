namespace RuleKit.Tests.Models
{
    internal sealed class RequiredIfModel
    {
        /// <summary>
        /// Gets or sets whether an invoice is requested.
        /// </summary>
        public bool WantsInvoice { get; set; }

        /// <summary>
        /// Gets or sets the tax identifier required for invoices.
        /// </summary>
        [RequiredIf(nameof(WantsInvoice), true)]
        public string? TaxIdentifier { get; set; }
    }
}
