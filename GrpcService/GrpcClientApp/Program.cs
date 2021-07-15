using Grpc.Core;
using Grpc.Net.Client;
using GrpcService;
using System;
using System.Threading.Tasks;

namespace GrpcClientApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5000");

            var customerClient = new Customer.CustomerClient(channel);
            var result = await customerClient.GetCustomerInfoAsync(new CustomerFindModel()
            {
                UserId = 1
            });                       

            Console.WriteLine($"First Name: {result.FirstName} \nLastName: {result.LastName}");


            var allCustomers = customerClient.GetAllCustomers(new AllCustomerModel());
            var allCustomersList = allCustomers.ResponseStream.ReadAllAsync().GetAsyncEnumerator();

            await foreach (var item in allCustomers.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"{item.FirstName} {item.LastName}");
            }


            Console.ReadLine();
        }
    }
}
