using System.ComponentModel.DataAnnotations;
using RuleKit.Tests.Helpers;
using RuleKit.Tests.Models;

namespace RuleKit.Tests.Conditional
{
    public class RequiredIfAttributeTests
    {
        /// <summary>
        /// Validates an informed value when the condition is met.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Success_When_Condition_Is_Met_And_Value_Is_Informed()
        {
            var model = new RequiredIfModel
            {
                WantsInvoice = true,
                TaxIdentifier = "12345678A"
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Empty(validationResults);
        }

        /// <summary>
        /// Validates missing values when the condition is met.
        /// </summary>
        /// <param name="value">The missing value to validate.</param>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Validate_Should_Return_Error_When_Condition_Is_Met_And_Value_Is_Missing(string? value)
        {
            var model = new RequiredIfModel
            {
                WantsInvoice = true,
                TaxIdentifier = value
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Single(validationResults);
        }

        /// <summary>
        /// Validates a missing value when the condition is not met.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Success_When_Condition_Is_Not_Met()
        {
            var model = new RequiredIfModel
            {
                WantsInvoice = false,
                TaxIdentifier = null
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Empty(validationResults);
        }

        /// <summary>
        /// Validates empty strings when they are explicitly allowed.
        /// </summary>
        /// <param name="value">The empty or white-space value.</param>
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Validate_Should_Return_Success_When_Empty_Strings_Are_Allowed(string value)
        {
            var model = new AllowEmptyStringsModel
            {
                IsRequired = true,
                Value = value
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Empty(validationResults);
        }

        /// <summary>
        /// Validates that exact string comparison distinguishes casing.
        /// </summary>
        [Fact]
        public void Validate_Should_Not_Meet_Exact_String_Condition_When_Casing_Is_Different()
        {
            var model = new ExactStringModel
            {
                CustomerType = "company",
                CompanyName = null
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Empty(validationResults);
        }

        /// <summary>
        /// Validates string conditions while ignoring casing.
        /// </summary>
        [Fact]
        public void Validate_Should_Meet_String_Condition_When_Casing_Is_Ignored()
        {
            var model = new IgnoreCaseModel
            {
                CustomerType = "company",
                CompanyName = null
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Single(validationResults);
        }

        /// <summary>
        /// Validates that exact string comparison distinguishes diacritics.
        /// </summary>
        [Fact]
        public void Validate_Should_Not_Meet_Exact_String_Condition_When_Diacritics_Are_Different()
        {
            var model = new ExactDiacriticsModel
            {
                VehicleType = "camion",
                Identifier = null
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Empty(validationResults);
        }

        /// <summary>
        /// Validates string conditions while ignoring diacritics.
        /// </summary>
        [Fact]
        public void Validate_Should_Meet_String_Condition_When_Diacritics_Are_Ignored()
        {
            var model = new IgnoreDiacriticsModel
            {
                VehicleType = "camion",
                Identifier = null
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Single(validationResults);
        }

        /// <summary>
        /// Validates string conditions while ignoring both casing and diacritics.
        /// </summary>
        [Fact]
        public void Validate_Should_Meet_String_Condition_When_Casing_And_Diacritics_Are_Ignored()
        {
            var model = new IgnoreCaseAndDiacriticsModel
            {
                VehicleType = "CAMION",
                Identifier = null
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Single(validationResults);
        }

        /// <summary>
        /// Validates enum conditions.
        /// </summary>
        [Fact]
        public void Validate_Should_Compare_Enum_Condition()
        {
            var model = new EnumConditionModel
            {
                Status = TestStatus.Approved,
                ApprovalCode = null
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Single(validationResults);
        }

        /// <summary>
        /// Validates nullable condition properties against their underlying value type.
        /// </summary>
        [Fact]
        public void Validate_Should_Compare_Nullable_Condition_With_Underlying_Value()
        {
            var model = new NullableConditionModel
            {
                Level = 1,
                Description = null
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Single(validationResults);
        }

        /// <summary>
        /// Validates a null condition value.
        /// </summary>
        [Fact]
        public void Validate_Should_Meet_Condition_When_Other_Value_And_Expected_Value_Are_Null()
        {
            var model = new NullConditionModel
            {
                Reason = null,
                Description = null
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Single(validationResults);
        }

        /// <summary>
        /// Validates a non-null condition value when null is expected.
        /// </summary>
        [Fact]
        public void Validate_Should_Not_Meet_Condition_When_Only_Expected_Value_Is_Null()
        {
            var model = new NullConditionModel
            {
                Reason = "Provided",
                Description = null
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Empty(validationResults);
        }

        /// <summary>
        /// Validates missing condition properties.
        /// </summary>
        [Fact]
        public void Validate_Should_Throw_When_Other_Property_Does_Not_Exist()
        {
            var model = new MissingPropertyModel();

            var exception = Assert.Throws<InvalidOperationException>(() => ValidationTestHelper.Validate(model));

            Assert.Contains(nameof(RequiredIfAttribute), exception.Message);
            Assert.Contains("Missing", exception.Message);
        }

        /// <summary>
        /// Validates expected values with incompatible types.
        /// </summary>
        [Fact]
        public void Validate_Should_Throw_When_Expected_Value_Type_Is_Different()
        {
            var model = new DifferentTypesModel
            {
                Level = 1,
                Description = null
            };

            var exception = Assert.Throws<InvalidOperationException>(() => ValidationTestHelper.Validate(model));

            Assert.Contains(typeof(long).FullName!, exception.Message);
            Assert.Contains(typeof(int).FullName!, exception.Message);
        }

        /// <summary>
        /// Validates null conditions on non-nullable properties.
        /// </summary>
        [Fact]
        public void Validate_Should_Throw_When_Null_Is_Expected_For_Non_Nullable_Property()
        {
            var model = new InvalidNullConditionModel();

            var exception = Assert.Throws<InvalidOperationException>(() => ValidationTestHelper.Validate(model));

            Assert.Contains("non-nullable", exception.Message);
        }

        /// <summary>
        /// Validates string comparison options on non-string properties.
        /// </summary>
        [Fact]
        public void Validate_Should_Throw_When_String_Options_Are_Used_With_Non_String_Property()
        {
            var model = new InvalidStringOptionsModel
            {
                IsRequired = true,
                Value = null
            };

            var exception = Assert.Throws<InvalidOperationException>(() => ValidationTestHelper.Validate(model));

            Assert.Contains(nameof(RequiredIfAttribute.IgnoreCase), exception.Message);
            Assert.Contains(nameof(RequiredIfAttribute.IgnoreDiacritics), exception.Message);
        }

        /// <summary>
        /// Validates missing condition property arguments.
        /// </summary>
        /// <param name="otherProperty">The missing property name.</param>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Constructor_Should_Throw_When_OtherProperty_Is_Missing(string? otherProperty)
        {
            var exception = Assert.ThrowsAny<ArgumentException>(() => new RequiredIfAttribute(otherProperty!, true));

            Assert.Equal("otherProperty", exception.ParamName);
        }

        /// <summary>
        /// Exposes the configured condition.
        /// </summary>
        [Fact]
        public void Constructor_Should_Set_Condition()
        {
            var attribute = new RequiredIfAttribute("IsRequired", true);

            Assert.Equal("IsRequired", attribute.OtherProperty);
            Assert.Equal(true, attribute.ExpectedValue);
        }

        /// <summary>
        /// Validates the default error message and affected member name.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Default_Error_Message_When_Value_Is_Invalid()
        {
            var model = new RequiredIfModel { WantsInvoice = true };
            var attribute = new RequiredIfAttribute(nameof(RequiredIfModel.WantsInvoice), true);

            var validationResult = attribute.GetValidationResult(null, CreateValidationContext(model, nameof(RequiredIfModel.TaxIdentifier)));

            Assert.NotNull(validationResult);
            Assert.Equal("The field TaxIdentifier is required when WantsInvoice equals True.", validationResult.ErrorMessage);
            Assert.Equal([nameof(RequiredIfModel.TaxIdentifier)], validationResult.MemberNames);
        }

        /// <summary>
        /// Validates a custom localized error message.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Custom_Error_Message_When_Configured()
        {
            var model = new RequiredIfModel { WantsInvoice = true };
            var attribute = new RequiredIfAttribute(nameof(RequiredIfModel.WantsInvoice), true)
            {
                ErrorMessage = "El campo {0} es obligatorio cuando {1} es {2}."
            };

            var validationResult = attribute.GetValidationResult(null, CreateValidationContext(model, nameof(RequiredIfModel.TaxIdentifier)));

            Assert.NotNull(validationResult);
            Assert.Equal("El campo TaxIdentifier es obligatorio cuando WantsInvoice es True.", validationResult.ErrorMessage);
        }

        /// <summary>
        /// Validates an error message provided by a resource type.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Resource_Error_Message_When_Configured()
        {
            var model = new RequiredIfModel { WantsInvoice = true };
            var attribute = new RequiredIfAttribute(nameof(RequiredIfModel.WantsInvoice), true)
            {
                ErrorMessageResourceName = nameof(TestResources.RequiredIfErrorMessage),
                ErrorMessageResourceType = typeof(TestResources)
            };

            var validationResult = attribute.GetValidationResult(null, CreateValidationContext(model, nameof(RequiredIfModel.TaxIdentifier)));

            Assert.NotNull(validationResult);
            Assert.Equal("Resource message for TaxIdentifier when WantsInvoice equals True.", validationResult.ErrorMessage);
        }

        private static ValidationContext CreateValidationContext(object model, string memberName)
        {
            return new ValidationContext(model)
            {
                DisplayName = memberName,
                MemberName = memberName
            };
        }

        private sealed class AllowEmptyStringsModel
        {
            public bool IsRequired { get; set; }

            [RequiredIf(nameof(IsRequired), true, AllowEmptyStrings = true)]
            public string? Value { get; set; }
        }

        private sealed class ExactStringModel
        {
            public string? CustomerType { get; set; }

            [RequiredIf(nameof(CustomerType), "Company")]
            public string? CompanyName { get; set; }
        }

        private sealed class IgnoreCaseModel
        {
            public string? CustomerType { get; set; }

            [RequiredIf(nameof(CustomerType), "Company", IgnoreCase = true)]
            public string? CompanyName { get; set; }
        }

        private sealed class ExactDiacriticsModel
        {
            public string? VehicleType { get; set; }

            [RequiredIf(nameof(VehicleType), "camión")]
            public string? Identifier { get; set; }
        }

        private sealed class IgnoreDiacriticsModel
        {
            public string? VehicleType { get; set; }

            [RequiredIf(nameof(VehicleType), "camión", IgnoreDiacritics = true)]
            public string? Identifier { get; set; }
        }

        private sealed class IgnoreCaseAndDiacriticsModel
        {
            public string? VehicleType { get; set; }

            [RequiredIf(nameof(VehicleType), "camión", IgnoreCase = true, IgnoreDiacritics = true)]
            public string? Identifier { get; set; }
        }

        private sealed class EnumConditionModel
        {
            public TestStatus Status { get; set; }

            [RequiredIf(nameof(Status), TestStatus.Approved)]
            public string? ApprovalCode { get; set; }
        }

        private sealed class NullableConditionModel
        {
            public int? Level { get; set; }

            [RequiredIf(nameof(Level), 1)]
            public string? Description { get; set; }
        }

        private sealed class NullConditionModel
        {
            public string? Reason { get; set; }

            [RequiredIf(nameof(Reason), null)]
            public string? Description { get; set; }
        }

        private sealed class MissingPropertyModel
        {
            [RequiredIf("Missing", true)]
            public string? Value { get; set; }
        }

        private sealed class DifferentTypesModel
        {
            public long Level { get; set; }

            [RequiredIf(nameof(Level), 1)]
            public string? Description { get; set; }
        }

        private sealed class InvalidNullConditionModel
        {
            public int Level { get; set; }

            [RequiredIf(nameof(Level), null)]
            public string? Description { get; set; }
        }

        private sealed class InvalidStringOptionsModel
        {
            public bool IsRequired { get; set; }

            [RequiredIf(nameof(IsRequired), true, IgnoreCase = true, IgnoreDiacritics = true)]
            public string? Value { get; set; }
        }

        private enum TestStatus
        {
            Pending,
            Approved
        }

        private static class TestResources
        {
            public static string RequiredIfErrorMessage => "Resource message for {0} when {1} equals {2}.";
        }
    }
}
