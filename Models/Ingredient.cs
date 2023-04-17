//using Newtonsoft.Json;


using System.Text.Json.Serialization;

namespace Temalabor.Models
{
    public class Ingredient
    { 
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public ICollection<Recipe> Recipes { get; set; }
    }
}
