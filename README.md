# IdentityServer4-v4-refresh-token-with-grace

There is no proof that when a client redeems a refresh_token that the client got the response.  In the case where you have a ```TokenUsage.OneTimeOnly```, the backend has already removed the original refresh_token.  A proposed mitigation is to allow the clients to try to redeem using the same old refresh_token.   

This implementation adds a grace period to a consumed ```TokenUsage.OneTimeOnly``` refresh_token.  
From the clients perspective, they can redeem the old refresh_token only 10 times, and only for 1 hour after the initial redeem attempt.  (all configurable)  

Since this is in place to account for clients not getting a response to a refresh_token request, any attempt to redeem a child refresh_token is **PROOF** that a delivery was made.  This triggers the backend to remove the parent of that child.  


## Usage

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

Most of this code is from the IdentityServer quickstart projects.

[ClientExtra](./src/ClientStore/Models/ClientExtra.cs)   
I needed more client configuration.  You will see throughout the code where I typecast as needed;  
```
var clientExtra = client as ClientExtra;
```

```
public bool? RefreshTokenGraceEnabled { get; set; }
public int? RefreshTokenGraceTTL { get; set; }
public int? RefreshTokenGraceMaxAttempts { get; set; }
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

4. A child_refresh_token, when it becomes a parent, removes its parent.  This is to prevent the case when the old parent comes in it doesn't exist anymore, and hence cannot remove this newly upgraded parent_refresh_token.  If we get a child_refresh_token we have proof that we delivered that child_refresh_token, hence the parent of that child is no longer needed.  


5. A parent_refresh_token has a MAX consumed attempts that it can have.  Once exceeded, the parent is removed.  
```
public int? RefreshTokenGraceMaxAttempts { get; set; }
```
6. A parent_refresh_token, once consumed, has an absolute TTL.  Once exceeded, the parent is removed.  
```
public int? RefreshTokenGraceTTL { get; set; }
```










