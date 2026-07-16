using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace RuleKit
{
    /// <summary>
    /// Makes a property required when another property contains a specified value.
    /// </summary>
    /// <remarks>
    /// String conditions use exact ordinal equality by default. <see cref="IgnoreCase"/> and <see cref="IgnoreDiacritics"/> can be enabled explicitly.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RequiredIfAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "The field {0} is required when {1} equals {2}.";

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredIfAttribute"/> class.
        /// </summary>
        /// <param name="otherProperty">The name of the property that contains the condition.</param>
        /// <param name="expectedValue">The value that makes the annotated property required.</param>
        /// <exception cref="ArgumentException"><paramref name="otherProperty"/> is <see langword="null"/>, empty or contains only white-space characters.</exception>
        public RequiredIfAttribute(string otherProperty, object? expectedValue) : base(DefaultErrorMessage)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(otherProperty);
            OtherProperty = otherProperty;
            ExpectedValue = expectedValue;
        }

        /// <summary>
        /// Gets the name of the property that contains the condition.
        /// </summary>
        public string OtherProperty { get; }

        /// <summary>
        /// Gets the value that makes the annotated property required.
        /// </summary>
        public object? ExpectedValue { get; }

        /// <summary>
        /// Gets or sets whether empty and white-space strings satisfy the requirement.
        /// </summary>
        public bool AllowEmptyStrings { get; set; }

        /// <summary>
        /// Gets or sets whether string conditions ignore differences between uppercase and lowercase characters.
        /// </summary>
        public bool IgnoreCase { get; set; }

        /// <summary>
        /// Gets or sets whether string conditions ignore diacritical marks such as accents.
        /// </summary>
        public bool IgnoreDiacritics { get; set; }

        /// <summary>
        /// Formats the validation error message.
        /// </summary>
        /// <param name="name">The name of the field that caused the validation failure.</param>
        /// <returns>The formatted validation error message.</returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, OtherProperty, GetExpectedValueDisplay());
        }

        /// <summary>
        /// Validates whether the property contains a required value when the configured condition is met.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context in which validation is performed.</param>
        /// <returns><see cref="ValidationResult.Success"/> when the value is valid; otherwise, a validation error.</returns>
        /// <exception cref="InvalidOperationException">The referenced property or expected value is not configured correctly.</exception>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Resolve and check the condition property first so configuration errors are reported even when the annotated property has a value.
            var otherPropertyInfo = GetReadableProperty(validationContext.ObjectType);
            var otherPropertyType = GetComparisonType(otherPropertyInfo.PropertyType);

            ValidateExpectedValueType(otherPropertyInfo.PropertyType, otherPropertyType);

            var otherValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance);

            // The annotated property is only required when the condition property matches the expected value.
            if (!IsConditionMet(otherValue))
            {
                return ValidationResult.Success;
            }

            // Delegate the meaning of "required" to the standard attribute so both attributes behave consistently.
            var requiredAttribute = new RequiredAttribute { AllowEmptyStrings = AllowEmptyStrings };

            if (requiredAttribute.IsValid(value))
            {
                return ValidationResult.Success;
            }

            var memberNames = validationContext.MemberName is null
                ? null
                : new[] { validationContext.MemberName };

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), memberNames);
        }

        private PropertyInfo GetReadableProperty(Type objectType)
        {
            var propertyInfo = objectType.GetProperty(OtherProperty, BindingFlags.Instance | BindingFlags.Public);

            if (propertyInfo is null || !propertyInfo.CanRead || propertyInfo.GetIndexParameters().Length > 0)
            {
                throw new InvalidOperationException($"{nameof(RequiredIfAttribute)} cannot find a readable property named '{OtherProperty}' on type '{objectType.FullName}'.");
            }

            return propertyInfo;
        }

        private void ValidateExpectedValueType(Type propertyType, Type comparisonType)
        {
            if ((IgnoreCase || IgnoreDiacritics) && comparisonType != typeof(string))
            {
                throw new InvalidOperationException($"{nameof(IgnoreCase)} and {nameof(IgnoreDiacritics)} can only be used when property '{OtherProperty}' is a string.");
            }

            if (ExpectedValue is null)
            {
                var acceptsNull = !propertyType.IsValueType || Nullable.GetUnderlyingType(propertyType) is not null;

                if (!acceptsNull)
                {
                    throw new InvalidOperationException($"{nameof(RequiredIfAttribute)} cannot compare non-nullable property '{OtherProperty}' with null.");
                }

                return;
            }

            if (ExpectedValue.GetType() != comparisonType)
            {
                throw new InvalidOperationException($"{nameof(RequiredIfAttribute)} cannot compare property '{OtherProperty}' of type '{comparisonType.FullName}' with a value of type '{ExpectedValue.GetType().FullName}'.");
            }
        }

        private bool IsConditionMet(object? otherValue)
        {
            if (otherValue is null || ExpectedValue is null)
            {
                return otherValue is null && ExpectedValue is null;
            }

            if (otherValue is not string otherString || ExpectedValue is not string expectedString)
            {
                return ExpectedValue.Equals(otherValue);
            }

            if (!IgnoreCase && !IgnoreDiacritics)
            {
                return string.Equals(otherString, expectedString, StringComparison.Ordinal);
            }

            var compareOptions = CompareOptions.None;

            if (IgnoreCase)
            {
                compareOptions |= CompareOptions.IgnoreCase;
            }

            if (IgnoreDiacritics)
            {
                compareOptions |= CompareOptions.IgnoreNonSpace;
            }

            return CultureInfo.InvariantCulture.CompareInfo.Compare(otherString, expectedString, compareOptions) == 0;
        }

        private string GetExpectedValueDisplay()
        {
            if (ExpectedValue is null)
            {
                return "null";
            }

            if (ExpectedValue is string stringValue)
            {
                return $"'{stringValue}'";
            }

            return Convert.ToString(ExpectedValue, CultureInfo.CurrentCulture) ?? ExpectedValue.ToString()!;
        }

        private static Type GetComparisonType(Type propertyType)
        {
            return Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        }
    }
}
