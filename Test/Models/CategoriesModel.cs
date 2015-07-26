using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Web.Helpers;

namespace Test.Models
{

    public interface ICategoriesContext
    {
        /*public ICategoriesContext()
            : base("ConnectionToTest")
        {
        }*/

        IEnumerable<Categories> GetAllCategories();
        Categories GetCategoryById(int id);
        Categories GetCategoryByValue(string value);
        Categories CreateCategory(Categories _category);
        string GetCategory(int id);

    }
    public class CategoriesContext : DbContext, ICategoriesContext
    {
        public CategoriesContext()
            : base("ConnectionToTest")
        {
        }

       /*public CategoriesContext()
        {
        }*/

        public Categories Categories = new Categories();
        public DbSet<Categories> categories { get; set; }
        
        public IEnumerable<Categories> GetAllCategories()
        {
            if (categories.Count() != 0)
            {
                return categories;
            }
            return null;
        }

        public Categories GetCategoryById(int id)
        {
            if (categories.Count() != 0)
            {
                foreach (var p in categories)
                {
                    if (p.id == id) return p;
                }
            }
            return null;
        }

        public Categories GetCategoryByValue(string value)
        {
            IEnumerable<Categories> _categories = categories;

            if (categories.Count() != 0)
            {
                foreach (var p in categories)
                {
                    if (p.value == value) return p;
                }
            }
            return null;
        }

        public Categories CreateCategory(Categories _category)
        {
            if (_category.value.Trim() != "")
            {
                CategoriesContext _db = new CategoriesContext();

                _db.categories.Add(_category);
                _db.SaveChanges();
                var catId = _db.categories.Select(id => id.id).Max();
                _category.id = catId;
                return _category;
            }
            else return null;
        }

        public string GetCategory(int id)
        {
            if (categories.Count() != 0)
            {
                foreach (var cat in categories)
                {
                    if (cat.id == id) return cat.value;
                }
            }
            return null;
        }


    }

    public class CategoriesModel
    {
        [Required]
        [Display(Name = "id")]
        public int id { get; set; }

        [Required]
        [Display(Name = "value")]
        public string value { get; set; }
    }



    public class Categories
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        // ID 
        public int id { get; set; }
        // значение категории
        public string value { get; set; }
    }
}