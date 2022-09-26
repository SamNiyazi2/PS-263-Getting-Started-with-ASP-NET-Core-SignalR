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
        // Corrects the error "'Failed to invoke 'GetUpdateForOrder' due to an error on the server.'"
        // public async Task GetUpdateForOrder(int orderId)
        public async Task GetUpdateForOrder(string orderId_string)
        {
            int.TryParse(orderId_string, out int orderId);

            CheckResult result;

            Models.Order order = OrderChecker.getOrder(orderId);

            do
            {
                result = _orderChecker.GetUpdate(order);

                // 01/12/2021 07:43 am - SSN - [20210112-0607] - [012] - M04-02 - Implementing a hub 
                // Thread.Sleep(1000);
                Thread.Sleep(30);

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


        string SELECT_GROUP_NAME = "SomeGroupName";

        public override Task OnConnectedAsync()
        {

            Debug.WriteLine("ssn-20210113-0231: OnConnectedAsync");

            //// 01/13/2021 03:35 am - SSN - [20210113-0333] - [001] - M04-05 - Context, Clients and Groups
            var connectionId = Context.ConnectionId;
            Task t = Task.Run(async () =>
               {

                   await Clients.Client(connectionId).SendAsync("SomeMessage_client_only", "(Code-101): Clients.Client(connectionId).SendAsync");
                   await Clients.AllExcept(connectionId).SendAsync("SomeMessage_allexcept_client", "(Code-102): Clients.AllExcept(connectionId).SendAsync");

                   //// A group is created when you add a client to a group.  It is removed when the last client is removed.
                   await Groups.AddToGroupAsync(connectionId, SELECT_GROUP_NAME);

                   await Clients.Group(SELECT_GROUP_NAME).SendAsync("GroupMessageGeneral", "Some message to the group");

               });

            t.Wait();

            return base.OnConnectedAsync();


        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {

            Debug.WriteLine("ssn-20210113-0232: OnDisconnectedAsync");

            // 01/13/2021 05:12 am - SSN - [20210113-0333] - [002] - M04-05 - Context, Clients and Groups
            var connectionId = Context.ConnectionId;
            await Groups.RemoveFromGroupAsync(connectionId, SELECT_GROUP_NAME);

            // return base.OnDisconnectedAsync(exception);
        }
    }
}
