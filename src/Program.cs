namespace ctf_sandbox;

public static class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                var port = Environment.GetEnvironmentVariable("PORT");
                if (port != null)
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.ListenAnyIP(int.Parse(port));
                    });
                }
                webBuilder.UseStartup<Startup>();
            });
    }
}