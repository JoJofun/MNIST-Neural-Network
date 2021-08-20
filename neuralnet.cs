using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MNISTForms
{
    class neuralnet
    {
        public int numLayers = 2;

        static Random rand = new Random();
        public inputlayer first = new inputlayer(784);
        public outputlayer last = new outputlayer(10, layer.Activation_Type.sigmoid, rand);

        List<layer> layerlist = new List<layer>();
        List<weightmatrix> matrixlist = new List<weightmatrix>();

        public int truth;//truth value -1 for none, 0-9 for, well 0-9

        public double lr = 0.01; //learning rate

        public bool trained = false;

        public int correct;

        public string getLayerList()
        {
            string toreturn = "";

            toreturn += "Input layer: 784" + System.Environment.NewLine;
            for (int i = 1; i < numLayers - 1; i++)
            {
                toreturn += String.Format("Layer {0,3}: {1,5}", i, layerlist[i].size);
                if (layerlist[i].type == layer.Activation_Type.tanh)
                {
                    toreturn += " type: TanH" + System.Environment.NewLine;
                }
                else if (layerlist[i].type == layer.Activation_Type.sigmoid)
                {
                    toreturn += " type: Sigmoid" + System.Environment.NewLine;
                }
                else if (layerlist[i].type == layer.Activation_Type.softmax)
                {
                    toreturn += " type: SoftMax" + System.Environment.NewLine;
                }
                else
                {
                    toreturn += " type: ReLU" + System.Environment.NewLine;
                }
            }

            toreturn += "Output layer: 10";
            {
                if (last.type == layer.Activation_Type.tanh)
                {
                    toreturn += " type: TanH" + System.Environment.NewLine;
                }
                else if (last.type == layer.Activation_Type.sigmoid)
                {
                    toreturn += " type: Sigmoid" + System.Environment.NewLine;
                }
                else if (last.type == layer.Activation_Type.softmax)
                {
                    toreturn += " type: SoftMax" + System.Environment.NewLine;
                }
                else
                {
                    toreturn += " type: ReLU" + System.Environment.NewLine;
                }
            }
            return toreturn;
        }

        public void autoBF(bool training)
        {

            AutoFProp();

            int best = -1;
            double bestdouble = double.MinValue;
            for (int i = 0; i < last.size; i++)
            {
                if (last.a[i] > bestdouble)
                {
                    bestdouble = last.a[i];
                    best = i;
                }
            }

            if (best == truth)
            {
                correct++;
            }

            if (training)
            {
                AutoBProp();
            }
        }

        public void xortest()//this is a test to see if my autoFprop works correctly
        {
            /*
            matrixlist[0].w[0, 0] = -1.54505;
            matrixlist[0].w[0, 1] = 1.93002;
            matrixlist[0].w[1, 0] = -1.54486;
            matrixlist[0].w[1, 1] = 1.92576;

            layerlist[1].b[0] = 2.26306;
            layerlist[1].b[1] = -0.652532;

            matrixlist[1].w[0, 0] = 1.91479;
            matrixlist[1].w[1, 0] = 2.02119;

            last.b[0] = -0.713742;
            */

            var rand = new Random();

            correct = 0;

            for (int i = 0; i < 20000; i++)
            {

                first.a[0] = rand.Next() % 2;
                first.a[1] = rand.Next() % 2;

                if (first.a[0] == first.a[1])
                {
                    truth = 0;
                }
                else
                {
                    truth = 1;
                }

                AutoFProp();
                //Console.WriteLine( i + " X0: " + first.a[0] + " X1: " + first.a[1] + " truth: " + truth + " prediction: " + last.a[0]);
                AutoBProp();

                if (Convert.ToInt32(last.a[0] + 0.1) == truth)
                {
                    correct++;
                }
            }
            //Console.WriteLine(correct + "/20000");

        }

        public void txtinput(string input)
        {
            if (!System.IO.File.Exists(input))
            {
                return;
            }
            System.IO.StreamReader file = new System.IO.StreamReader(input);

            string[] csvinfo;
            int lairs = 0;
            string sizes;

            if ((sizes = file.ReadLine()) != null)
            {
                csvinfo = sizes.Split(',');

                lairs = Int32.Parse(csvinfo[0]);


                layer.Activation_Type tipe;

                while (numLayers > 2)
                {
                    removeLayer();
                }

                for (int i = 1; i < lairs * 2 - 1; i += 2)
                {
                    string temp = csvinfo[i + 1];
                    if (temp == " sigmoid")
                    {
                        tipe = layer.Activation_Type.sigmoid;
                    }
                    else if (temp == " softmax")
                    {
                        tipe = layer.Activation_Type.softmax;
                    }
                    else if (temp == " tanh")
                    {
                        tipe = layer.Activation_Type.tanh;
                    }
                    else
                    {
                        tipe = layer.Activation_Type.relu;
                    }
                    int S = Int32.Parse(csvinfo[i]);
                    if (S != 0)
                    {
                        addLayer(S, tipe);
                    }
                    else
                    {
                        last.type = tipe;
                    }
                }


                //store weights and biases here
                string info;
                for (int i = 1; i < lairs; i++)
                {
                    info = file.ReadLine();
                    csvinfo = info.Split(',');
                    for (int j = 0; j < layerlist[i - 1].getSize(); j++)
                    {
                        for (int k = 0; k < layerlist[i].getSize(); k++)
                        {
                            //Console.WriteLine(j * layerlist[i].getSize() + k);
                            matrixlist[i - 1].w[j, k] = Convert.ToDouble(csvinfo[j * layerlist[i].getSize() + k]);
                        }
                    }

                    info = file.ReadLine();
                    csvinfo = info.Split(',');
                    int m = layerlist[i].getSize();
                    for (int j = 0; j < m; j++)
                    {
                        //Console.WriteLine(j + " and " + csvinfo.Length);
                        layerlist[i].b[j] = Convert.ToDouble(csvinfo[j]);
                    }
                }
            }

            file.Close();
        }

        public void txtoutput(string o)
        {
            if (System.IO.File.Exists(@Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName + "\\" + o))
            {
                System.IO.File.Delete(@Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName + "\\" + o);
            }
            StreamWriter file = new StreamWriter(@Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName + "\\" + o, append: true);

            string output = "";
            output += numLayers.ToString();

            for (int i = 1; i < numLayers - 1; i++)
            {
                output += ", " + layerlist[i].getSize().ToString();
                output += ", " + layerlist[i].type.ToString();
            }
            output += ", 0, " + last.type.ToString();
            file.WriteLine(output);
            for (int k = 0; k < numLayers - 1; k++)
            {
                output = "";
                for (int i = 0; i < layerlist[k].getSize(); i++)
                {
                    for (int j = 0; j < layerlist[k + 1].getSize(); j++)
                    {
                        output += matrixlist[k].w[i, j].ToString() + ", ";
                    }
                }
                output.Remove(output.Length - 2, 2);
                file.WriteLine(output);
                output = "";
                for (int j = 0; j < layerlist[k + 1].getSize(); j++)
                {
                    output += layerlist[k + 1].b[j].ToString() + ", ";
                }

                output.Remove(output.Length - 2, 2);
                file.WriteLine(output);
            }
            file.Close();

        }

        //constructor
        public neuralnet()
        {
            layerlist.Add(first);
            layerlist.Add(last);
            addLayer(10, layer.Activation_Type.sigmoid);
            //second = layerlist[1];
        }

        //function for adding a layer
        public void addLayer(int size, layer.Activation_Type tipe)//adds layer right before the end
        {
            int prevsyze = layerlist[numLayers - 2].getSize();//get size of layer before the one being inserted
            layerlist.Insert(numLayers - 1, new hiddenlayer(size, tipe, rand));
            int nextsyze = layerlist[numLayers].getSize();
            if (numLayers != 2)
            {
                matrixlist.RemoveAt(numLayers - 2);
            }
            else if (numLayers == 2)
            {
                matrixlist.Clear();
            }
            matrixlist.Add(new weightmatrix(prevsyze, size, rand));
            matrixlist.Add(new weightmatrix(size, nextsyze, rand));
            numLayers++;
        }


        //functions for removing a layer
        private bool removeLayer(int index)//removes the layer at specified index
        {
            if (index == 0 || index == numLayers - 1 || numLayers <= 2)
            {
                return false;
            }

            matrixlist.RemoveAt(index);
            matrixlist.RemoveAt(index - 1);
            layerlist.RemoveAt(index);

            int prevsyze = layerlist[index - 1].getSize();
            int nextsyze = layerlist[index].getSize();
            matrixlist.Insert(index - 1, new weightmatrix(prevsyze, nextsyze, rand));
            numLayers--;

            return true;

        }

        public bool removeLayer()
        {
            return removeLayer(numLayers - 2);
        }

        //forwars propogation
        public void AutoFProp()
        {
            for (int i = 0; i < numLayers - 1; i++)
            {
                //Console.WriteLine(i);
                FPropLayer(i);
            }
            //outputs();
        }
        private void FPropLayer(int lair)//layer should be the lower layer, ex: layer 0 x matrix 0 + bias 1 activated = layer
        {
            int nextsize = layerlist[lair + 1].getSize();
            int prevsize = layerlist[lair].getSize();

            //multiplication
            for (int i = 0; i < nextsize; i++)
            {
                //Console.WriteLine("I: " + i + " Nextsize: " + nextsize);
                double sum = 0.0;
                for (int j = 0; j < prevsize; j++)
                {
                    /*
                    Console.WriteLine(layerlist[lair].getSize());
                    Console.WriteLine(prevsize);
                    Console.WriteLine(layerlist[lair+ 1].getSize());
                    Console.WriteLine(nextsize);
                    Console.WriteLine("I: " + i + " J " + j);
                    Console.WriteLine(matrixlist.Count);*/

                    sum += layerlist[lair].a[j] * matrixlist[lair].w[j, i];
                }
                sum += layerlist[lair + 1].b[i];
                layerlist[lair + 1].s[i] = sum;
                //Console.WriteLine("Layer: " + lair + 1 + " i " + i + ": " + layerlist[lair + 1].s[i]);
            }
            layerlist[lair + 1].Activate();
        }



        //back propogation
        public void AutoBProp()
        {
            double[] t = new double[10];
            t[truth] = 1.0;
            //t[0] = truth;

            //Console.WriteLine(print_guess());

            //the back layer is special because its next to the truth
            for (int i = 0; i < last.size; i++)
            {

                double dt = last.a[i] - t[i];

                //sigmoid bprop
                if (last.type == layer.Activation_Type.sigmoid)
                {
                    //sigmoid integration
                    last.b0[i] = (1.0 - last.a[i]) * last.a[i] * dt;
                    last.b[i] = last.b[i] - lr * last.b0[i];
                }

                //tanh bprob
                if (last.type == layer.Activation_Type.tanh)
                {

                    //tanh integration
                    last.b0[i] = (1.0 - last.a[i] * last.a[i]) * dt;
                    last.b[i] = last.b[i] - (lr * last.b0[i]);

                }

                //relu bprop
                if (last.type == layer.Activation_Type.relu)
                {
                    //relu integration
                    if (dt > 0)
                    {
                        last.b0[i] = 1;
                    }
                    else
                    {
                        last.b0[i] = 0;
                    }
                    last.b[i] = last.b[i] - lr * last.b0[i];
                }



                //softmax bprop
                //not working so commenting out
                /*
                if (last.type == layer.Activation_Type.softmax)
                {
                        double sum = (last.a[i] - t[i]) * (1 - last.a[i]);
                        for(int j = 0; j < last.size; j++)
                        {
                            sum -= last.a[j] * (last.a[j] - t[j]);
                        }
                        sum *= last.a[i];
                        last.b0[i] = sum;
                        last.b[i] = last.b[i] - lr * last.b0[i];

                }
                */

                //Console.WriteLine(print_guess());
            }

            for (int i = numLayers - 2; i >= 0; i--)
            {
                BPropLayer(i);
            }


        }

        private void BPropLayer(int lair)
        {
            //layer thats going to be adjusted
            layer mylayer = layerlist[lair];

            //matrix above lair layer
            layer nextlayer = layerlist[lair + 1];

            //matrix thats going to be adjusted
            weightmatrix mymatrix = matrixlist[lair];


            for (int i = 0; i < mylayer.size; i++)
            {
                for (int j = 0; j < nextlayer.size; j++)
                {
                    mymatrix.w0[i, j] = mylayer.a[i] * nextlayer.b0[j];
                    mymatrix.w[i, j] = mymatrix.w[i, j] - (lr * mymatrix.w0[i, j]);
                    //Console.WriteLine(mymatrix.w0[i, j]);
                }
            }


            //lair layer
            if (lair != 0)
            {
                for (int i = 0; i < mylayer.size; i++)
                {

                    double sum = 0.0;
                    for (int j = 0; j < nextlayer.size; j++)
                    {
                        sum += mymatrix.w[i, j] * nextlayer.b0[j];

                    }

                    //sigmoid bprop
                    if (mylayer.type == layer.Activation_Type.sigmoid)
                    {
                        //sigmoid integration
                        mylayer.b0[i] = (1.0 - mylayer.a[i]) * mylayer.a[i] * sum;
                        mylayer.b[i] = mylayer.b[i] - lr * mylayer.b0[i];
                    }
                    //tanh bprob
                    else if (mylayer.type == layer.Activation_Type.tanh)
                    {

                        //tanh integration
                        mylayer.b0[i] = (1.0 - mylayer.a[i] * mylayer.a[i]) * sum;
                        mylayer.b[i] = mylayer.b[i] - lr * mylayer.b0[i];

                    }

                    //relu bprop
                    else if (mylayer.type == layer.Activation_Type.relu)
                    {
                        //relu integration
                        if (sum > 0)
                        {
                            mylayer.b0[i] = 1;
                        }
                        else
                        {
                            mylayer.b0[i] = 0;
                        }
                        mylayer.b[i] = mylayer.b[i] - lr * mylayer.b0[i];
                    }
                    /*

                    //softmax bprop
                    else if (mylayer.type == layer.Activation_Type.softmax)
                    {
                        mylayer.b0[i] = (1.0 - mylayer.a[i]) * mylayer.a[i] * sum;
                        mylayer.b[i] = mylayer.b[i] - lr * mylayer.b0[i];

                    }
                    */
                }

            }
        }




        public void readubyte(string imagepath, string labelpath, int lineno)
        {

            //Console.WriteLine(imagepath);
            FileStream input = File.OpenRead(@imagepath);



            input.Seek(16+ (784 * lineno), SeekOrigin.Begin);


            var buffer = new byte[784];
            input.Read(buffer, 0, 784);
            input.Close();


            FileStream labelinput = File.OpenRead(labelpath);
            labelinput.Seek(8 + lineno, SeekOrigin.Begin);
            truth = labelinput.ReadByte();
            labelinput.Close();

            for (int i = 0; i < 784; i++)
            {
                first.a[i] = buffer[i] / 255.0;
            }
        }




        //reading csv from file
        public void readcsv(System.IO.StreamReader file)//reads the csv for the line no
        {
            string line;
            if ((line = file.ReadLine()) != null)
            {
                //Console.WriteLine(line);
                //Console.WriteLine(line);
                string[] csvlist = line.Split(',');
                while (csvlist[0] == "label")
                {
                    line = file.ReadLine();

                    csvlist = line.Split(',');
                }
                // Console.WriteLine(Int32.Parse(csvlist[0]));


                truth = Int32.Parse(csvlist[0]);


                for (int i = 0; i < 784; i++)
                {
                    first.a[i] = ((Int32.Parse(csvlist[i + 1])) / 255.0);
                }

                //Console.WriteLine(truth);
            }


        }

        //reading from an image
        public bool read_image(string fylename)
        {
            if (!File.Exists(@fylename))
            {
                return false;
            }

            Bitmap input = new Bitmap(@fylename);
            if (input.Width != 28 || input.Height != 28)
            {
                return false;
            }

            for (int i = 0; i < 28; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    Color pixel = input.GetPixel(i, j);
                    int grey = (pixel.R + pixel.G + pixel.B) / 3;//it should already be greyscale but  im doing htis just in case
                    first.a[(i + 28 * j)] = ((grey)) / 255.0;
                }
            }
            truth = -1;
            return true;
        }





        //testing functions

        //prints the outputs
        public void outputs()
        {
            double[] t = new double[10];
            t[truth] = 1.0;
            Console.WriteLine("Truth: " + truth);
            double sum = 0.0;
            for (int i = 0; i < 10; i++)
            {
                sum += (last.a[i] - 1) * (last.a[i] - 1);
                Console.WriteLine(i + "  value: " + last.a[i]);
            }
            Console.WriteLine("Error: " + (sum / 2));
        }

        //prints the image in ascii art
        public void testpritn()
        {
            for (int i = 0; i < 28; i++)
            {
                string temp = "";
                for (int j = 0; j < 28; j++)
                {
                    if (first.a[i * 28 + j] != 0)
                    {
                        temp += '#';
                    }
                    else
                    {
                        temp += ' ';
                    }
                }
                Console.WriteLine(temp);
            }
        }


        //output text box funct
        public string get_guess()
        {
            string temp = "";

            int best = -1;
            double bestdouble = double.MinValue;
            for (int i = 0; i < last.size; i++)
            {
                if (last.a[i] > bestdouble)
                {
                    bestdouble = last.a[i];
                    best = i;
                }
            }



            if (truth != -1)
            {

                temp += "Truth: " + truth.ToString() + System.Environment.NewLine;
            }

            temp += "Guess: " + best.ToString() + System.Environment.NewLine;

            if (best == truth)
            {
                correct++;
                temp += "Correct! " + System.Environment.NewLine;
            }

            for (int i = 0; i < 10; i++)
            {
                temp += i + "  value: " + last.a[i] + System.Environment.NewLine;
            }
            return temp;
        }
        public int get_int_guess()
        {

            int best = -1;
            double bestdouble = double.MinValue;
            for (int i = 0; i < last.size; i++)
            {
                if (last.a[i] > bestdouble)
                {
                    bestdouble = last.a[i];
                    best = i;
                }
            }
            //Console.WriteLine(best);
            return best;
        }
    }
}
