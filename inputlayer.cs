namespace MNISTForms
{
    class inputlayer : layer
    {



        public inputlayer(int Size)
        {

            size = Size;
            a = new double[size];

        }



        //for testing
        //var rand = new Random();
        //only for testing
        //for(int i  = 0; i < size; i++)
        //{
        //    a[i] = rand.NextDouble() - 0.5;
        //}
    }
}
