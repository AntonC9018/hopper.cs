namespace Core
{
    public static class MoveSetup
    {
        static MoveSetup()
        {
            var baseDir = StatManager.DefaultFS.BaseDir;

            var move = new Move
            {
                power = 1,
                through = 0
            };

            baseDir.AddFile("move", move);
        }
    }
}