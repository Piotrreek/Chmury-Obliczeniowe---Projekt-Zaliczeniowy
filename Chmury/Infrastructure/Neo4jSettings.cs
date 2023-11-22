namespace Chmury.Infrastructure;

public class Neo4jSettings
{
    public Uri Neo4jConnection { get; set; } = default!;
    public string Neo4jUser { get; set; } = default!;
    public string Neo4jPassword { get; set; } = default!;
    public string Neo4jDatabase { get; set; } = default!;
}