using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Customer[] customerArray = new Customer[30];
            int arrayIndex = 0;

            // Automobile customer
            Customer newCust = new Customer();
            newCust.FirstName = "Joe";
            newCust.LastName = "Smith";
            Order newOrder = new Order();
            newOrder.PlaceOrder(ModelName.BMW520);
            newOrder.SaveOrder(newCust, newOrder);
            customerArray[arrayIndex++] = newCust;
/*
            newCust = new Customer();
            newCust.FirstName = "Tom";
            newCust.LastName = "Cruise";
            newOrder = new Order();
            newOrder.PlaceOrder(ModelName.BMW235, 28500, 1); // special offer
            newCust.SaveOrder(newOrder);
            customerArray[arrayIndex++] = newCust;

            // Motocycle customer
            newCust = new Customer();
            newCust.FirstName = "Sally";
            newCust.LastName = "Jones";
            newOrder = new Order();
            newOrder.PlaceOrder(ModelName.HondaCruiser);
            newCust.SaveOrder(newOrder);
            customerArray[arrayIndex++] = newCust;

            newCust = new Customer();
            newCust.FirstName = "Rick";
            newCust.LastName = "White";
            newOrder = new Order();
            newOrder.PlaceOrder(ModelName.HondaSport, 17500.0, 2); // special offer
            newCust.SaveOrder(newOrder);
            customerArray[arrayIndex++] = newCust;
*/
            SaveAndPrint orders = new SaveAndPrint();
            foreach (var c in customerArray)
            {
                if ( c == null ) break;                     // question
                orders.SaveToFile(c);
            }

            orders.Print();
        }
    }

    public enum ModelName
    {
        // for Automobile
        BMW520, 
        BMW320, 
        BMW235,
        // for Motocycle
        HondaTouring, 
        HondaCruiser, 
        HondaSport 
    }

    interface IPrint
    {
        void Print();
    }
    interface IFileSave
    {
        void SaveToFile(string[] item);
    }


    //public class Customer
    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Order[] Orders = new Order[5];
        //private int numOrders = 0;

        /*
        public void SaveOrder(Order order)
        {
            Orders[numOrders++] = order;
        }
        */ 
    }

    public class Order
    {
        public string Model { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public Vehicle OrderedVehicle { get; set; }

        // needs to be changed -> method injection
        public void PlaceOrder(ModelName modelName)
        {
            switch (modelName)
            {
                case ModelName.BMW520:
                    this.PlaceOrder(modelName, 50000.0, 1);
                    break;
                case ModelName.BMW320:
                    this.PlaceOrder(modelName, 45000.0, 1);
                    break;
                case ModelName.BMW235:
                    this.PlaceOrder(modelName, 30000.0, 1);
                    break;
                case ModelName.HondaCruiser:
                    this.PlaceOrder(modelName, 25000.0, 1);
                    break;
                case ModelName.HondaSport:
                    this.PlaceOrder(modelName, 20000.0, 1);
                    break;
                case ModelName.HondaTouring:
                    this.PlaceOrder(modelName, 15000.0, 1);
                    break;
                default:
                    break;
            }
        }

        public void PlaceOrder(ModelName modelName, double price, int qualtity)
        {
            Model = modelName.ToString();
            Price = price;
            Quantity = qualtity;

            switch (modelName)
            {
                case ModelName.BMW520:
                    OrderedVehicle = new Automobile(250, "Automobile", 4.0, 6.0, 2.5);
                    break;
                case ModelName.BMW320:
                    OrderedVehicle = new Automobile(220, "Automobile", 3.5, 5.0, 2);
                    break;
                case ModelName.BMW235:
                    OrderedVehicle = new Automobile();
                    break;
                case ModelName.HondaCruiser:
                    OrderedVehicle = new Motocycle(70, "Motocycle", 1.5, 1.5);
                    break;
                case ModelName.HondaSport:
                    OrderedVehicle = new Motocycle(60, "Motocycle", 1.2, 1.2);
                    break;
                case ModelName.HondaTouring:
                    OrderedVehicle = new Motocycle();
                    break;
                default:
                    break;
            }
        }

        public void SaveOrder(Customer customer, Order order)
        {
            customer.Orders[customer.Orders.Length] = order;
        }

    }

    public class Vehicle
    {
        public int Wheels { get; set; }
        public int HorsePower { get; set; }
        public string Description { get; set; }
        public virtual double CargoLength { get; set; }
        public virtual double CargoWidth { get; set; }
        public virtual double CargoHeight { get; set; }

        public virtual double CargoCapacity()
        {
            return CargoLength * CargoWidth * CargoHeight;
        }
    }

    public class Automobile : Vehicle
    {
        public Automobile() : this(200, "Automobile", 3.0, 5.0, 2.0)
        {
        }
        public Automobile(int horsePower, string description, double length, double width, double height)
        {
            base.Wheels = 4;
            base.HorsePower = horsePower;
            base.Description = description;
            base.CargoLength = length;
            base.CargoWidth = width;
            base.CargoHeight = height;
        }
    }

    public class Motocycle : Vehicle
    {
        public override double CargoLength 
        { 
            get { return base.CargoLength; }
            set { setLengthAndWidth(value); }
        }
        public override double CargoWidth
        {
            get { return base.CargoWidth; }
            set { setLengthAndWidth(value); }
        }

        private void setLengthAndWidth(double value)
        {
            base.CargoLength = value;
            base.CargoWidth = value;
        }

        public Motocycle() : this(50, "Motocycle", 1.0, 1.0)
        {
        }
        public Motocycle(int horsePower, string description, double radius, double height)
        {
            base.Wheels = 2;
            base.HorsePower = horsePower;
            base.Description = description;
            base.CargoWidth = radius;
            base.CargoLength = radius;
            base.CargoHeight = height;
        }

        public override double CargoCapacity()
        {
            return (Math.PI * CargoWidth * CargoLength * CargoHeight) * 2; // Left and Right sides
        }
    }

    class  SaveAndPrint : IPrint, IFileSave
    {
        private string orderHistory = "C:\\Orders\\OrderHistory.txt";
        private string backupFile = "C:\\Orders\\OrderHistory.bak";

        public void Print()
        {
            if (File.Exists(orderHistory))
            {
                Console.WriteLine("File not exists - No order records");
            }

            StreamReader sr = new StreamReader(orderHistory);
            string nextLine;
            while ((nextLine = sr.ReadLine()) != null)         
            {
                Console.WriteLine(nextLine);
            }
        }

        public void SaveToFile(Customer customer)
        {            
            StringCollection linesCollection = readOrderFromCustomer(customer);
            string[] linesArray = new string[linesCollection.Count];
            linesCollection.CopyTo(linesArray,0);
            
            StreamWriter sw = new StreamWriter(orderHistory, true);  // create file if not existed, append
            
            foreach (var oneLine in linesArray)
                sw.WriteLine(oneLine);
            sw.Close();
        }
        private StringCollection readOrderFromCustomer(Customer customer)
        {
            string nextLine;
            StringCollection results = new StringCollection();

            Console.WriteLine(customer.FirstName + ' ' + customer.LastName);
            nextLine = String.Format(customer.FirstName + ' ' + customer.LastName);
            results.Add(nextLine);
            foreach (var o in customer.Orders)
            {
                if (o == null) break;
                Console.WriteLine("{0} - {1:C} - {2, 0:N1} (square feet)",
                    o.OrderedVehicle.Description,
                    o.Price,
                    o.OrderedVehicle.CargoCapacity());
                nextLine=String.Format("{0} - {1:C} - {2, 0:N1} (square feet)",
                    o.OrderedVehicle.Description,
                    o.Price,
                    o.OrderedVehicle.CargoCapacity());
                results.Add(nextLine);

                Console.WriteLine("Quantity: {0} - Model Name: {1} - Horse Power: {2}",
                    o.Quantity,
                    o.Model,
                    o.OrderedVehicle.HorsePower);
                nextLine = String.Format("Quantity: {0} - Model Name: {1} - Horse Power: {2}",
                    o.Quantity,
                    o.Model,
                    o.OrderedVehicle.HorsePower);
                results.Add(nextLine);
            }
            Console.WriteLine();
            return results;

        }
    }
}
