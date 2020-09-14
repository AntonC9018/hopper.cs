using System;
using System.Collections.Generic;
using Utils.Vector;

namespace Core.Old
{
    public class Generator
    {

        enum Mark
        {
            WALL, TILE, HALLWAY, RESTRICTED, ENEMY, EMPTY
        }
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
        Options options;
        Mark[,] grid;
        List<Room> rooms;
        int generateCount;
        Node rootNode;

        public Generator(int w, int h, Options ops)
        {
            Width = w;
            Height = h;
            options = ops;
            grid = new Mark[w, h];
            rooms = new List<Room>();
        }

        public bool Generate()
        {
            generateCount++;
            if (generateCount > options.max_iter)
            {
                return false;
            }
            ResetGrid();
            rooms.Clear();

            IntVector2 startPos = (dim - rootNode.dim) / 2;
            Room startRoom = new Room(startPos, rootNode.dim);
            Write(startRoom);
            rooms.Add(startRoom);
            rootNode.room = startRoom;

            if (!Iterate(rootNode, null))
                Generate();

            return true;
        }

        bool Iterate(Node parentNode, Room ignoreRoom)
        {
            // foreach (IntVector2 dir in parentNode.GetOccupiedDirections())
            // {

            // }
            return true;
        }

        void Write(Room room)
        {

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