using Bogus;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace Cars_WPF_SystemPrograming;

public partial class MainWindow : Window
{
    private static object _sync = new object();
    private static List<string> fileNames = ["Mercedes","Bmw", "Toyota", "Kia", "Ford"];
    CancellationTokenSource cts;

    public MainWindow()
    {
        InitializeComponent();

        var brandsAndFiles = new Dictionary<string, string> {};

        foreach (var file in fileNames)
            brandsAndFiles.Add(file, $"{file}.json");
        



        foreach (var carName in brandsAndFiles)
        {
            string vendor = carName.Key;
            string filePath = carName.Value;


            if (!File.Exists(filePath))
            {
                List<Car> fakeCars = CarsDataGenerator.GeneratorFakeCars(vendor, 100);

                CarsDataGenerator.WriteJson(fakeCars, filePath);    
            }
        }

    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Stopwatch stopwatch = new Stopwatch();
        CarsList.Items.Clear();
        stopwatch.Restart();

        if(IsSingle.IsChecked == true)
        {
            singleMethod(stopwatch);
        }
        else
        {
            multiMethod(stopwatch);
        }
    }




    private void singleMethod(Stopwatch stopwatch)
    {
        stopwatch.Start();
        Thread thread = new Thread(() =>
        {
            lock (_sync)
            {
                List<Car> Cars = [];

                foreach (var file in fileNames)
                {
                    string fileRead = File.ReadAllText(file + ".json");
                    Cars = JsonSerializer.Deserialize<List<Car>>(fileRead) ?? new()!;

                    foreach (var car in Cars)
                        Dispatcher.Invoke(() => CarsList.Items.Add(car));
                }

                stopwatch.Stop();
                double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
                Dispatcher.Invoke(() => Time.Text = elapsedSeconds.ToString());
            }


        });
        thread.Start();
    }



    private void multiMethod(Stopwatch stopwatch)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.FullName + @"\Json");

        foreach (var item in fileNames)
        {
            ThreadPool.QueueUserWorkItem((o) => {

                cts = new CancellationTokenSource();
                stopwatch.Start();
                var filedirectory = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.FullName + @"\Json\";

                var car = File.ReadAllText(item + ".json");
                ObservableCollection<Car> tempCars = JsonSerializer.Deserialize<ObservableCollection<Car>>(car)!;

                for (int i = 0; i < tempCars.Count; i++)
                {
                    if (cts.Token.IsCancellationRequested) break;
                    Thread.Sleep(800);

                    stopwatch.Stop();
                    Dispatcher.Invoke(new Action(() =>
                    {
                        Time.Text = stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
                        CarsList.Items.Add(tempCars[i]);
                    }));
                }

            });
        }

    }

}
