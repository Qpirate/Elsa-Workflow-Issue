using AutoMapper;

namespace EM.AM.Issue.Profiles
{
	public class ClassA
	{
		public string Foo { get; set; }
	}
	public class ClassB
	{
		public string Bar { get; set; }
	}

	public class ClassAClassBProfile : Profile
	{
		public ClassAClassBProfile()
		{
			CreateMap<ClassA, ClassB>(MemberList.Destination)
			.ForMember(m => m.Bar, opts => opts.MapFrom(src => src.Foo));
		}
	}
}
