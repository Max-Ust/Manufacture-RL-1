using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRL_2
{
    class NeuralNetwork
    {
        public const double E = 0.1;
        public const double A = 0.1;
        public const double eps = 0.8;
        public const double D = 0.9;
        public const int TargetStep = 3;

        double[][,] W = new double[3][,];
        double[][,] dW = new double[3][,];
        
        int M;
        int N;
        int ACTS;

        int step = 0;
        TargetNetwork TN;

        InpN[] I;

        Neuron[][] H = new Neuron[2][];
        int h = 30;

        OutN[] O;

        static Random Rand = new Random();

        public NeuralNetwork(int m, int n, int acts)
        {
            I = new InpN[(m * n)];
            O = new OutN[(m * n * acts)];
            H[0] = new Neuron[h];
            H[1] = new Neuron[h];

            for(int i = 0; i < m*n; i++)
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



            dW[0] = new double[m * n + 1, h];
            dW[1] = new double[h + 1, h];
            dW[2] = new double[h + 1, m * n* acts];

            W[0] = new double[m * n + 1, h];
            W[1] = new double[h + 1, h];
            W[2] = new double[h + 1, m * n * acts];

            for(int i = 0; i < (m * n) + 1; i++)
            {
                for (int j = 0; j < h; j++)
                    W[0][i, j] = Rand.Next(-10, 10) / 10.0;
            }

            for (int i = 0; i < h + 1; i++)
            {
                for (int j = 0; j < h; j++)
                    W[1][i, j] = Rand.Next(-10, 10) / 10.0;
            }

            for (int i = 0; i < h + 1; i++)
            {
                for (int j = 0; j < (m * n * acts); j++)
                    W[2][i, j] = Rand.Next(-10, 10) / 10.0;
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
                    I[i * N + j].SetInp = State[i, j];
                }
            }

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < M * N; j++)
                {
                    sum += I[j].Outp() * W[0][j, i];
                }

                sum += W[0][M * N, i];

                H[0][i].SetInp = sum;
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

            for (int i = 0; i < M * N * ACTS; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    sum += H[1][j].Outp() * W[2][j, i];
                }

                sum += W[2][h, i];

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

        public int[,] Result()
        {
            int[,] State = new int[M, N];
            bool end = false;

            for (int j = 0; j < (M * N * 3 / 2); j++)
            {
                BuildRes(ref State, ARGMAX(Calculate(State)), ref end);

                if (end)
                    break;
            }

            return State;
        }

        public double Build(ref int[,] State, int[] StateAct, ref bool end)
        {
            double Rs = 0;
            bool earned;

            State[StateAct[0], StateAct[1]] = StateAct[2];

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
                    if (!earned && i < M - 1 && State[i + 1, j] == 2)
                        earned = true;
                    if (!earned && j < N - 1 && State[i, j + 1] == 2)
                        earned = true;

                    if (earned)
                        Rs++;
                }
            }

            return Rs / 10.0;
        }

        public void BuildRes(ref int[,] State, int[] StateAct, ref bool end)
        {
            State[StateAct[0], StateAct[1]] = StateAct[2];
        }

        public void Train(int iters)
        {
            TN.GetW = W;
            bool end = false;

            int[,] State = new int[M, N];

            for (int i = 0; i < iters; i++)
            {
                for(int k = 0; k < M; k++)
                {
                    for (int p = 0; p < N; p++)
                    {
                        State[k, p] = 0;
                    }
                }

                end = false;

                for (int j = 0; j < (M * N *  3 / 2); j++)
                {
                    Iter(ref State, ref end);

                    if (end)
                        break;
                }
            }

            for (int k = 0; k < M; k++)
            {
                for (int p = 0; p < N; p++)
                {
                    State[k, p] = 0;
                }
            }

            double[] TESTQ = Calculate(State);

            TESTQ[5] = 1;
        }

        public void Iter(ref int[,] State, ref bool end)
        {
            if (TargetStep == step)
            {
                TN.GetW = W;
                step = 0;
            }

            double[] Q;

            int[] Act = new int[3];

            double R;

            if (Rand.NextDouble() < eps)
            {
                this.Calculate(State);

                int[] randA = new int[3];
                randA[0] = Rand.Next(M);
                randA[1] = Rand.Next(N);
                randA[2] = Rand.Next(ACTS);

                R = Build(ref State, randA, ref end);
            }
            else
            {
                Q = this.Calculate(State);

                Act = ARGMAX(Q);

                R = Build(ref State, Act, ref end);
            }

            double[] Ideal = new double[M * N * ACTS];
            int[,] NextState = new int [M, N];

            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    for (int k = 0; k < ACTS; k++)
                    {

                        for (int p = 0; p < M; p++)
                        {
                            for (int l = 0; l < N; l++)
                            {
                                NextState[i, j] = State[i, j];
                            }
                        }

                        Act[0] = i;
                        Act[1] = j;
                        Act[2] = k;

                        BuildRes(ref NextState, Act, ref end);

                        Q = TN.Calculate(NextState);

                        int ML = Q.Length / M;
                        int NL = ML / N;

                        Ideal[ML * i + NL * j + k] = R + D * MAX(Q);
                    }
                }
            }


            double[][] del = new double[2][];
            del[0] = new double[h + 1];
            del[1] = new double[h + 1];

            double sum = 0;

            for (int i = 0; i < h + 1; i++)
            {
                for (int j = 0; j < M * N * ACTS; j++)
                {
                    sum += O[j].delta(Ideal[j]) * W[2][i, j];
                }

                del[1][i] = H[1][0].delta(sum);
                sum = 0;
            }

            for (int i = 0; i < h + 1; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    sum += del[1][j] * W[1][i, j];
                }

                del[0][i] = H[0][0].delta(sum);
                sum = 0;
            }

            for (int i = 0; i < M*N; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    dW[0][i, j] = A * dW[0][i, j] + E * I[i].Outp() * del[0][j];
                    W[0][i, j] += dW[0][i, j];
                }
            }
            for (int j = 0; j < h; j++)
            {
                dW[0][M * N, j] = A * dW[0][M * N, j] + E * 1 * del[0][j];
                W[0][M * N, j] += dW[0][M * N, j];
            }

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    dW[1][i, j] = A * dW[1][i, j] + E * H[0][i].Outp() * del[1][j];
                    W[1][i, j] += dW[1][i, j];
                }
            }
            for (int j = 0; j < h; j++)
            {
                dW[1][h, j] = A * dW[1][h, j] + E * 1 * del[1][j];
                W[1][h, j] += dW[1][h, j];
            }

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < M * N * ACTS; j++)
                {
                    dW[2][i, j] = A * dW[2][i, j] + E * H[1][i].Outp() * O[j].delta(Ideal[j]);
                    W[2][i, j] += dW[2][i, j];
                }
            }
            for (int j = 0; j < M * N * ACTS; j++)
            {
                dW[2][h, j] = A * dW[2][h, j] + E * 1 * O[j].delta(Ideal[j]);
                W[2][h, j] += dW[2][h, j];
            }

            step++;
        }

        public int[] ARGMAX(double[] Q)
        {
            double MaxA = Q[0];
            int[] MaxAInd = new int[3];

            int ML = Q.Length / M;
            int NL = ML / N;

            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    for (int k = 0; k < ACTS; k++)
                    {
                        if (MaxA < Q[ML * i + NL * j + k])
                        {
                            MaxA = Q[ML * i + NL * j + k];
                            MaxAInd[0] = i;
                            MaxAInd[1] = j;
                            MaxAInd[2] = k;
                        }
                    }
                }
            }

            return MaxAInd;
        }

        public double MAX(double[] Q)
        {
            double MaxA = Q[0];

            int ML = Q.Length / M;
            int NL = ML / N;

            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    for (int k = 0; k < ACTS; k++)
                    {
                        if (MaxA < Q[ML * i + NL * j + k])
                        {
                            MaxA = Q[ML * i + NL * j + k];
                        }
                    }
                }
            }

            return MaxA;
        }
    }
}
