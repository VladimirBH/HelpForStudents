namespace WebApi.Services
{
    public interface ICiaccoRandom
    {
        void SetSeed(int seed);
        int GetRand(int min, int max);
    }
}