using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ChessAPI.Controllers
{
    public class Version
    {
        public string name = "ChessAPI";
        public string version = "0.1";
    }

    public class VersionController : ApiController
    {
        public Version GetVersion()
        {
            return new Version();
        }
    }
}
