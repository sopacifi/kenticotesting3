using System;

using CMS.Ecommerce;

namespace DancingGoat.Models.Orders
{
    public class OrdersListViewModel
    {
        public int OrderID { get; set; }


        public string OrderInvoiceNumber { get; set; }


        public DateTime OrderDate { get; set; }


        public string StatusName { get; set; }


        public string FormattedTotalPrice { get; set; }


        public OrdersListViewModel()
        {
        }


        public OrdersListViewModel(OrderInfo order)
        {
            if (order == null)
            {
                return;
            }

            OrderID = order.OrderID;
            OrderInvoiceNumber = order.OrderInvoiceNumber;
            OrderDate = order.OrderDate;
            StatusName = OrderStatusInfoProvider.GetOrderStatusInfo(order.OrderStatusID)?.StatusDisplayName;
            FormattedTotalPrice = String.Format(CurrencyInfoProvider.GetCurrencyInfo(order.OrderCurrencyID).CurrencyFormatString, order.OrderTotalPrice);
        }
    }
}