﻿using MaxMind.MinFraud.Request;
using Xunit;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MaxMind.MinFraud.UnitTest.Request
{
    public class EmailTest
    {
        [Fact]
        public void TestAddress()
        {
            var address = "test@maxmind.com";
            var domain = "maxmind.com";

            var email = new Email(address: address);
            Assert.Equal(address, email.Address);
            Assert.Equal("977577b140bfb7c516e4746204fbdb01", email.AddressMD5);
            Assert.Equal(domain, email.Domain);

            var json = JsonConvert.SerializeObject(email);
            Assert.Equal(
                new JObject
                {
                    {"address", address},
                    {"domain", domain}
                },
                JObject.Parse(json)
            );
        }

        [Fact]
        public void TestAddressWithHashing()
        {
            var address = "test@maxmind.com";
            var md5 = "977577b140bfb7c516e4746204fbdb01";
            var domain = "maxmind.com";

            var email = new Email(address: address, hashAddress: true);
            Assert.Equal(address, email.Address);
            Assert.Equal(md5, email.AddressMD5);
            Assert.Equal("maxmind.com", email.Domain);

            var json = JsonConvert.SerializeObject(email);
            Assert.Equal(
                new JObject
                {
                    {"address", md5},
                    {"domain", domain}
                },
                JObject.Parse(json)
            );
        }

        [Fact]
        public void TestAddressMD5WithUppercase()
        {
            var email = new Email("Test@maxmind.com");
            Assert.Equal("977577b140bfb7c516e4746204fbdb01", email.AddressMD5);
        }


        [Fact]
        public void TestInvalidAddress()
        {
            Assert.Throws<ArgumentException>(() => new Email(address: "no-domain"));
        }

        [Fact]
        public void TestDomain()
        {
            var domain = "domain.com";
            var email = new Email(domain: domain);
            Assert.Equal(domain, email.Domain);
        }

        [Fact]
        public void TestInvalidDomain()
        {
            Assert.Throws<ArgumentException>(() => new Email(domain: " domain.com"));
        }
    }
}