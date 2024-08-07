using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulkyBook.Models.Masters;

namespace BulkyBook.Models.ViewModels;
public class ShoppingCartVM
{
    public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }

    public double OrderTotal { get; set; }
}
