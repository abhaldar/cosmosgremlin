using Microsoft.Extensions.DependencyInjection;

namespace CosmosGremlin
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
            .AddSingleton<IGremlinGraph, GremlinGraph>()
            .BuildServiceProvider();
            Gremlin gremlin = new Gremlin(serviceProvider.GetService<IGremlinGraph>());            
        }
    }
}
