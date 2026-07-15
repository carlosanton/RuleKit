namespace RuleKit.Tests.Models
{
    internal sealed class NotDefaultModel
    {
        /// <summary>
        /// Gets or sets the value to validate.
        /// </summary>
        [NotDefault]
        public Guid? Value { get; set; }
    }
}
