using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
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
        private Queen queen = new Queen();
        private DispatcherTimer timer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            statusReport.Text = queen.StatusReport;
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(1.5);
            timer.Start();

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            WorkShift_Click(this, new RoutedEventArgs());
        }

        private void AssignJob_Click(object sender, RoutedEventArgs e)
        {
            queen.AssignBee(jobSelector.Text);
            statusReport.Text = queen.StatusReport;
        }

        private void WorkShift_Click(object sender, RoutedEventArgs e)
        {
            queen.WorkTheNextShift();
            statusReport.Text = queen.StatusReport;

        }
    }
    static class HoneyVault
    {
        private static float honey = 35f;
        private static float nectar = 100f;
        public const float NECTAR_CONVERSION_RATIO = .19f;
        public const float LOW_LEVEL_WARNING = 10f;

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
       
        

        public static void CollectNectar(float amount)
        {
            if (amount > 0f) nectar += amount;
        }

        public static void ConvertNectarToHoney(float amount)
        {
            float nectarToConvert = amount;

            if (nectarToConvert > nectar) nectarToConvert = nectar;
            nectar -= nectarToConvert;
            honey += nectarToConvert * NECTAR_CONVERSION_RATIO;
            
            
            
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
        public string Job { get; private set; }
        public virtual float CostPerShift { get; }
        public Bee(string job)
        {
            Job = job;
        }
       public void WorkTheNextShift()
        {
            if (HoneyVault.ConsumeHoney(CostPerShift))
            {
                DoJob();
            }
        }

        protected virtual void DoJob()
        {
            /* overridden in the underclass */
        }
    }

    class Queen : Bee
    {
        
        public  string StatusReport { get; private set; }
        public override float CostPerShift { get { return 2.15f; } }
        private Bee[] workers = new Bee[0];
        private float eggs = 0;
        private float unassignedWorkers = 3;
        public const float HONEY_PER_UNASSIGNED_WORKER = 0.5f;
         public const float EGGS_PER_SHIFT = 0.45f;

        public Queen() : base("Queen")
        {
            AssignBee("Nectar Collector");
            AssignBee("Honey Manufacturer");
            AssignBee("Egg Care");
        }

        public void AssignBee(string job)
        {
            switch (job)
            {
                case  "Nectar Collector":

                    AddWorker(new NectarCollector());
                    break;
                case "Honey Manufacturer":

                    AddWorker(new HoneyManufacturer());
                    break;
                case "Egg Care":
                    AddWorker(new EggCare(this));
                    break;



            }
            UpdateStatusReport();
        }
        private  void UpdateStatusReport()
        {
            StatusReport = $"Reporting state of the Vault: \n{HoneyVault.StatusReport}\n" + 
                $"\nNumber of eggs: {eggs:0.0}\n" + 
                $"Unassigned workers: {unassignedWorkers:0.0}\n" + 
                $"{WorkerStatus("Nectar Collector")}\n{WorkerStatus("Honey Manufacturer")}" + 
                $"\n{WorkerStatus("Egg Care")}\nWORKERS TOTAL : {workers.Length}";
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
            
            eggs += EGGS_PER_SHIFT;
            foreach (Bee worker in workers)
            {
                worker.WorkTheNextShift();
            }
            HoneyVault.ConsumeHoney(HONEY_PER_UNASSIGNED_WORKER * unassignedWorkers);
            UpdateStatusReport();
        }
         public void CareForEggs(float eggsToConvert)
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
        public override float CostPerShift { get { return 1.95f; } }
        const float NECTAR_COLLECTED_PER_SHIFT = 33.25f;

        public NectarCollector() : base("Nectar Collector") { }
        protected override void DoJob()
        {
           
            HoneyVault.CollectNectar(NECTAR_COLLECTED_PER_SHIFT);
        }
    }
    class HoneyManufacturer : Bee
    {
        public override float CostPerShift { get { return 1.7f; } }
        const float NECTAR_PROCESSED_PER_SHIFT = 33.15f;
        public HoneyManufacturer() : base("Honey Manufacturer") { }
        protected override void DoJob()
        {
            
            HoneyVault.ConvertNectarToHoney(NECTAR_PROCESSED_PER_SHIFT);
        }
    }
    class EggCare : Bee
    {
        public override float CostPerShift { get { return 1.35f; } }
         public const float CARE_PROGRESS_PER_SHIFT = 0.15f;

        private Queen queen;
        public EggCare(Queen queen) : base("Egg Care") 
        {
            this.queen = queen;
            

        }

        protected override void DoJob()
        {
            
            queen.CareForEggs(CARE_PROGRESS_PER_SHIFT);
        }
    }
}
