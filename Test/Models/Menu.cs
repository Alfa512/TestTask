using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Test.Models
{

    public class MenuContext : DbContext
    {
        public MenuContext()
            : base("ConnectionToTest")
        {
        }

        public DbSet<Menu> menu { get; set; }
    }

    [Table("menu")]
    public class Menu
    {
         [Key]
         [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        // ID 
        public int id { get; set; }
        // заголовок
        public string title { get; set; }
        // ссылка
        public string link { get; set; }
    }
}