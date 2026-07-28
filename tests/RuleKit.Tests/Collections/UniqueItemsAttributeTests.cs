using System.ComponentModel.DataAnnotations;
using RuleKit.Tests.Helpers;
using RuleKit.Tests.Models;

namespace RuleKit.Tests.Collections
{
    public class UniqueItemsAttributeTests
    {
        /// <summary>
        /// Validates a collection whose items are unique.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Success_When_Items_Are_Unique()
        {
            var model = new UniqueItemsModel
            {
                Identifiers = [1, 2, 3]
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Empty(validationResults);
        }

        /// <summary>
        /// Validates a collection that contains a duplicate item.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Error_When_Item_Is_Duplicated()
        {
            var model = new UniqueItemsModel
            {
                Identifiers = [1, 2, 1]
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Single(validationResults);
        }

        /// <summary>
        /// Validates a null collection.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Success_When_Value_Is_Null()
        {
            var model = new UniqueItemsModel
            {
                Identifiers = null
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Empty(validationResults);
        }

        /// <summary>
        /// Validates an empty collection.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Success_When_Collection_Is_Empty()
        {
            var model = new UniqueItemsModel
            {
                Identifiers = []
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Empty(validationResults);
        }

        /// <summary>
        /// Validates that repeated null items are duplicates.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Error_When_Null_Item_Is_Duplicated()
        {
            var attribute = new UniqueItemsAttribute();
            string?[] values = [null, "value", null];

            var validationResult = attribute.GetValidationResult(values, CreateValidationContext());

            Assert.NotNull(validationResult);
        }

        /// <summary>
        /// Validates that standard string equality distinguishes casing and diacritics.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Success_When_String_Differences_Are_Not_Ignored()
        {
            var attribute = new UniqueItemsAttribute();
            var values = new[] { "Camión", "camion" };

            var validationResult = attribute.GetValidationResult(values, CreateValidationContext());

            Assert.Null(validationResult);
        }

        /// <summary>
        /// Validates duplicate strings while ignoring casing.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Error_When_Casing_Is_Ignored()
        {
            var attribute = new UniqueItemsAttribute
            {
                IgnoreCase = true
            };
            var values = new[] { "Backend", "backend" };

            var validationResult = attribute.GetValidationResult(values, CreateValidationContext());

            Assert.NotNull(validationResult);
        }

        /// <summary>
        /// Validates duplicate strings while ignoring diacritics.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Error_When_Diacritics_Are_Ignored()
        {
            var attribute = new UniqueItemsAttribute
            {
                IgnoreDiacritics = true
            };
            var values = new[] { "camión", "camion" };

            var validationResult = attribute.GetValidationResult(values, CreateValidationContext());

            Assert.NotNull(validationResult);
        }

        /// <summary>
        /// Validates duplicate strings while ignoring casing and diacritics.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Error_When_Casing_And_Diacritics_Are_Ignored()
        {
            var attribute = new UniqueItemsAttribute
            {
                IgnoreCase = true,
                IgnoreDiacritics = true
            };
            var values = new[] { "CAMIÓN", "camion" };

            var validationResult = attribute.GetValidationResult(values, CreateValidationContext());

            Assert.NotNull(validationResult);
        }

        /// <summary>
        /// Validates that unsupported values are treated as developer errors.
        /// </summary>
        [Fact]
        public void Validate_Should_Throw_When_Value_Type_Is_Not_Supported()
        {
            var attribute = new UniqueItemsAttribute();
            var unsupportedValues = new object[]
            {
                1,
                "not a collection"
            };

            foreach (var value in unsupportedValues)
            {
                var exception = Assert.Throws<InvalidOperationException>(() => attribute.GetValidationResult(value, CreateValidationContext()));

                Assert.Contains(nameof(UniqueItemsAttribute), exception.Message);
                Assert.Contains(value.GetType().FullName!, exception.Message);
            }
        }

        /// <summary>
        /// Validates that string comparison options cannot be used with non-string items.
        /// </summary>
        [Fact]
        public void Validate_Should_Throw_When_String_Options_Are_Used_With_Non_String_Items()
        {
            var attribute = new UniqueItemsAttribute
            {
                IgnoreCase = true
            };

            var exception = Assert.Throws<InvalidOperationException>(() => attribute.GetValidationResult(new[] { 1, 2 }, CreateValidationContext()));

            Assert.Contains(nameof(UniqueItemsAttribute), exception.Message);
            Assert.Contains(nameof(UniqueItemsAttribute.IgnoreCase), exception.Message);
            Assert.Contains(nameof(UniqueItemsAttribute.IgnoreDiacritics), exception.Message);
            Assert.Contains(typeof(int).FullName!, exception.Message);
        }

        /// <summary>
        /// Validates the default error message and affected member name.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Default_Error_Message_When_Item_Is_Duplicated()
        {
            var attribute = new UniqueItemsAttribute();

            var validationResult = attribute.GetValidationResult(new[] { 1, 1 }, CreateValidationContext());

            Assert.NotNull(validationResult);
            Assert.Equal("The field Items must contain unique items.", validationResult.ErrorMessage);
            Assert.Equal(["Items"], validationResult.MemberNames);
        }

        /// <summary>
        /// Validates a custom localized error message.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Custom_Error_Message_When_Configured()
        {
            var attribute = new UniqueItemsAttribute
            {
                ErrorMessage = "El campo {0} no puede contener elementos repetidos."
            };

            var validationResult = attribute.GetValidationResult(new[] { 1, 1 }, CreateValidationContext());

            Assert.NotNull(validationResult);
            Assert.Equal("El campo Items no puede contener elementos repetidos.", validationResult.ErrorMessage);
        }

        /// <summary>
        /// Validates an error message provided by a resource type.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Resource_Error_Message_When_Configured()
        {
            var attribute = new UniqueItemsAttribute
            {
                ErrorMessageResourceName = nameof(TestResources.UniqueItemsErrorMessage),
                ErrorMessageResourceType = typeof(TestResources)
            };

            var validationResult = attribute.GetValidationResult(new[] { 1, 1 }, CreateValidationContext());

            Assert.NotNull(validationResult);
            Assert.Equal("Resource message for Items.", validationResult.ErrorMessage);
        }

        private static ValidationContext CreateValidationContext()
        {
            return new ValidationContext(new object())
            {
                DisplayName = "Items",
                MemberName = "Items"
            };
        }

        private static class TestResources
        {
            public static string UniqueItemsErrorMessage => "Resource message for {0}.";
        }
    }
}
