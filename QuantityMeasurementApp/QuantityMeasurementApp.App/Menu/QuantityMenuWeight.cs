using QuantityMeasurementApp.Core.Entity;
using System;

namespace QuantityMeasurementApp.App.Menu
{
    public sealed class QuantityMenuWeight
    {
        public static  void Menu()
        {
            bool flag = true;
            Console.WriteLine("WEIGHT MENU\n");

            while (flag)
            {
                Console.WriteLine("1.Weight Equality");
                Console.WriteLine("2.Weight Unit-To-Unit Conversion");
                Console.WriteLine("3.Add Two Weight Units");
                Console.WriteLine("4.Add Two Weight Units to specific unit");
                Console.WriteLine("5.Exit");

                int choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1: IsEqualWeight(); break;
                    case 2: ConvertToUnit(); break;
                    case 3: AddUnit(); break;
                    case 4: AddUnitToSpecific(); break;
                    case 5:
                        flag = false;
                        Console.WriteLine("Thanks for visiting");
                        break;
                    default:
                        Console.Error.WriteLine("Invalid Input");
                        break;
                }
            }
        }

        private static void IsEqualWeight()
        {
            try
            {
                Console.Write("Enter first value: ");
                double v1 = double.Parse(Console.ReadLine());
                Console.Write("Enter first unit (Kilogram/Gram/Pound): ");
                string u1Text = Console.ReadLine();

                Console.Write("Enter second value: ");
                double v2 = double.Parse(Console.ReadLine());
                Console.Write("Enter second unit (Kilogram/Gram/Pound): ");
                string u2Text = Console.ReadLine();

                if (!Enum.TryParse(u1Text, ignoreCase: true, out WeightUnit u1) || !Enum.TryParse(u2Text, ignoreCase: true, out WeightUnit u2))
                {
                    Console.WriteLine("Invalid Unit. Allowed: Kilogram, Gram, Pound");
                    return;
                }

                var w1 = new Weight(v1, u1);
                var w2 = new Weight(v2, u2);

                Console.WriteLine("Result: " + w1.Equals(w2));
            }
            catch
            {
                Console.WriteLine("Invalid Input");
            }
        }

        private static void ConvertToUnit()
        {
            try
            {
                Console.Write("Enter value: ");
                double v = double.Parse(Console.ReadLine());

                Console.Write("Enter Source Unit (Kilogram/Gram/Pound): ");
                string sourceText = Console.ReadLine();

                Console.Write("Enter Target Unit (Kilogram/Gram/Pound): ");
                string targetText = Console.ReadLine();

                if (!Enum.TryParse(sourceText, ignoreCase: true, out WeightUnit source) || !Enum.TryParse(targetText, ignoreCase: true, out WeightUnit target))
                {
                    Console.WriteLine("Invalid Unit. Allowed: Kilogram, Gram, Pound");
                    return;
                }

                var w = new Weight(v, source);
                double converted = w.ConvertTo(target);

                Console.WriteLine($"Result: {converted} {target}");
            }
            catch
            {
                Console.WriteLine("Invalid Input");
            }
        }

        private static void AddUnit()
        {
            try
            {
                Console.Write("Enter first value: ");
                double v1 = double.Parse(Console.ReadLine());
                Console.Write("Enter first unit (Kilogram/Gram/Pound): ");
                string u1Text = Console.ReadLine();

                Console.Write("Enter second value: ");
                double v2 = double.Parse(Console.ReadLine());
                Console.Write("Enter second unit (Kilogram/Gram/Pound): ");
                string u2Text = Console.ReadLine();

                if (!Enum.TryParse(u1Text, ignoreCase: true, out WeightUnit u1) || !Enum.TryParse(u2Text, ignoreCase: true, out WeightUnit u2))
                {
                    Console.WriteLine("Invalid Unit. Allowed: Kilogram, Gram, Pound");
                    return;
                }

                var w1 = new Weight(v1, u1);
                var w2 = new Weight(v2, u2);

                Console.WriteLine("Result: " + w1.AddUnitTO(w2));
            }
            catch
            {
                Console.WriteLine("Invalid Input");
            }
        }

        private static void AddUnitToSpecific()
        {
            try
            {
                Console.Write("Enter first value: ");
                double v1 = double.Parse(Console.ReadLine());
                Console.Write("Enter first unit (Kilogram/Gram/Pound): ");
                string u1Text = Console.ReadLine();

                Console.Write("Enter second value: ");
                double v2 = double.Parse(Console.ReadLine());
                Console.Write("Enter second unit (Kilogram/Gram/Pound): ");
                string u2Text = Console.ReadLine();

                Console.Write("Enter target unit (Kilogram/Gram/Pound): ");
                string u3Text = Console.ReadLine();

                if (!Enum.TryParse(u1Text, ignoreCase: true, out WeightUnit u1) || !Enum.TryParse(u2Text, ignoreCase: true, out WeightUnit u2) || !Enum.TryParse(u3Text, ignoreCase: true, out WeightUnit u3))
                {
                    Console.WriteLine("Invalid Unit. Allowed: Kilogram, Gram, Pound");
                    return;
                }

                var w1 = new Weight(v1, u1);
                var w2 = new Weight(v2, u2);

                Console.WriteLine("Result: " + w1.AddSpicificTo(w2, u3));
            }
            catch
            {
                Console.WriteLine("Invalid Input");
            }
        }
    }
}