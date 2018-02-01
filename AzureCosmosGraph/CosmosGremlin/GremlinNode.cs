using System;
using System.Collections;
using System.Collections.Generic;

namespace CosmosGremlin
{
    public class GremlinNode<T> : IEnumerable<T>
    {
        private List<T> nodeList;
        public GremlinNode() : this(null) { }
        public GremlinNode(List<T> nodeList)
        {
            if (nodeList == null)
                this.nodeList = new List<T>();
            else
                this.nodeList = nodeList;
        }
        public void Add(T value)
        {
            nodeList.Add(value);
        }
        public bool Persists()
        {
            return nodeList != null;
        }
        public List<T> Get()
        {
            return nodeList;
        }
        public void Delete(T value)
        {
            nodeList.Remove(value);
        }
        public bool Contains(T value)
        {
            return nodeList.Contains(value);
        }
        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}