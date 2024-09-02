using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp
{


    abstract class Delivery
    {

        private string _address;

        public string Address
        {
            get { return _address; }

            //Использование инкапсуляции: защищенные модификаторы protected set для управление полями внутри классов-потомков
            protected set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _address = value;
                }
                else
                {
                    Console.WriteLine("Адрес не может быть пустым.");
                }
            }
        }

        public abstract void Deliver();
    }

    // Классы HomeDelivery, PickPointDelivery, ShopDelivery наследуют Delivery и переопределяют абстрактный метод Deliver.
    class HomeDelivery : Delivery
    {
        public string CourierName { get; protected set; }

        public HomeDelivery(string address, string courierName)
        {
            Address = address;
            CourierName = courierName;
        }

        public override void Deliver()
        {
            Console.WriteLine($"Ваш заказ по адресу адресу: {Address}, будет доставлен курьером {CourierName}");
        }
    }

    class PickPointDelivery : Delivery
    {
        public string PickPointAddress { get; protected set; }
        public string AccessCode { get; protected set; }

        public PickPointDelivery(string address, string pickPointAddress, string accessCode)
        {
            Address = address;
            PickPointAddress = pickPointAddress;
            AccessCode = accessCode;
        }

        public override void Deliver()
        {
            Console.WriteLine($"Доставка в пункт выдачи: {PickPointAddress}. Для получения заказа используйте код {AccessCode}.");
        }
    }

    // Использование композиции: класс ShopDelivery использует класс Shop для хранения информации о магазине
    class Shop
    {
        public string Name { get; set; }
        public string Location { get; set; }

        public Shop(string name, string location)
        {
            Name = name;
            Location = location;
        }
    }

    class ShopDelivery : Delivery
    {
        public Shop Shop { get; protected set; }

        public ShopDelivery(string address, Shop shop)
        {
            Address = address;
            Shop = shop;
        }

        public override void Deliver()
        {
            Console.WriteLine($"Ваш заказ доставлен в магазин {Shop.Name}, расположенный в {Shop.Location}");
        }
    }

    // Абстрактный обобщенный класс, демонстрирующий наследование обобщений и использование конструктора с параметрами
    abstract class OrderBase<TDelivery> where TDelivery : Delivery
    {
        public int Number { get; protected set; }
        public TDelivery Delivery { get; protected set; }

        protected OrderBase(int number, TDelivery delivery)
        {
            Number = number;
            Delivery = delivery;
        }

        public abstract void DisplayOrderDetails();
    }

    // Обобщенный класс Order, который наследует от OrderBase, демонстрируя наследование обобщений
    class Order<TDelivery> : OrderBase<TDelivery> where TDelivery : Delivery
    {
        public string Description { get; set; }
        public Customer Customer { get; set; }

        public Order(int number, TDelivery delivery, string description, Customer customer) : base(number, delivery)
        {
            Description = description;
            Customer = customer;
        }

        public override void DisplayOrderDetails()
        {
            Console.WriteLine($"Номер заказа: {Number}");
            Console.WriteLine($"Описание: {Description}");
            Console.WriteLine($"Заказчик: {Customer.Name}, Email: {Customer.Email}");
            Delivery.Deliver();
            Console.WriteLine();
        }

        // Метод для обновления номера заказа
        public void UpdateOrderNumber(int newNumber)
        {
            if (newNumber > 0)
            {
                Number = newNumber;
            }
            else
            {
                Console.WriteLine("Order number must be positive.");
            }
        }
    }

    // Статический класс с методами расширения
    static class OrderExtensions
    {
        public static void ChangeDescription<TDelivery>(this Order<TDelivery> order, string newDescription) where TDelivery : Delivery
        {
            order.Description = newDescription;
            Console.WriteLine($"Заказ изменен: {newDescription}");
        }

        // Обобщенный метод для сравнения двух заказов по номеру
        public static bool IsSameOrder<TDelivery>(this Order<TDelivery> order1, Order<TDelivery> order2) where TDelivery : Delivery
        {
            return order1.Number == order2.Number;
        }
    }

    // Класс для демонстрации использования индексаторов
    class OrderCollection<TDelivery> where TDelivery : Delivery
    {
        private List<Order<TDelivery>> orders = new List<Order<TDelivery>>();

        // Индексатор для доступа к заказам по индексу
        public Order<TDelivery> this[int index]
        {
            get { return orders[index]; }
            set { orders[index] = value; }
        }

        public void AddOrder(Order<TDelivery> order)
        {
            orders.Add(order);
        }

        // Статический метод для подсчета количества заказов
        public static int CountOrders(OrderCollection<TDelivery> collection)
        {
            return collection.orders.Count;
        }
    }

    
    class Customer
    {
        public string Name { get; set; }
        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                if (IsValidEmail(value))
                {
                    _email = value;
                }
                else
                {
                    Console.WriteLine("Некорректный email");
                }
            }
        }

        public Customer(string name, string email)
        {
            Name = name;
            Email = email;
        }

        // Проверка корректности email
        private bool IsValidEmail(string email)
        {
            return email.Contains('@');
        }
        // Использование перегрузки операторов
        public static bool operator ==(Customer c1, Customer c2)
        {
            return c1.Email == c2.Email;
        }

        public static bool operator !=(Customer c1, Customer c2)
        {
            return !(c1 == c2);
        }


        public override bool Equals(object obj)
        {
            if (obj is Customer customer)
            {
                return this == customer;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Email.GetHashCode();
        }

    }

    // Пример использования всех элементов
    internal class Program
    {
        static void Main(string[] args)
        {
            // Перегрузка операторов
            var customer1 = new Customer("Алиса", "alice@nomail.com");
            var customer2 = new Customer("Вовка", "vladimir.p@nomail.com");
            var customer3 = new Customer("Алиса", "alice@nomail.com");

            Console.WriteLine("Сравниваем покупателей 1 и 3");
            Console.WriteLine(customer1 == customer3);
            Console.WriteLine("Сравниваем покупателей 1 и 2");
            Console.WriteLine(customer1 == customer2);

            var homeDelivery = new HomeDelivery("ул. Ленина 1а", "Иван Пушкин");
            var order1 = new Order<Delivery>(1, homeDelivery, "iPhone 15", customer1);

            var pickPointDelivery = new PickPointDelivery("ул. Республики 240", "Пункт выдачи Ozon", "980456");
            var order2 = new Order<Delivery>(2, pickPointDelivery, "Мешок картошки", customer2);

            var shop = new Shop("Эльдорадо", "Торговый центр 'Березка'");
            var shopDelivery = new ShopDelivery("ул. Гагарина", shop);
            var order3 = new Order<Delivery>(1, shopDelivery, "Папуаское копьё", customer3);

            var orders = new OrderCollection<Delivery>();
            orders.AddOrder(order1);
            orders.AddOrder(order2);
            orders.AddOrder(order3);

            order1.DisplayOrderDetails();
            order2.DisplayOrderDetails();
            order3.DisplayOrderDetails();

            // Использование методов расширения
            order1.ChangeDescription("Папуаское копьё цвета черный титан");
            order1.DisplayOrderDetails();

            bool areOrdersSame = order1.IsSameOrder(order3);
            Console.WriteLine($"Сравниваем заказы. Они одинаковые? - {areOrdersSame}");
            Console.WriteLine();
            order3.UpdateOrderNumber(3);
            areOrdersSame = order1.IsSameOrder(order3);
            Console.WriteLine($"Еще раз сравниваем заказы. Они одинаковые? - {areOrdersSame}");
            Console.WriteLine();

            // Использование индексаторов
            var retrievedOrder = orders[1];
            retrievedOrder.DisplayOrderDetails();

            // Подсчет заказов с использованием статического метода
            int orderCount = OrderCollection<Delivery>.CountOrders(orders);
            Console.WriteLine($"Всего заказов: {orderCount}");

            Console.ReadKey();


        }
    }
}
