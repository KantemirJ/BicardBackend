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
                .ForMember(dest => dest.PathToPhoto, opt => opt.MapFrom(src => MapPhotoPath(src.Photo)))
                // Add other mappings as needed
                .ReverseMap();

            // Add more mappings for other DTOs and entities as needed
        }

        private string MapPhotoPath(IFormFile photo)
        {
            return photo != null ? _fileService.SaveFileAsync(photo, "PhotosOfDoctors").Result : null;
        }
        // Parameterless constructor added for AutoMapper
        public MappingProfile()
        {
        }
    }
}
