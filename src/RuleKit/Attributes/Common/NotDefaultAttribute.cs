using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace RuleKit
{
    /// <summary>
    /// Validates that a value type does not contain its default value.
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> values are considered valid. Boolean and non-null reference values are not supported and cause an <see cref="InvalidOperationException"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class NotDefaultAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "The field {0} must not have its default value.";

        /// <summary>
        /// Initializes a new instance of the <see cref="NotDefaultAttribute"/> class.
        /// </summary>
        public NotDefaultAttribute() : base(DefaultErrorMessage)
        {
        }

        /// <summary>
        /// Formats the validation error message.
        /// </summary>
        /// <param name="name">The name of the field that caused the validation failure.</param>
        /// <returns>The formatted validation error message.</returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name);
        }

        /// <summary>
        /// Validates that the value is different from the default value of its type.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context in which validation is performed.</param>
        /// <returns><see cref="ValidationResult.Success"/> when the value is valid; otherwise, a validation error.</returns>
        /// <exception cref="InvalidOperationException">
        /// The attribute is applied to a Boolean or non-null reference value.
        /// </exception>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Null does not have to be checked here. Consumers can add RequiredAttribute when the value is mandatory.
            if (value is null)
            {
                return ValidationResult.Success;
            }

            var valueType = value.GetType();

            // Requiring a Boolean value to be different from false would simply mean requiring true, which is not clear from this attribute's name.
            if (valueType == typeof(bool))
            {
                throw new InvalidOperationException($"{nameof(NotDefaultAttribute)} cannot validate values of type '{valueType.FullName}'.");
            }

            // NotDefault only has a useful meaning for value types. A reference type is either null or already different from its default value.
            if (!valueType.IsValueType)
            {
                throw new InvalidOperationException($"{nameof(NotDefaultAttribute)} cannot validate values of type '{valueType.FullName}'.");
            }

            // Get the zero-initialized value without running a custom parameterless constructor that the struct may declare.
            var defaultValue = RuntimeHelpers.GetUninitializedObject(valueType);

            // Any value different from the type's default value satisfies the rule.
            if (!value.Equals(defaultValue))
            {
                return ValidationResult.Success;
            }

            // Associate the validation error with the property when the context provides its name.
            var memberNames = validationContext.MemberName is null
                ? null
                : new[] { validationContext.MemberName };

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), memberNames);
        }
    }
}
