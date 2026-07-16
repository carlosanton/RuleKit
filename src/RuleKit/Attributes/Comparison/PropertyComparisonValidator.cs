using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace RuleKit
{
    internal static class PropertyComparisonValidator
    {
        internal static ValidationResult? Validate(object? value, ValidationContext validationContext, string otherProperty, PropertyComparisonOperator comparisonOperator, ValidationAttribute attribute)
        {
            var attributeName = attribute.GetType().Name;

            if (string.IsNullOrWhiteSpace(validationContext.MemberName))
            {
                throw new InvalidOperationException($"{attributeName} requires a validation context with a member name.");
            }

            // Resolve both properties before checking their values so an invalid model configuration never goes unnoticed when one value is null.
            var currentPropertyInfo = GetReadableProperty(validationContext.ObjectType, validationContext.MemberName, attributeName);
            var otherPropertyInfo = GetReadableProperty(validationContext.ObjectType, otherProperty, attributeName);
            var currentType = GetComparisonType(currentPropertyInfo.PropertyType);
            var otherType = GetComparisonType(otherPropertyInfo.PropertyType);

            if (currentType != otherType)
            {
                throw new InvalidOperationException($"{attributeName} cannot compare properties '{currentPropertyInfo.Name}' and '{otherPropertyInfo.Name}' because their types are different.");
            }

            if (currentType == typeof(string) || currentType == typeof(bool))
            {
                throw new InvalidOperationException($"{attributeName} cannot compare values of type '{currentType.FullName}'.");
            }

            if (!typeof(IComparable).IsAssignableFrom(currentType))
            {
                throw new InvalidOperationException($"{attributeName} cannot compare values of type '{currentType.FullName}' because the type does not implement {nameof(IComparable)}.");
            }

            var otherValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance);

            // A comparison cannot be made when either side is absent. RequiredAttribute can be added independently when a value is mandatory.
            if (value is null || otherValue is null)
            {
                return ValidationResult.Success;
            }

            var comparisonResult = ((IComparable)value).CompareTo(otherValue);
            var isValid = comparisonOperator switch
            {
                PropertyComparisonOperator.GreaterThan => comparisonResult > 0,
                PropertyComparisonOperator.GreaterThanOrEqualTo => comparisonResult >= 0,
                PropertyComparisonOperator.LessThan => comparisonResult < 0,
                PropertyComparisonOperator.LessThanOrEqualTo => comparisonResult <= 0,
                _ => throw new InvalidOperationException($"Unsupported comparison operator '{comparisonOperator}'.")
            };

            if (isValid)
            {
                return ValidationResult.Success;
            }

            var memberNames = new[] { validationContext.MemberName };

            return new ValidationResult(attribute.FormatErrorMessage(validationContext.DisplayName), memberNames);
        }

        private static PropertyInfo GetReadableProperty(Type objectType, string propertyName, string attributeName)
        {
            var propertyInfo = objectType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

            if (propertyInfo is null || !propertyInfo.CanRead || propertyInfo.GetIndexParameters().Length > 0)
            {
                throw new InvalidOperationException($"{attributeName} cannot find a readable property named '{propertyName}' on type '{objectType.FullName}'.");
            }

            return propertyInfo;
        }

        private static Type GetComparisonType(Type propertyType)
        {
            return Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        }
    }

    internal enum PropertyComparisonOperator
    {
        GreaterThan,
        GreaterThanOrEqualTo,
        LessThan,
        LessThanOrEqualTo
    }
}
