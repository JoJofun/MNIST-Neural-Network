using System;

namespace MNISTForms
{
    class outputlayer : layer
    {
        public outputlayer(int Size, layer.Activation_Type Type, Random rand)
        {

            type = Type;
            size = Size;
            a = new double[size];
            b = new double[size];
            s = new double[size];
            b0 = new double[size];


            rand.Next();

            for (int i = 0; i < size; i++)
            {
                b[i] = rand.NextDouble();
            }
        }

        public int get_guess()
        {
            int best = -1;
            double bestchance = 0.0;
            for (int i = 0; i < size; i++)
            {
                if (a[i] > bestchance)
                {
                    best = i;
                    bestchance = a[i];

                }

            }

            return best;
        }
    }
}
