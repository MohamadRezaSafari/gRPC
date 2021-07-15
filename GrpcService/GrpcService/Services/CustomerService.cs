using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcService.Services
{
    public class CustomerService : Customer.CustomerBase
    {
        private readonly ILogger<CustomerService> logger;

        public CustomerService(ILogger<CustomerService> logger)
        {
            this.logger = logger;
        }

        public override async Task GetAllCustomers(AllCustomerModel request, IServerStreamWriter<CustomerDataModel> responseStream, ServerCallContext context)
        {
            var allCustomers = new List<CustomerDataModel>();

            var c1 = new CustomerDataModel();
            c1.FirstName = "Bruce";
            c1.LastName = "Wayne";
            allCustomers.Add(c1);

            var c2 = new CustomerDataModel();
            c2.FirstName = "Homer";
            c2.LastName = "Simpsons";
            allCustomers.Add(c2);

            var c3 = new CustomerDataModel();
            c3.FirstName = "Tony";
            c3.LastName = "Stark";
            allCustomers.Add(c3);

            var c4 = new CustomerDataModel();
            c4.FirstName = "Mohamad-Reza";
            c4.LastName = "Safari";
            allCustomers.Add(c4);


            foreach (var item in allCustomers)
            {
                await responseStream.WriteAsync(item);
            }
        }


        public override Task<CustomerDataModel> GetCustomerInfo(CustomerFindModel request, ServerCallContext context)
        {
            var model = new CustomerDataModel();

            if (request.UserId == 1)
            {
                model.FirstName = "Mohamad Reza";
                model.LastName = "Safari";
            }
            else
            {
                model.FirstName = "...";
                model.LastName = "...";
            }

            return Task.FromResult(model);
        }
    }
}
