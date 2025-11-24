namespace HashiVaultCs.Utilities;

internal static class FormattableUriProvider
{
    private static readonly Dictionary<string, FormattableArgument> _display_name_to_enum_cache = [];

    private static readonly string username = "{username}";
    private static readonly string rolename = "{rolename}";
    private static readonly string engine = "{engine}";
    private static readonly string path = "{path}";

    public static FormattableUri AuthUserpassLogin => _auth_userpass_login;
    private static readonly FormattableUri _auth_userpass_login = new()
    {
        Value = $"/v1/auth/userpass/login/{username}",
        CachedUris = []
    };

    public static FormattableUri AuthApproleRoleId => _auth_approle_role_id;
    private static readonly FormattableUri _auth_approle_role_id = new()
    {
        Value = $"/v1/auth/approle/role/{rolename}/role-id",
        CachedUris = []
    };

    public static FormattableUri AuthApproleSecretId => _auth_approle_secret_id;
    private static readonly FormattableUri _auth_approle_secret_id = new()
    {
        Value = $"/v1/auth/approle/role/{rolename}/secret-id",
        CachedUris = []
    };

    public static FormattableUri AuthApproleLogin => _auth_approle_login;
    private static readonly FormattableUri _auth_approle_login = new()
    {
        Value = $"/v1/auth/approle/login",
        CachedUris = []
    };

    public static FormattableUri SecretsEngineDataPath => _secrets_engine_data_path;
    private static readonly FormattableUri _secrets_engine_data_path = new()
    {
        Value = $"/v1/{engine}/data/{path}",
        CachedUris = []
    };

    public static InternalOperation<string> GetPath(this FormattableUri formattable_uri, Dictionary<FormattableArgument, string> values)
    {
        int cache_key = values.GetCacheIdentifier();
        if (formattable_uri.CachedUris.TryGetValue(cache_key, out string? cached_uri) is true && string.IsNullOrWhiteSpace(cached_uri) is false)
        {
            return InternalOperation<string>.Success(cached_uri);
        }

        object?[] arguments = formattable_uri.Value.GetArguments();
        string[] arguments_names = new string[formattable_uri.Value.ArgumentCount];

        // Extract argument names from the arguments (which are your placeholder strings)
        for (int i = 0; i < formattable_uri.Value.ArgumentCount; i++)
        {
            string? arguments_name = arguments[i] as string;
            if (string.IsNullOrWhiteSpace(arguments_name))
            {
                return InternalOperation<string>.Failure("Invalid argument name found in the formattable string.");
            }

            arguments_names[i] = arguments_name;
        }

        object[] parameters = new object[formattable_uri.Value.ArgumentCount];
        for (int i = 0; i < formattable_uri.Value.ArgumentCount; i++)
        {
            FormattableArgument formattable_argument = GetFormattableArgumentByDisplayName(arguments_names[i]);
            if (values.TryGetValue(formattable_argument, out string? value) is false || string.IsNullOrWhiteSpace(value))
            {
                return InternalOperation<string>.Failure($"Missing value for argument '{arguments_names[i]}'.");
            }

            parameters[i] = value;
        }

        string result = string.Format(formattable_uri.Value.Format, parameters);
        formattable_uri.CachedUris.Add(cache_key, result);
        return InternalOperation<string>.Success(result);
    }

    internal static FormattableArgument GetFormattableArgumentByDisplayName(string display_name)
    {
        if (string.IsNullOrWhiteSpace(display_name))
        {
            return FormattableArgument.Default;
        }

        if (_display_name_to_enum_cache.TryGetValue(display_name, out FormattableArgument value) is true && value is not FormattableArgument.Default)
        {
            return value;
        }

        FieldInfo[] field_information_array = typeof(FormattableArgument).GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (FieldInfo field_information in field_information_array)
        {
            DisplayAttribute? display_attribute = field_information.GetCustomAttribute<DisplayAttribute>();
            if (display_attribute?.Name == display_name)
            {
                FormattableArgument formattable_argument = Enum.Parse<FormattableArgument>(field_information.Name);
                _display_name_to_enum_cache[display_name] = formattable_argument;
                return formattable_argument;
            }
        }

        return FormattableArgument.Default;
    }
}
