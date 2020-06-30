# IdentityServer4-v4-refresh-token-with-grace

There is no proof that when a client redeems a refresh_token that the client got the response.  In the case where you have a ```TokenUsage.OneTimeOnly```, the backend has already removed the original refresh_token.  A proposed mitigation is to allow the clients to try to redeem using the same old refresh_token.   

This implementation adds a grace period to a consumed ```TokenUsage.OneTimeOnly``` refresh_token.  
From the clients perspective, they can redeem the old refresh_token only 10 times, and only for 1 hour after the initial redeem attempt.  (all configurable)  

Since this is in place to account for clients not getting a response to a refresh_token request, any attempt to redeem a child refresh_token is **PROOF** that a delivery was made.  This triggers the backend to remove the parent of that child.  


## Running the Demo

```
cd IdentityServer4WithGrace
dotnet run
```
```
cd Api
dotnet run
```
```
cd ConsoleResourceOwnerFlowRefreshToken
dotnet run
```

## Implementation

### Adding the feature to IdentityServer  
[IdentityServer4WithGrace Configuration](./src/IdentityServer4WithGrace/Startup.cs)  
The following;  
```
   services.AddMyDefaultRefreshTokenStore();
   services.AddBackgroundServices();
   services.AddGraceRefreshTokenService();
```
is added **BEFORE** this;
```
 var builder = services.AddIdentityServer()
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddTestUsers(TestUsers.Users);
```
If you add your replacement services, IdentityServer will honor that.  

[ClientExtra configuration](./src/IdentityServer4WithGrace/Config.cs)  
```
 
///////////////////////////////////////////
// Console Resource Owner Flow Sample
//////////////////////////////////////////
new ClientExtra
{
  ClientId = "roclient",
  ClientSecrets =
  {
      new Secret("secret".Sha256())
  },

  AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

  AllowOfflineAccess = true,
  AllowedScopes =
  {
      IdentityServerConstants.StandardScopes.OpenId,
      IdentityServerConstants.StandardScopes.Email,
      IdentityServerConstants.StandardScopes.Address,
      "roles",
      "api1", "api2", "api4.with.roles"
  },
  AbsoluteRefreshTokenLifetime = 3600,
  RefreshTokenGraceEnabled = true,
  RefreshTokenGraceMaxAttempts = 10,
  RefreshTokenGraceTTL = 300,

  RequireRefreshClientSecret = false
}
 

```


### Feature Details

Most of this code is from the IdentityServer quickstart projects.

[ClientExtra](./src/ClientStore/Models/ClientExtra.cs)   
I needed more client configuration.  You will see throughout the code where I typecast as needed;  
```
var clientExtra = client as ClientExtra;
```

```
bool RefreshTokenGraceEnabled 
int RefreshTokenGraceTTL 
int RefreshTokenGraceMaxAttempts 
bool RequireRefreshClientSecret
```  

[RefreshTokenExtra](./src/GraceRefreshTokenService/Models/RefreshTokenExtra.cs)  
I needed more data to be written to persistant storage when tracking refresh_tokens.   
```
    public class RefreshTokenExtra : RefreshToken
    {
        public string RefeshTokenParent { get; set; }
        public string RefeshTokenChild { get; set; }
        public int ConsumedAttempts { get; set; } = 0;
    }
```
[MyDefaultRefreshTokenStore](./src/PersistantStorage/MyDefaultRefreshTokenStore.cs)  
This is my refresh_token store mainly to deal with writing my own RefreshTokenExtra type.  


[GraceRefreshTokenService](./src/GraceRefreshTokenService/GraceRefreshTokenService.cs)  
This is where the meat of the feature is implemented.  

The rules are as follows;
1. A parent_refresh_token tracks any children it creates  

2. A parent_refresh_token can only have one child, hence any old child_refresh_token is removed when the parent_refresh_token is consumed.  A new one gets created and put in its place.  

3. A child_refresh_token records its parent_refresh_token  

4. A child_refresh_token, when it becomes a parent, removes its parent.  This is to prevent the case when the old parent comes in it doesn't exist anymore, and hence cannot remove this newly upgraded parent_refresh_token.  If we get a child_refresh_token we have **PROOF** that we delivered that child_refresh_token, hence the parent of that child is no longer needed.  


5. A parent_refresh_token has a MAX consumed attempts that it can have.  Once exceeded, the parent is removed.  
```
public int? RefreshTokenGraceMaxAttempts { get; set; }
```
6. A parent_refresh_token, once consumed, has an absolute TTL.  Once exceeded, the parent is removed.  
```
public int? RefreshTokenGraceTTL { get; set; }
```


# RequireRefreshClientSecret feature

This isn't related to the refresh_token grace feature, but the problem typically arises when you have mobile apps with fragile internet connections.  
In this case the **NOT TRUSTED** clients are given a refresh_token that doesn't require a client_secret.

**NOTE**: Nothing in the OAuth2 spec calls out that a client_secret is **REQUIRED** to refresh a token.  Anyway, my call to deviate even if it was :)  


### Adding the feature to IdentityServer  
[IdentityServer4WithGrace Configuration](./src/IdentityServer4WithGrace/Startup.cs)  
The following;  
```
//////////////////////////////////////////////
// IdentityServer sometimes doesn't do a TryAddTransient
// so we have to replace the services with a remove then add.
//////////////////////////////////////////////
// replace IdentityServer's IClientSecretValidator with mine.
// note: This isn't needed for the refesh_token grace stuff
//       This is to allow a refresh_token to be redeemed without a client_secret
services.ReplaceClientSecretValidator<MyClientSecretValidator>();
```
[ReplaceClientSecretValidator](./src/MyValidators/Extensions/DependencyInjectionExtensions.cs)   


is added **AFTER** this;
```
 var builder = services.AddIdentityServer()
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddTestUsers(TestUsers.Users);
```
In this case IdentityServer **DOES NOT** honor adding my IClientSecretValidator upfront.

### Feature Details  
[MyClientSecretValidator](./src/MyValidators/MyClientSecretValidator.cs)    


# grant_type=arbitrary_resource_owner
This is an extension grant that lets you mint an access_token with anything you want in it.  


```
curl --location --request POST 'https://localhost:5001/connect/token' \
--header 'Content-Type: application/x-www-form-urlencoded' \
--header 'Authorization: Basic cm9jbGllbnQ6c2VjcmV0' \
--data-urlencode 'grant_type=arbitrary_resource_owner' \
--data-urlencode 'scope=passage geo torrent stats discovery offline_access' \
--data-urlencode 'arbitrary_claims={"top":["TopDog"],"role": ["admin","limited"],"query": ["dashboard", "licensing"],"seatId": ["8c59ec41-54f3-460b-a04e-520fc5b9973d"],"piid": ["2368d213-d06c-4c2a-a099-11c34adc3579"]}' \
--data-urlencode 'subject=66C8C5A139F65007808EF00716203B09B5C157C3' \
--data-urlencode 'access_token_lifetime=3600' \
--data-urlencode 'arbitrary_amrs=["agent:username:agent0@supporttech.com","agent:challenge:fullSSN","agent:challenge:homeZip"]' \
--data-urlencode 'arbitrary_audiences=["SE_GLOBAL_JWT"]' \
--data-urlencode 'custom_payload={"some_string": "data","some_number": 1234,"some_object": {"some_string": "data","some_number": 1234},"some_array": [{"a": "b"},{"b": "c"}]}'
```






