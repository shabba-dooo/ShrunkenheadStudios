using FluentNHibernate.Mapping;
using ShrunkenHeadStudios.Core.Objects;

namespace ShrunkenHeadStudios.Core.Mappings
{
    //Used to map Category to Category table and it's columns
    public class CategoryMap : ClassMap<Category>
    {
        public CategoryMap()
        {
            Id(x => x.ID);
            Map(x => x.Name).Length(50).Not.Nullable();
            Map(x => x.UrlSlug).Length(50).Not.Nullable();
            Map(x => x.Description).Length(200);
            HasMany(x => x.Posts).Inverse().Cascade.All().KeyColumn("Category");
        }
    }
}
