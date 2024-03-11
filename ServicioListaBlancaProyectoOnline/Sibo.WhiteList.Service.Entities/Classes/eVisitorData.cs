using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.Entities.Classes
{
    public class eVisitorData
    {
        public List<eGeneric> idType { get; set; }
        public List<eGeneric> whoAuthorized { get; set; }
        public List<eGeneric> visitedPerson { get; set; }
        public List<eGeneric> eps { get; set; }
        public eVisitor visitor { get; set; }
    }
}
