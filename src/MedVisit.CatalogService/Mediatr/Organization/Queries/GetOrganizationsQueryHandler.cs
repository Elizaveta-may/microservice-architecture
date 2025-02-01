using AutoMapper;
using MediatR;
using MedVisit.CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace MedVisit.CatalogService.Mediatr.Organization.Queries
{

    public record GetOrganizationsQuery : IRequest<IEnumerable<OrganizationDto>>;

    public class GetOrganizationsQueryHandler : IRequestHandler<GetOrganizationsQuery, IEnumerable<OrganizationDto>>
    {
        private readonly CatalogDbContext _context;
        private readonly IMapper _mapper;

        public GetOrganizationsQueryHandler(CatalogDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrganizationDto>> Handle(GetOrganizationsQuery query, CancellationToken ct)
        {
            var Organizations = await _context.Organizations.ToListAsync(ct);
            return _mapper.Map<IEnumerable<OrganizationDto>>(Organizations);
        }
    }
}
