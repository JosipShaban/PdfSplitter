using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace PdfSplitter.Repository
{
    public class Repo : IRepo
    {
        private readonly IWebHostEnvironment _env;

        public Repo(IWebHostEnvironment env)
        {
            _env = env;
        }



       

        public void CleanUp()
        {
            string dir = _env.ContentRootPath;

            // getting the directory
            DirectoryInfo di = new DirectoryInfo(dir);

            //getting the zipfiles
            FileInfo[] zipFiles = di.GetFiles("*.zip")
                                 .Where(p => p.Extension == ".zip" ).ToArray();

            //deleting the zipfiles
            foreach (FileInfo file in zipFiles)
            {
                file.Attributes = FileAttributes.Normal;
                System.IO.File.Delete(file.FullName);
            }

            //getting the pdf files
            FileInfo[] pdfFiles = di.GetFiles("*.pdf")
                                 .Where(p => p.Extension == ".pdf").ToArray();

            //deleting the pdf files
            foreach (FileInfo file in pdfFiles)
            {
                file.Attributes = FileAttributes.Normal;
                System.IO.File.Delete(file.FullName);
            }



        }

        public void CreatePdfFile(IFormFile file)
        {
            //creating a path and the pdf file on path
            string path = Path.Combine(_env.ContentRootPath, file.FileName);

            using (var stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                file.CopyTo(stream);
            };
        }

        public void CreateZip(string path, int pageCount,string fileName)
        {
            //making a list for split pdf pages
            List<PdfDocument> splitSharpPdfs = new List<PdfDocument>();

            //adding the pages to the list
            for (int i = 0; i < pageCount; i++)
            {

                PdfDocument sharpPdf = PdfReader.Open($"{path}-Page{i + 1}.pdf");

                splitSharpPdfs.Add(sharpPdf);


            }

            //adding the pages to the zipfile
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var pdf in splitSharpPdfs)
                    {
                        archive.CreateEntryFromFile(pdf.FullPath, Path.GetFileName(pdf.FullPath));

                    }
                    using (var fileStream = new FileStream(fileName + ".zip", FileMode.Create))
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        memoryStream.CopyTo(fileStream);
                    }
                }
            }

            //removing the pages
            foreach (var pdf in splitSharpPdfs)
            {
                System.IO.File.Delete(pdf.FullPath);
            }
        }

        public void SplitPdf(PdfDocument sourceFile, int pageCount,string fileName)
        {

            for (int i = 0; i < pageCount; i++)
            {   //where the old pdf is located
                string path = Path.Combine(_env.ContentRootPath, fileName);
                //new single page pdf
                PdfDocument singleDocument = new PdfDocument();
                //adding the page from old pff
                singleDocument.AddPage(sourceFile.Pages[i]);
                //saving it to app
                singleDocument.Save($"{path}-Page{i + 1}.pdf");

            }
        }
    }
}
