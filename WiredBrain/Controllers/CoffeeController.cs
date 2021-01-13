using System; 
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WiredBrain.Helpers;
using WiredBrain.Hubs;
using WiredBrain.Models;

namespace WiredBrain.Controllers
{

    [Route("[controller]")]
    public class CoffeeController : Controller
    {


        private readonly IHubContext<CoffeeHub> coffeeHub;


        // 01/12/2021 06:45 am - SSN - [20210112-0607] - [010] - M04-02 - Implementing a hub 
        static int OrderNo = 1000;

        public CoffeeController(IHubContext<CoffeeHub> coffeeHub)
        {
            this.coffeeHub = coffeeHub;
        }

        [HttpPost]
        public async Task<IActionResult> OrderCoffee( [FromBody]Order order)
        {
            int _orderNo = Interlocked.Increment(ref OrderNo);
            order.OrderNo = _orderNo;
            OrderChecker.addOrder(order);

            await coffeeHub.Clients.All.SendAsync("NewOrder", order);
            //Save order somewhere and get order id
            return Accepted(order.OrderNo); //return order id
        }
    }
}
