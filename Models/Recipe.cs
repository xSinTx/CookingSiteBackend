using System.Text.Json.Serialization;

namespace Temalabor.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Time { get; set; }
        public string Description { get; set; }
        public ICollection<Ingredient> Ingredients { get; set; }
        [JsonIgnore]
        public User User { get; set; }

    }
}
