path "kv/data/staging/*" {
  capabilities = [ "create", "read", "delete", "update", "list" ]
}

path "kv/staging/*" {
  capabilities = [ "create", "read", "update", "delete", "list" ]
}

path "kv/*" {
  capabilities = [ "list" ]
}
