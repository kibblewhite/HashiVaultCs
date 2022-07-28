path "auth/approle/role/staging/secret*" {
  capabilities = [ "create" ]
  min_wrapping_ttl = "1m"
  max_wrapping_ttl = "3m"
}

path "auth/approle/role/staging/secret-id" {
   capabilities = [ "update" ]
}

path "auth/approle/role/staging/role-id" {
   capabilities = [ "read" ]
}
