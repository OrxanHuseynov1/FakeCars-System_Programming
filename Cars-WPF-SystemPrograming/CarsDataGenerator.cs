using Bogus;
using System.IO;
using System.Text.Json;

namespace Cars_WPF_SystemPrograming;

public class CarsDataGenerator
{
    public static List<Car> GeneratorFakeCars(string vendor,int CarsCount)
    {
        var faker = new Faker<Car>()
            .RuleFor(c => c.ImagePath, f => f.Image.LoremFlickrUrl(250, 250, "car"))
            .RuleFor(c => c.Model, f => f.Vehicle.Model())
            .RuleFor(c => c.Vendor, vendor) 
            .RuleFor(c => c.Year, f => f.Date.Past(10).Year);

        return faker.Generate(CarsCount);
    }

    public static void WriteJson(List<Car> cars, string filePath)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(cars, options);
        File.WriteAllText(filePath,jsonString);
    }
}
