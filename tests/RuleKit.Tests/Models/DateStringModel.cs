namespace RuleKit.Tests.Models
{
    internal sealed class DateStringModel
    {
        /// <summary>
        /// Gets or sets the value to validate.
        /// </summary>
        [DateString("yyyyMMdd")]
        public string? Value { get; set; }
    }
}
