namespace RuleKit.Tests.Models
{
    internal sealed class EmailDomainModel
    {
        /// <summary>
        /// Gets or sets the email address to validate.
        /// </summary>
        [EmailDomain("empresa.com")]
        public string? Email { get; set; }
    }
}
