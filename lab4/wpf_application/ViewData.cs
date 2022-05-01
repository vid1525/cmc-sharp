using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.ComponentModel;

namespace wpf_application
{
    public class ViewData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private calculations.VMBenchmark benchmark = new calculations.VMBenchmark();
        private bool dataSaved = true;

        public calculations.VMBenchmark Benchmark
        {
            get { return benchmark; }
            set
            {
                benchmark = value;
                OnPropertyChanged("Benchmark");
            }
        }

        public bool DataSaved
        {
            get { return dataSaved; }
            set
            {
                dataSaved = value;
                OnPropertyChanged("DataSaved");
            }
        }

        public void AddVMTime(calculations.VMGrid vmGrid)
        {
            Benchmark.AddVMTime(vmGrid);
            OnPropertyChanged("Benchmark");
            DataSaved = false;
        }

        public void AddVMAccuracy(calculations.VMGrid vmGrid)
        {
            Benchmark.AddVMAccuracy(vmGrid);
            OnPropertyChanged("Benchmark");
            DataSaved = false;
        }

        public void Save(string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.WriteLine(Benchmark.TimeResults.Count);
                foreach (var x in Benchmark.TimeResults)
                {
                    writer.WriteLine($"{x.GridParams.ArgsVectorLength} {x.GridParams.GridRange[0]} {x.GridParams.GridRange[1]} {(int)x.GridParams.VMfParameter}");
                    writer.WriteLine($"{x.VmlHaExecTime} {x.VmlEpExecTime} {x.WithoutMklExecTime} {x.CoefOfExecVmlHa} {x.CoefOfExecVmlEp}");
                }
                writer.WriteLine(Benchmark.AccuracyResults.Count);
                foreach (var x in Benchmark.AccuracyResults)
                {
                    writer.WriteLine($"{x.GridParams.ArgsVectorLength} {x.GridParams.GridRange[0]} {x.GridParams.GridRange[1]} {(int)x.GridParams.VMfParameter}");
                    writer.WriteLine($"{x.MaxAbsDiffArgument} {x.MaxAbsDiffValue} {x.MaxAbsDiffValueVmlHa} {x.MaxAbsDiffValueVmlEp}");
                }
                writer.WriteLine($"{Benchmark.MinCoefOfExecVmlHa} {Benchmark.MinCoefOfExecVmlEp}");
            }
        }

        public void Load(string filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                Benchmark = new calculations.VMBenchmark();

                string line;
                string[] arr;

                var timesCount = Int32.Parse(reader.ReadLine());
                for (int i = 0; i < timesCount; ++i)
                {
                    var timeItem = new calculations.VMTime();
                    
                    line = reader.ReadLine();
                    arr = line.Split(' ');
                    timeItem.GridParams = new calculations.VMGrid(Int32.Parse(arr[0]), Single.Parse(arr[1]), Single.Parse(arr[2]), (calculations.VMf)Int32.Parse(arr[3]));
                    
                    line = reader.ReadLine();
                    arr = line.Split(' ');
                    timeItem.VmlHaExecTime = Single.Parse(arr[0]);
                    timeItem.VmlEpExecTime = Single.Parse(arr[1]);
                    timeItem.WithoutMklExecTime = Single.Parse(arr[2]);
                    timeItem.CoefOfExecVmlHa = Single.Parse(arr[3]);
                    timeItem.CoefOfExecVmlEp = Single.Parse(arr[4]);

                    Benchmark.TimeResults.Add(timeItem);
                }

                var accuracyCount = Int32.Parse(reader.ReadLine());
                for (int i = 0; i < accuracyCount; ++i)
                {
                    var accuracyItem = new calculations.VMAccuracy();

                    line = reader.ReadLine();
                    arr = line.Split(' ');
                    accuracyItem.GridParams = new calculations.VMGrid(Int32.Parse(arr[0]), Single.Parse(arr[1]), Single.Parse(arr[2]), (calculations.VMf)Int32.Parse(arr[3]));

                    line = reader.ReadLine();
                    arr = line.Split(' ');
                    accuracyItem.MaxAbsDiffArgument = Single.Parse(arr[0]);
                    accuracyItem.MaxAbsDiffValue = Single.Parse(arr[1]);
                    accuracyItem.MaxAbsDiffValueVmlHa = Single.Parse(arr[2]);
                    accuracyItem.MaxAbsDiffValueVmlEp = Single.Parse(arr[3]);

                    Benchmark.AccuracyResults.Add(accuracyItem);
                }
                line = reader.ReadLine();
                arr = line.Split(' ');
                Benchmark.MinCoefOfExecVmlHa = Single.Parse(arr[0]);
                Benchmark.MinCoefOfExecVmlEp = Single.Parse(arr[1]);
                OnPropertyChanged("Benchmark");
            }
        }
        
    }
}
