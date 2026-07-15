using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace RuleKit
{
    /// <summary>
    /// Validates that a string represents a valid date in a specified format.
    /// </summary>
    /// <remarks>
    /// Validation uses <see cref="CultureInfo.InvariantCulture"/> and <see cref="DateOnly.TryParseExact(string, string, IFormatProvider, DateTimeStyles, out DateOnly)"/>.
    /// <see langword="null"/> values are considered valid. Applying this attribute to a value other than a string causes an <see cref="InvalidOperationException"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class DateStringAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "The field {0} must be a valid date in the format '{1}'.";

        /// <summary>
        /// Initializes a new instance of the <see cref="DateStringAttribute"/> class.
        /// </summary>
        /// <param name="format">The exact date format expected.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="format"/> is <see langword="null"/>, empty, contains only white-space characters or is not a valid date format.
        /// </exception>
        public DateStringAttribute(string format) : base(DefaultErrorMessage)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(format);

            try
            {
                // Formatting a known date lets us detect malformed patterns when the attribute is created instead of during model validation.
                _ = DateOnly.MinValue.ToString(format, CultureInfo.InvariantCulture);
            }
            catch (FormatException exception)
            {
                throw new ArgumentException("The date format is not valid.", nameof(format), exception);
            }

            Format = format;
        }

        /// <summary>
        /// Gets the exact date format expected.
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// Formats the validation error message.
        /// </summary>
        /// <param name="name">The name of the field that caused the validation failure.</param>
        /// <returns>The formatted validation error message.</returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Format);
        }

        /// <summary>
        /// Validates that the value is a real date written in the configured format.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context in which validation is performed.</param>
        /// <returns><see cref="ValidationResult.Success"/> when the value is valid; otherwise, a validation error.</returns>
        /// <exception cref="InvalidOperationException">
        /// The attribute is applied to a value that is not a string.
        /// </exception>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Null values are handled by RequiredAttribute when the property is mandatory.
            if (value is null)
            {
                return ValidationResult.Success;
            }

            if (value is not string stringValue)
            {
                throw new InvalidOperationException($"{nameof(DateStringAttribute)} cannot validate values of type '{value.GetType().FullName}'.");
            }

            // ParseExact checks both the requested structure and whether the value represents a real calendar date.
            if (DateOnly.TryParseExact(stringValue, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                return ValidationResult.Success;
            }

            var memberNames = validationContext.MemberName is null
                ? null
                : new[] { validationContext.MemberName };

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), memberNames);
        }
    }
}
