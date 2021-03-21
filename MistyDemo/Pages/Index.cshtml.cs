using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MistyDemo.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MistyDemo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public MDevice Device {get; set;}

        // INJECT DEVICE SINGLETON USED BY BACKGROUND SERVICES FOR USE IN PAGE RENDERING...

        public IndexModel(ILogger<IndexModel> logger, MDevice device)
        {
            _logger = logger;
            Device = device;
        }

        public void OnGet()
        {
          
        }
    }
}
