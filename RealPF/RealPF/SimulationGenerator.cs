using System;
using System.Collections.Generic;
namespace RealPF
{
    public class Simulation
    {
        public double rangeError;
        public Shark currentShark;
        public Robot currentRobot;
        public Simulation(double rangeError, Shark currentShark, Robot currentRobot)
        {
            this.currentRobot = currentRobot;
            this.currentShark = currentShark;
            this.rangeError = rangeError;

        }
        public void update_real_range_list()
        {
            MyGlobals.real_range_list[currentShark.SHARKNUMBER][currentRobot.ROBOTNUMBER] = rangeError;
        }
    }
}
