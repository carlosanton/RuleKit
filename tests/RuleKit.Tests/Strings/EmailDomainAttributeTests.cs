using System.ComponentModel.DataAnnotations;
using RuleKit.Tests.Helpers;
using RuleKit.Tests.Models;

namespace RuleKit.Tests.Strings
{
    public class EmailDomainAttributeTests
    {
        /// <summary>
        /// Validates email addresses that use the configured domain.
        /// </summary>
        /// <param name="email">The email address to validate.</param>
        [Theory]
        [InlineData("usuario@empresa.com")]
        [InlineData("usuario.apellido@empresa.com")]
        [InlineData("usuario+notificaciones@empresa.com")]
        [InlineData("usuario@EMPRESA.COM")]
        public void Validate_Should_Return_Success_When_Email_Uses_Allowed_Domain(string email)
        {
            var model = new EmailDomainModel
            {
                Email = email
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Empty(validationResults);
        }

        /// <summary>
        /// Validates email addresses whose domain is not configured.
        /// </summary>
        /// <param name="email">The email address to validate.</param>
        [Theory]
        [InlineData("usuario@otraempresa.com")]
        [InlineData("usuario@sub.empresa.com")]
        [InlineData("usuario@falsaempresa.com")]
        public void Validate_Should_Return_Error_When_Email_Domain_Is_Not_Allowed(string email)
        {
            var model = new EmailDomainModel
            {
                Email = email
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Single(validationResults);
        }

        /// <summary>
        /// Validates values that do not have a valid email format.
        /// </summary>
        /// <param name="email">The invalid email address.</param>
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("usuario")]
        [InlineData("@empresa.com")]
        [InlineData("usuario@")]
        [InlineData("usuario@@empresa.com")]
        [InlineData(" usuario@empresa.com")]
        [InlineData("usuario@empresa.com ")]
        public void Validate_Should_Return_Error_When_Email_Format_Is_Invalid(string email)
        {
            var model = new EmailDomainModel
            {
                Email = email
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Single(validationResults);
        }

        /// <summary>
        /// Validates malformed subdomains even when subdomain support is enabled.
        /// </summary>
        /// <param name="email">The email address with an invalid subdomain.</param>
        [Theory]
        [InlineData("usuario@dominio_invalido.empresa.com")]
        [InlineData("usuario@-ventas.empresa.com")]
        [InlineData("usuario@ventas-.empresa.com")]
        [InlineData("usuario@ventas..empresa.com")]
        [InlineData("usuario@compañía.empresa.com")]
        public void Validate_Should_Return_Error_When_Subdomain_Format_Is_Invalid(string email)
        {
            var attribute = new EmailDomainAttribute("empresa.com")
            {
                AllowSubdomains = true
            };

            var validationResult = attribute.GetValidationResult(email, CreateValidationContext());

            Assert.NotNull(validationResult);
        }

        /// <summary>
        /// Validates a null value.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Success_When_Value_Is_Null()
        {
            var model = new EmailDomainModel
            {
                Email = null
            };

            var validationResults = ValidationTestHelper.Validate(model);

            Assert.Empty(validationResults);
        }

        /// <summary>
        /// Validates each domain in a configured domain list.
        /// </summary>
        /// <param name="email">The email address to validate.</param>
        [Theory]
        [InlineData("usuario@empresa.com")]
        [InlineData("usuario@empresa.es")]
        [InlineData("usuario@sub.empresa.es")]
        public void Validate_Should_Return_Success_When_Email_Uses_Any_Allowed_Domain(string email)
        {
            var attribute = new EmailDomainAttribute("empresa.com", "empresa.es", "sub.empresa.es");

            var validationResult = attribute.GetValidationResult(email, CreateValidationContext());

            Assert.Null(validationResult);
        }

        /// <summary>
        /// Validates subdomains when they are enabled explicitly.
        /// </summary>
        /// <param name="email">The email address to validate.</param>
        [Theory]
        [InlineData("usuario@empresa.com")]
        [InlineData("usuario@ventas.empresa.com")]
        [InlineData("usuario@interno.ventas.empresa.com")]
        public void Validate_Should_Return_Success_When_Subdomains_Are_Allowed(string email)
        {
            var attribute = new EmailDomainAttribute("empresa.com")
            {
                AllowSubdomains = true
            };

            var validationResult = attribute.GetValidationResult(email, CreateValidationContext());

            Assert.Null(validationResult);
        }

        /// <summary>
        /// Validates that enabling subdomains does not allow domains that only share the same suffix.
        /// </summary>
        /// <param name="email">The email address to validate.</param>
        [Theory]
        [InlineData("usuario@falsaempresa.com")]
        [InlineData("usuario@otra-empresa.com")]
        public void Validate_Should_Return_Error_When_Domain_Is_Not_A_Real_Subdomain(string email)
        {
            var attribute = new EmailDomainAttribute("empresa.com")
            {
                AllowSubdomains = true
            };

            var validationResult = attribute.GetValidationResult(email, CreateValidationContext());

            Assert.NotNull(validationResult);
        }

        /// <summary>
        /// Validates that unsupported data types are treated as developer errors.
        /// </summary>
        [Fact]
        public void Validate_Should_Throw_When_Value_Type_Is_Not_Supported()
        {
            var attribute = new EmailDomainAttribute("empresa.com");
            var unsupportedValues = new object[]
            {
                1,
                new Uri("mailto:usuario@empresa.com")
            };

            foreach (var value in unsupportedValues)
            {
                var exception = Assert.Throws<InvalidOperationException>(() => attribute.GetValidationResult(value, CreateValidationContext()));

                Assert.Contains(nameof(EmailDomainAttribute), exception.Message);
                Assert.Contains(value.GetType().FullName!, exception.Message);
            }
        }

        /// <summary>
        /// Validates that at least one domain is required.
        /// </summary>
        [Fact]
        public void Constructor_Should_Throw_When_No_Domains_Are_Provided()
        {
            var exception = Assert.Throws<ArgumentException>(() => new EmailDomainAttribute());

            Assert.Equal("domains", exception.ParamName);
        }

        /// <summary>
        /// Validates a null domain collection.
        /// </summary>
        [Fact]
        public void Constructor_Should_Throw_When_Domains_Are_Null()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new EmailDomainAttribute(null!));

            Assert.Equal("domains", exception.ParamName);
        }

