﻿using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace MaxMind.MinFraud.Request
{
    /// <summary>
    /// The email information for the transaction being sent to the
    /// web service.
    /// </summary>
    public sealed class Email
    {
        private readonly bool _hashAddress;
        private string _domain;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="address">The user's email address. This will be
        /// converted into an MD5 before being sent to the web service.
        /// </param>
        /// <param name="domain">The domain of the email address used in the
        /// transaction. If <c>address</c> is passed to the constructor
        /// and <c>domain</c> is not, the domain will be automatically
        /// set from the address.</param>
        /// <param name="hashAddress">By default, the <c>address</c> will
        /// be sent in plain text. If <c>hashAddress</c> is set to true,
        /// the address will instead be sent as an MD5 hash.</param>
        public Email(
            string address = null,
            string domain = null,
            bool hashAddress = false
        )
        {
            Address = address;
            if (domain == null && address != null)
            {
                var parts = address.Split('@');
                if (parts.Length == 2)
                    domain = parts[1];
            }
            Domain = domain;
            _hashAddress = hashAddress;
        }

        /// <summary>
        /// The MD5 generated from the email address.
        /// </summary>
        [JsonIgnore]
        public string AddressMD5
        {
            get
            {
                if (Address == null)
                    return null;
                using (var md5Generator = MD5.Create())
                {
                    var bytes = Encoding.UTF8.GetBytes(Address.ToLower());
                    var md5 = md5Generator.ComputeHash(bytes);
                    return BitConverter.ToString(md5)
                        .Replace("-", string.Empty)
                        .ToLower();
                }
            }
        }

        /// <summary>
        /// The email address used in the transaction.
        /// </summary>
        [JsonIgnore]
        [EmailAddress]
        public string Address { get; }

        [JsonProperty("address")]
        private string JsonAddress => _hashAddress ? AddressMD5 : Address;

        /// <summary>
        /// The domain of the email address.
        /// </summary>
        [JsonProperty("domain")]
        public string Domain
        {
            get => _domain;
            private set
            {
                if (Uri.CheckHostName(value) == UriHostNameType.Unknown)
                {
                    throw new ArgumentException($"The email domain {value} is not valid.");
                }
                _domain = value;
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"Address: {Address}, Domain: {Domain}";
        }
    }
}