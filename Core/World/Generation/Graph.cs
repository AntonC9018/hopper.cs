using System.Collections.Generic;
using Utils.Vector;
using System.Linq;

namespace Core.Generation
{
    public class FutureNode
    {
        public IntVector2 position;
        public Node node;

        public FutureNode(IntVector2 position, Node node)
        {
            this.position = position;
            this.node = node;
        }

        public override bool Equals(object obj)
        {
            return position == ((FutureNode)obj).position;
        }

        public override int GetHashCode()
        {
            return position.GetHashCode();
        }
    }

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
                    || nodes.Any(node => node.position.Equals(position)))
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

        public Node AddNode()
        {
            var selectedKvp = availableCells.GetRandom();
            availableCells.Remove(selectedKvp.Key);
            var node = new Node(selectedKvp.Key, nodes.Count, selectedKvp.Value);
            selectedKvp.Value.childNodes.Add(node);
            nodes.Add(node);
            ResetAvailable(node);
            return node;
        }
    }
}