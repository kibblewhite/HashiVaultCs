#!/bin/bash

## Update these vault shards with your own. This script is taken over with the image during a `docker build .`
VAULT_SHARD_ONE=pNxKrgN0wcTKGiDc4oUCuQlFlIpz4eQYapCmim0RoAzU
VAULT_SHARD_TWO=Bevd1r0qaW+S/HqT/nDYq4AwFNud/PVsOa2tdp+xZsV+

generate_unseal_request_01() {
cat -v <<EOF
    {"key": "${VAULT_SHARD_ONE}"}
EOF
}

generate_unseal_request_02() {
cat -v <<EOF
    {"key": "${VAULT_SHARD_TWO}"}
EOF
}

VAULT_STARTED=0

for i in {1..10}
do
  echo "Requesting https://127.0.0.1:8200/v1/sys/health"
  RESPONSE=$(curl -k -s -o /dev/null -w '%{http_code}' https://127.0.0.1:8200/v1/sys/health)
  echo "Response is ${RESPONSE}"

  if [ ${RESPONSE} -eq 200 ]; then
    echo "Vault Server is running"
    VAULT_STARTED=1
    break
  elif [ ${RESPONSE} -eq 501 ]; then
    echo "Vault Server is running, but not yet initialized"
    VAULT_STARTED=1
    break
  elif [ ${RESPONSE} -eq 503 ]; then
    echo "Vault Server is running, but it is sealead"
    VAULT_STARTED=1
    break
  else
    echo "Sleeping..."
    sleep 5
  fi
done


if [ ${VAULT_STARTED} -ne 1 ]; then
  echo "Vault Service is down"
  exit 0
fi

echo "Trying to unseal Vault"
curl -k -X PUT -d "$(generate_unseal_request_01)" https://127.0.0.1:8200/v1/sys/unseal
sleep 1
curl -k -X PUT -d "$(generate_unseal_request_02)" https://127.0.0.1:8200/v1/sys/unseal
echo "Unseal request done"
