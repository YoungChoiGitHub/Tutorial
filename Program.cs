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
            newOrder.ConfirmOrder(newCust, newOrder);
            customerArray[arrayIndex++] = newCust;

            newCust = new Customer();
            newCust.FirstName = "Tom";
            newCust.LastName = "Cruise";
            newOrder = new Order();
            newOrder.PlaceOrder(ModelName.BMW235, 28500, 1); // special offer
            newOrder.ConfirmOrder(newCust, newOrder);
            customerArray[arrayIndex++] = newCust;

            // Motocycle customer
            newCust = new Customer();
            newCust.FirstName = "Sally";
            newCust.LastName = "Jones";
            newOrder = new Order();
            newOrder.PlaceOrder(ModelName.HondaCruiser);
            newOrder.ConfirmOrder(newCust, newOrder);
            customerArray[arrayIndex++] = newCust;

            newCust = new Customer();
            newCust.FirstName = "Rick";
            newCust.LastName = "White";
            newOrder = new Order();
            newOrder.PlaceOrder(ModelName.HondaSport, 17500.0, 2); // special offer
            newOrder.ConfirmOrder(newCust, newOrder);
            customerArray[arrayIndex++] = newCust;

            // save and print the order history
            OrdersBackup orders = new OrdersBackup();
            foreach (var c in customerArray)
            {
                if ( c == null ) break;                     // question
                orders.SaveToFile(c);
            }

            Printer pr = new Printer();
            pr.PrintToConsole();

            orders.DeleteFile();  // for next test
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
        void PrintToConsole();
        void PrintToPrinter();
    }
    

    interface IFileSave
    {
        void SaveToFile(Customer item);
    }

    interface IFileDelete
    {
        void DeleteFile();
    }
    //public class Customer
    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Order[] Orders = new Order[5];
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

        public void PlaceOrder(ModelName modelName, double price, int quantity)
        {
            Model = modelName.ToString();
            if (price < 10000.0)
                throw new ArgumentOutOfRangeException("price");
            Price = price;
            if (quantity > 2)
                throw new ArgumentOutOfRangeException("quantity");
            Quantity = quantity;

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

        public void ConfirmOrder(Customer customer, Order order)
        {
            for (int i = 0; i < customer.Orders.Length; i++)
            {
                if (customer.Orders[i] == null)
                {
                    customer.Orders[i] = order;
                    break;
                }
                else if (i == customer.Orders.Length - 1)
                {
                    Console.WriteLine("Exceeds max. number of orders: 5");
                    break;
                }
            }
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

    class  OrdersBackup : IFileSave, IFileDelete
    {
        public static string OrderHistoryFile 
        { 
            get { return "C:\\Orders\\OrderHistory.txt";}  
        }
        public static string BackupFile
        {
            get { return "C:\\Orders\\OrderHistory.bak"; }
        }

        public void SaveToFile(Customer customer)
        {            
            StringCollection linesCollection = readOrderFromCustomer(customer);
            string[] linesArray = new string[linesCollection.Count];
            linesCollection.CopyTo(linesArray,0);
            
            StreamWriter sw = new StreamWriter(OrderHistoryFile, true);  // create file if not existed, append
            
            foreach (var oneLine in linesArray)
                sw.WriteLine(oneLine);
            sw.Close();
        }
        private StringCollection readOrderFromCustomer(Customer customer)
        {
            string nextLine;
            StringCollection results = new StringCollection();

            nextLine = String.Format("{0}", customer.FirstName + ' ' + customer.LastName);

            results.Add(nextLine);
            foreach (var o in customer.Orders)
            {
                if (o == null) break;
                nextLine=String.Format("{0} - {1:C} - {2, 0:N1} (square feet)",
                    o.OrderedVehicle.Description,
                    o.Price,
                    o.OrderedVehicle.CargoCapacity());
                results.Add(nextLine);

                nextLine = String.Format("Quantity: {0} - Model Name: {1} - Horse Power: {2}",
                    o.Quantity,
                    o.Model,
                    o.OrderedVehicle.HorsePower);
                results.Add(nextLine);
            }
            return results;

        }

        public void DeleteFile()
        {
            File.Copy(OrderHistoryFile, BackupFile, true); //overwrite
            File.Delete(OrderHistoryFile);
        }
    }
    class Printer : IPrint
    {
        public void PrintToConsole()
        {
            if (!File.Exists(OrdersBackup.OrderHistoryFile))
            {
                Console.WriteLine("File not exists - No order records");
            }

            StreamReader sr = new StreamReader(OrdersBackup.OrderHistoryFile);
            string nextLine;
            while ((nextLine = sr.ReadLine()) != null)
            {
                Console.WriteLine(nextLine);
            }
            sr.Close();
        }
       public void PrintToPrinter() 
       {
            //Future implementation
       }
    }
}
