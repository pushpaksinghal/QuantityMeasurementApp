using System;
using System.Collections.Generic;
using System.Text;
using QuantityMeasurementApp.Core.Entity;

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
                Console.WriteLine("2.Exit");
                int choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        isEqualFeet();
                        break;
                    case 2:
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
    }
}
