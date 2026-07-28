namespace RuleKit.Tests.Models
{
    internal sealed class UniqueItemsModel
    {
        /// <summary>
        /// Gets or sets the identifiers to validate.
        /// </summary>
        [UniqueItems]
        public IReadOnlyCollection<int>? Identifiers { get; set; }
    }
}
