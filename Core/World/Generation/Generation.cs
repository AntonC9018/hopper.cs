using System;
using System.Collections.Generic;
using System.Diagnostics;
using Hopper.Core.Utils;
using Hopper.Core.Utils.Vector;

namespace Hopper.Core.Generation
{
    public partial class Generator
    {
        public enum Mark
        {
            WALL = 'w', TILE = 't', HALLWAY = 'h', RESTRICTED = 'r', ENEMY = 'e', EMPTY = 0
        }
        public Mark[,] grid;

        public Graph graph;

        public class Options
        {
            public int max_hallway_length = 0;
            public int min_hallway_length = -1;
            public int min_hallway_width = 2;
            public int max_hallway_width = 2;
            public float enemy_density = 1 / 10;
            public int max_iter = 50;
        }

        public List<Room> rooms;

        Options options;
        Random rng;

        IntVector2 dimensions;

        public Generator(int w, int h, Options ops)
        {
            graph = new Graph(new IntVector2(5, 5));
            grid = new Mark[w, h];
            dimensions = new IntVector2(w, h);
            rooms = new List<Room>();
            options = ops;
            rng = new Random();
        }

        public void AddRoom(IntVector2 dimensions)
        {
            var node = rooms.Count == 0 ? graph.nodes[0] : graph.AddNode(rng);
            var room = new Room(dimensions, node);
            rooms.Add(room);
        }

        public Room NodeToRoom(Node node)
        {
            return rooms[node.id];
        }

        public void Generate()
        {
            rooms[0].SetPositionFromCenter(dimensions / 2);
            WriteRoom(rooms[0]);
            Iterate(rooms[0]);
        }

        public void Iterate(Room parent)
        {
            foreach (var node in parent.node.childNodes)
            {
                var child = NodeToRoom(node);
                GenerateRoom(parent, child);
                Iterate(child);
            }
        }

        public void GenerateRoom(Room parent, Room child)
        {
            IntVector2 direction = child.node.position - parent.node.position;
            IntVector2 orthogonal_direction = direction.RotateHalfPi();

            Vector2 parent_center = parent.Center;
            Vector2 parent_border = parent.GetBorderCenter(direction);
            IntVector2 parent_orthogonal_dimension_minus_one_vector = parent.GetDimensionMinusOneVector(orthogonal_direction);
            int parent_orthogonal_dimension = parent.GetDimension(orthogonal_direction);

            IntVector2 child_orthogonal_dimension_vector = child.GetDimensionVector(orthogonal_direction);
            IntVector2 child_orthogonal_dimension_minus_one_vector = child.GetDimensionMinusOneVector(orthogonal_direction);
            int child_orthogonal_dimension = child.GetDimension(orthogonal_direction);
            IntVector2 child_dimension_minus_one_vector = child.GetDimensionMinusOneVector(direction);

            int wall_width = 1;
            int useful_parent_width = parent_orthogonal_dimension - 2 * wall_width;
            int useful_child_width = child_orthogonal_dimension - 2 * wall_width;

            Debug.Assert(
                useful_child_width >= options.min_hallway_width
                    && useful_parent_width >= options.min_hallway_width,
                "Cannot be less"
            );

            int min_orthogonal_offset = -useful_child_width + options.min_hallway_width;
            int max_orthogonal_offset = useful_parent_width - options.min_hallway_width;

            Queue<int> offsets_queue = Functions.ShuffledRangeQueue(
                min_orthogonal_offset, max_orthogonal_offset, rng);

            while (offsets_queue.Count != 0)
            {
                int current_orthogonal_offset = offsets_queue.Dequeue();

                int current_useful_parent_width = useful_parent_width;
                int current_useful_child_width = useful_child_width;

                if (current_orthogonal_offset > 0)
                {
                    current_useful_parent_width -= current_orthogonal_offset;
                }
                else
                {
                    current_useful_child_width += current_orthogonal_offset;
                }

                int min_useful_width = Maths.Min(current_useful_parent_width, current_useful_child_width);

                int max_hallway_width = Maths.Min(options.max_hallway_width, min_useful_width);
                int min_hallway_width = options.min_hallway_width;

                int min_hallway_length = options.min_hallway_length;
                int max_hallway_length = options.max_hallway_length;

                int current_hallway_width = rng.Next(min_hallway_width, max_hallway_width + 1);
                int current_hallway_length = rng.Next(min_hallway_length, max_hallway_length + 1);

                int min_hallway_offset = 0;
                int max_hallway_offset = min_useful_width - current_hallway_width;

                int current_hallway_offset = rng.Next(min_hallway_offset, max_hallway_offset + 1);

                Vector2 child_corner = parent_border
                    + ((Vector2)parent_orthogonal_dimension_minus_one_vector) / 2
                    + direction * (current_hallway_length + 1)
                    + orthogonal_direction * current_orthogonal_offset;


                if (child_corner.IsWhole() == false)
                {
                    throw new System.Exception("Must be whole. Review the algo");
                }

                Vector2 child_center = child_corner
                    - ((Vector2)child_orthogonal_dimension_minus_one_vector) / 2
                    + ((Vector2)child_dimension_minus_one_vector) / 2;

                child.SetPositionFromCenter(child_center);

                if (!IntersectsRooms(child))
                {
                    // child.roomStuff = new RoomStuff
                    // {
                    //     hallway_length = current_hallway_length,
                    //     hallway_width = current_hallway_width,
                    //     wall_width = wall_width,
                    //     hallway_coord
                    // };

                    Vector2 hallway_start = parent_border
                        + ((Vector2)parent_orthogonal_dimension_minus_one_vector) / 2
                        - orthogonal_direction * wall_width
                        - orthogonal_direction * current_hallway_offset;

                    if (current_orthogonal_offset < 0)
                    {
                        hallway_start += orthogonal_direction * current_orthogonal_offset;
                    }

                    IntVector2 hallway_start_int = (IntVector2)hallway_start;

                    IntVector2 hallway_dims = direction * Maths.Abs(current_hallway_length + 2)
                        + orthogonal_direction * Maths.Abs(current_hallway_width + 2);

                    Vector2 hallway_center = hallway_start
                        + (Vector2)direction * (((float)current_hallway_length + 1) / 2);

                    Room hallway = new Room(hallway_dims, null);
                    hallway.SetPositionFromCenter(hallway_center);

                    if (!IntersectsRooms(hallway))
                    {
                        WriteRoom(child);

                        WriteHallway(
                            current_hallway_length + 2,
                            current_hallway_width,
                            direction,
                            hallway_start_int);
                        rooms.Add(hallway);

                        offsets_queue.Clear();
                    }

                }
            }
        }

