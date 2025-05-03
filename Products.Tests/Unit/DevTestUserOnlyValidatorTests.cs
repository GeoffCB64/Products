using Products.Services.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Tests.Unit
{
    public class DevTestUserOnlyValidatorTests
    {
        [Fact]
        public void ValidateCredentials_WithValidUser_ReturnsTrue()
        {
            var validator = new DevTestUserOnlyValidator();
            var result = validator.ValidateCredentials("username1", "pass123");

            Assert.True(result);
        }

        [Fact]
        public void ValidateCredentials_WithInvalidUser_ReturnsFalse()
        {
            var validator = new DevTestUserOnlyValidator();
            var result = validator.ValidateCredentials("hacker", "wrong");

            Assert.False(result);
        }
    }
}
