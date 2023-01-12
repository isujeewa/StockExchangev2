using IdentityModel;
using IdentityServer4.Models;

namespace AuthServer.Config
{
    public static class Configuration
    {
        // ApiResources define the apis in your system
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource( "AuthService","Auth Service")
                {
                UserClaims = new []
                {
                    JwtClaimTypes.Role,
                    JwtClaimTypes.Audience,
                    JwtClaimTypes.Email,
                    JwtClaimTypes.PhoneNumber
                },
                Scopes =new List<string>
                {
                    "AuthService"
                }
                },
                 new ApiResource( "StockApi","StockApi Service")
                {
                UserClaims = new []
                {
                    JwtClaimTypes.Role,
                    JwtClaimTypes.Audience,
                    JwtClaimTypes.Email
                },
                Scopes =new List<string>
                {
                    "StockApi"
                }
                },
                   new ApiResource( "TransactionApi","TransactionApi Service")
                {
                UserClaims = new []
                {
                    JwtClaimTypes.Role,
                    JwtClaimTypes.Audience,
                    JwtClaimTypes.Email
                },
                Scopes =new List<string>
                {
                    "TransactionApi"
                }
                },
            };
        }

        public static IEnumerable<ApiScope> GetApisScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope("AuthService", "Auth Service", new []{ JwtClaimTypes.Role, JwtClaimTypes.Email, JwtClaimTypes.PhoneNumber }),
                new ApiScope("StockApi", "StockApi Service", new []{ JwtClaimTypes.Role, JwtClaimTypes.Email, JwtClaimTypes.PhoneNumber }),
                new ApiScope("TransactionApi", "TransactionApi Service", new []{ JwtClaimTypes.Role, JwtClaimTypes.Email, JwtClaimTypes.PhoneNumber }),
            };
        }

        // Identity resources are data like user ID, name, or email address of a user
        // see: http://docs.identityserver.io/en/release/configuration/resources.html
        public static IEnumerable<IdentityResource> GetResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Phone(),
                new IdentityResource
                {
                      Name = "role",
                      UserClaims ={ JwtClaimTypes.Role}
                },
                 new IdentityResource
                {
                      Name = "id",
                      UserClaims ={ JwtClaimTypes.Id}
                },
                     new IdentityResource
                {
                      Name = "email",
                      UserClaims ={ JwtClaimTypes.Email}
                },
                    new IdentityResource
                {
                      Name = "phone_number",
                      UserClaims ={ JwtClaimTypes.PhoneNumber}
                }
            };
        }

        // client want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients(Dictionary<string, string> clientsUrl)
        {
            return new List<Client>
            {
                 new Client
                    {
                        ClientId = "ro.client",
                        AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                        ClientSecrets =
                        {
                            new Secret("secret".Sha256())
                        },
                        AllowedScopes =
                        {
                             "AuthService",
                             "role",
                             "email",
                             "StockApi",
                             "TransactionApi"
                        },
                        AlwaysIncludeUserClaimsInIdToken = true,
                        AllowOfflineAccess = true,
                    },
                new Client
                {
                    ClientId = "UserService",
                    ClientName = "UserService API",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes =
                    {
                        "AuthService",
                        "role"
                    },
                   ClientSecrets = new List<Secret> {new Secret("Pwd123SF$".Sha256())}
                },
             
            };
        }
    }
}
