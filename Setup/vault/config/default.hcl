storage "postgresql" {
  connection_url       = "postgres://username:password@hostname:5432/database?sslmode=require"
  table                = "vault_kv_store"
  ha_enabled           = true
  max_idle_connections = 1
  max_parallel         = 4
}

listener "tcp" {
  address              = "0.0.0.0:8200"
  tls_disable          = true
}

api_addr               = "http://localhost:8200"
ui                     = true

disable_cache          = true
disable_mlock          = true
disable_sealwrap       = true
