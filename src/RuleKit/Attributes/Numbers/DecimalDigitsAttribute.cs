using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace RuleKit
{
    /// <summary>
    /// Validates that a decimal does not exceed a specified number of integer and decimal digits.
    /// </summary>
    /// <remarks>
    /// The sign is not counted, while trailing decimal zeros are counted. <see langword="null"/> values are considered valid.
    /// Applying this attribute to a value other than <see cref="decimal"/> causes an <see cref="InvalidOperationException"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class DecimalDigitsAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "The field {0} must not exceed {1} integer digits and {2} decimal digits.";

        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalDigitsAttribute"/> class.
        /// </summary>
        /// <param name="maxIntegerDigits">The maximum number of integer digits allowed.</param>
        /// <param name="maxDecimalDigits">The maximum number of decimal digits allowed.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="maxIntegerDigits"/> is less than or equal to zero, or <paramref name="maxDecimalDigits"/> is less than zero.
        /// </exception>
        public DecimalDigitsAttribute(int maxIntegerDigits, int maxDecimalDigits) : base(DefaultErrorMessage)
        {
            if (maxIntegerDigits <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxIntegerDigits));
            }

            if (maxDecimalDigits < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxDecimalDigits));
            }

            MaxIntegerDigits = maxIntegerDigits;
            MaxDecimalDigits = maxDecimalDigits;
        }

        /// <summary>
        /// Gets the maximum number of integer digits allowed.
        /// </summary>
        public int MaxIntegerDigits { get; }

        /// <summary>
        /// Gets the maximum number of decimal digits allowed.
        /// </summary>
        public int MaxDecimalDigits { get; }

        /// <summary>
        /// Formats the validation error message.
        /// </summary>
        /// <param name="name">The name of the field that caused the validation failure.</param>
        /// <returns>The formatted validation error message.</returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, MaxIntegerDigits, MaxDecimalDigits);
        }

        /// <summary>
        /// Validates that the value does not exceed the configured number of integer and decimal digits.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context in which validation is performed.</param>
        /// <returns><see cref="ValidationResult.Success"/> when the value is valid; otherwise, a validation error.</returns>
        /// <exception cref="InvalidOperationException">
        /// The attribute is applied to a value that is not a <see cref="decimal"/>.
        /// </exception>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }

            if (value is not decimal decimalValue)
            {
                throw new InvalidOperationException($"{nameof(DecimalDigitsAttribute)} cannot validate values of type '{value.GetType().FullName}'.");
            }

            var formattedValue = decimalValue.ToString(CultureInfo.InvariantCulture);
            var unsignedValue = formattedValue[0] == '-' ? formattedValue[1..] : formattedValue;
            var decimalSeparatorIndex = unsignedValue.IndexOf('.');
            var integerDigits = decimalSeparatorIndex < 0 ? unsignedValue.Length : decimalSeparatorIndex;
            var decimalDigits = decimalSeparatorIndex < 0 ? 0 : unsignedValue.Length - decimalSeparatorIndex - 1;

            if (integerDigits <= MaxIntegerDigits && decimalDigits <= MaxDecimalDigits)
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
