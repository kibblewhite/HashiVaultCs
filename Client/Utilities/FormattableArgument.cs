namespace HashiVaultCs.Utilities;

internal enum FormattableArgument
{
    Default,

    [Display(Name = "{username}", ResourceType = typeof(string))]
    Username,

    [Display(Name = "{rolename}", ResourceType = typeof(string))]
    Rolename,

    [Display(Name = "{engine}", ResourceType = typeof(string))]
    Engine,

    [Display(Name = "{path}", ResourceType = typeof(string))]
    Path
}
