﻿using Neural_Network___Function_Plotter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neural_Network___Function_Plotter
{
    public class Neuron
    {
        private List<Connection> m_outputWeights = new List<Connection>();

        private readonly int myIndex;

        public Neuron(int numOutputs, int myIndex, Random rand)
        {
            this.myIndex = myIndex;

            //Does not include the bias neuron as it receives no inputs
            for (int c = 0; c < numOutputs; c++) //c -> connection
            {
                Connection connection = new Connection();

                if (rand.Next(0, 9999) % 2 == 0)
                    connection.Weight = rand.NextDouble();
                else
                    connection.Weight = -rand.NextDouble();

                m_outputWeights.Add(connection);
            }
        }

        private double input;
        public double Input { get { return input; } set { input = value; } }

        private double output;
        public double Output { get { return output; } set { output = value; } }

        //Public
        public static double eta = 0.1;
        /*
         * 0.0 - slow learner
         * 0.2 - medium learner
         * 1.0 - reckless learner
         */
        public static double alpha = 0.3;
        /*
         * 0.0 - no momentum
         * 0.5 - moderate momentum
         */

        public Connection getConnection(int index) { return m_outputWeights[index]; }

        public void calcOutputGradients(double targetVal)
        {
            double delta = targetVal - output;
            m_gradient = delta * MathUtils.TransferFunctionDerivative(input);
        }

        public void calcHiddenGradient(Layer nextLayer)
        {
            double dow = sumDOW(nextLayer);
            m_gradient = dow * MathUtils.TransferFunctionDerivative(input);
        }

        public void updateInputWeights(Layer prevLayer)
        {
            for (int n = 0; n < prevLayer.neurons.Count(); n++)
            {
                Neuron neuron = prevLayer.neurons[n];
                double oldDeltaWeight = neuron.getConnection(myIndex).DeltaWeight;

                //eta = overall learning rate
                //alpha = momentum -> adds a fraction of the previous deltaweight
                double newDeltaWeight = eta * neuron.Input * m_gradient + alpha * oldDeltaWeight;

                neuron.m_outputWeights[myIndex].DeltaWeight = newDeltaWeight;
                neuron.m_outputWeights[myIndex].Weight += newDeltaWeight;
            }
        }

        public void feedForward(Layer prevLayer)
        {
            input = 0.0;

            for (int n = 0; n < prevLayer.neurons.Count(); n++)
            {
                input += prevLayer.neurons[n].Output * prevLayer.neurons[n].getConnection(myIndex).Weight;
            }

            output = MathUtils.TransferFunction(input);
        }

        //Private

        private double m_gradient;

        private double sumDOW(Layer nexLayer)
        {
            double sum = 0.0;

            //Sum our contributions of the errors at the nodes we feed
            for (int n = 0; n < nexLayer.neurons.Count() - 1; n++)
            {
                sum += m_outputWeights[n].Weight * nexLayer.neurons[n].m_gradient;
            }

            return sum;
        }
    }
}