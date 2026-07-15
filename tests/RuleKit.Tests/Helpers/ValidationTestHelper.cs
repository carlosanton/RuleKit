using System.ComponentModel.DataAnnotations;

namespace RuleKit.Tests.Helpers
{
    internal static class ValidationTestHelper
    {
        /// <summary>
        /// Validates all properties of the specified object.
        /// </summary>
        /// <param name="instance">Object to validate.</param>
        /// <returns>Validation errors.</returns>
        internal static IReadOnlyCollection<ValidationResult> Validate(object instance)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(instance);

            Validator.TryValidateObject(
                instance,
                validationContext,
                validationResults,
                true);

            return validationResults;
        }
    }
}