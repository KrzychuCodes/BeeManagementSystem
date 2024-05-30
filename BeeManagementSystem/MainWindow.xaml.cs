using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeeManagementSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
           
        }
        private Queen queen;


    }
    static class HoneyVault
    {
        public static string StatusReport
        {
            get{
                string status = $"Units of honey : {honey:0.0}\n" + $"Units of nectar: {nectar:0.0}";
                string warnings = "";
                if(honey<LOW_LEVEL_WARNING) warnings += "LOW HONEY - ADD HONEY MANUFACTURERS";
                    
                if (nectar < LOW_LEVEL_WARNING) warnings += "LOW NECTAR - ADD NECTAR COLLECTORS";
                return status + warnings;
               }
        }
       
        private static float honey = 25f;
        private static float nectar = 100f;
        const float NECTAR_CONVERSION_RATIO = .19f;
        const float LOW_LEVEL_WARNING = 10f;

        public static void CollectNectar(float amount)
        {
            if (amount > 0) nectar += amount;
        }

        public static void ConvertNectarToHoney(float amount)
        {
            if (amount > nectar) 
            {
                amount = nectar;
                honey += amount * NECTAR_CONVERSION_RATIO;
            }
            else
            {
                honey += (nectar - amount) * NECTAR_CONVERSION_RATIO;
            }
            
            
        }
        public static bool ConsumeHoney( float amount)
        {
            if(amount <= honey)
            {
                honey -= amount;
                return true;
            }
            else
            {
                return false;
            }
        }

    }
    class Bee
    {
        public string Job;
        protected virtual float CostPerShift { get; }
        public Bee(string job)
        {
            Job = job;
        }
       public void WorkTheNextShift()
        {

        }

        protected virtual void DoJob()
        {

        }
    }

    class Queen : Bee
    {
        
        public  string StatusReport { get; private set; }
        protected override float CostPerShift { get { return 2.15f; } }
        private Bee[] workers;
        private float eggs;
        private float unassignedWorkers = 3;
        private const float HONEY_PER_UNASSIGNED_WORKER = 0.5f;

        public Queen() : base("Queen")
        {
            AssignBee(new Bee[] { new NectarCollector() });
            AssignBee(new Bee[] {new HoneyManufacturer()});
            AssignBee(new Bee[] { new EggCare() });
        }

        protected void AssignBee(Bee[] workers)
        {
            switch (Job)
            {
                case  "Nectar collector":

                    AddWorker(new NectarCollector());
                    break;
                case "Honey manufacturer":

                    AddWorker(new HoneyManufacturer());
                    break;
                case "Egg care":
                    AddWorker(new EggCare());
                    break;



            }
            
        }
        private  void UpdateStatusReport()
        {
            StatusReport = $"Reporting state of the Vault: \n{HoneyVault.StatusReport}\n" + 
                $"\nNumber of eggs: {eggs:0.0}\n" + 
                $"Unassigned workers: {unassignedWorkers:0.0}\n" + 
                $"{WorkerStatus("Nectar collector")}\n{WorkerStatus("Honey collector")}" + 
                $"\n{WorkerStatus("Egg care")}\nWORKERS TOTAL : {workers.Length}";
        }
        private  string WorkerStatus(string job)
        {
            int count = 0;
            foreach (Bee worker in workers)
            {
                if (worker.Job == job)
                {
                    count++;
                }

            }
            return $"{job}: {count}";
        }
        protected override void DoJob()
        {
            const float EGGS_PER_SHIFT = 0.45f;
            eggs += EGGS_PER_SHIFT;
            foreach (Bee worker in workers)
            {
                worker.WorkTheNextShift();
            }
            HoneyVault.ConsumeHoney(HONEY_PER_UNASSIGNED_WORKER * unassignedWorkers);
        }
         protected void CareForEggs(float eggsToConvert)
        {
            if(eggs >= eggsToConvert)
            {
                eggs -= eggsToConvert;
                unassignedWorkers += eggsToConvert;
            }
        }
        /// <summary>
        /// increase array workers of 1 space and add reference of type Bee
        /// <param name="worker">worker added to array workers</param>
        /// </summary>
        private void AddWorker(Bee worker)
        {
            if(unassignedWorkers >= 1)
            {
                unassignedWorkers--;
                Array.Resize(ref workers, workers.Length + 1);
                workers[workers.Length - 1] = worker;
            }
        }
       
    }
    class NectarCollector : Bee
    {
        protected override float CostPerShift { get { return 1.95f; } }
        public NectarCollector() : base("Nectar collector") { }
        protected override void DoJob()
        {
            base.DoJob();
        }
    }
    class HoneyManufacturer : Bee
    {
        protected override float CostPerShift { get { return 1.7f; } }
        public HoneyManufacturer() : base("Honey manufacturer") { }
        protected override void DoJob()
        {
            base.DoJob();
        }
    }
    class EggCare : Bee
    {
        protected override float CostPerShift { get { return 1.35f; } }
        public EggCare() : base("Egg care") { }
    }
}
