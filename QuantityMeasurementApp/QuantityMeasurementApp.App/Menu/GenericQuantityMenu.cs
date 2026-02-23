using System;

namespace QuantityMeasurementApp.App
{
    // UC10: One menu for any Quantity<UnitEnum> category (LengthUnit, WeightUnit, ...)
    public sealed class GenericQuantityMenu
    {
        public static void Menu<U>(string title) where U : struct, Enum
        {
            bool flag = true;
            Console.WriteLine($"WELCOME TO {title}\n");

            while (flag)
            {
                Console.WriteLine("1. Measurement Equality");
                Console.WriteLine("2. Unit-To-Unit Conversion");
                Console.WriteLine("3. Add Two Units (result in first unit)");
                Console.WriteLine("4. Add Two Units (result in target unit)");
                Console.WriteLine("5. Subtract Two Units (result in first unit)");
                Console.WriteLine("6. Subtract Two Units (result in target unit)");
                Console.WriteLine("7. Divide Two Units (result in first unit)");
                Console.WriteLine("8. Divide Two Units (result in target unit)");
                Console.WriteLine("9. Exit");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid Input");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        IsEqual<U>();
                        break;
                    case 2:
                        ConvertToUnit<U>();
                        break;
                    case 3:
                        AddUnit<U>();
                        break;
                    case 4:
                        AddUnitToSpecific<U>();
                        break;
                    case 5:                 //UC-12 subtract method 
                        SubtractToUnit<U>();
                        break;
                    case 6:
                        SubtractToSpecificUnit<U>();
                        break;
                    case 7:                 //UC-12 subtract method 
                        DivideToUnit<U>();
                        break;
                    case 8:
                        DivideToSpecificUnit<U>();
                        break;
                    case 9:
                        flag = false;
                        Console.WriteLine("Thanks for visiting");
                        break;
                    default:
                        Console.WriteLine("Invalid Input");
                        break;
                }
            }
        }

        private static void IsEqual<U>() where U : struct, Enum
        {
            try
            {
                Console.Write("Enter first value: ");
                double v1 = double.Parse(Console.ReadLine()!);

                Console.Write("Enter first unit: ");
                string u1Text = Console.ReadLine()!;

                Console.Write("Enter second value: ");
                double v2 = double.Parse(Console.ReadLine()!);

                Console.Write("Enter second unit: ");
                string u2Text = Console.ReadLine()!;

                if (!TryParseUnit(u1Text, out U u1) || !TryParseUnit(u2Text, out U u2))
                {
                    PrintAllowedUnits<U>();
                    return;
                }

                var q1 = new QuantityMeasurementApp.Core.Entity.Quantity<U>(v1, u1);
                var q2 = new QuantityMeasurementApp.Core.Entity.Quantity<U>(v2, u2);

                Console.WriteLine("Result: " + q1.Equals(q2));
            }
            catch
            {
                Console.WriteLine("Invalid Input");
            }
        }

        private static void ConvertToUnit<U>() where U : struct, Enum
        {
            try
            {
                Console.Write("Enter value: ");
                double v = double.Parse(Console.ReadLine()!);

                Console.Write("Enter Source Unit: ");
                string sourceText = Console.ReadLine()!;

                Console.Write("Enter Target Unit: ");
                string targetText = Console.ReadLine()!;

                if (!TryParseUnit(sourceText, out U source) || !TryParseUnit(targetText, out U target))
                {
                    PrintAllowedUnits<U>();
                    return;
                }

                var q = new QuantityMeasurementApp.Core.Entity.Quantity<U>(v, source);
                double converted = q.ConvertTo(target);

                Console.WriteLine($"Result: {converted} {target}");
            }
            catch
            {
                Console.WriteLine("Invalid Input");
            }
        }

        private static void AddUnit<U>() where U : struct, Enum
        {
            try
            {
                Console.Write("Enter first value: ");
                double v1 = double.Parse(Console.ReadLine()!);

                Console.Write("Enter first unit: ");
                string u1Text = Console.ReadLine()!;

                Console.Write("Enter second value: ");
                double v2 = double.Parse(Console.ReadLine()!);

                Console.Write("Enter second unit: ");
                string u2Text = Console.ReadLine()!;

                if (!TryParseUnit(u1Text, out U u1) || !TryParseUnit(u2Text, out U u2))
                {
                    PrintAllowedUnits<U>();
                    return;
                }

                var q1 = new QuantityMeasurementApp.Core.Entity.Quantity<U>(v1, u1);
                var q2 = new QuantityMeasurementApp.Core.Entity.Quantity<U>(v2, u2);

                Console.WriteLine("Result: " + q1.AddUnitTO(q2));
            }
            catch
            {
                Console.WriteLine("Invalid Input");
            }
        }

