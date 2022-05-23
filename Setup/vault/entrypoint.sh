#!/bin/sh
VAULT_RETRIES=3

## Update these vault shards with your own. This script is taken over with the image during a ```docker build .```
VAULT_SHARD_ONE=dc761cbad9c291e6704010e73d66c7f59d3ceb65bff5ca8e216e222a6b239536e6
VAULT_SHARD_TWO=8818fbf9dc92e29491bbcc229f04d8169768aff51956b6ab8e7f858318e98f19e8

export VAULT_ADDR='http://localhost:8200'
export VAULT_API_ADDR='http://localhost:8200'

/bin/vault server -config=/vault/config/default.hcl -log-level=trace &
echo "Vault is starting...";

until /bin/vault status > /dev/null 2>&1 || [ "$VAULT_RETRIES" -eq 0 ]; do
	echo "Waiting for vault to start...: $((VAULT_RETRIES--))";
	sleep 1
done

if [ "$VAULT_RETRIES" -eq 0 ]; then
    echo "Vault is either sealed or in error state, attempting to unseal...";
    /bin/vault-unseal unseal --address http://localhost:8200 --shard=$VAULT_SHARD_ONE --shard=$VAULT_SHARD_TWO
fi

echo "Entering 10m sleep loop...";
while true; do
    sleep 10m
    echo "Ping :)";
    if [ /bin/vault status > /dev/null 2>&1 ]; then
        exit;
    fi
done
