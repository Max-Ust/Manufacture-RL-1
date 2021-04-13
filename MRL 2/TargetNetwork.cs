using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRL_2
{
    class TargetNetwork // Нейросеть цели
    {
        double[][,] W = new double[3][,];

        int M;
        int N;
        int ACTS;

        InpN[] I;

        Neuron[][] H = new Neuron[2][];
        int h;

        OutN[] O;

        static Random Rand = new Random();

        public TargetNetwork(double[][,] NewW, int m, int n, int acts, int newH)
        {
            h = newH;

            I = new InpN[(m * n)];
            O = new OutN[(m * n * acts)];
            H[0] = new Neuron[h];
            H[1] = new Neuron[h];

            for (int i = 0; i < m * n; i++)
            {
                I[i] = new InpN(Rand.NextDouble());
            }
            for (int i = 0; i < m * n * acts; i++)
            {
                O[i] = new OutN(Rand.NextDouble());
            }
            for (int i = 0; i < h; i++)
            {
                H[0][i] = new Neuron(Rand.NextDouble());
                H[1][i] = new Neuron(Rand.NextDouble());
            }

            W = NewW;

            M = m;
            N = n;
            ACTS = acts;
        }

        public double[][,] GetW
        {
            set
            {
                W = value;
            }
        }

        public double[] Calculate(int[,] State)
        {
            int ML = N * ACTS;
            int NL = ACTS;

            double sum = 0;

            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    I[i * N + j].SetInp = State[i, j];
                }
            }

            for (int k = 0; k < h; k++)
            {
                /*for (int j = 0; j < M * N; j++)
                {
                    sum += I[j].Outp() * W[0][j, i];
                }*/

                for (int i = 0; i < M; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        sum += I[i * N + j].Outp() * W[0][i * N + j, k];
                    }
                }

                sum += W[0][M * N, k];

                H[0][k].SetInp = sum;
                sum = 0;
            }

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    sum += H[0][j].Outp() * W[1][j, i];
                }

                sum += W[1][h, i];

                H[1][i].SetInp = sum;
                sum = 0;
            }

            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    for (int k = 0; k < ACTS; k++)
                    {
                        for (int l = 0; l < h; l++)
                            sum += H[1][l].Outp() * W[2][l, ML * i + NL * j + k];


                        sum += W[2][h, ML * i + NL * j + k];

                        O[ML * i + NL * j + k].SetInp = sum;
                    }
                }
            }

            double[] OutRes = new double[M * N * ACTS];

            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    for (int k = 0; k < ACTS; k++)
                    {
                        OutRes[ML * i + NL * j + k] = O[ML * i + NL * j + k].Outp();
                    }
                }
            }

            return OutRes;
        }
    }
}
