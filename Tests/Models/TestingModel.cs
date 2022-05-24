namespace Tests;

public partial class UnitTest
{
    internal class TestingModel
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public NestedTestingModel? NestedValue { get; set; }
    }
}