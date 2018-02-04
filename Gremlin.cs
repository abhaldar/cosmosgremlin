namespace CosmosGremlin
{
    internal class Gremlin
    {
        private readonly IGremlinGraph gremlinGraph;
        public Gremlin(IGremlinGraph gremlinGraph)
        {
            this.gremlinGraph = gremlinGraph;
        }
    }
}