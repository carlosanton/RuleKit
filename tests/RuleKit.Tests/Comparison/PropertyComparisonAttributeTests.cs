using System.ComponentModel.DataAnnotations;
using RuleKit.Tests.Helpers;
using RuleKit.Tests.Models;

namespace RuleKit.Tests.Comparison
{
    public class PropertyComparisonAttributeTests
    {
        /// <summary>
        /// Validates values greater than the referenced property.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="isValid">Whether the value should be valid.</param>
        [Theory]
        [InlineData(11, true)]
        [InlineData(10, false)]
        [InlineData(9, false)]
        public void GreaterThan_Should_Compare_Value_With_Other_Property(int value, bool isValid)
        {
            var model = new PropertyComparisonModel
            {
                ReferenceValue = 10,
                GreaterValue = value
            };

            AssertValidationResult(model, isValid);
        }

        /// <summary>
        /// Validates values greater than or equal to the referenced property.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="isValid">Whether the value should be valid.</param>
        [Theory]
        [InlineData(11, true)]
        [InlineData(10, true)]
        [InlineData(9, false)]
        public void GreaterThanOrEqualTo_Should_Compare_Value_With_Other_Property(int value, bool isValid)
        {
            var model = new PropertyComparisonModel
            {
                ReferenceValue = 10,
                GreaterOrEqualValue = value
            };

            AssertValidationResult(model, isValid);
        }

        /// <summary>
        /// Validates values less than the referenced property.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="isValid">Whether the value should be valid.</param>
        [Theory]
        [InlineData(9, true)]
        [InlineData(10, false)]
        [InlineData(11, false)]
        public void LessThan_Should_Compare_Value_With_Other_Property(int value, bool isValid)
        {
            var model = new PropertyComparisonModel
            {
                ReferenceValue = 10,
                LessValue = value
            };

            AssertValidationResult(model, isValid);
        }

        /// <summary>
        /// Validates values less than or equal to the referenced property.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="isValid">Whether the value should be valid.</param>
        [Theory]
        [InlineData(9, true)]
        [InlineData(10, true)]
        [InlineData(11, false)]
        public void LessThanOrEqualTo_Should_Compare_Value_With_Other_Property(int value, bool isValid)
        {
            var model = new PropertyComparisonModel
            {
                ReferenceValue = 10,
                LessOrEqualValue = value
            };

            AssertValidationResult(model, isValid);
        }

        /// <summary>
        /// Validates a date range using the natural end-date declaration.
        /// </summary>
        /// <param name="endDate">The end date to validate.</param>
        /// <param name="isValid">Whether the date range should be valid.</param>
        [Theory]
        [InlineData("2026-07-14", false)]
        [InlineData("2026-07-15", true)]
        [InlineData("2026-07-16", true)]
        public void GreaterThanOrEqualTo_Should_Validate_Date_Range(string endDate, bool isValid)
        {
            var model = new DateRangeModel
            {
                StartDate = new DateOnly(2026, 7, 15),
                EndDate = DateOnly.Parse(endDate)
            };

            AssertValidationResult(model, isValid);
        }

