#region Packages
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Graphs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace CosmosGremlin
{
    public class GremlinGraph : IGremlinGraph, IDisposable
    {
        #region Class Variables
        public static GremlinNode<object> gremlinNode;
        private DocumentClient client;
        public DocumentCollection graphCollection;
        private GraphDatabase graphDatabase;
        public List<string> queryResult;
        #endregion

        private GremlinGraph()
        {
            if (gremlinNode == null)
                gremlinNode = new GremlinNode<object>();
        }

        public void AddDirectedEdge(string from, string to, string relationship)
        {
            try
            {
                if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to) && !string.IsNullOrEmpty(relationship))
                {
                    string key = Utility.CreateGuid(from + to).ToString();
                    string query = $"g.V('{from}').addE('{relationship}').to(g.V('{to}'))";
                    var edge = new KeyValuePair<string, string>(key, query);
                    if (!gremlinNode.Contains(edge))
                        gremlinNode.Add(edge);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Adding Edge Is: {ex}");
            }
        }

        public void AddNode(Dictionary<string, string> value)
        {
            try
            {
                if (value.Count > 1 && value != null)
                {
                    string query = "";
                    foreach (var row in value)
                    {
                        if (row.Key.Equals("Label"))
                            query = $@"g.addV('{row.Value}')";
                        else
                            query = query + $".property('{row.Key}','{row.Value}')";
                    }
                    string key = Utility.CreateGuid(query).ToString();
                    var edge = new KeyValuePair<string, string>(key, query);
                    if (!gremlinNode.Contains(edge))
                        gremlinNode.Add(edge);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Adding Node Is: {ex}");
            }
        }

        public void AddUndirectedEdge(string from, string to, string relationship)
        {
            try
            {
                if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to) && !string.IsNullOrEmpty(relationship))
                {
                    //From-To
                    string key = Utility.CreateGuid(from + to).ToString();
                    string query = $"g.V('{from}').addE('{relationship}').to(g.V('{to}'))";
                    var edge = new KeyValuePair<string, string>(key, query);
                    if (!gremlinNode.Contains(edge))
                        gremlinNode.Add(edge);
                    //To-From
                    key = Utility.CreateGuid(to + from).ToString();
                    query = $"g.V('{to}').addE('{relationship}').to(g.V('{from}'))";
                    edge = new KeyValuePair<string, string>(key, query);
                    if (!gremlinNode.Contains(edge))
                        gremlinNode.Add(edge);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Adding Edge Is: {ex}");
            }
        }

        public void ClearGraph(CancellationToken token)
        {
            if (token == null)
                throw new KeyNotFoundException();
            string key = Guid.NewGuid().ToString();
            string query = "g.V().drop()";
            gremlinNode.Add(new KeyValuePair<string, string>(key, query));
        }

        public bool Contains(Dictionary<string, string> value)
        {
            return gremlinNode.Contains(value);
        }

        public void Create(GraphDatabase graphDatabase)
        {
            try
            {
                //Checking is the List inside GremlinNode class exists or not before executing rest of the block
                if (!gremlinNode.Persists())
                    throw new InvalidOperationException();
                if (client == null)
                {
                    this.graphDatabase = graphDatabase;
                    DatabaseConnection().Wait();
                }

                //Sending Query to Graph DB
                Console.WriteLine("Sending Query to Azure Cosmos - GRAPH API...");
                foreach (KeyValuePair<string, string> query in gremlinNode.Get())
                {
                    IDocumentQuery<dynamic> documentQuery = client.CreateGremlinQuery<dynamic>(graphCollection, query.Value);
                    while (documentQuery.HasMoreResults)
                    {
                        CreateDocumentAsync(documentQuery).Wait();
                    }
                }
                Console.WriteLine("Data Sent to Cosmos Graph API...");
                Console.WriteLine();

                //Clean up the object store with Graph Entity Queue List
                //Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gremlin Creation Error Is: {ex}");
            }
        }

        private async Task CreateDocumentAsync(IDocumentQuery<dynamic> documentQuery)
        {
            try
            {
                queryResult = new List<string>();
                foreach (dynamic result in await documentQuery.ExecuteNextAsync())
                {
                    queryResult.Add(Newtonsoft.Json.JsonConvert.SerializeObject(result));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gremlin Creation Error Is: {ex}");
            }
        }

        private async Task DatabaseConnection()
        {
            //Connecting to Azure CosmosDb - GraphAPI [Gremlin]
            client = new DocumentClient(
                new Uri(graphDatabase.Endpoint),
                graphDatabase.Key,
                new ConnectionPolicy { ConnectionMode = ConnectionMode.Direct, ConnectionProtocol = Protocol.Tcp });

            graphCollection = await client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(graphDatabase.Database),
                new DocumentCollection { Id = graphDatabase.Id },
                new RequestOptions { OfferThroughput = graphDatabase.Throughput });
        }

        public void Dispose()
        {
            if (gremlinNode != null)
                gremlinNode = null;
            if (client != null)
                client.Dispose();
        }

        public void DropNode(Dictionary<string, string> value)
        {
            throw new NotImplementedException();
        }

        public List<string> FindNode(Dictionary<string, string> value, GraphDatabase graphDatabase)
        {
            try
            {
                if (value.Count > 0 && value != null)
                {
                    string query = "g.V()";
                    foreach (var row in value)
                    {
                        if (row.Key.Contains("Label"))
                            query = query + $@".hasLabel('{row.Value}')";
                        else if (row.Key.Contains("In"))
                            query = query + $@".in('{row.Value}')";
                        else if (row.Key.Contains("Out"))
                            query = query + $@".out('{row.Value}')";
                        else if (row.Key.Contains("Simple Path"))
                            query = query + ".simplePath()";
                        else if (row.Key.Contains("Title"))
                            query = query + $".has('Title','{row.Value}')";
                        else
                            query = query + $".has('{row.Key}','{row.Value}')";
                    }
                    string key = Utility.CreateGuid(query).ToString();
                    var edge = new KeyValuePair<string, string>(key, query);
                    if (!gremlinNode.Contains(edge))
                        gremlinNode.Add(edge);
                    Create(graphDatabase);
                    //var response = new List<string>();
                    //response.Add(queryResult);
                    return queryResult;
                }
                throw new KeyNotFoundException();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Adding Node Is: {ex}");
                return null;
            }
        }

        public List<object> TraverseGraph(Dictionary<string, string> value)
        {
            throw new NotImplementedException();
        }

        public void UpdateNode(Dictionary<string, string> value)
        {
            throw new NotImplementedException();
        }
    }
}
