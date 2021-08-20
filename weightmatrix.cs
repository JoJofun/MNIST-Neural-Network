using System;

namespace MNISTForms
{
    class weightmatrix
    {
        public int prevSize;
        public int nextSize;
        public double[,] w;
        public double[,] w0;

        public weightmatrix(int prev, int next, Random rand)//initalize matrix to random weight
        {
            prevSize = prev;
            nextSize = next;
            w = new double[prev, next];//doing next,prev internally for a significant speed boost because of how arrays are stored

            rand.Next();
            for (int i = 0; i < prev; i++)
            {
                for (int j = 0; j < next; j++)
                {
                    w[i, j] = rand.NextDouble() - 0.5;
                    //Console.WriteLine(matrix[i, j]);
                }
            }

            //average err needs to be locked when writing/reading for train function
            w0 = new double[prev, next];
            for (int i = 0; i < prev; i++)
            {
                for (int j = 0; j < next; j++)
                {
                    w0[i, j] = 0.0;
                }
            }

        }
        public void prntsiz()
        {
            Console.WriteLine("Matrix Next: " + nextSize + " prev: " + prevSize);
        }
    }
}