        /// <summary>
        /// Validates that a null current value is accepted.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Success_When_Current_Value_Is_Null()
        {
            var model = new PropertyComparisonModel
            {
                ReferenceValue = 10,
                GreaterValue = null
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Empty(validationResults);
        }

        /// <summary>
        /// Validates that a null referenced value is accepted.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Success_When_Other_Value_Is_Null()
        {
            var model = new PropertyComparisonModel
            {
                ReferenceValue = null,
                GreaterValue = 10
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Empty(validationResults);
        }

        /// <summary>
        /// Validates enum values using their declared ordering.
        /// </summary>
        [Fact]
        public void Validate_Should_Compare_Enum_Values()
        {
            var model = new EnumComparisonModel
            {
                MinimumStatus = TestStatus.Pending,
                CurrentStatus = TestStatus.Completed
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Empty(validationResults);
        }

        /// <summary>
        /// Validates custom structs that implement <see cref="IComparable"/>.
        /// </summary>
        [Fact]
        public void Validate_Should_Compare_Custom_Comparable_Structs()
        {
            var model = new CustomComparisonModel
            {
                MinimumValue = new ComparableValue(10),
                CurrentValue = new ComparableValue(11)
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Empty(validationResults);
        }

        /// <summary>
        /// Validates missing referenced property names.
        /// </summary>
        [Fact]
        public void Validate_Should_Throw_When_Other_Property_Does_Not_Exist()
        {
            var model = new MissingPropertyModel
            {
                Value = 10
            };

            var exception = Assert.Throws<InvalidOperationException>(() => ValidationTestHelper.Validate(model));

            Assert.Contains(nameof(GreaterThanAttribute), exception.Message);
            Assert.Contains("Missing", exception.Message);
        }

        /// <summary>
        /// Validates properties with different numeric types.
        /// </summary>
        [Fact]
        public void Validate_Should_Throw_When_Property_Types_Are_Different()
        {
            var model = new DifferentTypesModel
            {
                ReferenceValue = 10,
                Value = 11
            };

            var exception = Assert.Throws<InvalidOperationException>(() => ValidationTestHelper.Validate(model));

            Assert.Contains(nameof(GreaterThanAttribute), exception.Message);
            Assert.Contains("types are different", exception.Message);
        }

        /// <summary>
        /// Validates unsupported comparable types.
        /// </summary>
        /// <param name="useBoolean">Whether to validate the Boolean model instead of the string model.</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Validate_Should_Throw_When_Property_Type_Is_Not_Supported(bool useBoolean)
        {
            object model = useBoolean
                ? new BooleanComparisonModel { ReferenceValue = false, Value = true }
                : new StringComparisonModel { ReferenceValue = "A", Value = "B" };

            var exception = Assert.Throws<InvalidOperationException>(() => ValidationTestHelper.Validate(model));

            Assert.Contains(nameof(GreaterThanAttribute), exception.Message);
        }

        /// <summary>
        /// Validates types that do not implement <see cref="IComparable"/>.
        /// </summary>
        [Fact]
        public void Validate_Should_Throw_When_Property_Type_Is_Not_Comparable()
        {
            var model = new NonComparableModel
            {
                ReferenceValue = new NonComparableValue(10),
                Value = new NonComparableValue(11)
            };

            var exception = Assert.Throws<InvalidOperationException>(() => ValidationTestHelper.Validate(model));

            Assert.Contains(nameof(IComparable), exception.Message);
        }

        /// <summary>
        /// Validates missing referenced property arguments for every comparison attribute.
        /// </summary>
        [Fact]
        public void Constructor_Should_Throw_When_OtherProperty_Is_Missing()
        {
            var constructors = GetAttributeConstructors();

            foreach (var constructor in constructors)
            {
                var nullException = Assert.ThrowsAny<ArgumentException>(() => constructor(null!));
                var emptyException = Assert.Throws<ArgumentException>(() => constructor(string.Empty));
                var whiteSpaceException = Assert.Throws<ArgumentException>(() => constructor(" "));

                Assert.Equal("otherProperty", nullException.ParamName);
                Assert.Equal("otherProperty", emptyException.ParamName);
                Assert.Equal("otherProperty", whiteSpaceException.ParamName);
            }
        }

        /// <summary>
        /// Exposes the configured referenced property for every comparison attribute.
        /// </summary>
        [Fact]
        public void Constructor_Should_Set_OtherProperty()
        {
            Assert.Equal("ReferenceValue", new GreaterThanAttribute("ReferenceValue").OtherProperty);
            Assert.Equal("ReferenceValue", new GreaterThanOrEqualToAttribute("ReferenceValue").OtherProperty);
            Assert.Equal("ReferenceValue", new LessThanAttribute("ReferenceValue").OtherProperty);
            Assert.Equal("ReferenceValue", new LessThanOrEqualToAttribute("ReferenceValue").OtherProperty);
        }

        /// <summary>
        /// Validates the default messages for every comparison attribute.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Default_Error_Message_When_Value_Is_Invalid()
        {
            var cases = new (ValidationAttribute Attribute, string MemberName, int Value, string ErrorMessage)[]
            {
                (new GreaterThanAttribute(nameof(PropertyComparisonModel.ReferenceValue)), nameof(PropertyComparisonModel.GreaterValue), 10, "The field GreaterValue must be greater than ReferenceValue."),
                (new GreaterThanOrEqualToAttribute(nameof(PropertyComparisonModel.ReferenceValue)), nameof(PropertyComparisonModel.GreaterOrEqualValue), 9, "The field GreaterOrEqualValue must be greater than or equal to ReferenceValue."),
                (new LessThanAttribute(nameof(PropertyComparisonModel.ReferenceValue)), nameof(PropertyComparisonModel.LessValue), 10, "The field LessValue must be less than ReferenceValue."),
                (new LessThanOrEqualToAttribute(nameof(PropertyComparisonModel.ReferenceValue)), nameof(PropertyComparisonModel.LessOrEqualValue), 11, "The field LessOrEqualValue must be less than or equal to ReferenceValue.")
            };
            var model = new PropertyComparisonModel { ReferenceValue = 10 };

            foreach (var testCase in cases)
            {
                var validationResult = testCase.Attribute.GetValidationResult(testCase.Value, CreateValidationContext(model, testCase.MemberName));

                Assert.NotNull(validationResult);
                Assert.Equal(testCase.ErrorMessage, validationResult.ErrorMessage);
                Assert.Equal([testCase.MemberName], validationResult.MemberNames);
            }
        }

        /// <summary>
        /// Validates custom messages for every comparison attribute.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Custom_Error_Message_When_Configured()
        {
            var attributes = GetInvalidAttributes();
            var model = new PropertyComparisonModel { ReferenceValue = 10 };

            foreach (var testCase in attributes)
            {
                testCase.Attribute.ErrorMessage = "El campo {0} no cumple su relación con {1}.";

                var validationResult = testCase.Attribute.GetValidationResult(testCase.Value, CreateValidationContext(model, testCase.MemberName));

                Assert.NotNull(validationResult);
                Assert.Equal($"El campo {testCase.MemberName} no cumple su relación con ReferenceValue.", validationResult.ErrorMessage);
            }
        }

        /// <summary>
        /// Validates resource messages for every comparison attribute.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Resource_Error_Message_When_Configured()
        {
            var attributes = GetInvalidAttributes();
            var model = new PropertyComparisonModel { ReferenceValue = 10 };

            foreach (var testCase in attributes)
            {
                testCase.Attribute.ErrorMessageResourceName = nameof(TestResources.ComparisonErrorMessage);
                testCase.Attribute.ErrorMessageResourceType = typeof(TestResources);

                var validationResult = testCase.Attribute.GetValidationResult(testCase.Value, CreateValidationContext(model, testCase.MemberName));

                Assert.NotNull(validationResult);
                Assert.Equal($"Resource message for {testCase.MemberName} and ReferenceValue.", validationResult.ErrorMessage);
            }
        }

        private static void AssertValidationResult(object model, bool isValid)
        {
            var validationResults = ValidationTestHelper.Validate(model);

            if (isValid)
            {
                Assert.Empty(validationResults);
            }
            else
            {
                Assert.Single(validationResults);
            }
        }

        private static ValidationContext CreateValidationContext(object model, string memberName)
        {
            return new ValidationContext(model)
            {
                DisplayName = memberName,
                MemberName = memberName
            };
        }

        private static IReadOnlyCollection<Func<string, ValidationAttribute>> GetAttributeConstructors()
        {
            return
            [
                otherProperty => new GreaterThanAttribute(otherProperty),
                otherProperty => new GreaterThanOrEqualToAttribute(otherProperty),
                otherProperty => new LessThanAttribute(otherProperty),
                otherProperty => new LessThanOrEqualToAttribute(otherProperty)
            ];
        }

        private static IReadOnlyCollection<(ValidationAttribute Attribute, string MemberName, int Value)> GetInvalidAttributes()
        {
            return
            [
                (new GreaterThanAttribute(nameof(PropertyComparisonModel.ReferenceValue)), nameof(PropertyComparisonModel.GreaterValue), 10),
                (new GreaterThanOrEqualToAttribute(nameof(PropertyComparisonModel.ReferenceValue)), nameof(PropertyComparisonModel.GreaterOrEqualValue), 9),
                (new LessThanAttribute(nameof(PropertyComparisonModel.ReferenceValue)), nameof(PropertyComparisonModel.LessValue), 10),
                (new LessThanOrEqualToAttribute(nameof(PropertyComparisonModel.ReferenceValue)), nameof(PropertyComparisonModel.LessOrEqualValue), 11)
            ];
        }

        private sealed class MissingPropertyModel
        {
            [GreaterThan("Missing")]
            public int Value { get; set; }
        }

        private sealed class DifferentTypesModel
        {
            public int ReferenceValue { get; set; }

            [GreaterThan(nameof(ReferenceValue))]
            public long Value { get; set; }
        }

        private sealed class StringComparisonModel
        {
            public string? ReferenceValue { get; set; }

            [GreaterThan(nameof(ReferenceValue))]
            public string? Value { get; set; }
        }

        private sealed class BooleanComparisonModel
        {
            public bool ReferenceValue { get; set; }

            [GreaterThan(nameof(ReferenceValue))]
            public bool Value { get; set; }
        }

        private sealed class NonComparableModel
        {
            public NonComparableValue ReferenceValue { get; set; }

            [GreaterThan(nameof(ReferenceValue))]
            public NonComparableValue Value { get; set; }
        }

        private sealed class EnumComparisonModel
        {
            public TestStatus MinimumStatus { get; set; }

            [GreaterThan(nameof(MinimumStatus))]
            public TestStatus CurrentStatus { get; set; }
        }

        private sealed class CustomComparisonModel
        {
            public ComparableValue MinimumValue { get; set; }

            [GreaterThan(nameof(MinimumValue))]
            public ComparableValue CurrentValue { get; set; }
        }

        private enum TestStatus
        {
            Pending,
            Completed
        }

        private readonly record struct NonComparableValue(int Value);

        private readonly record struct ComparableValue(int Value) : IComparable
        {
            public int CompareTo(object? obj)
            {
                if (obj is not ComparableValue other)
                {
                    throw new ArgumentException("The compared value has an incompatible type.", nameof(obj));
                }

                return Value.CompareTo(other.Value);
            }
        }

        private static class TestResources
        {
            public static string ComparisonErrorMessage => "Resource message for {0} and {1}.";
        }
    }
}
