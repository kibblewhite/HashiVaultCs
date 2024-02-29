storage "postgresql" {
  table                               = "vault_kv_store"
  ha_enabled                          = false
  max_idle_connections                = 1
  max_parallel                        = 4
}

listener "tcp" {
  address                             = "0.0.0.0:8200"
  tls_disable                         = false
  tls_cert_file                       = "/vault/config/certs/vault-svc.crt"
  tls_key_file                        = "/vault/config/certs/vault-svc.pem"
  tls_disable_client_certs            = true 
  tls_require_and_verify_client_cert  = false

}

ui                                    = true
disable_cache                         = true
disable_mlock                         = true
disable_sealwrap                      = true
