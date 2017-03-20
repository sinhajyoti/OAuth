using System;

namespace MVCNet.Portal.Auth.Models
{
    public enum OAuthGrant
    {
        Code = 1,
        Implicit = 2,
        ResourceOwner = 3,
        Client = 4
    }

    public class Client
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ClientSecretHash { get; set; }
        public OAuthGrant AllowedGrant { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
    }
}