// Import necessary namespaces

using System.Collections;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration.Attributes;

// Entry point of the script
await MainAsync();

// Asynchronous main method
async Task MainAsync()
{
    // Set the total number of orders
    const int orderCount = 1000000;

    // Get the current directory
    var directory = Directory.GetCurrentDirectory();

    // Define the file path for the CSV file
    var filePath = $"{directory}\\Orders.csv";

    try
    {
        // List to hold orders
        List<Order> orders;

        // Check if the file exists
        if (FileExists(filePath))
        {
            // If the file exists, read orders from the file
            orders = new List<Order>(CSVHelper.ReadCsvFile<Order>(filePath));
        }
        else
        {
            // If the file does not exist, generate and save new orders
            Console.WriteLine("File does not exist. Generating and saving a new file.");
            await GenerateOrders(orderCount, filePath);
            orders = new List<Order>(CSVHelper.ReadCsvFile<Order>(filePath));
        }

        // Get orders due today
        var ordersDueToday = GetOrdersDueToday(orders).ToList();

        ProcessOrders(ordersDueToday);
    }
    catch(Exception e)
    {
        // Handle any exceptions that occur during processing
        Console.WriteLine("MainAsync error");
    }
}

// Method to filter and retrieve orders due today
IEnumerable<Order> GetOrdersDueToday(List<Order> allOrders)
{
    // Get today's date
    var today = DateTime.Today;

    // Filter orders due today
    var ordersDueToday = allOrders
        .Where(order => order.DueDate.Date == today)
        .ToList();

    return ordersDueToday;
}

// Method to process an individual order
void ProcessOrders(ICollection orders)
{
    // Output the number of orders processed for expiry and total orders
    Console.WriteLine($"{orders.Count} Orders processed for expiry");

    // Uncomment the following line to enqueue a background Hangfire job
    //foreach (var order in orders)
    //{
    //    BackgroundJob.Enqueue(() => ProcessOrder(order));
    //}
}

// Asynchronous method to generate random orders
Task GenerateOrders(int orderCount, string file)
{
    // Lists to hold orders and possible statuses
    var orders = new List<Order>();
    var random = new Random();
    var statuses = new[] { "Pending", "Processing", "Completed", "Cancelled" };

    // Loop to generate random orders
    for (var i = 1; i <= orderCount; i++)
    {
        // Generate random order and due dates
        var randomOrderDate = random.Next(1, 30);
        var orderDate = DateTime.Today.AddDays(-randomOrderDate);
        var randomDueDate = random.Next(1, 30);
        var dueDate = orderDate.AddDays(randomOrderDate + (randomDueDate >= 15 ? 0 : randomDueDate));
        var status = statuses[random.Next(statuses.Length)]; // Random status

        // Create and add a new order to the list
        var order = new Order()
        {
            RowId = i,
            CustomerName = $"Customer {i}",
            ProductName = $"Product {i}",
            Quantity = i * 10,
            Price = i * 10.0m,
            OrderDate = orderDate,
            DueDate = dueDate,
            Status = status
        };

        orders.Add(order);
    }

    // Create and save the CSV file
    CreateAndSaveCsv(orders, file);
    return Task.CompletedTask;
}

// Method to check if a file exists
bool FileExists(string relativePath)
{
    var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
    return System.IO.File.Exists(fullPath);
}

// Method to create and save a CSV file
void CreateAndSaveCsv(List<Order> data, string file)
{
    // Open or create a file stream
    using var fileStream = new FileStream($"{file}", FileMode.OpenOrCreate);
    using var streamWriter = new StreamWriter(fileStream);

    // Define the CSV file header
    const string fileHeader = $"Row ID,Customer Name,Product Name,Quantity,Price,Order Date,Due Date,Status";
    streamWriter.WriteLine(fileHeader);

    // Write each order to the CSV file
    foreach (var row in data)
    {
        streamWriter.WriteLine($"{row.RowId},{row.CustomerName},{row.ProductName},{row.Quantity},{row.Price},{row.OrderDate},{row.DueDate},{row.Status}");
    }
}

// Helper class for CSV operations
public class CSVHelper
{
    // Method to read data from a CSV file and return a list of objects
    public static List<T> ReadCsvFile<T>(string filePath)
    {
        // Open a stream reader for the CSV file
        using var reader = new StreamReader(filePath);

        // Create a CsvReader with the specified culture
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        // Return the records as a list
        return csv.GetRecords<T>().ToList();
    }
}

// Model class representing an order with attributes mapped to CSV headers
public class Order
{
    [Name("Row ID")] public int RowId { get; set; }
    [Name("Customer Name")] public string CustomerName { get; set; }
    [Name("Product Name")] public string ProductName { get; set; }
    [Name("Quantity")] public int Quantity { get; set; }
    [Name("Price")] public decimal Price { get; set; }
    [Name("Order Date")] public DateTime OrderDate { get; set; }
    [Name("Due Date")] public DateTime DueDate { get; set; }
    [Name("Status")] public string Status { get; set; }
}