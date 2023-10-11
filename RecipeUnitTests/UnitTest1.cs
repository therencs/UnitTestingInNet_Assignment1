using UnitTestingA1Base.Data;
using UnitTestingA1Base.Models;

namespace RecipeUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        private BusinessLogicLayer _initializeBusinessLogic()
        {
            return new BusinessLogicLayer(new AppStorage());
        }

        #region FirstEndpoint Tests

        [TestMethod]
        public void GetRecipesByIngredient_ValidId_ReturnsRecipesWithIngredient()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int ingredientId = 6;
            int recipeCount = 2;

            // act
            HashSet<Recipe> recipes = bll.GetRecipesByIngredient(ingredientId, null);

            // assert
            Assert.AreEqual(recipeCount, recipes.Count);
        }

        [TestMethod]
        public void GetRecipesByIngredient_ValidName_ReturnsRecipesWithIngredient()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string ingredientName = "Parmesan Cheese";
            int expectedRecipesCount = 2;

            // act

            HashSet<Recipe> recipes = bll.GetRecipesByIngredient(null, ingredientName);

            // assert
            Assert.AreEqual(expectedRecipesCount, recipes.Count);

        }

        [TestMethod]
        public void GetRecipesByIngredient_InvalidId_ReturnsNull()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int testId = 0;

            // act
            HashSet<Recipe> recipes = bll.GetRecipesByIngredient(testId, null);

            // assert
            Assert.AreEqual(null, recipes);
        }

        [TestMethod]
        public void GetRecipesByIngredient_NameWithNoMatches_ReturnsEmptyCollection()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string testString = "spaghettio";

            // act
            HashSet<Recipe> recipes = bll.GetRecipesByIngredient(null, testString);

            // assert
            Assert.IsTrue(recipes.Count == 0);
        }

        [TestMethod]
        public void GetRecipesByIngredient_ContainsValidName_ReturnsRecipesWithIngredient()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string ingredientName = "spa";
            int expectedRecipesCount = 1;

            // act
            HashSet<Recipe>? recipes = bll.GetRecipesByIngredient(null, ingredientName);

            // assert
            Assert.AreEqual(expectedRecipesCount, recipes.Count);
        }

        #endregion

        [TestMethod]

        public void GetRecipesByDiet_ValidId_ReturnsRecipesWithDiet()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int testId = 3;
            int expectedRecipesCount = 3;

            // act
            HashSet<Recipe>? recipes = bll.GetRecipesByDiet(testId, null);

            // assert
            Assert.AreEqual(expectedRecipesCount, recipes.Count);
        }

        [TestMethod]

        public void GetRecipesByDiet_ContainsValidName_ReturnsRecipesWithDiet()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string testString = "Lactose";
            int expectedRecipesCount = 2;

            // act
            HashSet<Recipe>? recipes = bll.GetRecipesByDiet(null, testString);

            // assert
            Assert.AreEqual(expectedRecipesCount, recipes.Count);
        }

        [TestMethod]

        public void GetRecipes_IdAsArgument_ReturnsRecipe()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int expectedRecipeCount = 1;

            // act
            HashSet<Recipe>? recipes = bll.GetRecipes(1, null);

            // assert
            Assert.AreEqual(expectedRecipeCount, recipes.Count());
        }

        [TestMethod]

        public void GetRecipes_StringAsArgument_ReturnsRecipes()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int expectedRecipeCount = 1;

            // act
            HashSet<Recipe>? recipes = bll.GetRecipes(1, null);

            // assert
            Assert.AreEqual(expectedRecipeCount, recipes.Count());
        }

        [TestMethod]

        public void TryPostNewRecipe_RecipeNameAlreadyExists_ThrowInvalidOperation()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string existingRecipeName = "Spaghetti Carbonara";

            // act
            Action PostNewRecipeAct = () => bll.TryPostNewRecipe(new NewRecipeWithIngredients
            {
                Name = existingRecipeName,
                Description = "Classic Roman pasta dish with eggs, cheese, and pancetta.",
                Servings = 2,
                Ingredients = new List<NewIngredient> {
                        new NewIngredient
                            {
                                Name = "Spaghetti",
                                Amount = 200,  // Set the appropriate amount
                                MeasurementUnit = MeasurementUnit.Grams  // Set the appropriate unit
                            },
                        new NewIngredient
                            {
                                Name = "Eggs",
                                Amount = 2,  // Set the appropriate amount
                                MeasurementUnit = MeasurementUnit.Milliletres  // Set the appropriate unit
                            },
                        new NewIngredient
                            {
                                Name = "Pancetta",
                                Amount = 50,  // Set the appropriate amount
                                MeasurementUnit = MeasurementUnit.Grams  // Set the appropriate unit
                            }
                    }
            });

            // assert
            Assert.ThrowsException<InvalidOperationException>(PostNewRecipeAct);
        }

        [TestMethod]

        public void TryPostNewRecipe_UniqueRecipeName_AddNewDataToStorage()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string UniqueRecipeName = "Carrot Cake";
            int ExpectedRecipesCount = 13;

            // act
            bll.TryPostNewRecipe(new NewRecipeWithIngredients
            {
                Name = UniqueRecipeName,
                Description = "Delicious carrot cake with cream cheese frosting.",
                Servings = 12,
                Ingredients = new List<NewIngredient>{
                    new NewIngredient
                    {
                        Name = "Carrots",
                        Amount = 250,  // Set the appropriate amount
                        MeasurementUnit = MeasurementUnit.Grams  // Set the appropriate unit
                    },
                    new NewIngredient
                    {
                        Name = "Flour",
                        Amount = 300,  // Set the appropriate amount
                        MeasurementUnit = MeasurementUnit.Grams  // Set the appropriate unit
                    },
                    new NewIngredient
                    {
                        Name = "Sugar",
                        Amount = 200,  // Set the appropriate amount
                        MeasurementUnit = MeasurementUnit.Grams  // Set the appropriate unit
                    },
                    new NewIngredient
                    {
                        Name = "Eggs",
                        Amount = 3,  // Set the appropriate amount
                        MeasurementUnit = MeasurementUnit.Grams  // Set the appropriate unit
                    },
                    new NewIngredient
                    {
                        Name = "Cream Cheese",
                        Amount = 200,  // Set the appropriate amount
                        MeasurementUnit = MeasurementUnit.Grams  // Set the appropriate unit
                    }
                }
            });

            // assert

            Assert.AreEqual(ExpectedRecipesCount, bll.GetRecipes(null, null).Count());
        }

        [TestMethod]

        public void DeleteIngredient_IngredientInMultipleRecipes_ThrowException()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();

            // act

            Action DeleteIngredientAct = () => bll.DeleteIngredient(8, null);

            // assert
            Assert.ThrowsException<InvalidOperationException>(DeleteIngredientAct);
        }

        [TestMethod]
        public void DeleteRecipe_WithValidName_DeletesRecipeAndAssociatedIngredients()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int ExpectedRecipesCount = 11;

            // Act
            bll.DeleteRecipe(null, "Cucumber Salad"); // Using Name

            // Assert
            Assert.AreEqual(ExpectedRecipesCount, bll.GetRecipes(null, null).Count());
        }

    }
}