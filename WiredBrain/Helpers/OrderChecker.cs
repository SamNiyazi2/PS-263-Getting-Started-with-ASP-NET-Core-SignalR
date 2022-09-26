using StackExchange.Redis;
using System;
using System.Collections.Concurrent;

namespace WiredBrain.Helpers
{
    public class OrderChecker
    {
        private readonly Random random;

        // 01/12/2021 06:24 am - SSN - [20210112-0607] - [007] - M04-02 - Implementing a hub 
        // private int index;


        private readonly string[] Status =
            {"Grinding beans", "Steaming milk", "Taking a sip (quality control)", "On transit to counter", "Picked up"};

        public OrderChecker(Random random)
        {
            this.random = random;
        }



        // 01/12/2021 06:07 am - SSN - [20210112-0607] - [001] - M04-02 - Implementing a hub 
        static ConcurrentDictionary<int, Models.Order> cd = new ConcurrentDictionary<int, Models.Order>();

        public static bool addOrder(Models.Order order )
        {
            return cd.TryAdd(order.OrderNo,order);
        }

        public static Models.Order getOrder(int orderId)
        {
            cd.TryGetValue(orderId, out Models.Order order);
            return order;
        }

        // 01/12/2021 06:24 am - SSN - [20210112-0607] - [006] - M04-02 - Implementing a hub 
        // public CheckResult GetUpdate(int orderNo)
        public CheckResult GetUpdate(Models.Order order)
        {
            if (random.Next(1, 5) == 4)
            {
                if (Status.Length - 1 > order.Index)
                {
                    order.Index++;
                    var result = new CheckResult
                    {
                        Order = order,
                        New = true,
                        Update = Status[order.Index],
                        Finished = Status.Length - 1 == order.Index
                    };
                    if (result.Finished)
                        order.Index = 0;
                    return result;
                }
            }

            return new CheckResult { New = false };
        }
    }

    public class CheckResult
    {
        public bool New { get; set; }
        public string Update { get; set; }
        public bool Finished { get; set; }

        // 01/12/2021 06:19 am - SSN - [20210112-0607] - [003] - M04-02 - Implementing a hub 
        public Models.Order Order { get; set; }
    }
}
