using RecipeBook.Api.Entities;

namespace RecipeBook.Api.Data.Seed;

/// <summary>
/// Lookup rows (categories, ingredients) that are part of the schema's initial state.
/// They are added through HasData(), so they are written by the migration itself
/// and get fixed Ids (the README / .http examples rely on these Ids).
/// </summary>
public static class LookupSeedData
{
    public static readonly Category[] Categories =
    [
        new() { Id = 1, Name = "სუპები" },
        new() { Id = 2, Name = "სალათები" },
        new() { Id = 3, Name = "ცომეული" },
        new() { Id = 4, Name = "ხორციანი კერძები" },
        new() { Id = 5, Name = "დესერტები" },
        new() { Id = 6, Name = "სასმელები" },
        new() { Id = 7, Name = "ბოსტნეულის კერძები" }
    ];

    public static readonly Ingredient[] Ingredients =
    [
        new() { Id = 1, Name = "ფქვილი" },
        new() { Id = 2, Name = "რძე" },
        new() { Id = 3, Name = "კარაქი" },
        new() { Id = 4, Name = "შაქარი" },
        new() { Id = 5, Name = "მარილი" },
        new() { Id = 6, Name = "საფუარი" },
        new() { Id = 7, Name = "კვერცხი" },
        new() { Id = 8, Name = "სულგუნი" },
        new() { Id = 9, Name = "იმერული ყველი" },
        new() { Id = 10, Name = "ხახვი" },
        new() { Id = 11, Name = "ნიორი" },
        new() { Id = 12, Name = "პომიდორი" },
        new() { Id = 13, Name = "კიტრი" },
        new() { Id = 14, Name = "ნიგოზი" },
        new() { Id = 15, Name = "ქინძი" },
        new() { Id = 16, Name = "ბადრიჯანი" },
        new() { Id = 17, Name = "ქათამი" },
        new() { Id = 18, Name = "საქონლის ხორცი" },
        new() { Id = 19, Name = "მზესუმზირის ზეთი" },
        new() { Id = 20, Name = "წყალი" },
        new() { Id = 21, Name = "ღვინის ძმარი" },
        new() { Id = 22, Name = "შავი პილპილი" },
        new() { Id = 23, Name = "ხმელი სუნელი" },
        new() { Id = 24, Name = "ბროწეული" },
        new() { Id = 25, Name = "მაწონი" }
    ];
}
