using System.Collections.Generic;
using Hopper.Utils.Vector;
using System.Linq;
using System;

namespace Hopper.Core.Generation
{

    public class Node
    {
        public int id;
        public Node parent;
        public IntVector2 position;
        public List<Node> childNodes;

        public Node(IntVector2 pos, int index, Node parent)
        {
            position = pos;
            childNodes = new List<Node>();
            this.id = index;
            this.parent = parent;
        }
    }

    public class Graph
    {
        public IntVector2 dimensions;
        public List<Node> nodes;
        public Dictionary<IntVector2, Node> availableCells;

        public Graph(IntVector2 dimensions)
        {
            this.dimensions = dimensions;
            availableCells = new Dictionary<IntVector2, Node>();
            nodes = new List<Node>();
            var rootNode = new Node(dimensions / 2, 0, null);
            ResetAvailable(rootNode);
            nodes.Add(rootNode);
        }

        public void ResetAvailable(Node node)
        {
            foreach (var position in node.position.OrthogonallyAdjacent)
            {
                if (position.x >= dimensions.x
                    || position.y >= dimensions.y
                    || position.x < 0
                    || position.y < 0
                    || nodes.Any(nd => nd.position == position))
                {
                    continue;
                }

                if (!availableCells.ContainsKey(position))
                {
                    availableCells.Add(position, node);
                }
            }
        }

        public void Print()
        {
            System.Console.WriteLine();

            for (int y = 0; y < dimensions.y; y++)
            {
                for (int x = 0; x < dimensions.x; x++)
                {
                    var vec = new IntVector2(x, y);
                    if (availableCells.ContainsKey(vec))
                    {
                        System.Console.Write("K ");
                    }
                    else
                    {
                        System.Console.Write("- ");
                    }
                }
                System.Console.WriteLine();
            }
        }

        public Node AddNode(Random rng)
        {
            var selectedKvp = availableCells.GetRandom(rng);
            availableCells.Remove(selectedKvp.Key);
            var node = new Node(selectedKvp.Key, nodes.Count, selectedKvp.Value);
            selectedKvp.Value.childNodes.Add(node);
            nodes.Add(node);
            ResetAvailable(node);
            return node;
        }
    }
}