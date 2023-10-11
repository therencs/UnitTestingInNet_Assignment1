using UnitTestingA1Base.Models;

namespace UnitTestingA1Base.Data
{
    public class NewRecipeWithIngredients
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Servings { get; set; }
        public List<NewIngredient> Ingredients { get; set; }

    }

    public class NewIngredient
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public MeasurementUnit MeasurementUnit { get; set; }
    }
}
