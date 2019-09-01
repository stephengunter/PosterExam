using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.DtoMapper
{
	public class MappingConfig
	{
		public static MapperConfiguration CreateConfiguration()
		{
			var mappingConfig = new MapperConfiguration(mc =>
			{
				mc.AddProfile(new TermMappingProfile());
				mc.AddProfile(new SubjectMappingProfile());
				mc.AddProfile(new QuestionMappingProfile());
			});

			return mappingConfig;
		}
	}
}
