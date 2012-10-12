using System;
using System.IO;
using System.Text;
using Helpers;

namespace Demo
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string text1 = GetFileContent("file1.txt");
            string text2 = GetFileContent("file2.txt");

            this.text1Literal.Text = text1;
            this.text2Literal.Text = text2;
            this.resultLiteral.Text = HtmlDiff.Execute(text1, text2);
        }

        private string GetFileContent(string fileName)
        {
            var localFile = Server.MapPath("files\\" + fileName);
            StreamReader reader = new StreamReader(localFile, Encoding.UTF8);
            var content = reader.ReadToEnd();
            reader.Close();
            reader.Dispose();
            if (!string.IsNullOrEmpty(content))
            {
                content = content.Replace(" ", "&nbsp;");
                content = content.Replace("\n", "<br/>");
            }
            return content;
        }
    }
}
