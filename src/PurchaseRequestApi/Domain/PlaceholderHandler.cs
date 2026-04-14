using Infrastructure.Database;
using Infrastructure.Database.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public class PlaceholderHandler
    {
        private AppDbContext _db;
        public PlaceholderHandler(AppDbContext dbContext)
        {
            _db = dbContext;
        }

        public Product GetProductById(Guid id)
        {
            return _db.Products.FirstOrDefault(x => x.Id == id);
        }

        public Product CreateProduct(string name, string desc)
        {
            var product = new Product() { Name = name, Description = desc };

            _db.Products.Add(product);
            _db.SaveChanges();

            return product;
        }

        public void UpdateProduct(Guid id, string name, string desc)
        {
            var product = GetProductById(id);

            if (product != null)
            {
                product.Name = name;
                product.Description = desc;

                _db.Products.Update(product);
                _db.SaveChanges();
            }
        }

        public void DeleteProduct(Guid id)
        {
            var product = GetProductById(id);

            if (product != null)
            {
                _db.Products.Remove(product);
                _db.SaveChanges();
            }
        }
    }
}
