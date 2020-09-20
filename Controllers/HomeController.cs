using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using PdfSplitter.Models;
using PdfSplitter.Repository;

namespace PdfSplitter.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IRepo _repo;

        public HomeController( IWebHostEnvironment env,IRepo repo)
        {
            _env = env;
            _repo = repo;
        }

        public ActionResult Index()

        {
           //Cleans up any remaining  files
            _repo.CleanUp();
            
            return View( new UploadFile());
        }

        [HttpPost]
        public ActionResult Upload(UploadFile file)
        {


            //path to our file that we have uploaded

            var dir = _env.ContentRootPath;

            string path = Path.Combine(dir, file._File.FileName);


          
            //creating the uploaded file as pdf

            


            try
            {
                _repo.CreatePdfFile(file._File);

                //reading the pdf           
                PdfDocument pdfDocument = PdfReader.Open(path, PdfDocumentOpenMode.Import);



                //neccessary info that will be needed as paramaters
                int pageCount = pdfDocument.PageCount;

                string fileName = file._File.FileName;



                //storing the info for the next action
                TempData["pageCount"] = pageCount;

                TempData["fileName"] = file._File.FileName;



                //spliting the pdf to single-page pdfs
                _repo.SplitPdf(pdfDocument, pageCount, fileName);
            }
            catch (Exception)
            {

                return View("Error");
            }
            



            //deleting the first file - not needed anymore
            System.IO.File.Delete(path);



            return RedirectToAction("Download");
        }

        public ActionResult Download()
        {


            //neccessary info again
            var dir = _env.ContentRootPath;
            
            string fileName =(string) TempData["fileName"];

            int pageCount = (int)TempData["pageCount"];

            string path = Path.Combine(dir, fileName);


            //creating a zipfile from split pages and then removing them
            _repo.CreateZip(path, pageCount, fileName);
            
            
            //sending the name of the original file
            return View("Download",fileName + ".zip");
            

        }
        
        public ActionResult Link(string fileName)
        {

            var dir = _env.ContentRootPath;


            //reading the zipfile, removing it from the app and sending it to the user

            try
            {   //read
                byte[] fileBytes = System.IO.File.ReadAllBytes(fileName);
                //delete
                System.IO.File.Delete(fileName);
                //send
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception)
            {

                return View("Index");
            }
            

            
        }
    }
}
