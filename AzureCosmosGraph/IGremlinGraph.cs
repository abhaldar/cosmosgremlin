using System.Collections.Generic;
using System.Threading;

namespace CosmosGremlin
{
    internal interface IGremlinGraph
    {
        void AddNode(Dictionary<string, string> value);
        void AddUndirectedEdge(string from, string to, string relationship);
        void AddDirectedEdge(string from, string to, string relationship);
        void UpdateNode(Dictionary<string, string> value);
        List<string> FindNode(Dictionary<string, string> value, GraphDatabase graphDatabase);
        List<object> TraverseGraph(Dictionary<string, string> value);
        void DropNode(Dictionary<string, string> value);
        void ClearGraph(CancellationToken token);
        void Create(GraphDatabase graphDatabase);
        bool Contains(Dictionary<string, string> value);
    }
}