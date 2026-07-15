namespace RuleKit.Tests.Models
{
    internal sealed class DecimalDigitsModel
    {
        /// <summary>
        /// Gets or sets the value to validate.
        /// </summary>
        [DecimalDigits(3, 2)]
        public decimal? Value { get; set; }
    }
}
