using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands.Extract
{
    public class ManuscriptFormatter
    {
        protected StringBuilder Buffer = new StringBuilder();

        public override string ToString()
        {
            return Buffer.ToString();
        }


        public virtual void AddScene(String Text) { throw new NotImplementedException(); }
        public virtual void AddSceneBreak(String Text) { throw new NotImplementedException(); }
        public virtual void BeginChapter(String Title) { throw new NotImplementedException(); }
        public virtual void EndChapter() { throw new NotImplementedException(); }
    }
}
