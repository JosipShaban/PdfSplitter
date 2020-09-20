using Microsoft.AspNetCore.Http;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace PdfSplitter.Repository
{
   public interface IRepo
    {
        public void CleanUp();

        public void CreatePdfFile(IFormFile file);

        public void SplitPdf(PdfDocument sourceFile, int pageCount, string fileName);

        public void CreateZip(string path, int pageCount,string fileName);
      
    }
}
