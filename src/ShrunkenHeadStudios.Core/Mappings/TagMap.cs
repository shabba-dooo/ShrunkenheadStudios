using FluentNHibernate.Mapping;
using ShrunkenHeadStudios.Core.Objects;

namespace ShrunkenHeadStudios.Core.Mappings
{
    //Used to map Tags to Tags table and it's columns
    public class TagMap : ClassMap<Tag>
    {
        public TagMap()
        {
            Id(x => x.ID);
            Map(x => x.Name).Length(50).Not.Nullable();
            Map(x => x.UrlSlug).Length(50).Not.Nullable();
            Map(x => x.Description).Length(200);
            HasManyToMany(x => x.Posts).Cascade.All().Inverse().Table("PostTagMap");
        }
    }
}
