using System.Xml.Linq;
using UnitTestingA1Base.Models;

namespace UnitTestingA1Base.Data
{
    public class BusinessLogicLayer
    {
        private AppStorage _appStorage;

        public BusinessLogicLayer(AppStorage appStorage) {
            _appStorage = appStorage;
        }

        private Ingredient _ingredient { get; set; }
        private DietaryRestriction _diet { get; set; }

        public HashSet<Recipe>? GetRecipesByIngredient(int? id, string? name)
        {
            Ingredient? ingredient = _ingredient;
            HashSet<Recipe> recipes = new HashSet<Recipe>();
            bool invalidPK = false;

            if (id != null)
            {
                ingredient = _appStorage.Ingredients.FirstOrDefault(i => i.Id == id);

                if (ingredient != null)
                {
                    HashSet<RecipeIngredient> recipeIngredients = _appStorage.RecipeIngredients.Where(rI => rI.IngredientId == ingredient.Id).ToHashSet();

                    recipes = _appStorage.Recipes.Where(r => recipeIngredients.Any(ri => ri.RecipeId == r.Id)).ToHashSet();
                } else
                {
                    invalidPK = true;
                }

            } else if (name != null)
            {
                ingredient = _appStorage.Ingredients.FirstOrDefault(i => i.Name.ToLower().Contains(name.ToLower()));

                if (ingredient != null)
                {
                    HashSet<RecipeIngredient> recipeIngredients = _appStorage.RecipeIngredients.Where(rI => rI.IngredientId == ingredient.Id).ToHashSet();
                    recipes = _appStorage.Recipes.Where(r => recipeIngredients.Any(ri => ri.RecipeId == r.Id)).ToHashSet();
                }
            }

            if (invalidPK == true)
            {
                return null;
            } else
            {
                return recipes;
            }   
        }

        public HashSet<Recipe>? GetRecipesByDiet(int? id, string? name)
        {
            DietaryRestriction? dietaryrestriction = _diet;
            HashSet<Recipe> recipes = new HashSet<Recipe>();
            Ingredient? ingredient = _ingredient;
            bool invalidPK = false;

            if (id != null)
            {
                dietaryrestriction = _appStorage.DietaryRestrictions.FirstOrDefault(i => i.Id == id);

                if (dietaryrestriction != null)
                {
                    HashSet<int> ingredientIdsForDiet = _appStorage.RecipeIngredients
                        .Where(ir => _appStorage.IngredientRestrictions
                        .Any(i => i.DietaryRestrictionId == id && ir.IngredientId == i.IngredientId))
                        .Select(ir => ir.RecipeId)
                        .DistinctBy(id => id)
                        .ToHashSet();

                    recipes = _appStorage.Recipes
                        .Where(recipe => ingredientIdsForDiet.Contains(recipe.Id))
                        .ToHashSet();

                }

            }
            else if (name != null)
            {
                dietaryrestriction = _appStorage.DietaryRestrictions.FirstOrDefault(i => i.Name.ToLower().Contains(name.ToLower()));

                if (dietaryrestriction != null)
                {
                    HashSet<int> ingredientIdsForDiet = _appStorage.RecipeIngredients
                        .Where(ir => _appStorage.IngredientRestrictions
                        .Any(i => i.DietaryRestrictionId == dietaryrestriction.Id && ir.IngredientId == i.IngredientId))
                        .Select(ir => ir.RecipeId)
                        .DistinctBy(Id => Id)
                        .ToHashSet();

                    recipes = _appStorage.Recipes
                        .Where(recipe => ingredientIdsForDiet.Contains(recipe.Id))
                        .ToHashSet();
                }
            }

            if (invalidPK == true)
            {   
                return null;
            }
            else
            {
                return recipes;
            }
        }

        public HashSet<Recipe> GetRecipes(int? id, string? name)
        {
            HashSet<Recipe> recipes = new HashSet<Recipe>();

            if (id != null)
            {
                recipes.UnionWith(_appStorage.Recipes.Where(i => i.Id == id));
            }
            else if (name != null)
            {
                recipes.UnionWith(_appStorage.Recipes.Where(i => i.Name == name));
            } else
            {
                recipes = _appStorage.Recipes;
            }

            return recipes;
            
        }

