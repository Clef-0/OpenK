namespace GameProject
{
    public static class Program
    {
        static void Main()
        {
            using (var game = new KProject())
            {
                game.Run();
            }
        }
    }
}
