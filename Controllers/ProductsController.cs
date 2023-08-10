using CMSProductSystem.Data;
using CMSProductSystem.Models;
using CMSProductSystem.ModelsDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CMSProductSystem.Controllers
{
    public class ProductsController : Controller
    {
        CmsproductSystemContext _db = new CmsproductSystemContext();
        public IActionResult Index()
        {
            Random rnd = new Random();
            List<ProizvodiKategorije> model = (from p in _db.Proizvod
                                               join k in _db.Kategorija
                                               on p.CategoryID equals k.ID
                                               orderby Guid.NewGuid()
                                               select new ProizvodiKategorije { ProizvodPodaci = p, NazivKategorije = k.Naziv }).Take(10).ToList();
            return View(model);
        }


        [Route("api/proizvodi/{id?}")]
        public IActionResult PopisProizvodaApi(int? id)
        {
            List<ProizvodiKategorije> modelDb = (from p in _db.Proizvod
                                               join k in _db.Kategorija
                                               on p.CategoryID equals k.ID
                                               select new ProizvodiKategorije { ProizvodPodaci = p, NazivKategorije = k.Naziv }).ToList();

            //List<Proizvod> modelDb = (from p in _db.Proizvod select p).ToList();

            if (id != 0 && id != null)
            {
                modelDb = modelDb.Where(p => p.ProizvodPodaci.ID.Equals(id)).ToList();
            }

            List<ProizvodDetailViewModel> model =  new List<ProizvodDetailViewModel>(); 

            foreach (var stavkaDb in modelDb)
            {
                ProizvodDetailViewModel stavka = new ProizvodDetailViewModel();
                stavka.ID = stavkaDb.ProizvodPodaci.ID;
                stavka.Naziv = stavkaDb.ProizvodPodaci.Naziv;
                stavka.Opis = stavkaDb.ProizvodPodaci.Opis;
                stavka.Kolicina = stavkaDb.ProizvodPodaci.Kolicina;
                stavka.Cijena = stavkaDb.ProizvodPodaci.Cijena;
                stavka.Slika = stavkaDb.ProizvodPodaci.Slika;
                stavka.KategorijaNaziv = stavkaDb.NazivKategorije;
                model.Add(stavka);
            }


            return Json(model);
        }

        [Authorize(Roles = "Administratori,Korisnici,Operateri")]
        public IActionResult Proizvodi(int page)
        {
            List<ProizvodiKategorije> model = (from p in _db.Proizvod
                                    join k in _db.Kategorija
                                    on p.CategoryID equals k.ID
                                    select new ProizvodiKategorije { ProizvodPodaci = p, NazivKategorije = k.Naziv }).ToList();

            double BrojRedaka = model.Count;
            int BrojStranica = (int)Math.Ceiling(BrojRedaka / 5);

            ViewBag.BrojStranica = BrojStranica;
            int aktivna;

            if (page == 0 || page == 1)
            {
                model = model.Take(5).ToList();
                aktivna = 1;
            }
            else
            {
                model = model.Skip((page - 1) * 5).Take(5).ToList();
                aktivna = page;
            }
            ViewBag.Aktivna = aktivna;

            return View(model);
        }
        [Authorize(Roles = "Administratori,Korisnici,Operateri")]
        public IActionResult KategorijaProizvoda(int CategoryID)
        {

        List<SelectListItem> KategorijaPopis = new List<SelectListItem>
        {
            new SelectListItem { Value = "-1", Text = "Odaberi"}
        };
        KategorijeLista(KategorijaPopis, 0);
            ViewBag.KategorijaPopis=KategorijaPopis;

            List<ProizvodiKategorije> model = (from p in _db.Proizvod
                                               join k in _db.Kategorija
                                               on p.CategoryID equals k.ID
                                               select new ProizvodiKategorije { ProizvodPodaci = p, NazivKategorije = k.Naziv }).ToList();
            if(CategoryID != 0)
            {
                model = model.Where(p => p.ProizvodPodaci.CategoryID == CategoryID).ToList();
            }
            return View(model);
        }

        [Authorize(Roles = "Administratori")]
        public IActionResult CreateProizvod()
        {            
            Proizvod model = new Proizvod();
            KategorijeLista(model.KategorijaLista, 0);
            return View(model);
        }

        [Authorize(Roles = "Administratori")]
        [HttpPost]
        public IActionResult CreateProizvod(Proizvod model)
        {
            KategorijeLista(model.KategorijaLista, 0);

            if (model.CategoryID.ToString() == "-1")
            {
                ModelState.AddModelError("CategoryID", "Kategorija je obavezan izbor");
            }

            if (model.Kolicina.ToString() == "0")
            {
                ModelState.AddModelError("Kolicina", "Količina mora biti veća od 0");
            }

            if (model.Cijena.ToString() == "0")
            {
                ModelState.AddModelError("Cijena", "Cijena mora biti veća od 0");
            }

            if (model.SlikaOdb == null)
            {
                ModelState.AddModelError("SlikaOdb", "Slika je obavezan podatak");
            }

            string uniqueFileName = null;

            if (model.SlikaOdb != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.SlikaOdb.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.SlikaOdb.CopyTo(fileStream);
                }
            }
            model.Slika = "images/" + uniqueFileName;

            if (ModelState.IsValid)
            {
                _db.Proizvod.Add(model);
                _db.SaveChanges();
                return RedirectToAction("Proizvodi");
            }
            return View(model);
        }
        [Authorize(Roles = "Administratori,Korisnici,Operateri")]
        public IActionResult DetailsProizvod(int id)
        {
            ProizvodiKategorije model = (from p in _db.Proizvod
                                               join k in _db.Kategorija
                                               on p.CategoryID equals k.ID
                                         where p.ID.Equals(id)
                                               select new ProizvodiKategorije { ProizvodPodaci = p, NazivKategorije = k.Naziv }).FirstOrDefault();
            return View(model);
        }

        [Authorize(Roles = "Administratori")]
        public IActionResult EditProizvod(int id)
        {
            Proizvod model = (from p in _db.Proizvod                                       
                                         where p.ID.Equals(id)
                                         select p).Single();
            KategorijeLista(model.KategorijaLista, model.CategoryID);
            return View(model);
        }

        [Authorize(Roles = "Administratori")]
        [HttpPost]
        public IActionResult EditProizvod(Proizvod model)
        {
            KategorijeLista(model.KategorijaLista, model.CategoryID);

            if (model.CategoryID.ToString() == "-1")
            {
                ModelState.AddModelError("CategoryID", "Kategorija je obavezan izbor");
            }

            if (model.Kolicina.ToString() == "0")
            {
                ModelState.AddModelError("Kolicina", "Količina mora biti veća od 0");
            }

            if (model.Cijena.ToString() == "0")
            {
                ModelState.AddModelError("Cijena", "Cijena mora biti veća od 0");
            }

            if (model.SlikaOdb == null && (model.Slika.ToString()=="" || model.Slika.ToString() == "images/nophoto.jpg"))
            {
                ModelState.AddModelError("SlikaOdb", "Slika je obavezan podatak");
            }

            string uniqueFileName = null;

            if (model.SlikaOdb != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.SlikaOdb.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.SlikaOdb.CopyTo(fileStream);
                }
                model.Slika = "images/" + uniqueFileName;
            }
            

            if (ModelState.IsValid)
            {
                Proizvod modelDb = (from p in _db.Proizvod
                                  where p.ID.Equals(model.ID)
                                  select p).Single();
                
                modelDb.Naziv = model.Naziv;
                modelDb.Opis = model.Opis;
                modelDb.Slika = model.Slika;                
                modelDb.Kolicina = model.Kolicina;
                modelDb.Cijena = model.Cijena;
                modelDb.CategoryID = model.CategoryID;

                _db.SaveChanges();
                return RedirectToAction("Proizvodi");
            }
            return View(model);
        }

        [Authorize(Roles = "Administratori")]
        public IActionResult DeleteProizvod(int id)
        {
            Proizvod model = (from p in _db.Proizvod
                              where p.ID.Equals(id)
                              select p).Single();
            KategorijeLista(model.KategorijaLista, model.CategoryID);
            return View(model);
        }

        [Authorize(Roles = "Administratori")]
        public IActionResult DeleteProizvodConfirm(int id)
        {
            Proizvod model = (from p in _db.Proizvod
                              where p.ID.Equals(id)
                              select p).Single();
            _db.Remove(model);
            _db.SaveChanges();
            TempData["poruka"] = "Proizvod uspješno izbrisan";
            return RedirectToAction("Proizvodi");
        }

        private void KategorijeLista(List<SelectListItem> lista, int IdKateg)
        {
            var dohvatikategorije = (from k in _db.Kategorija select k).ToList();
            foreach (var kategorija in dohvatikategorije)
            {
                SelectListItem clanListe = new SelectListItem();
                clanListe.Text = kategorija.Naziv;
                clanListe.Value = kategorija.ID.ToString();

                if (clanListe.Value == IdKateg.ToString())
                {
                    clanListe.Selected = true;
                }
                lista.Add(clanListe);
            }
        }
    }
}
