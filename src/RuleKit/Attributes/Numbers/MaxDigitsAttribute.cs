using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Numerics;

namespace RuleKit
{
    /// <summary>
    /// Validates that an integer does not contain more than a specified number of digits.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class MaxDigitsAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "The field {0} cannot contain more than {1} digits.";

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxDigitsAttribute"/> class.
        /// </summary>
        /// <param name="maxDigits">The maximum number of digits allowed.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="maxDigits"/> is less than or equal to zero.
        /// </exception>
        public MaxDigitsAttribute(int maxDigits) : base(DefaultErrorMessage)
        {
            if (maxDigits <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxDigits));
            }

            MaxDigits = maxDigits;
        }

        /// <summary>
        /// Gets the maximum number of digits allowed.
        /// </summary>
        public int MaxDigits { get; }

        /// <summary>
        /// Formats the validation error message.
        /// </summary>
        /// <param name="name">The name of the field that caused the validation failure.</param>
        /// <returns>The formatted validation error message.</returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, MaxDigits);
        }

        /// <summary>
        /// Validates that the value does not exceed the configured number of digits.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context in which validation is performed.</param>
        /// <returns><see cref="ValidationResult.Success"/> when the value is valid; otherwise, a validation error.</returns>
        /// <exception cref="InvalidOperationException">
        /// The attribute is applied to a value that is not an integer.
        /// </exception>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }

            var digitCount = value switch
            {
                byte number => CountDigits(number),
                sbyte number => CountDigits(number),
                short number => CountDigits(number),
                ushort number => CountDigits(number),
                int number => CountDigits(number),
                uint number => CountDigits(number),
                long number => CountDigits(number),
                ulong number => CountDigits(number),
                Int128 number => CountDigits(number),
                UInt128 number => CountDigits(number),
                nint number => CountDigits(number),
                nuint number => CountDigits(number),
                BigInteger number => CountDigits(number),
                _ => throw new InvalidOperationException($"{nameof(MaxDigitsAttribute)} cannot validate values of type '{value.GetType().FullName}'.")
            };

            if (digitCount <= MaxDigits)
            {
                return ValidationResult.Success;
            }

            var memberNames = validationContext.MemberName is null
                ? null
                : new[] { validationContext.MemberName };

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), memberNames);
        }

        private static int CountDigits<T>(T value) where T : IFormattable
        {
            var formattedValue = value.ToString(null, CultureInfo.InvariantCulture);

            return formattedValue[0] == '-'
                ? formattedValue.Length - 1
                : formattedValue.Length;
        }
    }
}
