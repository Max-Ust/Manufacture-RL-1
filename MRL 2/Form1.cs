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
    // Здесь представлена вторая версия программы, которая на данный момент находится на стадии разработки

    public partial class Form1 : Form
    {
        static Random rand = new Random();

        int iters = 1000; // Число итераций
        int m = 2; // Ширина
        int n = 1; // Высота
        int acts = 3; // Кол-во возможных объектов (включая пустое поле)

        int[,] S = new int[2, 1];

        bool[] Rules = new bool[3]; //

        public Form1()
        {
            InitializeComponent();

            DoubleBuffered = true;
        }

        private void обучитьToolStripMenuItem_Click(object sender, EventArgs e) // Создание нейросети и ее обучение
        {
            S = new int[m, n];

            NeuralNetwork NN = new NeuralNetwork(m, n, acts);

            NN.Train(iters);

            /*for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    S[i, j] = rand.Next(3);
                }
            }*/

            // MessageBox.Show("Обучение завершено.", "Обучение", MessageBoxButtons.OK);

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
            }
        }

        private void числоИтерацийToolStripMenuItem_Click(object sender, EventArgs e) // Окно изменения числа итераций
        {
            Iterations IForm = new Iterations(iters);

            IForm.ShowDialog();

            if (IForm.DialogResult == DialogResult.OK)
            {
                iters = IForm.GetI;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e) // Прорисовка поля; O обозначает завод первого этапа обработки, X — второго
        {
            int block;

            if ((Size.Width - 60) / m < (Size.Height - 83) / n)
                block = (Size.Width - 60) / m * 9 / 10;
            else
                block = ((Size.Height - 83) / n) * 9 / 10;

            if ((block - 12) <= 0)
                return;

            for(int i = 0; i <= n; i++)
            {
                e.Graphics.DrawLine(new Pen(Color.Black, 6), new Point(30, 53 + i * block), new Point(30 + m * block, 53 + i * block));
            }
            for (int i = 0; i <= m; i++)
            {
                e.Graphics.DrawLine(new Pen(Color.Black, 6), new Point(30 + i * block, 50), new Point(30 + i * block, 56 + n * block));
            }

            string texticon = " ";

            for(int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    switch(S[i, j])
                    {
                        case 0:
                            texticon = " ";
                            break;
                        case 1:
                            texticon = "O";
                            break;
                        case 2:
                            texticon = "X";
                            break;
                    }
                    e.Graphics.DrawString(texticon, new Font("Arial", block - 12, GraphicsUnit.Pixel), new SolidBrush(Color.Black), 30 + 6 + i * block, 53 + 6 + j * block);
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
    }
}
