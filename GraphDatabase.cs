namespace CosmosGremlin
{
    public partial class GraphDatabase
    {
        public string Database { get; set; }
        public string Id { get; set; }
        public string Endpoint { get; set; }
        public string Key { get; set; }
        public int Throughput { get; set; }
    }
}