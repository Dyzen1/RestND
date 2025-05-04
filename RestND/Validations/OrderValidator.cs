using Mysqlx.Crud;
using RestND.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.Validations
{
    public class OrderValidator : GeneralValidations
    {
        Order order;
        public OrderValidator(Order order)
        {
            this.order = order;
        }

    }
}
