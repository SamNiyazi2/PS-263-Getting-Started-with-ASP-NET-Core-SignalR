using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WiredBrain.Helpers;

namespace WiredBrain.Hubs
{
    public class CoffeeHub : Hub
    {
        private readonly OrderChecker _orderChecker;

        public CoffeeHub(OrderChecker orderChecker)
        {
            _orderChecker = orderChecker;
        }

        // 01/12/2021 06:27 am - SSN - [20210112-0607] - [008] - M04-02 - Implementing a hub 
         public async Task GetUpdateForOrder(int orderId) 
        {
            CheckResult result;

            Models.Order order = OrderChecker.getOrder(orderId);

            do
            {
                result = _orderChecker.GetUpdate(order);

                // 01/12/2021 07:43 am - SSN - [20210112-0607] - [012] - M04-02 - Implementing a hub 
                // Thread.Sleep(1000);
                Thread.Sleep(1000);

                if (result.New)
                {
                    // 01/12/2021 06:20 am - SSN - [20210112-0607] - [004] - M04-02 - Implementing a hub 
                    // await Clients.Caller.SendAsync("ReceiveOrderUpdate", result.Update);
                    await Clients.Caller.SendAsync("ReceiveOrderUpdate", result);
                }

            } while (!result.Finished);

            // 01/12/2021 07:05 am - SSN - [20210112-0607] - [011] - M04-02 - Implementing a hub 
            // We don't want to close connection after every request.
            //            await Clients.Caller.SendAsync("Finished");


            // 01/13/2021 02:30 am - SSN - [20210112-0607] - [013] - M04-02 - Implementing a hub 

        }

        public override Task OnConnectedAsync()
        {
            Debug.WriteLine("ssn-20210113-0231: OnConnectedAsync");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Debug.WriteLine("ssn-20210113-0232: OnDisconnectedAsync");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
