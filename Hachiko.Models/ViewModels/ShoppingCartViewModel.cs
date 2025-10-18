namespace Hachiko.Models.ViewModels;

public class ShoppingCartViewModel
{
    public string UserId { get; set; }
    public IEnumerable<ShoppingCart> ShoppingCarts { get; set; }
    public double OrderTotal { get; set; }
}