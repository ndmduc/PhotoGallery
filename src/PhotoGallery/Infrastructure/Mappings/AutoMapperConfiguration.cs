using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoGallery.Infrastructure.Mappings
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            AutoMapper.Mapper.Initialize(x =>
            {
                x.AddProfile<DomainToViewModelMappingProfile>();
            });
        }
    }
}
