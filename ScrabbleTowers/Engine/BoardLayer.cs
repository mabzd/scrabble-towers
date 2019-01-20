using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ScrabbleTowers.Engine
{
    public class BoardLayer : IEnumerable<Tile>
    {
        private class Node
        {
            public Node(Tile tile)
            {
                Tile = tile;
                Neighbors = new List<Node>(8);
            }

            public Tile Tile { get; }
            public ICollection<Node> Neighbors { get; }
        }

        private struct Direction
        {
            public Direction(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; }
            public int Y { get; }
        }

        public const int MAX_TILES = 9;
        private const int WILDCARD_POSITION = MAX_TILES / 2;
        private readonly ICollection<Node> Nodes;

        private readonly ICollection<Direction> Directions = new[]
        {
            new Direction(1, 0),
            new Direction(1, -1),
            new Direction(0, -1),
            new Direction(-1, -1),
            new Direction(-1, 0),
            new Direction(-1, 1),
            new Direction(0, 1),
            new Direction(1, 1),
        };

        public BoardLayer(IEnumerable<char?> glyphs)
            : this(glyphs
                .Select((g, i) => g == null 
                    ? null
                    : new Tile(g.Value, i))
                .Where(t => t != null))
        {
        }

        public BoardLayer(IEnumerable<Tile> tiles)
        {
            var nodes = CreateNodes(tiles);
            LinkNodes(nodes);
            Nodes = nodes
                .Where(n => n != null)
                .ToList();
        }

        public bool IsAllowed(string word)
        {
            return Traverse(Nodes, 0, word);
        }

        private bool Traverse(ICollection<Node> nodes, int visited, string word)
        {
            var glyph = word[0];
            word = word.Substring(1);

            foreach (var node in nodes)
            {
                var position = node.Tile == null 
                    ? WILDCARD_POSITION
                    : node.Tile.Position;

                var bit = 1 << position;
                var isVisited = (visited & bit) == bit;

                if (!isVisited && (node.Tile.Glyph == glyph || node.Tile == null))
                {
                    if (word == "")
                        return true;

                    if (Traverse(node.Neighbors, visited | bit, word))
                        return true;
                }
            }

            return false;
        }

        private Node[] CreateNodes(IEnumerable<Tile> tiles)
        {
            var wildcardCovered = false;
            var nodes = new Node[MAX_TILES];

            foreach (var tile in tiles)
            {
                if (nodes[tile.Position] != null)
                    throw new InvalidOperationException("Tile on the same position");
                
                if (tile.Position == WILDCARD_POSITION)
                    wildcardCovered = true;
                
                nodes[tile.Position] = new Node(tile);
            }

            if (!wildcardCovered)
                nodes[WILDCARD_POSITION] = new Node(null);

            return nodes;
        }

        private void LinkNodes(Node[] nodes)
        {
            for (int position = 0; position < MAX_TILES; position++)
            {
                var node = nodes[position];

                if (node != null)
                {
                    foreach (var direction in Directions)
                    {
                        var neighbor = GetNeighbor(nodes, position, direction);

                        if (neighbor != null)
                            node.Neighbors.Add(neighbor);
                    }
                }
            }
        }

        private Node GetNeighbor(Node[] nodes, int position, Direction direction)
        {
            var (x, y) = Tile.ToXy(position);

            x += direction.X;
            y += direction.Y;

            if (x < 0 || x > 2 || y < 0 || y > 2)
                return null;

            var newPosition = Tile.ToPosition(x, y);
            return nodes[newPosition];
        }

        public IEnumerator<Tile> GetEnumerator()
        {
            return Nodes.Select(n => n.Tile).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
