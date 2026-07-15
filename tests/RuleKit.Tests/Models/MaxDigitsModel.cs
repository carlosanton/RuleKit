namespace RuleKit.Tests.Models
{
    internal sealed class MaxDigitsModel
    {
        /// <summary>
        /// Value to validate.
        /// </summary>
        [MaxDigits(5)]
        public long? Value { get; set; }
    }
}