using System.ComponentModel.DataAnnotations;

namespace RuleKit
{
    /// <summary>
    /// Validates the maximum number of digits allowed.
    /// </summary>
    public sealed class MaxDigitsAttribute : ValidationAttribute
    {
        private readonly int _maxDigits;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maxDigits">Maximum number of digits allowed.</param>
        public MaxDigitsAttribute(int maxDigits)
        {
            if (maxDigits <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxDigits));
            }

            _maxDigits = maxDigits;
        }

        /// <summary>
        /// Validates the value.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        /// <param name="validationContext">Validation context.</param>
        /// <returns>Validation result.</returns>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            var valueString = value.ToString()!.Replace("-", string.Empty);

            if (valueString.Length <= _maxDigits)
            {
                return ValidationResult.Success;
            }

            var errorMessage = string.IsNullOrEmpty(ErrorMessage)
                ? $"The field {validationContext.DisplayName} cannot contain more than {_maxDigits} digits."
                : ErrorMessage;

            return new ValidationResult(errorMessage);
        }
    }
}