using System;

namespace MNISTForms
{
    class hiddenlayer : layer
    {
        public hiddenlayer(int Size, layer.Activation_Type Type, Random rand)
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
                b[i] = rand.NextDouble() - 0.5;
            }
        }

        public hiddenlayer(layer tocopy)
        {
            size = tocopy.size;
            a = new double[size];
            b = new double[size];
            s = new double[size];
            b0 = new double[size];
            for (int i = 0; i < size; i++)
            {
                b[i] = tocopy.b[i];
            }
            type = tocopy.type;
        }
    }
}
