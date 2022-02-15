using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <inheritdoc cref="RegistrarService"/>
    /// <see>https://developer.dnsimple.com/v2/registrar/delegation/</see>
    public partial class RegistrarService
    {
        /// <summary>
        /// Lists the name servers for the domain.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domain">The domain name</param>
        /// <returns>The list of name servers for the domain</returns>
        /// <see>https://developer.dnsimple.com/v2/registrar/delegation/#getDomainDelegation</see>
        public DelegationResponse GetDomainDelegation(long accountId, string domain)
        {
            var builder = BuildRequestForPath(DelegationPath(accountId, domain));

            return new DelegationResponse(Execute(builder.Request));
        }

        /// <summary>
        /// Changes the domain name servers.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domain">The domain name</param>
        /// <param name="delegation">A list of name server names as strings</param>
        /// <returns>The list of updated name servers for the domain</returns>
        /// <see>https://developer.dnsimple.com/v2/registrar/delegation/#changeDomainDelegation</see>
        public DelegationResponse ChangeDomainDelegation(long accountId, string domain, IList<string> delegation)
        {
            var builder = BuildRequestForPath(DelegationPath(accountId, domain));
            builder.Method(Method.PUT);
            builder.AddJsonPayloadRaw(delegation);

            return new DelegationResponse(Execute(builder.Request));
        }

        /// <summary>
        /// Delegate a domain to vanity name servers.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domain">The domain name</param>
        /// <param name="delegation">A list of name servers as strings</param>
        /// <returns>The list of nameservers updated to vanity for the domain</returns>
        /// <see>https://developer.dnsimple.com/v2/registrar/delegation/#changeDomainDelegationToVanity</see>
        public ListResponse<VanityDelegation> ChangeDomainDelegationToVanity(long accountId, string domain, List<string> delegation)
        {
            var builder = BuildRequestForPath(VanityDelegationPath(accountId, domain));
            builder.Method(Method.PUT);
            builder.AddJsonPayloadRaw(delegation);

            return new ListResponse<VanityDelegation>(Execute(builder.Request));
        }

        /// <summary>
        /// De-delegate a domain from vanity name servers.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domain">The domain name</param>
        /// <see>https://developer.dnsimple.com/v2/registrar/delegation/#changeDomainDelegationFromVanity</see>
        public EmptyResponse ChangeDomainDelegationFromVanity(long accountId, string domain)
        {
            var builder = BuildRequestForPath(VanityDelegationPath(accountId, domain));
            builder.Method(Method.DELETE);

            return new EmptyResponse(Execute(builder.Request));
        }
    }

    /// <summary>
    /// Represents the response from the API call to get the domain delegation.
    /// </summary>
    public class DelegationResponse
    {
        public List<string> Data { get; }

        public DelegationResponse(JToken json) => Data = JsonTools<string>.DeserializeList(json);

        public DelegationResponse(IRestResponse response) => Data = JsonTools<string>.DeserializeList(
            JObject.Parse(response.Content)
            );
    }

    /// <summary>
    /// Represents a vanity delegation
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct VanityDelegation
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Ipv4 { get; set; }
        public string Ipv6 { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
