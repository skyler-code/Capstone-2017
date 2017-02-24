using System;
﻿using DataObjects;
using DataAccessLayer;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    /// <summary>
    /// Christian Lopez
    /// Created on 2017/02/15
    /// 
    /// Manages the logic regarding Product Lots
    /// </summary>
    public class ProductLotManager : IProductLotManager
    {
		/// <summary>
		/// William Flood
		/// Created on 2017/02/15
		/// 
		/// Manages the logic regarding adding a Product Lots
		/// </summary>
        public int AddProductLot(ProductLot toAdd)
        {
            var accessor = new ProductLotAccessor();
            accessor.ProductLotInstance = toAdd;
            try
            {
                return DatabaseMainAccessor.Create(accessor);
            } catch
            {
                throw;
            }
		}
		
        public ProductLot RetrieveNewestProductLotBySupplier(Supplier supplier)
        {
            ProductLot pl = null;
            if (null != supplier)
            {
                try
                {
                    pl = ProductLotAccessor.RetrieveNewestProductLot(supplier);
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return pl;
        }


        public bool AddProduct(ProductLot p)
        {
            throw new NotImplementedException();
        }
    }
}
