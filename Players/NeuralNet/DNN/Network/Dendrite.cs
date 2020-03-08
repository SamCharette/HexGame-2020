using System;
using System.Collections.Generic;
using System.Text;

namespace Players.NeuralNet.DNN.Network
{
    public class Dendrite
    {
        public Pulse InputPulse { get; set; }

        public double SynapticWeight { get; set; }

        public bool Learnable { get; set; } = true;
    }
}