        private void WriteHallway(int length, int width, IntVector2 direction, IntVector2 start)
        {
            IntVector2 orthogonal_direction = -direction.RotateHalfPi();
            int wall_width = 1;

            for (int i = 0; i < length; i++)
            {
                IntVector2 current_anchor = start + direction * i;
                for (int j = 0; j < width; j++)
                {
                    IntVector2 current_position = current_anchor + orthogonal_direction * j;
                    grid[current_position.x, current_position.y] = Mark.HALLWAY;
                }
                for (int j = 1; j <= wall_width; j++)
                {
                    IntVector2 current_position = current_anchor - orthogonal_direction * j;
                    grid[current_position.x, current_position.y] = Mark.WALL;

                    current_position = current_anchor + orthogonal_direction * (j + width - 1);
                    grid[current_position.x, current_position.y] = Mark.WALL;
                }
            }
        }

        private void WriteRoom(Room room)
        {
            // int wall_width = 1
            for (int x = 0; x < room.dimensions.x; x++)
            {
                grid[room.position.x + x, room.position.y] = Mark.WALL;
                grid[room.position.x + x, room.position.y + room.dimensions.y - 1] = Mark.WALL;
            }
            for (int y = 1; y < room.dimensions.y - 1; y++)
            {
                grid[room.position.x, room.position.y + y] = Mark.WALL;
                grid[room.position.x + room.dimensions.x - 1, room.position.y + y] = Mark.WALL;
            }
            for (int x = 1; x < room.dimensions.x - 1; x++)
            {
                for (int y = 1; y < room.dimensions.y - 1; y++)
                {
                    grid[room.position.x + x, room.position.y + y] = Mark.TILE;
                }
            }
        }

        private bool IntersectsRooms(Room target_room)
        {
            int wall_width = 1;

            Vector2 target_room_center = target_room.Center;
            Vector2 twos = new Vector2(2.0f, 2.0f);
            Vector2 walls_target_dimensions = twos * wall_width;

            foreach (var room in rooms)
            {
                if (target_room == room)
                    continue;
                // if (room.position == null)
                //     continue;

                Vector2 room_center = room.Center;
                Vector2 center_difference = (target_room_center - room_center).Abs();
                Vector2 walls_dimensions = twos * wall_width;

                Vector2 useful_width_sum = (Vector2)(target_room.dimensions + room.dimensions);
                useful_width_sum -= walls_target_dimensions;
                useful_width_sum -= walls_dimensions;

                Vector2 half_dimension_sum = (useful_width_sum) / 2;
                Vector2 delta = center_difference - half_dimension_sum;

                if (delta.x < 0 && delta.y < 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}