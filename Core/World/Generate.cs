using System;
using System.Collections.Generic;
using Vector;

namespace Core
{
    public class Generator
    {

        public int Width
        {
            get => dim.x;
            set => dim.x = value;
        }
        public int Height
        {
            get => dim.y;
            set => dim.y = value;
        }
        public IntVector2 dim;
        public Options options;

        public Generator(int w, int h, Options ops)
        {
            Width = w;
            Height = h;
            options = ops;
            grid = new Mark[w, h];
        }

        enum Mark
        {
            WALL, TILE, HALLWAY, RESTRICTED, ENEMY, EMPTY
        }

        Mark[,] grid;
        int generateCount;
        Node rootNode;
        public bool Generate()
        {
            generateCount++;
            if (generateCount > options.max_iter)
            {
                return false;
            }
            ResetGrid();

            IntVector2 startPos = (dim - rootNode.dim) / 2;
            Room startRoom = new Room(startPos, rootNode.dim);
            // TODO: complete
            return true;
        }

        private void ResetGrid()
        {
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    grid[i, j] = Mark.EMPTY;
        }

    }

    public class Room
    {
        private IntVector2 startPos;
        private IntVector2 dim;

        public Room(IntVector2 startPos, IntVector2 dim)
        {
            this.startPos = startPos;
            this.dim = dim;
        }
    }

    public class Node
    {
        public IntVector2 posInGraph;
        public IntVector2 dim;
        public List<Node> neighbors;
        public Room room;
    }

    public class Options
    {
        public int max_hallway_length = 5;
        public int min_hallway_length = 0;
        public int min_hallway_width = 1;
        public int max_hallway_width = 2;
        public float enemy_density = 1 / 10;
        public int max_iter = 50;
    }
}