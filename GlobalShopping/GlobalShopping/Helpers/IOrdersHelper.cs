using GlobalShopping.Common;
using GlobalShopping.Models;

namespace GlobalShopping.Helpers
{

    public interface IOrdersHelper
    {
        Task<Response> ProcessOrderAsync(ShowCartViewModel model);

        Task<Response> CancelOrderAsync(int id);
    }

}
