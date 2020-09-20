using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PdfSplitter.Models
{
    public class UploadFile
    {
        [RegularExpression(@"^.+\.(([pP][dD][fF]))$", ErrorMessage = "Incorrect file format")]
        [Required]
        public IFormFile _File { get; set; }
    }
}