        /// <summary>
        /// Validates malformed domain configurations.
        /// </summary>
        /// <param name="domain">The invalid domain.</param>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("@empresa.com")]
        [InlineData("usuario@empresa.com")]
        [InlineData("empresa..com")]
        [InlineData("-empresa.com")]
        [InlineData("empresa-.com")]
        [InlineData("empresa.com.")]
        [InlineData("empresa con espacios.com")]
        [InlineData("compañía.es")]
        public void Constructor_Should_Throw_When_Domain_Is_Invalid(string? domain)
        {
            var exception = Assert.Throws<ArgumentException>(() => new EmailDomainAttribute(domain!));

            Assert.Equal("domains", exception.ParamName);
        }

        /// <summary>
        /// Exposes an immutable copy of the configured domains.
        /// </summary>
        [Fact]
        public void Constructor_Should_Set_Domains_Without_Keeping_Mutable_Input()
        {
            var domains = new[] { "empresa.com", "empresa.es" };
            var attribute = new EmailDomainAttribute(domains);

            domains[0] = "otraempresa.com";

            Assert.Equal(["empresa.com", "empresa.es"], attribute.Domains);
        }

        /// <summary>
        /// Validates the default error message and affected member name.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Default_Error_Message_When_Value_Is_Invalid()
        {
            var attribute = new EmailDomainAttribute("empresa.com", "empresa.es");

            var validationResult = attribute.GetValidationResult("usuario@otraempresa.com", CreateValidationContext());

            Assert.NotNull(validationResult);
            Assert.Equal("The field Email must be a valid email address with one of these domains: 'empresa.com', 'empresa.es'.", validationResult.ErrorMessage);
            Assert.Equal(["Email"], validationResult.MemberNames);
        }

        /// <summary>
        /// Validates a custom localized error message.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Custom_Error_Message_When_Configured()
        {
            var attribute = new EmailDomainAttribute("empresa.com")
            {
                ErrorMessage = "El campo {0} debe ser un correo válido con uno de estos dominios: {1}."
            };

            var validationResult = attribute.GetValidationResult("usuario@otraempresa.com", CreateValidationContext());

            Assert.NotNull(validationResult);
            Assert.Equal("El campo Email debe ser un correo válido con uno de estos dominios: 'empresa.com'.", validationResult.ErrorMessage);
        }

        /// <summary>
        /// Validates an error message provided by a resource type.
        /// </summary>
        [Fact]
        public void Validate_Should_Return_Resource_Error_Message_When_Configured()
        {
            var attribute = new EmailDomainAttribute("empresa.com")
            {
                ErrorMessageResourceName = nameof(TestResources.EmailDomainErrorMessage),
                ErrorMessageResourceType = typeof(TestResources)
            };

            var validationResult = attribute.GetValidationResult("usuario@otraempresa.com", CreateValidationContext());

            Assert.NotNull(validationResult);
            Assert.Equal("Resource message for Email with domains 'empresa.com'.", validationResult.ErrorMessage);
        }

        private static ValidationContext CreateValidationContext()
        {
            return new ValidationContext(new object())
            {
                DisplayName = "Email",
                MemberName = "Email"
            };
        }

        private static class TestResources
        {
            public static string EmailDomainErrorMessage => "Resource message for {0} with domains {1}.";
        }
    }
}
