## Generating Self-Signed SSL Certificates for Vault 

Place the following text into a request configuration file named `req.cnf`, perhaps in a sperate folder.

In this example we have placed this into a subdirectory `vault/config/certs` to build our certificate's key, pem and crt files.

```
[dn]
CN = key-vault-svc

[req]
distinguished_name = dn
x509_extensions = v3_req
prompt = no

[v3_req]
authorityKeyIdentifier = keyid, issuer
keyUsage = digitalSignature, nonRepudiation, keyEncipherment, dataEncipherment
extendedKeyUsage = serverAuth
subjectAltName = @alt_names

[alt_names]
DNS.1 = key-vault-db
DNS.2 = key-vault-svc
DNS.3 = key-vault-svc.domain-atom.tld
DNS.4 = 127.0.0.1
DNS.5 = localhost
IP.1 = 127.0.0.1
```

> PEM Pass Phrase: <set-what-ever-you-want-it-to-be-here />
> Leave export passwords empty/blank

Generate the key file
```bash
openssl genrsa -des3 -out key-vault-db.key 2048
```

Use the key file and the request configurations `req.cnf` created earlier to create the certificate.
```bash
openssl req -x509 -new -nodes -key key-vault-db.key -sha256 -days 5475 -out key-vault-db.crt -config req.cnf
```

RSA-Encrypt the key file into our pem file.
```bash
openssl rsa -in key-vault-db.key -out key-vault-db.pem
```

Manually check that the certificate is valid. Check that the 'X509v3 Subject Alternative Name' are present and correct as configured in the `req.cnf` file.
```bash
openssl x509 -in key-vault-db.crt -text
```

### Connecting via the Vault CLI using the Self-Signed SSL Certificate

There are a number of issue surrounding the use of self-signed certificate wuth the Vault-CLI, so be aware that there are some points to be aware of that is beyond this docuentation.

When you attempt to connect to the vault using the CLI, it will most likely return an error stating the following:
```bash
/ # vault status
Error checking seal status: Get "https://127.0.0.1:8200/v1/sys/seal-status": x509: certificate signed by unknown authority
```

Therefore it is possible to by-pass this during development only (NOT in production) to set the following environment varaible:
```bash
export VAULT_SKIP_VERIFY="true"
```

A normal interaction will look like the following:
```bash
/ # export VAULT_ADDR="https://127.0.0.1:8200"
/ # export VAULT_API_ADDR="https://127.0.0.1:8200"
/ # export VAULT_SKIP_VERIFY="true"
/ # export VAULT_TOKEN="<your-token-herer />"
/ # vault status
Key             Value
---             -----
Seal Type       shamir
Initialized     true
Sealed          false
Total Shares    12
Threshold       2
Version         1.10.3
Storage Type    postgresql
Cluster Name    vault-cluster-36788daa
Cluster ID      55cf3e14-5cb5-321a-be0a-04abd98466d6
HA Enabled      false
```