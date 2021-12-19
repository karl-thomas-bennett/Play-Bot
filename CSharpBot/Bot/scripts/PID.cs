using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.scripts
{
    class PID
    {
        float proportionalConstant = 0;
        float integralConstant = 0;
        float derivativeConstant = 0;

        float tauConstant = 0;

        float integrator = 0;
        float prevError = 0;
        float differentiator = 0;
        float prevMesurement = 0;

        float timeStep;

        float goal = 0;
        float limMax;
        float limMin;

        Func<float, float, float, float[]> modifier;

        public PID(float limMin, float limMax, Func<float, float, float, float[]> modifier = null, float timeStep = 1/60)
        {
            if(modifier != null)
            {
                this.modifier = modifier;
            }
            else
            {
                this.modifier = (measurement, prevMeasurement, goal) => new float[] {measurement, goal};
            }
            this.limMin = limMin;
            this.limMax = limMax;
            this.timeStep = timeStep;
        }

        public PID SetGoal(float goal)
        {
            if(this.goal != goal)
            {
                differentiator = 0;
            }
            this.goal = goal;
            
            return this;
        }

        public PID P(float p)
        {
            proportionalConstant = p;
            return this;
        }
        public PID I(float i)
        {
            integralConstant = i;
            return this;
        }
        public PID D(float d)
        {
            derivativeConstant = d;
            return this;
        }

        public float Next(float measurement)
        {
            float[] modifications = modifier(measurement, prevMesurement, goal);
            measurement = modifications[0];
            goal = modifications[1];
            float output = 0;
            float error = goal - measurement;
            float proportional = proportionalConstant * error;

            integrator += 0.5f * integralConstant * timeStep * (error + prevError);

            float integratorMin, integratorMax;

            if(limMax > proportional)
            {
                integratorMax = limMax - proportional;
            }
            else
            {
                integratorMax = 0;
            }
            if (limMin < proportional)
            {
                integratorMin = limMin - proportional;
            }
            else
            {
                integratorMin = 0;
            }

            if(integrator > integratorMax)
            {
                integrator = integratorMax;
            }
            else if(integrator < integratorMin)
            {
                integrator = integratorMin;
            }

            differentiator = (2 * -derivativeConstant * (measurement - prevMesurement)
                           + (2 * tauConstant - timeStep) * differentiator)
                           / (2 * tauConstant + timeStep);
            output = proportional + integrator + differentiator;

            if(output > limMax)
            {
                output = limMax;
            }
            else if(output < limMin)
            {
                output = limMin;
            }

            prevError = error;
            prevMesurement = measurement;

            return output;
        }

    }
}