        public void TryPostNewRecipe(NewRecipeWithIngredients newRecipe)
        {

            if (!_appStorage.Recipes.Any(r => r.Name.ToLower() == newRecipe.Name.ToLower()))
            {
                Recipe recipe = new Recipe
                {
                    Id = _appStorage.GeneratePrimaryKey(),  // Generate a new ID
                    Name = newRecipe.Name,
                    Description = newRecipe.Description,
                    Servings = newRecipe.Servings,
                };

                foreach (NewIngredient i in newRecipe.Ingredients)
                {
                    bool ingredientExists = _appStorage.Ingredients.Any(ing => ing.Name == i.Name);

                    if (ingredientExists == true)
                    {
                        _appStorage.RecipeIngredients.Add(new RecipeIngredient
                        {
                            IngredientId = _appStorage.Ingredients.First(ing => ing.Name == i.Name).Id,
                            RecipeId = recipe.Id,  // Use the newly created Recipe's ID
                            Amount = 1,  // You should set the appropriate value here
                            MeasurementUnit = i.MeasurementUnit  // You should set the appropriate unit
                        });
                    }
                    else
                    {
                        Ingredient newIngredient = new Ingredient
                        {
                            Id = _appStorage.GeneratePrimaryKey(),  // Generate a new ID
                            Name = i.Name,
                        };

                        _appStorage.Ingredients.Add(newIngredient);  // Add the new ingredient to the storage

                        _appStorage.RecipeIngredients.Add(new RecipeIngredient
                        {
                            IngredientId = newIngredient.Id,
                            RecipeId = recipe.Id,  // Use the newly created Recipe's ID
                            Amount = 1,  // You should set the appropriate value here
                            MeasurementUnit = i.MeasurementUnit,  // You should set the appropriate unit
                        });
                    }

                    _appStorage.Recipes.Add(recipe);
                }
            } else
            {
                throw new InvalidOperationException();
            }
        }

        public void DeleteIngredient(int? id, string? name)
        {
            Ingredient? ingredient = null;

            if (id != null)
            {
                ingredient = _appStorage.Ingredients.FirstOrDefault(i => i.Id == id);
            } else if (name != null )
            {
                ingredient = _appStorage.Ingredients.FirstOrDefault(i => i.Name.ToLower().Contains(name.ToLower()));
            }
                
            if (ingredient == null)
            {
                throw new InvalidOperationException("Ingredient not found.");
            }

            bool ingredientInUse = _appStorage.RecipeIngredients.Any(ri => ri.IngredientId == id);

            if (ingredientInUse)
            {
                throw new InvalidOperationException("Cannot delete ingredient used in multiple recipes.");
            }

            _appStorage.Ingredients.Remove(ingredient);

            List<int> recipeIdsToDelete = _appStorage.RecipeIngredients.Where(ri => ri.IngredientId == id).Select(ri => ri.RecipeId).Distinct().ToList();
            foreach (int recipeId in recipeIdsToDelete)
            {
                Recipe? recipe = _appStorage.Recipes.FirstOrDefault(r => r.Id == recipeId);
                if (recipe != null)
                {
                    _appStorage.Recipes.Remove(recipe);
                }

                List<RecipeIngredient> recipeIngredientsToDelete = _appStorage.RecipeIngredients.Where(ri => ri.RecipeId == recipeId).ToList();
                foreach (RecipeIngredient recipeIngredient in recipeIngredientsToDelete)
                {
                    _appStorage.RecipeIngredients.Remove(recipeIngredient);
                }
            }
        }

        public void DeleteRecipe(int? id, string? name)
        {
            Recipe? recipe = null;

            if (id != null)
            {
                recipe = _appStorage.Recipes.FirstOrDefault(i => i.Id == id);
            }
            else if (name != null)
            {
                recipe = _appStorage.Recipes.FirstOrDefault(i => i.Name.ToLower().Contains(name.ToLower()));
            }

            if (recipe == null)
            {
                
                throw new InvalidOperationException("Recipe not found.");
            }

            List<RecipeIngredient> recipeIngredientsToDelete = _appStorage.RecipeIngredients.Where(ri => ri.RecipeId == id).ToList();
            foreach (RecipeIngredient recipeIngredient in recipeIngredientsToDelete)
            {
                _appStorage.RecipeIngredients.Remove(recipeIngredient);
            }

            
            _appStorage.Recipes.Remove(recipe);
        }

    }
}
