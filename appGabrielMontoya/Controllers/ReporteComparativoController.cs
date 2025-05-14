using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using appGabrielMontoya.Data;

namespace appGabrielMontoya.Controllers
{
    public class ReporteComparativoController : Controller
    {
        private readonly AppDBContext _appDBContext;

        public ReporteComparativoController(AppDBContext context)
        {
            _appDBContext = context;
        }

       
    }
}
