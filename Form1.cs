using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


namespace MNISTForms
{
    public partial class Form1 : Form
    {
        string fileinput = "";
        neuralnet MyNN;

        bool hiddenoptions = true;
        public Form1()
        {
            InitializeComponent();
            MyNN = new neuralnet();
            textBox2.Text = MyNN.getLayerList();
            comboBox1.SelectedIndex = 1;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 1;
            comboBox4.SelectedIndex = 2;
            numericUpDown3.Value = (decimal)MyNN.lr;


            //hide buttons
            hiddenoptions = !hiddenoptions;
            numericUpDown2.Visible = hiddenoptions;
            numericUpDown3.Visible = hiddenoptions;
            comboBox3.Visible = hiddenoptions;
            comboBox1.Visible = hiddenoptions;
            button2.Visible = hiddenoptions;
            button4.Visible = hiddenoptions;
            button5.Visible = hiddenoptions;
            button6.Visible = hiddenoptions;
            button7.Visible = hiddenoptions;
            label2.Visible = hiddenoptions;
            label3.Visible = hiddenoptions;
            textBox3.Visible = hiddenoptions;

            //load up trained values
            fileinput = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName + "\\19.csv";
            if (File.Exists(@fileinput))//if file exists
            {
                MyNN.txtinput(fileinput);

                MyNN.trained = true;

                //write success message here
                textBox2.Text = MyNN.getLayerList();
                textBox2.Update();

                textBox2.Refresh();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            //open menu to select file
            openFileDialog1.ShowDialog();
            fileinput = openFileDialog1.FileName;
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }

            if (File.Exists(@fileinput))//if file exists
            {
                //print image
                pictureBox1.Image = Image.FromFile(@fileinput);

                //read the image then propogate
                MyNN.read_image(fileinput);
                MyNN.AutoFProp();

                textBox2.Text = MyNN.get_guess();
                Console.WriteLine(MyNN.get_guess());
                textBox2.Update();

                textBox2.Refresh();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //MyNN.xortest();
            //MyNN.txtoutput();

            layer.Activation_Type tipe;
            if (comboBox3.SelectedIndex == 0)
            {
                tipe = layer.Activation_Type.tanh;
            }
            else if (comboBox3.SelectedIndex == 1)
            {
                tipe = layer.Activation_Type.sigmoid;
            }
            else
            {
                tipe = layer.Activation_Type.relu;
            }
            /*else
            {
                tipe = layer.Activation_Type.softmax;
            }*/
            MyNN.last.type = tipe;
            MyNN.trained = false;
            textBox2.Text = MyNN.getLayerList();
            textBox2.Update();

            textBox2.Refresh();

            warning();

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            string dir = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName;
            for (int j = 0; j < numericUpDown1.Value; j++)
            {
                textBox2.Text += "Running Epoch " + (j + 1).ToString();
                textBox2.Update();

                textBox2.Refresh();

                int epochL;

                string filenaym;
                string filenaym2 = "";
                if (comboBox2.SelectedIndex == 0)
                {
                    epochL = 60000;
                    filenaym = "\\mnist_train.csv";
                }
                else if(comboBox2.SelectedIndex == 1)
                {
                    epochL = 10000;
                    filenaym = "\\mnist_test.csv";

                }
                else if(comboBox2.SelectedIndex == 2)
                {
                    epochL = 60000;
                    filenaym = (dir + "\\train-images.idx3-ubyte");
                    filenaym2 = (dir + "\\train-labels.idx1-ubyte");

                }
                else if(comboBox2.SelectedIndex == 3)
                {
                    epochL = 10000;
                    filenaym = dir + "\\t10k-images.idx3-ubyte";
                    filenaym2 = dir + "\\t10k-labels.idx1-ubyte";

                }
                else 
                {
                    epochL = 50;
                    filenaym = dir + "\\images\\image";

                }
                progressBar1.Maximum = epochL;

                Console.WriteLine(comboBox2.SelectedIndex);

                //FOR CSV reading
                if (comboBox2.SelectedIndex == 0 || comboBox2.SelectedIndex == 1)
                {

                    System.IO.StreamReader file = new System.IO.StreamReader(@Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName + filenaym);
                    progressBar1.Value = 0;
                    MyNN.correct = 0;
                    for (int i = 0; i < epochL; i++)
                    {
                        //inputlayer x = new inputlayer(784);
                        //Console.WriteLine("Test");
                        MyNN.readcsv(file);
                        //MyNN.testpritn();
                        //Console.WriteLine("iteration: " + i);

                        /*
                        Thread t = new Thread(MyNN.autoBF);
                        t.Start();
                        t.Join();
                        */

                        MyNN.autoBF(checkBox1.Checked);
                        progressBar1.Increment(1);
                        progressBar1.Update();
                    }

                }

                else if (comboBox2.SelectedIndex == 2 || comboBox2.SelectedIndex == 3)//basic mnist reading
                {
                    MyNN.correct = 0;
                    progressBar1.Value = 0;
                    for (int l = 0; l < epochL; l++)
                    {
                        //Console.WriteLine(l);
                        MyNN.readubyte(filenaym, filenaym2, l);
                        MyNN.autoBF(checkBox1.Checked);
                        progressBar1.Increment(1);
                        progressBar1.Update();
                    }
                }

                else{//nicks dataset
                    MyNN.correct = 0;
                    int[] truths = new int[] { 0, 2, 4, 7, 8, 2, 7, 9, 2, 4, 7, 6, 8, 3, 0, 5, 2, 3, 6, 3, 5, 4, 8, 1, 3, 6, 0, 8, 3, 8, 5, 8, 1, 9, 5, 9, 4, 2, 6, 1, 4, 1, 4, 6, 2, 1, 0, 8, 3, 7 };

                    int[] guesses = new int[50];

                    MyNN.correct = 0;



                    progressBar1.Value = 0;
                    progressBar1.Maximum = 50;
                    for (int i = 0; i < 50; i++)
                    {

                        string fyle = filenaym + i.ToString() + ".png";
                        if (File.Exists(@fyle))//if file exists
                        {
                            Console.WriteLine(fyle);

                            //print image
                            //pictureBox1.Image = Image.FromFile(@fyle);

                            //read the image then propogate
                            MyNN.read_image(fyle);
                            MyNN.AutoFProp();
                            guesses[i] = MyNN.get_int_guess();
                            if (truths[i] == guesses[i])
                            {
                                MyNN.correct++;
                            }


                            progressBar1.Increment(1);
                            progressBar1.Update();

                            textBox2.Update();

                            textBox2.Refresh();
                        }

                    }

                }


                chart1.Series[comboBox2.SelectedIndex].Points.Add(MyNN.correct / (double)epochL);
                textBox2.Text = "Epoch " + (j + 1) + ' ';
                textBox2.Text += MyNN.correct.ToString() + '/' + epochL.ToString() + " Correct! " + (MyNN.correct / ((double)epochL) * 100.0).ToString() + '%' + System.Environment.NewLine;
                chart1.Update();
            }

            if (checkBox1.Checked)
            {
                MyNN.trained = true;
                textBox2.Text += "Done training!";
            }

            textBox2.Update();

            textBox2.Refresh();
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MyNN.txtoutput(textBox3.Text);
            textBox2.Text = "weights and biases exported to" + System.Environment.NewLine + @Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName + "\\" + textBox3.Text;
        }

        private void button5_Click(object sender, EventArgs e)
        {

            MyNN.trained = false;
            layer.Activation_Type tipe;
            if (comboBox1.SelectedIndex == 0)
            {
                //Console.WriteLine("tanh");
                tipe = layer.Activation_Type.tanh;
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                //Console.WriteLine("sig");
                tipe = layer.Activation_Type.sigmoid;
            }
            else
            {
                tipe = layer.Activation_Type.relu;
            }
            /*else
            {
                tipe = layer.Activation_Type.softmax;
            }*/
            MyNN.addLayer(Convert.ToInt32(numericUpDown2.Value), tipe);
            textBox2.Text = MyNN.getLayerList();
            textBox2.Update();

            textBox2.Refresh();

            warning();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void warning()
        {
            string toreturn = "";
            if (!MyNN.trained)
            {
                toreturn += "Neural Network is untrained, please train or load a pre-trained set" + System.Environment.NewLine;
            }
            textBox2.Text = toreturn;
            chart1.Series["MNIST_TRAIN"].Points.Clear();
            chart1.Series["MNIST_TEST"].Points.Clear();
            textBox2.Update();

            textBox2.Refresh();
        }

        private void button6_Click(object sender, EventArgs e)
        {

            MyNN.trained = false;
            MyNN.removeLayer();
            textBox2.Text = MyNN.getLayerList();
            textBox2.Update();

            textBox2.Refresh();
            warning();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //open menu to select file
            openFileDialog2.ShowDialog();
            fileinput = openFileDialog2.FileName;
            if (File.Exists(@fileinput))//if file exists
            {
                MyNN.txtinput(fileinput);
                
                MyNN.trained = true;

                //write success message here
                textBox2.Text = MyNN.getLayerList();
                textBox2.Update();

                textBox2.Refresh();
            }
            if (!MyNN.trained)
            {
                warning();
            }

        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            MyNN.lr = (double)numericUpDown3.Value;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            hiddenoptions = !hiddenoptions;
            numericUpDown2.Visible = hiddenoptions;
            numericUpDown3.Visible = hiddenoptions;
            comboBox3.Visible = hiddenoptions;
            comboBox1.Visible = hiddenoptions;
            button2.Visible = hiddenoptions;
            button4.Visible = hiddenoptions;
            button5.Visible = hiddenoptions;
            button6.Visible = hiddenoptions;
            button7.Visible = hiddenoptions;
            label2.Visible = hiddenoptions;
            label3.Visible = hiddenoptions;
            textBox3.Visible = hiddenoptions;
        }

        private void button9_Click(object sender, EventArgs e)
        {

            string dir = @Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName;
            string test = (dir + "\\train-images.idx3-ubyte");
            string label = (dir + "\\train-labels.idx3-ubyte");
            MyNN.readubyte(test, dir, 7);
            Bitmap draw = new Bitmap(28,28);
            for (int i = 0; i < 28; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    int grey = (int)(MyNN.first.a[j * 28 + i] * 255);
                    draw.SetPixel(i, j, Color.FromArgb(grey,grey,grey));
                }
            }

            pictureBox1.Image = draw;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox4.SelectedIndex == 1 || comboBox4.SelectedIndex == 3)
            {
                numericUpDown4.Maximum = 10000;
            }
            else if (comboBox4.SelectedIndex == 0 || comboBox4.SelectedIndex == 2)
            {
                numericUpDown4.Maximum = 60000;
            }
            else
            {
                numericUpDown4.Maximum = 50;

            }

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {

            textBox2.Text = "";
            string dir = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName;
            int epochL;

            string filenaym;
            string filenaym2 = "";
            if (comboBox4.SelectedIndex == 0)
            {
                filenaym = "\\mnist_train.csv";

                System.IO.StreamReader file = new System.IO.StreamReader(@Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName + filenaym);

                for (int i = 0; i < numericUpDown4.Value; i++)
                {
                    file.ReadLine();
                }
                MyNN.readcsv(file);
                MyNN.AutoFProp();
                Bitmap draw = new Bitmap(28, 28);
                for (int i = 0; i < 28; i++)
                {
                    for (int j = 0; j < 28; j++)
                    {
                        int grey = (int)(MyNN.first.a[j * 28 + i] * 255);
                        draw.SetPixel(i, j, Color.FromArgb(grey, grey, grey));
                    }
                }

                pictureBox1.Image = draw;
                textBox2.Text = MyNN.get_guess();
                textBox2.Update();

                textBox2.Refresh();
            }
            else if (comboBox4.SelectedIndex == 1)
            {
                filenaym = "\\mnist_test.csv";

                System.IO.StreamReader file = new System.IO.StreamReader(@Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName + filenaym);

                for (int i = 0; i < numericUpDown4.Value; i++)
                {
                    file.ReadLine();
                }
                MyNN.readcsv(file);
                MyNN.AutoFProp();
                Bitmap draw = new Bitmap(28, 28);
                for (int i = 0; i < 28; i++)
                {
                    for (int j = 0; j < 28; j++)
                    {
                        int grey = (int)(MyNN.first.a[j * 28 + i] * 255);
                        draw.SetPixel(i, j, Color.FromArgb(grey, grey, grey));
                    }
                }

                pictureBox1.Image = draw;
                textBox2.Text = MyNN.get_guess();
                textBox2.Update();

                textBox2.Refresh();

            }
            else if (comboBox4.SelectedIndex == 2)
            {
                filenaym = (dir + "\\train-images.idx3-ubyte");
                filenaym2 = (dir + "\\train-labels.idx1-ubyte");

                MyNN.readubyte(@filenaym, @filenaym2, (int)numericUpDown4.Value - 1);
                Bitmap draw = new Bitmap(28, 28);
                for (int i = 0; i < 28; i++)
                {
                    for (int j = 0; j < 28; j++)
                    {
                        int grey = (int)(MyNN.first.a[j * 28 + i] * 255);
                        draw.SetPixel(i, j, Color.FromArgb(grey, grey, grey));
                    }
                }
                MyNN.AutoFProp();

                textBox2.Text = MyNN.get_guess();
                textBox2.Update();

                textBox2.Refresh();

                pictureBox1.Image = draw;

            }
            else if (comboBox4.SelectedIndex == 3)
            {
                    filenaym = dir + "\\t10k-images.idx3-ubyte";
                    filenaym2 = dir + "\\t10k-labels.idx1-ubyte";

                MyNN.readubyte(@filenaym, @filenaym2, (int)numericUpDown4.Value - 1);
                Bitmap draw = new Bitmap(28, 28);
                for (int i = 0; i < 28; i++)
                {
                    for (int j = 0; j < 28; j++)
                    {
                        int grey = (int)(MyNN.first.a[j * 28 + i] * 255);
                        draw.SetPixel(i, j, Color.FromArgb(grey, grey, grey));
                    }
                }
                MyNN.AutoFProp();

                textBox2.Text = MyNN.get_guess();
                textBox2.Update();

                textBox2.Refresh();
                pictureBox1.Image = draw;


            }
            else
            {
                filenaym = dir + "\\images\\image";
                int[] trr = new int[] { 0, 2, 4, 7, 8, 2, 7, 9, 2, 4, 7, 6, 8, 3, 0, 5, 2, 3, 6, 3, 5, 4, 8, 1, 3, 6, 0, 8, 3, 8, 5, 8, 1, 9, 5, 9, 4, 2, 6, 1, 4, 1, 4, 6, 2, 1, 0, 8, 3, 7 };

                Console.WriteLine(MyNN.truth);
                string fyle = filenaym + numericUpDown4.Value.ToString() + ".png";
                //Console.WriteLine(fyle);
                progressBar1.Increment(1);
                progressBar1.Update();
                if (File.Exists(@fyle))//if file exists
                {
                    //print image
                    pictureBox1.Image = Image.FromFile(@fyle);

                    //read the image then propogate
                    MyNN.read_image(@fyle);
                    MyNN.AutoFProp();


                    MyNN.AutoFProp();
                    MyNN.truth = trr[(int)numericUpDown4.Value];
                    textBox2.Text = MyNN.get_guess();
                    textBox2.Update();

                    textBox2.Refresh();
                }

            }
        }
    }
}
