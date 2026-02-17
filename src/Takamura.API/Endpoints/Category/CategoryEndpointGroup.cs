namespace Takamura.API.Endpoints.Category;

using AttachSubCategory;
using CreateCategory;
using RetrieveCategories;

public static class CategoryEndpointGroup
{
    private const string Section = "api/category";

    public static void Map(WebApplication app)
    {
        var group = app.MapGroup(Section);

        RetrieveCategoriesEndpoint.Map(group);
        CreateCategoryEndpoint.Map(group);
        AttachSubCategoryEndpoint.Map(group);
    }
}
