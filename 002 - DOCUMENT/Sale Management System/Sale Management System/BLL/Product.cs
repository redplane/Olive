using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sale_Management_System
{
    public class Product
    {
        public string CODE;
        public string NAME;
        public double QUANTITY;
        public double PRICE;
        public string UNIT;
        public string NOTE;

        #region Constructors

        public Product()
        {
        }

        public Product(string szProductCode, string szProductName, double dProductQuantity, double dProductPrice, string szProductNote)
        {
            CODE = szProductCode;
            NAME = szProductName;
            QUANTITY = dProductQuantity;
            PRICE = dProductPrice;
            NOTE = szProductNote;
        }

        #endregion

    }
}
