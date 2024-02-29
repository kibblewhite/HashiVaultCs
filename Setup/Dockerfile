FROM hashicorp/vault:latest
ENV IMAGE_NAME="vault-svc"
LABEL name="vault-svc"

RUN apk update && apk add bash curl nano

## Follow the README.md in the Setup/vault/config/certs directory
COPY vault/config/certs/vault-svc.crt /vault/config/certs/vault-svc.crt
COPY vault/config/certs/vault-svc.pem /vault/config/certs/vault-svc.pem
COPY vault/config/certs/vault-svc.key /vault/config/certs/vault-svc.key

COPY vault/config/default.hcl /vault/config/default.hcl
COPY vault/unseal.sh /vault/unseal.sh
COPY vault/entrypoint.sh /vault/entrypoint.sh
RUN chmod 755 /vault/unseal.sh /vault/entrypoint.sh

EXPOSE 8200
ENTRYPOINT ["/vault/entrypoint.sh"]
