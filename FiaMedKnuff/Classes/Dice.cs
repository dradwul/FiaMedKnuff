using System;

namespace FiaMedKnuff.Classes
{
    public class Dice
    {
        private Random Random { get; set; } = new Random();

        public int ThrowDie(int numberOfFaces)
        {
            return Random.Next(1, numberOfFaces+1);
        }
    }
}
