namespace RuleKit.Tests.Models
{
    internal sealed class DateRangeModel
    {
        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateOnly? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        [GreaterThanOrEqualTo(nameof(StartDate))]
        public DateOnly? EndDate { get; set; }
    }
}
