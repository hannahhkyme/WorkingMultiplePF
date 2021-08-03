using System;
using System.Collections.Generic;
namespace RealPF
{
    class Program //main file
    {
        public int NUMBER_OF_SHARKS;
        public int NUMBER_OF_ROBOTS;
        public int NUMBER_OF_PARTICLEFILTERS;
        public List<List<ParticleFilter>> particleFilterList = new List<List<ParticleFilter>>();
        public List<List<Simulation>> simulationList = new List<List<Simulation>>();
        public void create_real_range_list()
        {
            foreach(Shark s1 in MyGlobals.shark_list)
            {
                List<double> newList = new List<double>();
                foreach(Robot r1 in MyGlobals.robot_list)
                {
                    newList.Add(20000);
                }
                MyGlobals.real_range_list.Add(newList);
            }
        }

        public void create_and_initialize_sharks()
        {
            int SharkIndex = 0;
            for(int i=0; i<NUMBER_OF_SHARKS; ++i)
            {
                Shark s1 = new Shark();
                s1.SHARKNUMBER = SharkIndex;
                MyGlobals.shark_list.Add(s1);
                SharkIndex += 1;
            }
        }
        public void create_and_initialize_robots()
        {
            int RobotIndex = 0;
            for (int i = 0; i < NUMBER_OF_ROBOTS; ++i)
            {
                Robot r1 = new Robot();
                r1.ROBOTNUMBER = RobotIndex;
                MyGlobals.robot_list.Add(r1);
                RobotIndex += 1;
            }
        }

        public void create_and_initialize_particle_filter()
        {
            for (int s = 0; s < NUMBER_OF_SHARKS; ++s)
            {
                List<ParticleFilter> partylist = new List<ParticleFilter>();
                for (int r = 0; r < NUMBER_OF_ROBOTS; ++r)
                {
                    ParticleFilter p1 = new ParticleFilter();
                    p1.create();
                    partylist.Add(p1);
                }
                particleFilterList.Add(partylist);
            }
        }

        public void create_simulation()
        {
            foreach (Shark s1 in MyGlobals.shark_list)
            {
                List<Simulation> currentSharkSim = new List<Simulation>();
                foreach(Robot r1 in MyGlobals.robot_list)
                {
                    double rangeError = s1.calc_range_error(r1);
                    Simulation sim = new Simulation(rangeError, s1, r1);
                    currentSharkSim.Add(sim);

                }
                simulationList.Add(currentSharkSim);
            }
        }
        //[[Sim1, Sim2], [Sim1, Sim2]]
        public void update_simulation_list(int sharkNumber, int robotNumber)
        {
            simulationList[sharkNumber][robotNumber].rangeError = MyGlobals.shark_list[sharkNumber].calc_range_error(MyGlobals.robot_list[robotNumber]);
        }
        public void update_robots()
        {
            foreach (Robot r1 in MyGlobals.robot_list)
            {
                r1.update_robot_position();
            }
        }
        public void update_sharks()
        {
            foreach (Shark s1 in MyGlobals.shark_list)
            {
                s1.update_shark();
            }
        }
        
        public void update_pfs()
        {
            foreach (List<ParticleFilter> pflist in particleFilterList)
            {
                foreach (ParticleFilter pf1 in pflist)
                {
                    pf1.update();
                }
                
            }
        }
        public void clear_real_range_list()
        {
            List<List<double>> newList = new List<List<double>>();
            MyGlobals.real_range_list = newList;
            create_real_range_list();
        }

        public bool get_simulation()
        {
            if (MyGlobals.random_num.Next(0,11)>5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public double calc_range_error(List<double> mean_particle, int sharkNum1)
        {

            double auvRange = Math.Sqrt(Math.Pow((mean_particle[1] - MyGlobals.shark_list[sharkNum1].Y), 2) + Math.Pow((mean_particle[0] - MyGlobals.shark_list[sharkNum1].X), 2));
            return auvRange;
        }

        static void Main(string[] args)
        {
            Program main = new Program();
            main.NUMBER_OF_ROBOTS = 4;
            main.NUMBER_OF_SHARKS = 3;
            main.NUMBER_OF_PARTICLEFILTERS = main.NUMBER_OF_ROBOTS * main.NUMBER_OF_SHARKS;
            main.create_and_initialize_sharks();
            main.create_and_initialize_robots();
            main.create_and_initialize_particle_filter();

            
            main.create_simulation();
            main.create_real_range_list();
            while(true)
            {
                main.update_sharks();
                main.update_robots();
                if (main.get_simulation())
                {
                    main.update_simulation_list(0, 0);
                    main.update_simulation_list(0, 1);
                    main.update_simulation_list(0, 2);
                    main.update_simulation_list(0, 3);
                    if (main.get_simulation())
                    {
                        main.update_simulation_list(1, 0);
                        main.update_simulation_list(1, 1);
                        main.update_simulation_list(1, 2);
                        main.update_simulation_list(1, 3);

                    }
                    main.update_simulation_list(2, 0);
                    main.update_simulation_list(2, 1);
                    main.update_simulation_list(2, 2);
                    main.update_simulation_list(2, 3);
                }
                main.clear_real_range_list();
                foreach (List<Simulation> simlist in main.simulationList)
                {
                    foreach (Simulation sim in simlist)
                    {
                        sim.update_real_range_list();
                    }
                }
                int sharkNum = 0;
                foreach (List<ParticleFilter> pflist in main.particleFilterList)
                {
                    foreach (ParticleFilter pf in pflist)
                    {
                        pf.update();
                        pf.update_weights(MyGlobals.real_range_list, sharkNum);
                        pf.correct();
                       
                    }
                    
                    sharkNum += 1;
                }
                int sharkNum1 = 0;
                List<double> range_error_list = new List<double>();
                foreach (List<ParticleFilter> pflist in main.particleFilterList)
                {
                    foreach (ParticleFilter pf in pflist)
                    {
                        pf.predicting_shark_location();
                        double range_error =main.calc_range_error(pf.predicting_shark_location(), sharkNum1);
                        range_error_list.Add(range_error);
                    }
                    sharkNum1 += 1;
                }
                Console.WriteLine("Iteration Here");
                Console.WriteLine(range_error_list[0]);
                Console.WriteLine(range_error_list[1]);
                Console.WriteLine(range_error_list[2]);
                Console.WriteLine(range_error_list[3]);
                Console.WriteLine("Iteration 2");
                Console.WriteLine(range_error_list[4]);
                Console.WriteLine(range_error_list[5]);
                Console.WriteLine(range_error_list[6]);
                Console.WriteLine(range_error_list[7]);
                Console.WriteLine("Iteration 3");
                Console.WriteLine(range_error_list[8]);
                Console.WriteLine(range_error_list[9]);
                Console.WriteLine(range_error_list[10]);
                Console.WriteLine(range_error_list[11]);
            }
        }
    }
}
