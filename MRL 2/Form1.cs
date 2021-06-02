using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MRL_2
{
    // Здесь представлена вторая версия программы

    public partial class Form1 : Form
    {
        static Random rand = new Random();

        int iters = 100000; // Число итераций
        int m = 3; // Ширина
        int n = 3; // Высота
        int acts = 3; // Кол-во возможных объектов (включая пустое поле)

        int[,] S;

        bool[,] Allowed;
        bool[,] Restricted;

        Image FirstF = Image.FromFile("FirstFactory.bmp");
        Image SecondF = Image.FromFile("SecondFactory.bmp");
        Image ThirdF = Image.FromFile("ThirdFactory.bmp");
        Image Conveyor = Image.FromFile("Conveyor.bmp");

        bool[] Rules = new bool[2];

        public Form1()
        {
            InitializeComponent();

            DoubleBuffered = true;

            S = new int[m, n];

            Rules[0] = false;
            Rules[1] = false;

            Allowed = new bool[m, n];
            Restricted = new bool[m, n];
        }

        private void обучитьToolStripMenuItem_Click(object sender, EventArgs e) // Создание нейросети и ее обучение
        {
            S = new int[m, n];

            NeuralNetwork NN = new NeuralNetwork(m, n, acts, Rules, Allowed, Restricted);

            //NN.Train(iters);

            NN.Result();

            //MessageBox.Show("Обучение завершено.", "Обучение", MessageBoxButtons.OK);

            S = NN.Result();

            Invalidate();
        }

        private void размерПоляToolStripMenuItem_Click(object sender, EventArgs e) // Окно изменения размеров поля
        {
            Size SZ = new Size(m, n);

            SZ.ShowDialog();

            if (SZ.DialogResult == DialogResult.OK)
            {
                m = SZ.GetM;
                n = SZ.GetN;
                S = new int[m, n];
                Allowed = new bool[m, n];
                Restricted = new bool[m, n];
            }

            Invalidate();
        }

        private void правилаToolStripMenuItem_Click(object sender, EventArgs e) // Окно выбора параметров обучения
        {
            RulesForm RForm = new RulesForm(Rules);

            RForm.ShowDialog();

            if (RForm.DialogResult == DialogResult.OK)
            {
                Rules = RForm.GetR;

                if (!Rules[0] && !Rules[1])
                    acts = 3;
                else if (!Rules[0] && Rules[1])
                    acts = 4;
                else if (Rules[0] && !Rules[1])
                    acts = 4;
                else if (Rules[0] && Rules[1])
                    acts = 5;

                S = new int[m, n];
            }

            Invalidate();
        }

        private void числоИтерацийToolStripMenuItem_Click(object sender, EventArgs e) // Окно изменения числа итераций
        {
            Iterations IForm = new Iterations(iters);

            IForm.ShowDialog();

            if (IForm.DialogResult == DialogResult.OK)
            {
                iters = IForm.GetITER;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e) // Прорисовка поля
        {
            int block;

            if ((Size.Width - 60) / m < (Size.Height - 83) / n)
                block = (Size.Width - 60) / m * 9 / 10;
            else
                block = ((Size.Height - 83) / n) * 9 / 10;

            if ((block - 12) <= 0)
                return;

            for (int i = 0; i <= n; i++)
            {
                e.Graphics.DrawLine(new Pen(Color.Black, 6), new Point(30, 53 + i * block), new Point(30 + m * block, 53 + i * block));
            }
            for (int i = 0; i <= m; i++)
            {
                e.Graphics.DrawLine(new Pen(Color.Black, 6), new Point(30 + i * block, 50), new Point(30 + i * block, 56 + n * block));
            }

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if(Restricted[i , j])
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.LightPink), 30 + 3 + i * block, 53 + 3 + j * block, block - 6, block - 6);
                    }
                    else if(Allowed[i, j])
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.LightGreen), 30 + 3 + i * block, 53 + 3 + j * block, block - 6, block - 6);
                    }
                }
            }


            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if(!Rules[0] && !Rules[1])
                    {
                        switch (S[i, j])
                        {
                            case 2:
                                e.Graphics.DrawImage(FirstF, 30 + 6 + i * block, 53 + 6 + j * block, block - 12, block - 12);
                                break;
                            case 1:
                                e.Graphics.DrawImage(SecondF, 30 + 6 + i * block, 53 + 6 + j * block, block - 12, block - 12);
                                break;
                        }
                    }

                    else if (Rules[0] && !Rules[1])
                    {
                        switch (S[i, j])
                        {
                            case 3:
                                e.Graphics.DrawImage(FirstF, 30 + 6 + i * block, 53 + 6 + j * block, block - 12, block - 12);
                                break;
                            case 2:
                                e.Graphics.DrawImage(SecondF, 30 + 6 + i * block, 53 + 6 + j * block, block - 12, block - 12);
                                break;
                            case 1:
                                e.Graphics.DrawImage(ThirdF, 30 + 6 + i * block, 53 + 6 + j * block, block - 12, block - 12);
                                break;
                        }
                    }

                    else if (!Rules[0] && Rules[1])
                    {
                        switch (S[i, j])
                        {
                            case 2:
                                e.Graphics.DrawImage(FirstF, 30 + 6 + i * block, 53 + 6 + j * block, block - 12, block - 12);
                                break;
                            case 1:
                                e.Graphics.DrawImage(SecondF, 30 + 6 + i * block, 53 + 6 + j * block, block - 12, block - 12);
                                break;
                            case 3:
                                e.Graphics.DrawImage(Conveyor, 30 + 6 + i * block, 53 + 6 + j * block, block - 12, block - 12);
                                break;
                        }
                    }

                    else if (Rules[0] && Rules[1])
                    {
                        switch (S[i, j])
                        {
                            case 3:
                                e.Graphics.DrawImage(FirstF, 30 + 6 + i * block, 53 + 6 + j * block, block - 12, block - 12);
                                break;
                            case 2:
                                e.Graphics.DrawImage(SecondF, 30 + 6 + i * block, 53 + 6 + j * block, block - 12, block - 12);
                                break;
                            case 1:
                                e.Graphics.DrawImage(ThirdF, 30 + 6 + i * block, 53 + 6 + j * block, block - 12, block - 12);
                                break;
                            case 4:
                                e.Graphics.DrawImage(Conveyor, 30 + 6 + i * block, 53 + 6 + j * block, block - 12, block - 12);
                                break;
                        }
                    }
                }
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.X < 33 || e.Y < 56)
                return;

            int block;

            if ((Size.Width - 60) / m < (Size.Height - 83) / n)
                block = (Size.Width - 60) / m * 9 / 10;
            else
                block = ((Size.Height - 83) / n) * 9 / 10;

            if ((block - 12) <= 0)
                return;

            int i = (e.X - 33) / block;
            int j = (e.Y - 56) / block;

            if (i < 0 || i >= m || j < 0 || j >= n)
                return;

            if(e.Button == MouseButtons.Left)
            {
                if (Allowed[i, j])
                    Allowed[i, j] = false;
                else
                {
                    Allowed[i, j] = true;
                    Restricted[i, j] = false;
                }
            }
            else if(e.Button == MouseButtons.Right)
            {
                if (Restricted[i, j])
                    Restricted[i, j] = false;
                else
                {
                    Restricted[i, j] = true;
                    Allowed[i, j] = false;
                }
            }

            Invalidate();
        }
    }
}
