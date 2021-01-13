namespace WiredBrain.Models
{
    public class Order
    {
        public string Product { get; set; }
        public string Size { get; set; }

        // 01/12/2021 06:17 am - SSN - [20210112-0607] - [002] - M04-02 - Implementing a hub 
        public int OrderNo { get; set; }
        public int Index { get; set; }
    }
}
