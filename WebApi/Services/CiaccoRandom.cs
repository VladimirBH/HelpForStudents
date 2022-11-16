namespace WebApi.Services
{
    public class CiaccoRandom : ICiaccoRandom
    {
        private static int tree = 0;
        public void SetSeed(int seed)
        {
            tree = Math.Abs(seed) % 9999999+1;
            GetRand(0, 9999999);
        }
        public int GetRand(int min, int max)
        {
            tree = (tree * 125) % 2796203;
            return tree % (max-min+1) + min;
        }
    }
}