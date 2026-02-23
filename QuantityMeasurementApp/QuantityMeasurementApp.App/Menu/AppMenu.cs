using System;
using System.Collections.Generic;
using System.Text;

namespace QuantityMeasurementApp.App.Menu
{
    public sealed class AppMenu
    {
        public void Menu()
        {
            bool flag = true;
            while (flag)
            {
                Console.WriteLine("WELCOME TO THE QUANTITY MENAGEMENT APP");
                Console.WriteLine("1.LENGTH");
                Console.WriteLine("2.WEIGHT");
                Console.WriteLine("3.EXIT");
                int choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1: QuantityMenuLength.Menu(); break;
                    case 2: QuantityMenuWeight.Menu(); break;
                    case 3: flag = false; Console.WriteLine("THANKS FOR WISITING"); break;
                    default: flag = false; break;
                }
            }
        }
    }
}
