using System.Runtime.InteropServices;
using NoParamlessCtor.Shared.Attributes;

namespace MathStuff.GraphTheory
{
    public sealed partial class Graph
    {
        private readonly Vertex[] _vertices;

        private readonly Edge[] _edges;

        private readonly VertexData[] _vertexData;

        internal readonly bool IsDirectedGraph;

        public readonly bool IsConnected;

        [NoParamlessCtor]
        public readonly partial struct VertexData(uint[] edgeIndices)
        {
            internal readonly uint[] EdgeIndices = edgeIndices;

            public uint Degree => unchecked((uint) EdgeIndices.Length);
        }

        public Graph(Vertex[] vertices, Edge[] edges, bool isDirectedGraph)
        {
            var vertexSet = new HashSet<Vertex>();

            var edgeSet = new HashSet<Edge>();

            foreach (var vertex in vertices)
            {
                if (!vertexSet.Add(vertex))
                {
                    throw new ArgumentException($"Duplicate vertex: {vertex}");
                }
            }

            var vertexToEdgeIndicesMap = new Dictionary<Vertex, List<uint>>();

            uint currentEdgeIndex = 0;

            foreach (var edge in edges)
            {
                if (!vertexSet.Contains(edge.From))
                {
                    throw new ArgumentException($"Edge from vertex {edge.From} which is not in the vertex set");
                }

                if (!vertexSet.Contains(edge.To))
                {
                    throw new ArgumentException($"Edge to vertex {edge.To} which is not in the vertex set");
                }

                if (!edgeSet.Add(edge))
                {
                    throw new ArgumentException($"Duplicate edge: {edge}");
                }

                if (!isDirectedGraph)
                {
                    var reversedEdge = edge.Invert();

                    if (!edgeSet.Add(reversedEdge))
                    {
                        throw new ArgumentException($"Duplicate edge: {reversedEdge}");
                    }
                }

                var leftVertex = edge.From;

                var rightVertex = edge.To;

                var leftVertexEdgeIndices = GetOrCreateEdgeIndices(leftVertex, vertexToEdgeIndicesMap);

                var rightVertexEdgeIndices = GetOrCreateEdgeIndices(rightVertex, vertexToEdgeIndicesMap);

                leftVertexEdgeIndices.Add(currentEdgeIndex);

                rightVertexEdgeIndices.Add(currentEdgeIndex);

                currentEdgeIndex++;

                continue;

                static List<uint> GetOrCreateEdgeIndices(Vertex vertex, Dictionary<Vertex, List<uint>> map)
                {
                    ref var slot = ref CollectionsMarshal.GetValueRefOrAddDefault(map, vertex, out var exists);

                    if (!exists)
                    {
                        slot = [];
                    }

                    return slot!;
                }
            }

            _vertices = vertices;

            _edges = edges;

            IsDirectedGraph = isDirectedGraph;

            var vertexDataList = new List<VertexData>();

            foreach (var vertex in vertices)
            {
                if (!vertexToEdgeIndicesMap.TryGetValue(vertex, out var edgeIndices))
                {
                    edgeIndices = [];
                }

                vertexDataList.Add(new(edgeIndices.ToArray()));
            }

            var vertexData = _vertexData = vertexDataList.ToArray();

            IsConnected = vertexData.All(x => x.Degree > 0);
        }

        private uint GetVertexIndex(Vertex vertex)
        {
            // For what is worth, this should be faster than using a dict
            // since the size of the array is likely to be small,
            // which means we are hitting CPU cache for most part
            return unchecked((uint) _vertices.IndexOf(vertex));
        }

        public VertexData GetVertexData(Vertex vertex)
        {
            // It is fine for the index to be out of range,
            // since the array access will just throw an exception...

            var vertexIndex = GetVertexIndex(vertex);

            var vertexData = _vertexData;

            // This pattern elides bounds check ( GetVertexIndex() returns uint )
            if (vertexIndex < vertexData.Length)
            {
                return vertexData[vertexIndex];
            }

            throw new ArgumentException($"Vertex {vertex} is not in the graph");
        }

        public override string ToString()
        {
            var verticesText = string.Join(", ", _vertices);

            var edgesText = string.Join(", ", _edges.Select(x => x.GetLabel(this)));

            return
            $$"""
            Vertices: { {{verticesText}} }
            
            Edges: { {{edgesText}} }
            
            Is Directed: {{IsDirectedGraph}}
            
            Is Connected: {{IsConnected}}
            """;
        }
    }
}