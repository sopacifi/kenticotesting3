using System.Linq;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using Kentico.Membership;

using DancingGoat.Models.Orders;
using DancingGoat.Repositories;

using CMS.Ecommerce;

namespace DancingGoat.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderRepository mOrderRepository;
        private readonly IShoppingService mShoppingService;


        private CustomerInfo CurrentCustomer => mShoppingService.GetCurrentCustomer();


        public OrdersController(IOrderRepository orderRepository, IShoppingService shoppingService)
        {
            mOrderRepository = orderRepository;
            mShoppingService = shoppingService;
        }


        // GET: Orders
        [Authorize]
        public ActionResult Index()
        {
            var currentCustomer = mShoppingService.GetCurrentCustomer();
            var orders = currentCustomer != null
                ? mOrderRepository.GetByCustomerId(currentCustomer.CustomerID).Select(order => new OrdersListViewModel(order))
                : Enumerable.Empty<OrdersListViewModel>();

            return View(orders);
        }


        // GET: Orders/OrderDetail
        [Authorize]
        public ActionResult OrderDetail(int? orderID)
        {
            if (orderID == null)
            {
                return RedirectToAction("Index");
            }

            var order = mOrderRepository.GetById(orderID.Value);

            if ((order == null) || (order.OrderCustomerID != CurrentCustomer?.CustomerID))
            {
                return RedirectToAction("NotFound", "HttpErrors");
            }

            var currency = CurrencyInfoProvider.GetCurrencyInfo(order.OrderCurrencyID);

            return View(new OrderDetailViewModel(currency.CurrencyFormatString)
            {
                InvoiceNumber = order.OrderInvoiceNumber,
                TotalPrice = order.OrderTotalPrice,
                StatusName = OrderStatusInfoProvider.GetOrderStatusInfo(order.OrderStatusID)?.StatusDisplayName,
                OrderAddress = new OrderAddressViewModel(order.OrderBillingAddress),
                OrderItems = OrderItemInfoProvider.GetOrderItems(order.OrderID).Select(orderItem =>
                {
                    return new OrderItemViewModel
                    {
                        SKUID = orderItem.OrderItemSKUID,
                        SKUName = orderItem.OrderItemSKUName,
                        SKUImagePath = orderItem.OrderItemSKU.SKUImagePath,
                        TotalPriceInMainCurrency = orderItem.OrderItemTotalPriceInMainCurrency,
                        UnitCount = orderItem.OrderItemUnitCount,
                        UnitPrice = orderItem.OrderItemUnitPrice
                    };
                })
            });
        }
    }
}