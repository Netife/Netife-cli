namespace Netife_cli.Model;

public class RuntimeConfiguration
{
    public string DispatcherHost { get; set; } = "0.0.0.0";

    public string DispatcherPort { get; set; } = "7890";

    public string JsRemoteHost { get; set; } = "0.0.0.0";

    public string JsRemotePort { get; set; } = "7892";

    public string FrontendHost { get; set; } = "0.0.0.0";

    public string FrontendPort { get; set; } = "7891";
}