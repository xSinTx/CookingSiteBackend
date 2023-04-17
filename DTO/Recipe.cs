namespace Temalabor.DTO
{
    public class Recipe
    {
        public Recipe (int id, string title, int time, string description)
        {
            Id = id;
            Title = title;
            Time = time;
            Description = description;
        }

        public int Id { get; private set; }
        public string Title { get; private set; }
        public int Time { get; private set; }
        public string Description { get; private set; }
        public Ingredient[] Ingredients { get; set; }
        public User User { get; set; }
    }
}
