using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRL_2
{
    class NeuralNetwork
    {
        public const double E = 0.7;
        public const double A = 0.3;
        public const double eps = 0.5;
        public const double D = 0.8;
        public const int TargetStep = 5;

        double[][,] W = new double[3][,];
        double[][,] dW = new double[3][,];

        int M;
        int N;
        int ACTS;

        int step = 0;
        TargetNetwork TN;

        InpN[] I;

        Neuron[][] H = new Neuron[2][];

        OutN[] O;

        static Random Rand = new Random();

        public NeuralNetwork(int m, int n, int acts)
        {
            I = new InpN[(m * n)];
            O = new OutN[(m * n * acts)];
            H[0] = new Neuron[24];
            H[1] = new Neuron[24];

            for(int i = 0; i < m*n; i++)
            {
                I[i] = new InpN(Rand.NextDouble());
            }
            for (int i = 0; i < m * n * acts; i++)
            {
                O[i] = new OutN(Rand.NextDouble());
            }
            for (int i = 0; i < 24; i++)
            {
                H[0][i] = new Neuron(Rand.NextDouble());
                H[1][i] = new Neuron(Rand.NextDouble());
            }



            dW[0] = new double[m*n, 24];
            dW[1] = new double[24, 24];
            dW[2] = new double[24, m * n* acts];

            W[0] = new double[m * n, 24];
            W[1] = new double[24, 24];
            W[2] = new double[24, m * n * acts];

            for(int i = 0; i < (m * n); i++)
            {
                for (int j = 0; j < 24; j++)
                    W[0][i, j] = Rand.NextDouble();
            }

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 24; j++)
                    W[1][i, j] = Rand.NextDouble();
            }

            for (int i = 0; i < 24; i++)
            {
                for (int j = 0; j < (m * n * acts); j++)
                    W[2][i, j] = Rand.NextDouble();
            }

            M = m;
            N = n;
            ACTS = acts;

            TN = new TargetNetwork(W, M, N, ACTS);
        }
        
        public double[] Calculate(int[,] State)
        {
            double sum = 0;

            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    I[i].SetInp = State[i, j];
                }
            }

            for (int i = 0; i < 24; i++)
            {
                for (int j = 0; j < M * N; j++)
                {
                    sum += I[j].Outp() * W[0][j, i];
                }

                H[0][i].SetInp = sum;
                sum = 0;
            }

            for (int i = 0; i < 24; i++)
            {
                for (int j = 0; j < 24; j++)
                {
                    sum += H[0][j].Outp() * W[1][j, i];
                }

                H[1][i].SetInp = sum;
                sum = 0;
            }

            for (int i = 0; i < M * N * ACTS; i++)
            {
                for (int j = 0; j < 24; j++)
                {
                    sum += H[1][j].Outp() * W[2][j, i];
                }

                O[i].SetInp = sum;
                sum = 0;
            }

            double[] OutRes = new double[M * N * ACTS];

            for (int i = 0; i < M * N * ACTS; i++)
            {
                OutRes[i] = O[i].Outp();
            }

            return OutRes;
        }

        public void Result(double a, double b, double Ideal)
        {

        }

        public double Build(ref int[,] State, int[] StateAct, ref bool end)
        {
            State[StateAct[0], StateAct[1]] = StateAct[2];

            double Rs = 0;
            bool earned;

            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (State[i, j] != 1)
                        continue;

                    earned = false;

                    if (!earned && i > 0 && State[i - 1, j] == 2)
                        earned = true;
                    if (!earned && j > 0 && State[i, j - 1] == 2)
                        earned = true;
                    if (!earned && i < 2 && State[i + 1, j] == 2)
                        earned = true;
                    if (!earned && j < 2 && State[i, j + 1] == 2)
                        earned = true;

                    if (earned)
                        Rs++;
                }
            }

            return Rs;
        }
        public void Build(ref int[,] State, int[] StateAct)
        {
            State[StateAct[0], StateAct[1]] = StateAct[2];
        }

        /*public void Build(ref int[,] State, int[] StateAct, ref bool end) 
        {
            //
            State[StateAct[0], StateAct[1]] = StateAct[2];
        }*/

        public void Train(int[,] State, int iters)
        {
            TN.GetW = W;
            bool end = false;

            for (int i = 0; i < iters; i++)
            {
                for (int j = 0; j < (M * N *  2); j++)
                {
                    Iter(ref State, ref end);

                    if (end)
                        break;
                }
            }

            double[] Test = this.Calculate(State);

            Test[5] = 1;


        }

        public void Iter(ref int[,] State, ref bool end)
        {
            if (TargetStep == step)
            {
                TN.GetW = W;
                step = 0;
            }

            double[] Q = this.Calculate(State);
            double R;

            int ML = Q.Length / M;
            int NL = ML / N;

            if (Rand.NextDouble() < eps)
            {// рандом
                int[] randA = new int[3];
                randA[0] = Rand.Next(M);
                randA[1] = Rand.Next(N);
                randA[2] = Rand.Next(ACTS);

                R = Build(ref State, randA, ref end);
            }
            else
            {
                double MaxA = Q[0];
                int[] MaxAInd = new int[3];

                for (int i = 0; i < M; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        for (int k = 0; k < ACTS; k++)
                        {
                            if(MaxA < Q[ML * i + NL * j + k])
                            {
                                MaxA = Q[ML * i + NL * j + k];
                                MaxAInd[0] = i;
                                MaxAInd[1] = j;
                                MaxAInd[2] = k;
                            }
                        }
                    }
                }

                R = Build(ref State, MaxAInd, ref end);
            }

            double[] Ideal = new double[M * N * ACTS];
            double[] TQ;
            int[,] NextState = new int [M, N];
            NextState = State;
            int[] Act = new int[3];

            for(int i = 0; i < M * N * ACTS; i++)
            {
                Act[0] = i / ML;
                Act[1] = i / NL;

            }


            double[][] del = new double[2][];
            del[0] = new double[24];
            del[1] = new double[24];


            for(int i = 0; i < M * N * ACTS; i++)
            {
                Ideal[i] = 1;
            }

            double sum = 0;

            for (int i = 0; i < 24; i++)
            {
                for (int j = 0; j < M * N * ACTS; j++)
                {
                    sum += O[j].delta(Ideal[j]) * W[2][i, j];
                }

                del[1][i] = H[1][i].delta(sum);
                sum = 0;
            }

            for (int i = 0; i < 24; i++)
            {
                for (int j = 0; j < 24; j++)
                {
                    sum += del[1][j] * W[1][i, j];
                }

                del[0][i] = H[0][i].delta(sum);
                sum = 0;
            }

            for (int i = 0; i < M*N; i++)
            {
                for (int j = 0; j < 24; j++)
                {
                    dW[0][i, j] = A * dW[0][i, j] + E * I[i].Outp() * del[0][j];
                    W[0][i, j] += dW[0][i, j];
                }
            }

            for (int i = 0; i < 24; i++)
            {
                for (int j = 0; j < 24; j++)
                {
                    dW[1][i, j] = A * dW[1][i, j] + E * H[0][i].Outp() * del[1][j];
                    W[1][i, j] += dW[1][i, j];
                }
            }

            for (int i = 0; i < 24; i++)
            {
                for (int j = 0; j < M * N * ACTS; j++)
                {
                    dW[2][i, j] = A * dW[2][i, j] + E * H[1][i].Outp() * O[j].delta(Ideal[j]);
                    W[2][i, j] += dW[2][i, j];
                }
            }
        }
    }
}
