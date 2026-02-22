using QuantityMeasurementApp.Core.Entity;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;
using static QuantityMeasurementApp.Core.Entity.Length;

namespace QuantityMeasurementApp.App
{
    public sealed class QuantityMenu
    {
        //creating a menu class to showcase our work properly
        public void Menu()
        {
            bool flag = true;
            Console.WriteLine("WELCOME TO QUANTITY MANAGENMENT APP\n");
            while (flag)
            {

                Console.WriteLine("1.Feet Measurement Equality");
                Console.WriteLine("2.Inches Measurement Equality");
                Console.WriteLine("3.Generic Measurement Equality");
                Console.WriteLine("4.Unit-To-Unit Conversion");
                Console.WriteLine("5.Exit");
                int choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        isEqualFeet();
                        break;
                    case 2:
                        isEqualInches();
                        break;
                    case 3:
                        isEqualLength();
                        break;
                    case 4:
                        ConvertToUnit();
                        break;
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
        // making a seperate method to keep the menu simple and not over crouded
        //UC-1
        // these are not needed anymore
        private static  void isEqualFeet()
        {
            try
            {
                Console.Write("Enter first feet value: ");
                Feet f1 = new Feet(double.Parse(Console.ReadLine()));
                Console.Write("Enter second feet value: ");
                Feet f2 = new Feet(double.Parse(Console.ReadLine()));

                bool result = f1.Equals(f2);
                Console.WriteLine("Result: " + result);
            }
            catch
            {
                Console.WriteLine("Invalid Input");
            }
        }
        //UC-2
        //These are not needed anymore
        private static void isEqualInches()
        {
            try
            {
                Console.Write("Enter first Inch value: ");
                Inches f1 = new Inches(double.Parse(Console.ReadLine()));
                Console.Write("Enter second Inch value: ");
                Inches f2 = new Inches(double.Parse(Console.ReadLine()));

                bool result = f1.Equals(f2);
                Console.WriteLine("Result: " + result);
            }
            catch
            {
                Console.WriteLine("Invalid Input");
            }
        }
        //UC-3
        private static void isEqualLength()
        {
            try
            {
                Console.Write("Enter first value: ");
                double l1=double.Parse(Console.ReadLine());
                Console.Write("Enter first unit (Feet/Inch/Yard/Centimeters): ");
                string u1Text = Console.ReadLine();
                Console.Write("Enter second value: ");
                double l2=double.Parse(Console.ReadLine());
                Console.Write("Enter second unit (Feet/Inch/Yard/Centimeters): ");
                string u2Text = Console.ReadLine();

                if (!Enum.TryParse(u1Text, ignoreCase: true, out LengthUnit u1) || !Enum.TryParse(u2Text, ignoreCase: true, out LengthUnit u2))
                {
                    Console.WriteLine("Invalid Unit. Allowed: Feet, Inch ,Yard ,Centimeters");
                    return;
                }
                var v1 = new Length(l1, u1);
                var v2 = new Length(l2, u2);
                Console.WriteLine("Result: " + v1.Equals(v2));
            }
            catch
            {
                Console.WriteLine("Invalid Input");
            }
        }
        //UC-5
        private static void ConvertToUnit()
        {
            try
            {
                Console.Write("Enter value");
                double l1 = double.Parse(Console.ReadLine());
                Console.Write("Enter Source Unit");
                string u1Text = Console.ReadLine();
                Console.Write("Enter Target Unit");
                string u2Text = Console.ReadLine();

                if (!Enum.TryParse(u1Text, ignoreCase: true, out LengthUnit u1) || !Enum.TryParse(u2Text, ignoreCase: true, out LengthUnit u2))
                {
                    Console.WriteLine("Invalid Unit. Allowed: Feet, Inch ,Yard ,Centimeters");
                    return;
                }
                var v1 = new Length(l1, u1);
                double l2 = v1.ConvertTo(u2);

                Console.WriteLine("Result: " + l1 + " " + u2Text);
            }
            catch
            {
                Console.WriteLine("Invaild Input");
            }
        }


    }
}
