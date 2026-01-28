using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.DTO
{
    public class PrintDocumentDto
    {
       public string FileName { get; set; }
	   public bool IsDraftMode { get; set; }
	   public string Attachment { get; set; }
	   public string FilePath { get; set; }
    }
}