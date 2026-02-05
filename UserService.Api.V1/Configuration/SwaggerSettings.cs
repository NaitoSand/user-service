namespace UserService.Api.V1.Configuration;

public class SwaggerSettings
{
    public bool Enabled { get; set; } = true;
    public string Title { get; set; } = string.Empty;
    public string Version { get; set; } = "v1";
    public string Description { get; set; } = string.Empty;

    public SwaggerContact Contact { get; set; } = new();

    public class SwaggerContact
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}