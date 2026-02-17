using Takamura.API.Extensions;
using Takamura.Application.Features.Category.AttachSubCategory;

namespace Takamura.API.Endpoints.Category.AttachSubCategory;

public static class AttachSubCategoryEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/{id}", async (AttachSubCategoryService service, int id, AttachSubCategoryRequest input)
            => await service.Handle(input.ToServiceInput(id)).ToHttp())
            .Produces(StatusCodes.Status204NoContent);
    }
}
