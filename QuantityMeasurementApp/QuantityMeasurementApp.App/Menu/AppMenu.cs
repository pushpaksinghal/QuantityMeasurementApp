using QuantityMeasurementApp.ApplicationLayer.Menu;
using QuantityMeasurementApp.ModelLayer.Enums;
using System;

namespace QuantityMeasurementApp.ApplicationLayer.Menu
{
    public class AppMenu
    {
        private readonly IServiceProvider _serviceProvider;

        public AppMenu(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Show()
        {
            bool flag = true;
            while (flag)
            {
                Console.Clear();
                Console.WriteLine("WELCOME TO THE QUANTITY MANAGEMENT APP");
                Console.WriteLine("======================================");
                Console.WriteLine("1. LENGTH");
                Console.WriteLine("2. WEIGHT");
                Console.WriteLine("3. VOLUME");
                Console.WriteLine("4. TEMPERATURE");
                Console.WriteLine("5. EXIT");
                Console.Write("\nSelect an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid Input");
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    continue;
                }

                flag = ExecuteChoice(choice);
            }
        }

        private bool ExecuteChoice(int choice)
        {
            switch (choice)
            {
                case 1:
                    ShowQuantityMenu<LengthUnit>("Length");
                    break;
                case 2:
                    ShowQuantityMenu<WeightUnit>("Weight");
                    break;
                case 3:
                    ShowQuantityMenu<VolumeUnit>("Volume");
                    break;
                case 4:
                    ShowQuantityMenu<TemperatureUnit>("Temperature");
                    break;
                case 5:
                    Console.WriteLine("\nTHANKS FOR VISITING!");
                    return false;
                default:
                    Console.WriteLine("Invalid Input");
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    break;
            }
            return true;
        }

        private void ShowQuantityMenu<T>(string title) where T : struct, Enum
        {
            var menu = _serviceProvider.GetService(typeof(GenericQuantityMenu<T>)) as GenericQuantityMenu<T>;
            menu?.Show(title);
        }
    }
}