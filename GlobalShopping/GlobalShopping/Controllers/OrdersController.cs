using AspNetCoreHero.ToastNotification.Abstractions;
using GlobalShopping.Data;
using GlobalShopping.Data.Entities;
using GlobalShopping.Enums;
using GlobalShopping.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace GlobalShopping.Controllers
{

    
    public class OrdersController : Controller
    {
        private readonly DataContext _context;
		private readonly IToastifyService _toastify;
        private readonly IOrdersHelper _ordersHelper;

        public OrdersController(DataContext context,IToastifyService toastify, IOrdersHelper ordersHelper)
        {
            _context = context;
			_toastify = toastify;
            _ordersHelper = ordersHelper;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sales
                .Include(s => s.User)
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Sale sale = await _context.Sales
                .Include(s => s.User)
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Dispatch(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Sale sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            if (sale.OrderStatus != OrderStatus.Nuevo)
            {
                _toastify.Error("Error: Solo se pueden despachar pedidos que estén en estado NUEVO.");
            }
            else
            {
                sale.OrderStatus = OrderStatus.Despachado;
                _context.Sales.Update(sale);
                await _context.SaveChangesAsync();
                _toastify.Success("El pedido ha sido DESPACHADO.");
            }

            return RedirectToAction(nameof(Details), new { Id = sale.Id });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Send(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Sale sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            if (sale.OrderStatus != OrderStatus.Despachado)
            {
                _toastify.Error("Solo se pueden enviar pedidos que estén en estado DESPACHADO.");
            }
            else
            {
                sale.OrderStatus = OrderStatus.Enviado;
                _context.Sales.Update(sale);
                await _context.SaveChangesAsync();
                _toastify.Success("El pedido ha sido ENVIADO.");
            }

            return RedirectToAction(nameof(Details), new { Id = sale.Id });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Confirm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Sale sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            if (sale.OrderStatus != OrderStatus.Enviado)
            {
                _toastify.Error("Solo se pueden confirmar pedidos que estén en estado ENVIADO.");
            }
            else
            {
                sale.OrderStatus = OrderStatus.Confirmado;
                _context.Sales.Update(sale);
                await _context.SaveChangesAsync();
                _toastify.Success("El pedido ha sido CONFIRMADO.");
            }

            return RedirectToAction(nameof(Details), new { Id = sale.Id });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Sale sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            if (sale.OrderStatus == OrderStatus.Cancelado)
            {
                _toastify.Error("No se puede cancelar un pedido que esté en estado CANCELADO.");
            }
            else
            {
                await _ordersHelper.CancelOrderAsync(sale.Id);
                _toastify.Success("El pedido ha sido CANCELADO.");
            }

            return RedirectToAction(nameof(Details), new { Id = sale.Id });
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> MyOrders()
        {
            return View(await _context.Sales
                .Include(s => s.User)
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .Where(s => s.User.UserName == User.Identity.Name)
                .ToListAsync());
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> MyDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Sale sale = await _context.Sales
                .Include(s => s.User)
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }



    }

}
