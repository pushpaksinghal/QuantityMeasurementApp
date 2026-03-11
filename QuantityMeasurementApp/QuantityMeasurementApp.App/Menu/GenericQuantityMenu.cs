
using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.ModelLayer.Entity;
using System;

namespace QuantityMeasurementApp.ApplicationLayer.Menu
{
    public class GenericQuantityMenu<T> where T : struct, Enum
    {
        private readonly IQuantityConversionService _conversionService;
        private readonly IQuantityArithmeticService _arithmeticService;
        private readonly QuantityEqualityComparer<T> _equalityComparer;
        private readonly QuantityValidationService _validator;
        private readonly string _unitTypeName;

        public GenericQuantityMenu(
            IQuantityConversionService conversionService,
            IQuantityArithmeticService arithmeticService,
            QuantityEqualityComparer<T> equalityComparer,
            QuantityValidationService validator)
        {
            _conversionService = conversionService;
            _arithmeticService = arithmeticService;
            _equalityComparer = equalityComparer;
            _validator = validator;
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

        private Quantity<T> ReadQuantity(string prompt)
        {
            Console.Write($"Enter {prompt} value: ");
            if (!double.TryParse(Console.ReadLine(), out double value))
                throw new ArgumentException("Invalid value format");

            Console.Write($"Enter {prompt} unit: ");
            string unitText = Console.ReadLine()!;

            if (!TryParseUnit(unitText, out T unit))
            {
                PrintAllowedUnits();
                throw new ArgumentException("Invalid unit");
            }

            return new Quantity<T>(value, unit);
        }

        private void CheckEquality()
        {
            Console.WriteLine("\n--- Equality Check ---");
            var q1 = ReadQuantity("first");
            var q2 = ReadQuantity("second");

            bool areEqual = _equalityComparer.Equals(q1, q2);
            Console.WriteLine($"\nResult: {q1.Value} {q1.Unit} {(areEqual ? "==" : "!=")} {q2.Value} {q2.Unit}");
        }

        private void ConvertUnit()
        {
            Console.WriteLine("\n--- Unit Conversion ---");
            var quantity = ReadQuantity("source");

            Console.Write("Enter target unit: ");
            string targetText = Console.ReadLine()!;

            if (!TryParseUnit(targetText, out T target))
            {
                PrintAllowedUnits();
                return;
            }

            var result = _conversionService.ConvertTo(quantity, target);
            Console.WriteLine($"\nResult: {quantity.Value} {quantity.Unit} = {result.Value} {result.Unit}");
        }

        private void AddUnits(bool useTargetUnit)
        {
            Console.WriteLine(useTargetUnit ? "\n--- Addition to Specific Unit ---" : "\n--- Addition ---");

            var q1 = ReadQuantity("first");
            var q2 = ReadQuantity("second");

            Quantity<T> result;

            if (useTargetUnit)
            {
                Console.Write("Enter target unit: ");
                string targetText = Console.ReadLine()!;

                if (!TryParseUnit(targetText, out T target))
                {
                    PrintAllowedUnits();
                    return;
                }

                result = _arithmeticService.AddToSpecificUnit(q1, q2, target);
                Console.WriteLine($"\nResult: {q1.Value} {q1.Unit} + {q2.Value} {q2.Unit} = {result.Value} {result.Unit}");
            }
            else
            {
                result = _arithmeticService.AddUnit(q1, q2);
                Console.WriteLine($"\nResult: {q1.Value} {q1.Unit} + {q2.Value} {q2.Unit} = {result.Value} {result.Unit}");
            }
        }

        private void SubtractUnits(bool useTargetUnit)
        {
            Console.WriteLine(useTargetUnit ? "\n--- Subtraction to Specific Unit ---" : "\n--- Subtraction ---");

            var q1 = ReadQuantity("first");
            var q2 = ReadQuantity("second");

            if (useTargetUnit)
            {
                Console.Write("Enter target unit: ");
                string targetText = Console.ReadLine()!;

                if (!TryParseUnit(targetText, out T target))
                {
                    PrintAllowedUnits();
                    return;
                }

                var result = _arithmeticService.SubtractUnit(q1, q2, target);
                Console.WriteLine($"\nResult: {q1.Value} {q1.Unit} - {q2.Value} {q2.Unit} = {result.Value} {result.Unit}");
            }
            else
            {
                var result = _arithmeticService.SubtractUnit(q1, q2, q1.Unit);
                Console.WriteLine($"\nResult: {q1.Value} {q1.Unit} - {q2.Value} {q2.Unit} = {result.Value} {result.Unit}");
            }
        }

        private void DivideUnits()
        {
            Console.WriteLine("\n--- Division ---");

            var q1 = ReadQuantity("dividend (first)");
            var q2 = ReadQuantity("divisor (second)");

            double result = _arithmeticService.DivideUnit(q1, q2);
            Console.WriteLine($"\nResult: {q1.Value} {q1.Unit} ÷ {q2.Value} {q2.Unit} = {result:F4} (dimensionless)");
        }

        private bool TryParseUnit(string text, out T unit)
        {
            return Enum.TryParse(text, ignoreCase: true, out unit);
        }

        private void PrintAllowedUnits()
        {
            Console.WriteLine("Invalid Unit. Allowed: " + string.Join(", ", Enum.GetNames(typeof(T))));
        }
    }
}