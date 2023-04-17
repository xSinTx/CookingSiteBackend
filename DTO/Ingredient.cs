namespace Temalabor.DTO
{
    public class Ingredient
    {
        public Ingredient(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
    }
}
