ARG GO_VERSION=1.16-alpine3.12

FROM golang:${GO_VERSION} AS builder
LABEL org.opencontainers.image.source="https://github.com/omegion/vault-unseal"
WORKDIR /app
RUN apk update && \
  apk add ca-certificates gettext git make curl unzip && \
  rm -rf /tmp/* && \
  rm -rf /var/cache/apk/* && \
  rm -rf /var/tmp/* && \
  git clone https://github.com/omegion/vault-unseal.git
RUN cd /app/vault-unseal && make build-for-container

FROM vault:latest
ENV IMAGE_NAME="vault-svc"
LABEL name="vault-svc"
COPY --from=builder /app/vault-unseal/dist/vault-unseal-linux /bin/vault-unseal
COPY vault/config/default.hcl /vault/config/default.hcl
COPY vault/entrypoint.sh /vault/entrypoint.sh
RUN chmod 755 /vault/entrypoint.sh

EXPOSE 8200
ENTRYPOINT ["/vault/entrypoint.sh"]
