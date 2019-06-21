using System;
using System.Security.Cryptography;

namespace eva.core.framework.framework
{
    /// <summary>
    /// A Class which generates Random Numbers using RNGCryptoServiceProvider used to get better randomness
    /// but is a little slow
    /// </summary>
    public sealed class SecureRandom : RandomNumberGenerator
    {
        /// <summary>
        /// RNGCryptoServiceProvider class
        /// </summary>
        private readonly RandomNumberGenerator rng = new RNGCryptoServiceProvider();


        /// <summary>
        /// Returns the next random number
        /// </summary>
        /// <returns>a random number</returns>
        public int Next()
        {
            var data = new byte[sizeof(int)];
            rng.GetBytes(data);
            return BitConverter.ToInt32(data, 0) & (int.MaxValue - 1);
        }

        /// <summary>
        /// Returns a random number which is smaller than max value
        /// </summary>
        /// <param name="maxValue">the max value of the random number</param>
        /// <returns>a random number smaller than max value</returns>
        public int Next(int maxValue)
        {
            return Next(0, maxValue);
        }

        /// <summary>
        /// Returns a random number which is smaller than max value and greater than the min
        /// </summary>
        /// <param name="maxValue">the max value of the random number</param>
        /// <param name="minValue">the min value of the random number</param>
        /// <returns>a random number between max and min</returns>
        public int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException();
            }
            return (int)Math.Floor((minValue + ((double)maxValue - minValue) * NextDouble()));
        }

        /// <summary>
        /// Function used to fetch a random double
        /// </summary>
        /// <returns>the double value</returns>
        private double NextDouble()
        {
            var data = new byte[sizeof(uint)];
            rng.GetBytes(data);
            var randUint = BitConverter.ToUInt32(data, 0);
            return randUint / (uint.MaxValue + 1.0);
        }

        /// <summary>
        /// Fetches bytes from eng crypto service
        /// </summary>
        /// <param name="data">the data to be converted to bytes</param>
        public override void GetBytes(byte[] data)
        {
            rng.GetBytes(data);
        }

        /// <summary>
        /// fetches non-zero bytes from rng crypto service
        /// </summary>
        /// <param name="data">the data whose bytes are to be fetched</param>
        public override void GetNonZeroBytes(byte[] data)
        {
            rng.GetNonZeroBytes(data);
        }
    }
}
