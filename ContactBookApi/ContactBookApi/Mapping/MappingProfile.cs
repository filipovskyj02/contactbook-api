using AutoMapper;
using ContactBookApi.Domain;
using ContactBookApi.Dtos.Contact;

namespace ContactBookApi.Mapping;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Contact, GetContactDto>();
    }
}