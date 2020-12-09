namespace Hopper.Utils
{
    public static class Assert
    {
        public static void That(bool expression, string message = "")
        {
            if (expression == false)
            {
                throw new System.Exception(message);
            }
        }
    }
}