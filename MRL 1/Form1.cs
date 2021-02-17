using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MRL_1
{
    public partial class Form1 : Form
    {
        static Random rand = new Random();

        double A = 0.9; // Скорость обучение (альфа)
        double D = 0.8; // Дисконт фактор
        double E = 0.5; // Эпсилон
        int iters = 80000; // Число итераций

        double[,,,,,,,,,,,] Q = new double[3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3];
        // Инициализируем Q-таблицу

        int[,] S = new int[3, 3]; // Состояние среды
        int[,] Si = new int[3, 3]; // Предыдущее состояние среды
        int[] Act = new int[3]; // Количество действий

        double R; // Награда

        // Обучение агента при запуске
        public Form1()
        {
            InitializeComponent();

            bool end;

            for (int i = 0; i < iters; i++)
            {
                S = new int[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } }; // Обнуляем среду при начале новой итерации
                end = false;

                for(int j = 0; j < 15; j++)
                {
                    for (int m = 0; m < 3; m++)
                    {
                        for (int n = 0; n < 3; n++)
                        {
                            Si[m, n] = S[m, n]; // Запоминаем прошлое состояние среды
                        }
                    }

                    if (rand.NextDouble() < E) // Метод эпсилон-жадности
                    {
                        Act[0] = rand.Next(3);
                        Act[1] = rand.Next(3);
                        Act[2] = rand.Next(3);
                    }
                    else
                        Act = ARGMAX(Q, S);


                    S[Act[0], Act[1]] = Act[2]; 
                    R = RCount(S);
                    // Делаем ход и получаем награду за него

                    Q[Si[0, 0], Si[0, 1], Si[0, 2], Si[1, 0], Si[1, 1], Si[1, 2], Si[2, 0], Si[2, 1], Si[2, 2], Act[0], Act[1], Act[2]] = (1 - A) * Q[Si[0, 0], Si[0, 1], Si[0, 2], Si[1, 0], Si[1, 1], Si[1, 2], Si[2, 0], Si[2, 1], Si[2, 2], Act[0], Act[1], Act[2]] + A * (R + D * MAX(Q, S));
                    // Обновляем Q-таблицу с помощью уравнения Беллмана

                    if (end)
                        break;
                }
            }

            S = new int[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
        }

        static double RCount(int[,] S)
        {
            double Rs = 0;
            bool earned;

            for(int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (S[i, j] != 1)
                        continue;

                    earned = false;

                    if (!earned && i > 0 && S[i - 1, j] == 2)
                        earned = true;
                    if (!earned && j > 0 && S[i, j - 1] == 2)
                        earned = true;
                    if (!earned && i < 2 && S[i + 1, j] == 2)
                        earned = true;
                    if (!earned && j < 2 && S[i, j + 1] == 2)
                        earned = true;

                    if (earned)
                        Rs++;

                    // Получем награду за каждую обрабатываемую "шахту"
                }
            }

            return Rs;
        }

        static double MAX(double[,,,,,,,,,,,] Q, int[,] S)
        {
            int[] m = new int[3];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (Q[S[0, 0], S[0, 1], S[0, 2], S[1, 0], S[1, 1], S[1, 2], S[2, 0], S[2, 1], S[2, 2], i, j, k] > Q[S[0, 0], S[0, 1], S[0, 2], S[1, 0], S[1, 1], S[1, 2], S[2, 0], S[2, 1], S[2, 2], m[0], m[1], m[2]])
                        {
                            m[0] = i;
                            m[1] = j;
                            m[2] = k;
                        }
                    }
                }
            }

            return Q[S[0, 0], S[0, 1], S[0, 2], S[1, 0], S[1, 1], S[1, 2], S[2, 0], S[2, 1], S[2, 2], m[0], m[1], m[2]];
        }

        static int[] ARGMAX(double[,,,,,,,,,,,] Q, int[,] S)
        {
            int[] m = new int[3];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (Q[S[0, 0], S[0, 1], S[0, 2], S[1, 0], S[1, 1], S[1, 2], S[2, 0], S[2, 1], S[2, 2], i, j, k] > Q[S[0, 0], S[0, 1], S[0, 2], S[1, 0], S[1, 1], S[1, 2], S[2, 0], S[2, 1], S[2, 2], m[0], m[1], m[2]])
                        {
                            m[0] = i;
                            m[1] = j;
                            m[2] = k;
                        }
                    }
                }
            }

            return m;
        }

        // Вывод получившегося результата

        private void button1_Click(object sender, EventArgs e)
        {
            bool end = false;

            S = new int[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };

            for (int j = 0; j < 15; j++)
            {
                Act = ARGMAX(Q, S);

                S[Act[0], Act[1]] = Act[2];
                Invalidate();

                if (end)
                    break;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // O — "шахта", X — "завод"

            if (S[0, 0] == 0)
                textBox1.Text = " ";
            else if (S[0, 0] == 1)
                textBox1.Text = "O";
            else
                textBox1.Text = "X";
            if (S[0, 1] == 0)
                textBox2.Text = " ";
            else if (S[0, 1] == 1)
                textBox2.Text = "O";
            else
                textBox2.Text = "X";
            if (S[0, 2] == 0)
                textBox3.Text = " ";
            else if (S[0, 2] == 1)
                textBox3.Text = "O";
            else
                textBox3.Text = "X";
            if (S[1, 0] == 0)
                textBox4.Text = " ";
            else if (S[1, 0] == 1)
                textBox4.Text = "O";
            else
                textBox4.Text = "X";
            if (S[1, 1] == 0)
                textBox5.Text = " ";
            else if (S[1, 1] == 1)
                textBox5.Text = "O";
            else
                textBox5.Text = "X";
            if (S[1, 2] == 0)
                textBox6.Text = " ";
            else if (S[1, 2] == 1)
                textBox6.Text = "O";
            else
                textBox6.Text = "X";
            if (S[2, 0] == 0)
                textBox7.Text = " ";
            else if (S[2, 0] == 1)
                textBox7.Text = "O";
            else
                textBox7.Text = "X";
            if (S[2, 1] == 0)
                textBox8.Text = " ";
            else if (S[2, 1] == 1)
                textBox8.Text = "O";
            else
                textBox8.Text = "X";
            if (S[2, 2] == 0)
                textBox9.Text = " ";
            else if (S[2, 2] == 1)
                textBox9.Text = "O";
            else
                textBox9.Text = "X";
        }
    }

}
