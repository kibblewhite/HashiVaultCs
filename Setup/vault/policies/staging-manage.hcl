# Allow a management of staging kv
path "kv/data/staging/*" {
    capabilities = ["create", "read", "update", "delete", "list"]
}