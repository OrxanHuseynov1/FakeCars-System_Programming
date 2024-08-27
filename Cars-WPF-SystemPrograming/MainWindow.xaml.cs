﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace Cars_WPF_SystemPrograming;

public partial class MainWindow : Window
{
    private static object _sync = new object();
    private static List<string> fileNames = ["Mercedes","Bmw", "Toyota", "Kia", "Ford"];
    public MainWindow()
    {
        InitializeComponent();

        var brandsAndFiles = new Dictionary<string, string>
        {
            //{ "Mercedes",fileNames[0] },
            //{"BMW",fileNames[1]},
            //{"Toyota","toyota.json" },
            //{"Kia","kia.json" },
            //{"Ford","ford.json" }
        };

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
        if(IsSingle.IsChecked == true)
        {

            Stopwatch stopwatch = new Stopwatch();
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
                            Dispatcher.Invoke(() => ListView.Items.Add(car));
                    }

                    stopwatch.Stop();
                    double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
                    Dispatcher.Invoke(() => Time.Text = elapsedSeconds.ToString());
                }


            });
            thread.Start();
            
        }
    }
}