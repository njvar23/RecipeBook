using Microsoft.EntityFrameworkCore;
using RecipeBook.Api.Entities;
using RecipeBook.Api.Services.Interfaces;

namespace RecipeBook.Api.Data.Seed;

/// <summary>
/// Seeds demo users and sample recipes on startup (only when the Users table is empty).
/// Users are seeded here and not with HasData() because their passwords must be
/// hashed with BCrypt, which produces a different hash every run.
/// Categories and ingredients are seeded by the migration (see LookupSeedData).
/// </summary>
public static class DbSeeder
{
    public const string AdminEmail = "admin@recipebook.ge";
    public const string AdminPassword = "Admin123!";
    public const string NinoEmail = "nino@recipebook.ge";
    public const string NinoPassword = "Nino1234!";
    public const string GiorgiEmail = "giorgi@recipebook.ge";
    public const string GiorgiPassword = "Giorgi123!";

    public static async Task SeedAsync(AppDbContext db, IPasswordHasher hasher, CancellationToken ct = default)
    {
        if (await db.Users.AnyAsync(ct))
            return;

        var now = DateTime.UtcNow;

        var admin = new User
        {
            Email = AdminEmail,
            DisplayName = "Admin",
            Role = UserRole.Admin,
            PasswordHash = hasher.Hash(AdminPassword),
            CreatedAt = now
        };

        var nino = new User
        {
            Email = NinoEmail,
            DisplayName = "ნინო",
            Role = UserRole.User,
            PasswordHash = hasher.Hash(NinoPassword),
            CreatedAt = now,
            ChefProfile = new ChefProfile
            {
                Bio = "აჭარული და იმერული სამზარეულოს მოყვარული.",
                Speciality = "ქართული ცომეული",
                YearsOfExperience = 6
            }
        };

        // Giorgi has no chef profile on purpose: shows that the 1:1 is optional
        var giorgi = new User
        {
            Email = GiorgiEmail,
            DisplayName = "გიორგი",
            Role = UserRole.User,
            PasswordHash = hasher.Hash(GiorgiPassword),
            CreatedAt = now
        };

        db.Users.AddRange(admin, nino, giorgi);

        var khachapuri = new Recipe
        {
            Title = "ხაჭაპური აჭარული",
            Description = "ნავის ფორმის ხაჭაპური კვერცხითა და კარაქით.",
            CookingMinutes = 45,
            Servings = 2,
            Difficulty = Difficulty.Medium,
            CategoryId = 3,
            Author = nino,
            CreatedAt = now,
            Steps =
            [
                new() { Order = 1, Instruction = "თბილ რძეში გახსენით საფუარი, დაამატეთ ფქვილი და მარილი, მოზილეთ ცომი." },
                new() { Order = 2, Instruction = "ცომი დატოვეთ თბილ ადგილას 1 საათით, რომ ააფუვდეს." },
                new() { Order = 3, Instruction = "გააბრტყელეთ ნავის ფორმით, ჩადეთ გახეხილი ყველი და გამოაცხვეთ 220°-ზე." },
                new() { Order = 4, Instruction = "ბოლო წუთებში ჩაასხით კვერცხი, მიადეთ კარაქი და მიირთვით ცხელი." }
            ],
            RecipeIngredients =
            [
                new() { IngredientId = 1, Quantity = 500, Unit = MeasurementUnit.Gram },
                new() { IngredientId = 2, Quantity = 250, Unit = MeasurementUnit.Ml },
                new() { IngredientId = 6, Quantity = 1, Unit = MeasurementUnit.Tsp },
                new() { IngredientId = 5, Quantity = 1, Unit = MeasurementUnit.Tsp },
                new() { IngredientId = 8, Quantity = 300, Unit = MeasurementUnit.Gram },
                new() { IngredientId = 9, Quantity = 200, Unit = MeasurementUnit.Gram },
                new() { IngredientId = 7, Quantity = 3, Unit = MeasurementUnit.Piece },
                new() { IngredientId = 3, Quantity = 50, Unit = MeasurementUnit.Gram }
            ]
        };

        var badrijani = new Recipe
        {
            Title = "ბადრიჯანი ნიგვზით",
            Description = "შემწვარი ბადრიჯნის რულეტები ნიგვზის პასტით.",
            CookingMinutes = 40,
            Servings = 4,
            Difficulty = Difficulty.Easy,
            CategoryId = 7,
            Author = nino,
            CreatedAt = now,
            Steps =
            [
                new() { Order = 1, Instruction = "ბადრიჯანი დაჭერით სიგრძეზე თხელ ნაჭრებად და შეწვით ზეთზე." },
                new() { Order = 2, Instruction = "ნიგოზი, ნიორი და ქინძი დაფქვით, დაამატეთ ხმელი სუნელი, ძმარი და მარილი." },
                new() { Order = 3, Instruction = "თითოეულ ნაჭერზე წაუსვით პასტა და დაახვიეთ რულეტად." },
                new() { Order = 4, Instruction = "მორთეთ ბროწეულის მარცვლებით." }
            ],
            RecipeIngredients =
            [
                new() { IngredientId = 16, Quantity = 3, Unit = MeasurementUnit.Piece },
                new() { IngredientId = 14, Quantity = 200, Unit = MeasurementUnit.Gram },
                new() { IngredientId = 11, Quantity = 3, Unit = MeasurementUnit.Piece },
                new() { IngredientId = 15, Quantity = 30, Unit = MeasurementUnit.Gram },
                new() { IngredientId = 23, Quantity = 1, Unit = MeasurementUnit.Tsp },
                new() { IngredientId = 21, Quantity = 1, Unit = MeasurementUnit.Tbsp },
                new() { IngredientId = 19, Quantity = 4, Unit = MeasurementUnit.Tbsp },
                new() { IngredientId = 5, Quantity = 1, Unit = MeasurementUnit.Tsp },
                new() { IngredientId = 24, Quantity = 0.5m, Unit = MeasurementUnit.Piece }
            ]
        };

        var chikhirtma = new Recipe
        {
            Title = "ჩიხირთმა",
            Description = "ქათმის ბულიონი კვერცხითა და ძმრით.",
            CookingMinutes = 60,
            Servings = 4,
            Difficulty = Difficulty.Medium,
            CategoryId = 1,
            Author = giorgi,
            CreatedAt = now,
            Steps =
            [
                new() { Order = 1, Instruction = "ქათამი მოხარშეთ წყალში მარილით, ამოიღეთ და დაჭერით." },
                new() { Order = 2, Instruction = "ხახვი მოშუშეთ კარაქზე, დაამატეთ ფქვილი და ცოტა ბულიონი." },
                new() { Order = 3, Instruction = "კვერცხი ათქვიფეთ ძმართან ერთად და ნელ-ნელა ჩაასხით ცხელ ბულიონში." },
                new() { Order = 4, Instruction = "დააყარეთ დაჭრილი ქინძი და მიირთვით." }
            ],
            RecipeIngredients =
            [
                new() { IngredientId = 17, Quantity = 800, Unit = MeasurementUnit.Gram },
                new() { IngredientId = 20, Quantity = 1500, Unit = MeasurementUnit.Ml },
                new() { IngredientId = 10, Quantity = 2, Unit = MeasurementUnit.Piece },
                new() { IngredientId = 3, Quantity = 30, Unit = MeasurementUnit.Gram },
                new() { IngredientId = 1, Quantity = 1, Unit = MeasurementUnit.Tbsp },
                new() { IngredientId = 7, Quantity = 2, Unit = MeasurementUnit.Piece },
                new() { IngredientId = 21, Quantity = 2, Unit = MeasurementUnit.Tbsp },
                new() { IngredientId = 15, Quantity = 20, Unit = MeasurementUnit.Gram },
                new() { IngredientId = 5, Quantity = 1, Unit = MeasurementUnit.Tsp }
            ]
        };

        var salad = new Recipe
        {
            Title = "პომიდვრისა და კიტრის სალათი ნიგვზით",
            Description = "კლასიკური ქართული სალათი.",
            CookingMinutes = 10,
            Servings = 2,
            Difficulty = Difficulty.Easy,
            CategoryId = 2,
            Author = giorgi,
            CreatedAt = now,
            Steps =
            [
                new() { Order = 1, Instruction = "პომიდორი, კიტრი და ხახვი დაჭერით." },
                new() { Order = 2, Instruction = "ნიგოზი ნიორთან და მარილთან ერთად დაფქვით." },
                new() { Order = 3, Instruction = "აურიეთ ბოსტნეული ნიგვზის მასასთან და მიირთვით." }
            ],
            RecipeIngredients =
            [
                new() { IngredientId = 12, Quantity = 3, Unit = MeasurementUnit.Piece },
                new() { IngredientId = 13, Quantity = 2, Unit = MeasurementUnit.Piece },
                new() { IngredientId = 10, Quantity = 1, Unit = MeasurementUnit.Piece },
                new() { IngredientId = 14, Quantity = 100, Unit = MeasurementUnit.Gram },
                new() { IngredientId = 11, Quantity = 1, Unit = MeasurementUnit.Piece },
                new() { IngredientId = 5, Quantity = 1, Unit = MeasurementUnit.Tsp }
            ]
        };

        db.Recipes.AddRange(khachapuri, badrijani, chikhirtma, salad);

        db.Comments.Add(new Comment
        {
            Recipe = khachapuri,
            User = giorgi,
            Text = "ძალიან გემრიელი გამოვიდა, მადლობა!",
            CreatedAt = now
        });

        db.Favorites.Add(new Favorite { User = giorgi, Recipe = khachapuri, AddedAt = now });

        await db.SaveChangesAsync(ct);
    }
}
