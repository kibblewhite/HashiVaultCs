# HashiVaultC# (HashiVaultCs)

This project is to help develop and understand how to build a HashiCorp Vault Client in csharp.

It's funtionality is minimal but feel free to add to the project. I will do my best to keep up, and if you like the project, please give it a git-star. Thanks.

Currently as a first version, this code does very little. The following API paths are the only endpoints that it can communicate with:
- /v1/auth/userpass/login/{username}
- /v1/auth/approle/role/{rolename}/role-id
- /v1/auth/approle/role/{rolename}/secret-id
- /v1/auth/approle/login
- /v1/{engine}/data/{path}

With these endpoints it is enough to:
- Login using the userpass authentication method.
- Use the vault token from the userpass login to obtain the role-id and generate a secret-id.
- With the same userpass vault token, use the role-id and the secret-id to login under the approle authentication method and get a another token which can be then used to read from the kv secrets store.
- Finally read values from the kv secrets engine store.

To check out some sample code, just head over to the unit test(s) to see this in action, though they are not yet 100% completed.

I hope that the documentation below is enough to get the newer amongst us started with HashiCorp Vault (https://www.vaultproject.io/)

I took inspiration from this article to get me started: https://www.infoq.com/articles/creating-http-sdks-dotnet-6/

##### What's next...
- Adding ARM64 support - timing this after Visual Studio 17.3 preview/rc release (and some of the issues have been ironed out)
  - In the `.output` directory I generated a bunch of assemblies for win and linux x64 and arm64 platforms (the build process in documented using the Dockerfiles)
  - There appears to little to no difference between the assembly files (loaded them into hexcmp) which leads me to believe under these conditions there is no need to generate both x64 and arm64 binaries for the nuget packages.
- I would like to add some basic reading of PKI secrets and getting certificates into my applications next.
- After that I would like to add the [Wrapping Token concept](https://www.vaultproject.io/docs/concepts/response-wrapping).

------

## Getting Started

You'll need a running Vault service
- Although it is beyond the scope of this project, there is included a ```Dockerfile``` and ```docker-compose.yml``` included to help get going with a postgresql service backend and utilising a Vault-Unseal script `unseal.sh`.
- You might need to change the `DATABASE_URL` environment variable in the `docker-compose.yml` file. Currently it is set to connect to a postgres instance running on the docker's hosting machine.
- If it is a first run of the vault service, you will need to [establish the vault's unsealing tokens](https://learn.hashicorp.com/tutorials/vault/getting-started-ui) and then first update the `unseal.sh` with at least two of the vault tokens configured correctly and then re-run the build process with the command from the `docker-compose build --no-cache` found in the `docker-compose.yml`.

Included with this is the config files, entrypoint script and there is also the vault sql for creating the postgres database table(s).
>  It is not possible to offer support to run your Vault Service. Please keep questions and requests limited to the code base only, thank you.

If you already have a running vault service, you will need to have access to the vault CLI, and then set the vault token environment variable as following:

```bash
export VAULT_TOKEN=<vault-token-value />
export VAULT_ADDR=http://127.0.0.1:8200
```

The vault token value can be the root vault token or other valid vault token.



------

## Vault Configurations

- Create a username/password authentication method
    - This will be used to read role-id values and generate secret-id values from the staging approle secrets only, nothing more.
        - You can see that the paths defined in the ACL policy ```setup/vault/config/policies/approles/staging-userpass.hcl``` will allow access to this part of the system.
    - For the purpose of this documentation we will create a single username/password. However, this will not reflect the "real-world" configurations.
    - In development, a unique username/password entry should be created per developer required (they can use this in their development environment such as docker desktop to start their applications)
- Create a basic kv secrets store and enter some test key-value data entries (test=testing & heylo=world)
- Create the approle authentication method
    - Create the ACL policy that will allow access to the KV secrets store as path ```kv/data/staging/*``` - again this can be seen in the policy file ```setup/vault/policies/approle/staging-approle.hcl```
        - Note the extra ```/data/``` path inserted after the ```kv/``` - this is to let the policy know that the path being accessed is for the version 2 secret store (kv-v2), and not version 1.
    - For the purpose of documentation, we will create an approle named ```staging``` and apply the newly created policy to that approle entry.

Where it mentions to run a commmand "Only once per database", this means that once the command has been run, you do not need to run it again and can omit that step. If you are working on an existing service using an existing database, you can safely assume that this step has already been done.

------

#### Username/Password Authentication Method

Enabling username/password authentication method (Only once per database)
```bash
vault auth enable userpass
```

Create the policy that allows creation of rewrap tokens, list policies and read the newly created policy
```bash
vault policy write staging-userpass setup/vault/policies/userpass/staging-userpass.hcl
vault policy list
vault policy read staging-userpass
```

Create the userpass creditials with the newly created policy, and list usernames
```bash
vault write auth/userpass/users/<username /> password=<password /> policies=staging-userpass
vault list auth/userpass/users
```

#### Create Key Value Storage Engine

```bash
vault secrets enable -version=2 kv
```

Please note the lack of the extra ```/data/``` path, as this not required when writing or reading a version 2 variant of the kv secrets store.
```bash
vault kv put kv/staging/service-svc test=testing heylo=world
```

#### App Role Configurations

Enabling approle authentication method (Only once per database)
```bash
vault auth enable approle
```

Create the policy for the approle, list policies and read the newly created policy
```bash
vault policy write staging-approle setup/vault/policies/approle/staging-approle.hcl
vault policy list
vault policy read staging-approle
```

Create named role and associated policies
```bash
vault write auth/approle/role/staging \
    secret_id_ttl=30m \
    token_num_uses=100 \
    token_ttl=30m \
    token_max_ttl=30m \
    secret_id_num_uses=100 \
    policies=staging-approle
vault read auth/approle/role/staging
```

Let's test the secret-id generation. Any results from this are not used later in the documentation, so these steps are optional.

Fetch the RoleID of the AppRole.
```bash
vault read auth/approle/role/staging/role-id
```

Generate/Retrieve a SecretID.
```bash
vault write -f auth/approle/role/staging/secret-id
```

Generate token
```bash
vault write auth/approle/login role_id=<role-id /> secret_id=<secret-id />
```

Congratulations, you should now have a valid token which can be used to obtain key-value pairs from the vault.

To finish you can execute the following, where the approle-token was produced from the previous generate token vault write command:
```bash
rm -Rf ~/.vault-token
export VAULT_TOKEN=<approle-token />
vault kv get kv/staging/service-svc
```

The generation of the token above is just to test if the process has worked.
An example of the commands can be seen below, to provide more context of it's use:

```bash

/ # vault read auth/approle/role/staging/role-id
Key        Value
---        -----
role_id    59b5cc05-d7f7-71c3-6ba6-4bc8fbbd2168

/ # vault write -f auth/approle/role/staging/secret-id
Key                   Value
---                   -----
secret_id             daa0a954-2791-04e1-79d0-204e4b8ae64f
secret_id_accessor    5451d54e-1318-2a74-5edd-9ebf018e9941
secret_id_ttl         1m

/ # vault write auth/approle/login role_id=59b5cc05-d7f7-71c3-6ba6-4bc8fbbd2168 secret_id=daa0a954-2791-04e1-79d0-204e4b8ae64f
Key                     Value
---                     -----
token                   hvs.CAESIBfa3idmMGpTzS7KuxTCpWQXcEmOryi6SlB5A-Dl1V67Gh4KHGh2cy5xT2ZNTHJ0Y0RrQlU0REhLNDlnSlZRSXg
token_accessor          HSxzYQi5Crl1w2LSvwghJWgz
token_duration          30m
token_renewable         true
token_policies          ["default","staging-approle"]
identity_policies       []
policies                ["default","staging-approle"]
token_meta_role_name    staging

```
...
##### Extracting the vault token from the above response!
You can see that in the response above, that there is a key field simply defined as ```token```, and that the value starting with ```hvs.CAESIBf...``` is the token that we can save for later use as the VAULT_TOKEN environment variable.

---

#### Obtain secret-id token as newly created user

The final part, is to get a vault token that is useable for the duration specified in approle creation command, of 30 minutes (30m).

The steps are as followed:

Make sure we are logged out of the vault by clearing any tokens.
```bash
unset VAULT_TOKEN
rm -Rf ~/.vault-token
```

Login to the vault using our userpass authentication creditials (this will create the ```~/.vault-token``` file.)
```bash
vault login -method=userpass username=<username />
/ # Password (will be hidden): <password />
```

Get the role-id and generate a secret-id for the approle 'staging' as logged in user
```bash
vault read auth/approle/role/staging/role-id
vault write -f auth/approle/role/staging/secret-id
```

Generate a token using the previously retrieved role-id and newly created secret-id. 
Important: Save the ```token``` value as the '*approle-token*' for use later...
```bash
vault write auth/approle/login role_id=<role-id /> secret_id=<secret-id />
```

Logout of the vault as the newly created user by removing the vault-token from the home directory
```bash
rm -Rf ~/.vault-token
```

Using the '*approle-token*' that you just obtained, set the VAULT_TOKEN environment varaible to this value
```bash
export VAULT_TOKEN=<approle-token />
```

With the newly set VAULT_TOKEN, attempt to access the values in the kv/staging/* secrets store, again noting the lack of the extra ```/data/``` path as it is not required.
```bash
vault kv get kv/staging/service-svc
```

Sample of what to expect to see:
```bash
/ # vault kv get kv/staging/service-svc
======= Secret Path =======
kv/data/staging/service-svc

======= Metadata =======
Key                Value
---                -----
created_time       2022-05-22T13:02:52.183297Z
custom_metadata    <nil>
deletion_time      n/a
destroyed          false
version            1

==== Data ====
Key      Value
---      -----
heylo    world
test     testing
```


### Removing Vault Configurations

Remove named role
```bash
vault delete auth/approle/role/staging
```

Disable approle authentication method
```bash
vault auth disable approle
```

Disable kv secret engine
```bash
vault secrets disable kv
```

Remove userpass entry
```bash
vault delete auth/userpass/users/<username />
```

Disable userpass authentication method
```bash
vault auth disable userpass
```

Remove ACL Policies
```bash
vault policy delete staging-approle
vault policy delete staging-userpass
```

-----

## Collated Commands to CURL Requests

The following is the same set of commands as above, but using CURL and the API to access the same set of features and requests.

Using a pre-shared username password combo, get the ```client_token``` which will only allow access to the approle authentication method to read a role-id and write a secret-id only.
```bash
vault login -method=userpass username=<username />
curl --request POST --data "{ \"password\": \"<password />\" }" http://localhost:8200/v1/auth/userpass/login/<username />
```

Using the ```client_token``` as the vault token value, read the role-id and write/retrieve a secret-id
```bash
vault read auth/approle/role/staging/role-id
curl --header "X-Vault-Token: ..." --request GET http://localhost:8200/v1/auth/approle/role/staging/role-id

vault write -f auth/approle/role/staging/secret-id
curl --header "X-Vault-Token: ..." --request POST --data "{}" http://localhost:8200/v1/auth/approle/role/staging/secret-id
```

Create a token using the role-id and the newly generated secret-id, get a new ```client_token``` which will have the ability to read and write from the kv secrets storage engine.
```bash
vault write auth/approle/login role_id=<role-id /> secret_id=<secret-id />
curl --header "X-Vault-Token: ..." --request POST --data "{ \"role_id\": \"<role-id />\", \"secret_id\": \"<secret-id />\" }" http://localhost:8200/v1/auth/approle/login
```

Use the ```client_token``` from the previous command as a newly generated vault token, read from the kv secrets storage engine.
```bash
vault kv get kv/staging/service-svc
curl --header "X-Vault-Token: ..." --request GET http://localhost:8200/v1/kv/data/staging/service-svc
```

Below is the CURL commands being ran and the responses, so you can see the context of how it should work.
- The username/password have been redacted. 
- Please note the change in the vault token for the last command.
```bash
curl --request POST --data "{ \"password\": \"<password />\" }" http://localhost:8200/v1/auth/userpass/login/<username />
{"request_id":"23610a44-3b4f-5f5d-f2b0-c452117ba89b","lease_id":"","renewable":false,"lease_duration":0,"data":null,"wrap_info":null,"warnings":null,"auth":{"client_token":"hvs.CAESII17x5GQYw0QhYKi95kxUUjdhi0Q-X1G6fbddsTxQcPAGh4KHGh2cy5vRWJwUzNvdWlpMXExc3NZT0ltbkk5MjA","accessor":"al1urMYAjwT3E8Gc6e3DtuHb","policies":["default","staging-userpass"],"token_policies":["default","staging-userpass"],"metadata":{"username":"kibble"},"lease_duration":2764800,"renewable":true,"entity_id":"b5d38ffe-8c15-46e6-931f-86cc074c8c2e","token_type":"service","orphan":true,"mfa_requirement":null,"num_uses":0}}

curl --header "X-Vault-Token: hvs.CAESII17x5GQYw0QhYKi95kxUUjdhi0Q-X1G6fbddsTxQcPAGh4KHGh2cy5vRWJwUzNvdWlpMXExc3NZT0ltbkk5MjA" --request GET http://localhost:8200/v1/auth/approle/role/staging/role-id
{"request_id":"e060d82e-2224-7995-f539-a993696e8d6b","lease_id":"","renewable":false,"lease_duration":0,"data":{"role_id":"f2d9ae61-8055-93a7-5913-cbc176864d2a"},"wrap_info":null,"warnings":null,"auth":null}

curl --header "X-Vault-Token: hvs.CAESII17x5GQYw0QhYKi95kxUUjdhi0Q-X1G6fbddsTxQcPAGh4KHGh2cy5vRWJwUzNvdWlpMXExc3NZT0ltbkk5MjA" --request POST --data "{}" http://localhost:8200/v1/auth/approle/role/staging/secret-id
{"request_id":"27a8497f-de31-f902-e19b-955b8ff435b6","lease_id":"","renewable":false,"lease_duration":0,"data":{"secret_id":"afbb0d99-bcdc-f722-ec3d-a4a465a674b9","secret_id_accessor":"9dc3edfc-228a-b216-d627-094f3ce9aad4","secret_id_ttl":1800},"wrap_info":null,"warnings":null,"auth":null}

curl --header "X-Vault-Token: hvs.CAESII17x5GQYw0QhYKi95kxUUjdhi0Q-X1G6fbddsTxQcPAGh4KHGh2cy5vRWJwUzNvdWlpMXExc3NZT0ltbkk5MjA" --request POST --data "{ \"role_id\": \"f2d9ae61-8055-93a7-5913-cbc176864d2a\", \"secret_id\": \"afbb0d99-bcdc-f722-ec3d-a4a465a674b9\" }" http://localhost:8200/v1/auth/approle/login
{"request_id":"cad4f7d8-63f1-b09c-ecf1-23c016419c70","lease_id":"","renewable":false,"lease_duration":0,"data":null,"wrap_info":null,"warnings":null,"auth":{"client_token":"hvs.CAESILacrRZCXR-e1GTcitvww7EdFdq7C4ftIRDro6lkE3wZGh4KHGh2cy5iTWhvMDhvcFVFMzQzODZtYm9wVEZEcVA","accessor":"4IsCFPg9kJQdNaVnknLFeHvx","policies":["default","staging-approle"],"token_policies":["default","staging-approle"],"metadata":{"role_name":"staging"},"lease_duration":1800,"renewable":true,"entity_id":"dc5683f8-830a-c1fa-2363-15148b72f772","token_type":"service","orphan":true,"mfa_requirement":null,"num_uses":100}}

curl --header "X-Vault-Token: hvs.CAESILacrRZCXR-e1GTcitvww7EdFdq7C4ftIRDro6lkE3wZGh4KHGh2cy5iTWhvMDhvcFVFMzQzODZtYm9wVEZEcVA" --request GET http://localhost:8200/v1/kv/data/staging/service-svc
{"request_id":"30f5d4ec-ccb1-fcc2-f69d-142218651368","lease_id":"","renewable":false,"lease_duration":0,"data":{"data":{"heylo":"world","test":"testing"},"metadata":{"created_time":"2022-05-22T13:42:53.8885279Z","custom_metadata":null,"deletion_time":"","destroyed":false,"version":3}},"wrap_info":null,"warnings":null,"auth":null}
```

This is here just to remind me of how to publish to nuget, however do refer to the `Dockerfile.nuget.publish` file:

```bash
docker run -it -v ${PWD}:/root/HashiVaultCs --entrypoint "/bin/bash" "mcr.microsoft.com/dotnet/sdk:7.0-jammy-amd64"
cd ~/HashiVaultCs/
dotnet build -c Release
dotnet pack -c Release -o "Client/bin/Release/net7.0/publish"
dotnet nuget push "Client/bin/Release/net7.0/publish/HashiVaultCs.*.nupkg" -k [api-key-here /] -s https://api.nuget.org/v3/index.json
exit
```
