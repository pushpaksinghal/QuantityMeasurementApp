using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.ModelLayer.DTOs;
using System;

namespace QuantityMeasurementApp.ApplicationLayer.Menu
{
    public class GenericQuantityMenu<T> where T : struct, Enum
    {
        private readonly QuantityApplicationService _applicationService;
        private readonly QuantityEqualityComparer<T> _equalityComparer;
        private readonly string _unitTypeName;

        public GenericQuantityMenu(
            QuantityApplicationService applicationService,
            QuantityEqualityComparer<T> equalityComparer)
        {
            _applicationService = applicationService;
            _equalityComparer = equalityComparer;
            _unitTypeName = typeof(T).Name.Replace("Unit", "");
        }

        public void Show(string title)
        {
            bool flag = true;

            while (flag)
            {
                Console.Clear();
                Console.WriteLine($"WELCOME TO {title}\n");
                Console.WriteLine("1. Equality Check");
                Console.WriteLine("2. Unit-To-Unit Conversion");
                Console.WriteLine("3. Add Two Units (result in first unit)");
                Console.WriteLine("4. Add Two Units (result in target unit)");
                Console.WriteLine("5. Subtract Two Units (result in first unit)");
                Console.WriteLine("6. Subtract Two Units (result in target unit)");
                Console.WriteLine("7. Divide Two Units");
                Console.WriteLine("8. Exit");
                Console.Write("\nSelect an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid Input");
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    continue;
                }

                try
                {
                    flag = ExecuteChoice(choice);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
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
                    CheckEquality();
                    break;
                case 2:
                    ConvertUnit();
                    break;
                case 3:
                    AddUnits(useTargetUnit: false);
                    break;
                case 4:
                    AddUnits(useTargetUnit: true);
                    break;
                case 5:
                    SubtractUnits(useTargetUnit: false);
                    break;
                case 6:
                    SubtractUnits(useTargetUnit: true);
                    break;
                case 7:
                    DivideUnits();
                    break;
                case 8:
                    Console.WriteLine("Thanks for visiting");
                    return false;
                default:
                    Console.WriteLine("Invalid Input");
                    break;
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            return true;
        }

        private QuantityDTO ReadQuantityDto(string prompt)
        {
            Console.Write($"Enter {prompt} value: ");
            if (!double.TryParse(Console.ReadLine(), out double value))
                throw new ArgumentException("Invalid value format.");

            Console.Write($"Enter {prompt} unit: ");
            string unitText = Console.ReadLine() ?? string.Empty;

            if (!TryParseUnit(unitText))
            {
                PrintAllowedUnits();
                throw new ArgumentException("Invalid unit.");
            }

            return new QuantityDTO
            {
                Value = value,
                Unit = unitText
            };
        }

        private void CheckEquality()
        {
            Console.WriteLine("\n--- Equality Check ---");

            var request = new BinaryQuantityRequestDto
            {
                FirstQuantity = ReadQuantityDto("first"),
                SecondQuantity = ReadQuantityDto("second")
            };

            var result = _applicationService.CheckEquality<T>(request, _equalityComparer);
            Console.WriteLine($"\nResult: {result.Message}");
        }

        private void ConvertUnit()
        {
            Console.WriteLine("\n--- Unit Conversion ---");

            var source = ReadQuantityDto("source");

            Console.Write("Enter target unit: ");
            string targetText = Console.ReadLine() ?? string.Empty;

            if (!TryParseUnit(targetText))
            {
                PrintAllowedUnits();
                return;
            }

            var request = new ConversionRequestDto
            {
                SourceQuantity = source,
                TargetUnit = targetText
            };

            var result = _applicationService.ConvertUnit<T>(request);
            Console.WriteLine($"\nResult: {result.Message}");
        }

        private void AddUnits(bool useTargetUnit)
        {
            Console.WriteLine(useTargetUnit
                ? "\n--- Addition to Specific Unit ---"
                : "\n--- Addition ---");

            var request = new BinaryQuantityRequestDto
            {
                FirstQuantity = ReadQuantityDto("first"),
                SecondQuantity = ReadQuantityDto("second")
            };

            if (useTargetUnit)
            {
                Console.Write("Enter target unit: ");
                string targetText = Console.ReadLine() ?? string.Empty;

                if (!TryParseUnit(targetText))
                {
                    PrintAllowedUnits();
                    return;
                }

                request.TargetUnit = targetText;
                var result = _applicationService.AddUnitsToTarget<T>(request);
                Console.WriteLine($"\nResult: {result.Message}");
            }
            else
            {
                var result = _applicationService.AddUnits<T>(request);
                Console.WriteLine($"\nResult: {result.Message}");
            }
        }

        private void SubtractUnits(bool useTargetUnit)
        {
            Console.WriteLine(useTargetUnit
                ? "\n--- Subtraction to Specific Unit ---"
                : "\n--- Subtraction ---");

            var request = new BinaryQuantityRequestDto
            {
                FirstQuantity = ReadQuantityDto("first"),
                SecondQuantity = ReadQuantityDto("second")
            };

            if (useTargetUnit)
            {
                Console.Write("Enter target unit: ");
                string targetText = Console.ReadLine() ?? string.Empty;

                if (!TryParseUnit(targetText))
                {
                    PrintAllowedUnits();
                    return;
                }

                request.TargetUnit = targetText;
                var result = _applicationService.SubtractUnitsToTarget<T>(request);
                Console.WriteLine($"\nResult: {result.Message}");
            }
            else
            {
                var result = _applicationService.SubtractUnits<T>(request);
                Console.WriteLine($"\nResult: {result.Message}");
            }
        }

        private void DivideUnits()
        {
            Console.WriteLine("\n--- Division ---");

            var request = new BinaryQuantityRequestDto
            {
                FirstQuantity = ReadQuantityDto("dividend (first)"),
                SecondQuantity = ReadQuantityDto("divisor (second)")
            };

            var result = _applicationService.DivideUnits<T>(request);
            Console.WriteLine($"\nResult: {result.Message}");
        }

        private bool TryParseUnit(string text)
        {
            return Enum.TryParse<T>(text, true, out _);
        }

        private void PrintAllowedUnits()
        {
            Console.WriteLine($"{_unitTypeName} Unit Invalid. Allowed: " +
                              string.Join(", ", Enum.GetNames(typeof(T))));
        }
    }
}