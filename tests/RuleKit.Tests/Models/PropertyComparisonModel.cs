namespace RuleKit.Tests.Models
{
    internal sealed class PropertyComparisonModel
    {
        /// <summary>
        /// Gets or sets the reference value used by comparison attributes.
        /// </summary>
        public int? ReferenceValue { get; set; }

        /// <summary>
        /// Gets or sets a value that must be greater than the reference value.
        /// </summary>
        [GreaterThan(nameof(ReferenceValue))]
        public int? GreaterValue { get; set; }

        /// <summary>
        /// Gets or sets a value that must be greater than or equal to the reference value.
        /// </summary>
        [GreaterThanOrEqualTo(nameof(ReferenceValue))]
        public int? GreaterOrEqualValue { get; set; }

        /// <summary>
        /// Gets or sets a value that must be less than the reference value.
        /// </summary>
        [LessThan(nameof(ReferenceValue))]
        public int? LessValue { get; set; }

        /// <summary>
        /// Gets or sets a value that must be less than or equal to the reference value.
        /// </summary>
        [LessThanOrEqualTo(nameof(ReferenceValue))]
        public int? LessOrEqualValue { get; set; }
    }
}
