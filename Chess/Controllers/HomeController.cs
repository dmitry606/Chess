using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Chess.Repositories;

namespace Chess.Controllers
{
    public class HomeController : Controller
    {
		private IBoardRepository _gameRepository;

		public HomeController(IBoardRepository gameRep)
		{
			_gameRepository = gameRep;
		}

        public ViewResult Index()
        {
            return View();
        }
    }
}