#!/bin/sh

export VAULT_SKIP_VERIFY="true"
export VAULT_ADDR="https://127.0.0.1:${PORT}"
export VAULT_API_ADDR="https://127.0.0.1:${PORT}"
export VAULT_DB_QUERY_PARAM="?sslmode=disable"
export VAULT_PG_CONNECTION_URL="${DATABASE_URL}${VAULT_DB_QUERY_PARAM}"

nohup bash /vault/unseal.sh &

echo "Vault is starting on address > ${VAULT_API_ADDR}";
echo "Vault database > ${VAULT_PG_CONNECTION_URL}"
/bin/vault server -address=$VAULT_API_ADDR -config=/vault/config/default.hcl -log-level=trace
