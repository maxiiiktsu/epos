using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pokl.system
{


    internal class Product
    {
        /*public string id;
        public string name;
        public double weight;
        public double price;

        public void set_id(string id)
        {
            this.id = id;
        }

        public void set_name(string name)
        {
            this.name = name;
        }

        public void set_weight(double weight)
        {
            this.weight = weight;
        }

        public void set_price(double price)
        {
            this.price = price;
        }*/

        public string Id { get; set; }
        public string Name { get; set; }
        public double Weight { get; set; }
        public double Price { get; set; }

        /*public override string ToString()
        {
            return "id: "+this.id+", name: "+this.name+", weight: "+this.weight+", price: "+this.price;
        }*/

        public override string ToString()
        {
            return $"Id: {this.Id}, Name: {this.Name}, Weight: {this.Weight}, Price: {this.Price}";
        }






    }
}
    
