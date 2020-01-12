using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApplicationCore.DtoMapper
{
	public class MappingConfig
	{
		public static MapperConfiguration CreateConfiguration()
		{
			return new MapperConfiguration(cfg => {
				cfg.AddMaps(typeof(TermMappingProfile).Assembly);
			});
			//var mappingConfig = new MapperConfiguration(cfg =>
			//{
			//	//foreach (var profile in profiles)
			//	//{
			//	//	var resolvedProfile = container.Resolve(profile) as Profile;
			//	//	cfg.AddProfile(resolvedProfile);
			//	//}
			//	mc.AddProfile(new TermMappingProfile());
			//	mc.AddProfile(new SubjectMappingProfile());
			//	mc.AddProfile(new QuestionMappingProfile());
			//	mc.AddProfile(new OptionMappingProfile());
			//	mc.AddProfile(new ResolveMappingProfile());
			//	mc.AddProfile(new RecruitMappingProfile());
			//	mc.AddProfile(new UploadFileMappingProfile());
			//});


			//return mappingConfig;
		}

	}

	
}
