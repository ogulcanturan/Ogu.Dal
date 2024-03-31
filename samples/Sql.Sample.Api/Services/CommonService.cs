using Sql.Sample.Api.Domain.UnitOfWork;
using Sql.Sample.Api.Services.Interfaces;

namespace Sql.Sample.Api.Services
{
    internal sealed class CommonService : ICommonService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CommonService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}