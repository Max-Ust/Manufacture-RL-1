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
        public Neuron()
        {
            inp = 0.25;
        }
        public double Outp()
        {
            return 1 / (1 + Math.Pow(Math.E, -1 * inp));
        }

        public double MOR()
        {
            return (1 - this.Outp()) * this.Outp();
        }

        public double delta(double sum)
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
            //return this.MOR() * (Ideal - this.Outp()) * (Ideal - this.Outp());
            return this.MOR() * (Ideal - this.Outp());
        }
    }
}
