using QuantityMeasurementApp.ApplicationLayer.Menu;
using QuantityMeasurementApp.ModelLayer.Enums;
using QuantityMeasurementApp.RepositoryLayer.Interfaces;
using QuantityMeasurementApp.RepositoryLayer.Records;
using System;

namespace QuantityMeasurementApp.ApplicationLayer.Menu
{
    public class AppMenu
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IQuantityHistoryRepository _historyRepository;

        public AppMenu(IServiceProvider serviceProvider, IQuantityHistoryRepository historyRepository)
        {
            _serviceProvider = serviceProvider;
            _historyRepository = historyRepository;
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
                Console.WriteLine("5. VIEW HISTORY");
                Console.WriteLine("6. VIEW HISTORY BY ID");
                Console.WriteLine("7. DELETE HISTORY");
                Console.WriteLine("8. EXIT");
                Console.Write("\nSelect an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid Input");
                    Pause();
                    continue;
                }

                flag = ExecuteChoice(choice);

                if (flag)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
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
                    ViewAllHistory();
                    break;

                case 6:
                    ViewHistoryById();
                    break;

                case 7:
                    DeleteHistory();
                    break;
                case 8:
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
            if(menu is null)
            {
                Console.WriteLine("Unable to load menu");
                Console.WriteLine("\nPress any Key to Continue");
                Console.ReadKey();
                return;
            }
            menu?.Show(title);
        }
            
        private void ViewAllHistory()
        {
            List<QuantityHistoryRecord> records = _historyRepository.GetAllRecords();

            if (records.Count == 0)
            {
                Console.WriteLine("No history found.");
                return;
            }

            foreach (QuantityHistoryRecord record in records)
            {
                Console.WriteLine(record);
            }
        }

        private void ViewHistoryById()
        {
            Console.Write("Enter record id: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid id.");
                return;
            }

            QuantityHistoryRecord? record = _historyRepository.GetRecordById(id);

            if (record == null)
            {
                Console.WriteLine("Record not found.");
                return;
            }

            Console.WriteLine(record);
        }

        private void DeleteHistory()
        {
            Console.Write("Enter record id to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid id.");
                return;
            }

            bool deleted = _historyRepository.DeleteRecord(id);

            Console.WriteLine(deleted
                ? "Record deleted successfully."
                : "Record not found or could not be deleted.");
        }
        private void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}