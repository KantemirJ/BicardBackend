using BicardBackend.Models;
using BicardBackend.DTOs;
using AutoMapper;
using BicardBackend.Services;

namespace BicardBackend.Profiles
{
    public class MappingProfile : Profile
    {
        private readonly FileService _fileService;
        public MappingProfile(FileService fileService)
        {
            _fileService = fileService;
            CreateMap<DoctorDto, Doctor>()
                .ForMember(dest => dest.PathToPhoto, opt => opt.MapFrom(src => src.Photo != null ? _fileService.SaveFileAsync(src.Photo, "PhotosOfDoctors") : null))
                // Add other mappings as needed
                .ReverseMap();

            // Add more mappings for other DTOs and entities as needed
        }
    }
}
