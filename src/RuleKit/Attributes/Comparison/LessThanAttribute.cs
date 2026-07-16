using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace RuleKit
{
    /// <summary>
    /// Validates that a property value is less than another property value.
    /// </summary>
    /// <remarks>
    /// Both properties must have the same comparable type. Strings and Boolean values are not supported. If either value is <see langword="null"/>, validation succeeds.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class LessThanAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "The field {0} must be less than {1}.";

        /// <summary>
        /// Initializes a new instance of the <see cref="LessThanAttribute"/> class.
        /// </summary>
        /// <param name="otherProperty">The name of the property to compare with.</param>
        /// <exception cref="ArgumentException"><paramref name="otherProperty"/> is <see langword="null"/>, empty or contains only white-space characters.</exception>
        public LessThanAttribute(string otherProperty) : base(DefaultErrorMessage)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(otherProperty);
            OtherProperty = otherProperty;
        }

        /// <summary>
        /// Gets the name of the property to compare with.
        /// </summary>
        public string OtherProperty { get; }

        /// <inheritdoc/>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, OtherProperty);
        }

        /// <inheritdoc/>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            return PropertyComparisonValidator.Validate(value, validationContext, OtherProperty, PropertyComparisonOperator.LessThan, this);
        }
    }
}