        private static void AddUnitToSpecific<U>() where U : struct, Enum
        {
            try
            {
                Console.Write("Enter first value: ");
                double v1 = double.Parse(Console.ReadLine()!);

                Console.Write("Enter first unit: ");
                string u1Text = Console.ReadLine()!;

                Console.Write("Enter second value: ");
                double v2 = double.Parse(Console.ReadLine()!);

                Console.Write("Enter second unit: ");
                string u2Text = Console.ReadLine()!;

                Console.Write("Enter target unit: ");
                string targetText = Console.ReadLine()!;

                if (!TryParseUnit(u1Text, out U u1) ||
                    !TryParseUnit(u2Text, out U u2) ||
                    !TryParseUnit(targetText, out U target))
                {
                    PrintAllowedUnits<U>();
                    return;
                }

                var q1 = new QuantityMeasurementApp.Core.Entity.Quantity<U>(v1, u1);
                var q2 = new QuantityMeasurementApp.Core.Entity.Quantity<U>(v2, u2);

                Console.WriteLine("Result: " +
                    QuantityMeasurementApp.Core.Entity.Quantity<U>.AddToSpecificUnit(q1, q2, target));
            }
            catch
            {
                Console.WriteLine("Invalid Input");
            }
        }
        private static void SubtractToUnit<U>() where U : struct, Enum
        {
            try
            {
                Console.Write("Enter first value: ");
                double v1 = double.Parse(Console.ReadLine()!);

                Console.Write("Enter first unit: ");
                string u1Text = Console.ReadLine()!;

                Console.Write("Enter second value: ");
                double v2 = double.Parse(Console.ReadLine()!);

                Console.Write("Enter second unit: ");
                string u2Text = Console.ReadLine()!;

                if (!TryParseUnit(u1Text, out U u1) ||
                    !TryParseUnit(u2Text, out U u2) )
                {
                    PrintAllowedUnits<U>();
                    return;
                }

                var q1 = new QuantityMeasurementApp.Core.Entity.Quantity<U>(v1, u1);
                var q2 = new QuantityMeasurementApp.Core.Entity.Quantity<U>(v2, u2);

                Console.WriteLine("Result: " + q1.SubtractUnitTo(q2));
            }
            catch
            {
                Console.WriteLine("Invalid Input");
            }
        }
        private static void SubtractToSpecificUnit<U>() where U : struct, Enum
        {
            try
            {
                Console.Write("Enter first value: ");
                double v1 = double.Parse(Console.ReadLine()!);

                Console.Write("Enter first unit: ");
                string u1Text = Console.ReadLine()!;

                Console.Write("Enter second value: ");
                double v2 = double.Parse(Console.ReadLine()!);

                Console.Write("Enter second unit: ");
                string u2Text = Console.ReadLine()!;

                Console.Write("Enter target unit: ");
                string targetText = Console.ReadLine()!;

                if (!TryParseUnit(u1Text, out U u1) ||
                    !TryParseUnit(u2Text, out U u2) ||
                    !TryParseUnit(targetText, out U target))
                {
                    PrintAllowedUnits<U>();
                    return;
                }

                var q1 = new QuantityMeasurementApp.Core.Entity.Quantity<U>(v1, u1);
                var q2 = new QuantityMeasurementApp.Core.Entity.Quantity<U>(v2, u2);

                Console.WriteLine("Result: " + q1.SubtractUnitTo(q2, target));
            }
            catch
            {
                Console.WriteLine("Invalid Input");
            }
        }

        private static void DivideToUnit<U>() where U : struct, Enum
        {
            try
            {
                Console.Write("Enter first value: ");
                double v1 = double.Parse(Console.ReadLine()!);

                Console.Write("Enter first unit: ");
                string u1Text = Console.ReadLine()!;

                Console.Write("Enter second value: ");
                double v2 = double.Parse(Console.ReadLine()!);

                Console.Write("Enter second unit: ");
                string u2Text = Console.ReadLine()!;

                if (!TryParseUnit(u1Text, out U u1) ||
                    !TryParseUnit(u2Text, out U u2))
                {
                    PrintAllowedUnits<U>();
                    return;
                }

                var q1 = new QuantityMeasurementApp.Core.Entity.Quantity<U>(v1, u1);
                var q2 = new QuantityMeasurementApp.Core.Entity.Quantity<U>(v2, u2);

                Console.WriteLine("Result: " + q1.DivideUnitTo(q2));
            }
            catch
            {
                Console.WriteLine("Invalid Input");
            }
        }
        private static void DivideToSpecificUnit<U>() where U : struct, Enum
        {
            try
            {
                Console.Write("Enter first value: ");
                double v1 = double.Parse(Console.ReadLine()!);

                Console.Write("Enter first unit: ");
                string u1Text = Console.ReadLine()!;

                Console.Write("Enter second value: ");
                double v2 = double.Parse(Console.ReadLine()!);

                Console.Write("Enter second unit: ");
                string u2Text = Console.ReadLine()!;

                Console.Write("Enter target unit: ");
                string targetText = Console.ReadLine()!;

                if (!TryParseUnit(u1Text, out U u1) ||
                    !TryParseUnit(u2Text, out U u2) ||
                    !TryParseUnit(targetText, out U target))
                {
                    PrintAllowedUnits<U>();
                    return;
                }

                var q1 = new QuantityMeasurementApp.Core.Entity.Quantity<U>(v1, u1);
                var q2 = new QuantityMeasurementApp.Core.Entity.Quantity<U>(v2, u2);

                Console.WriteLine("Result: " + q1.DivideUnitTo(q2, target));
            }
            catch
            {
                Console.WriteLine("Invalid Input");
            }
        }
        private static bool TryParseUnit<U>(string text, out U unit) where U : struct, Enum
        {
            return Enum.TryParse(text, ignoreCase: true, out unit);
        }

        private static void PrintAllowedUnits<U>() where U : struct, Enum
        {
            Console.WriteLine("Invalid Unit. Allowed: " + string.Join(", ", Enum.GetNames(typeof(U))));
        }
    }
}