using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands.Extract
{
    public class LatexFormatter : ManuscriptFormatter
    {
        public LatexFormatter()
        {
            Buffer.Append(
@"\makeatletter
\newcommand{\scene}[1]{%
  \par\nobreak\@afterheading
  \bgroup
  \begin{center}\addvspace{\topsep}#1\end{center}
  \egroup\@afterindentfalse\@afterheading
}
\makeatother

\documentclass[oneside]{book}
\begin{document}
\tableofcontents
\pagenumbering{arabic}


");
        }

        public override string ToString()
        {
            Buffer.Append("\n\\end{document}\n");
            return Buffer.ToString();
        }

        public override void BeginChapter(string Title)
        {
            Buffer.AppendFormat("\n\\chapter{{{0}}}\n", Title);
        }

        public override void EndChapter()
        {
        }

        public override void AddScene(string Text)
        {
            Buffer.Append(Text.Trim('\n','\r').Replace("\n","\n\n"));
        }

        public override void AddSceneBreak(string Text)
        {
            Buffer.AppendFormat("\n\\scene{{{0}}}\n", Text);
        }
    }
}
