using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands.Extract
{
    public class PlainTextFormatter : ManuscriptFormatter
    {
        public override void BeginChapter(string Title)
        {
            Buffer.AppendFormat("\n\n\n\n{0}\n\n\n", Title);
        }

        public override void EndChapter()
        {

        }

        public override void AddScene(string Text)
        {
            Buffer.Append(Text.Trim('\n','\r'));
        }

        public override void AddSceneBreak(string Text)
        {
            Buffer.AppendFormat("\n\n{0}\n\n", Text);
        }
    }
}
