using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRL_2
{
    class Neuron
    {
        protected double inp;

        public Neuron(double inp)
        {
            this.inp = inp;
        }
        public double Outp() // Функция активации
        {
            return 1 / (1 + Math.Pow(Math.E, -1 * inp));
            // return (-1 + Math.Pow(Math.E, 2 * inp)) / (1 + Math.Pow(Math.E, 2 * inp));
        }

        public double MOR() // Функция обратного распространения
        {
            return (1 - this.Outp()) * this.Outp();
            // return 1 - this.Outp() * this.Outp();
        }

        public double delta(double sum) // Дельта
        {
            return this.MOR() * sum;
        }

        public double SetInp
        {
            set
            {
                inp = value;
            }
        }
    }

    class InpN : Neuron
    {
        public InpN(double inp) : base(inp)
        {

        }

        public new double Outp()
        {
            return (inp + 1) / 10.0;
            //return inp;
        }
    }

    class OutN : Neuron
    {
        public OutN(double inp) : base(inp)
        {

        }

        public new double Outp()
        {
            return inp;
        }

        public new double MOR()
        {
            return 1;
        }

        public new double delta(double Ideal)
        {
            return this.MOR() * (Ideal - this.Outp());
        }
    }
}
