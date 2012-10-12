using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FindLCS
{
    class Program
    {
        static void Main22(string[] args)
        {
            PrintLCSLength();
            PrintLCS();

            Console.ReadLine();
        }

        /// <summary>
        /// 输出str2在str1中的公共子序列的长度
        /// </summary>
        private static void PrintLCSLength()
        {
            //输出3，LCS:out
            Console.WriteLine(GetLCSLength("computer", "houseboat"));

            //输出4，LCS:comp
            Console.WriteLine(GetLCSLength("computer", "compabd"));

            //输出4，LCS:omte
            Console.WriteLine(GetLCSLength("computer", "aomcdtec"));
        }
        /// <summary>
        /// 输出str2在str1中的公共子序列
        /// </summary>
        private static void PrintLCS()
        {
            string str1;
            string str2;

            //输出:omte
            str1 = "computer"; str2 = "houseboat";
            Console.WriteLine(BackTrack(str1, str2, str1.Length, str2.Length, GetLCSMatrix(str1, str2)).First());

            //输出:comp
            str1 = "computer"; str2 = "compabd";
            Console.WriteLine(BackTrack(str1, str2, str1.Length, str2.Length, GetLCSMatrix(str1, str2)).First());

            //输出:omte
            str1 = "computer"; str2 = "aomcdtec";
            Console.WriteLine(BackTrack(str1, str2, str1.Length, str2.Length, GetLCSMatrix(str1, str2)).First());
        }

        private static int GetLCSLength(string str1, string str2)
        {
            return GetLCSMatrix(str1, str2)[str1.Length, str2.Length];
        }
        private static int[,] GetLCSMatrix(string str1, string str2)
        {
            int[,] table = new int[str1.Length + 1, str2.Length + 1];

            if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
            {
                return table;
            }

            for (int i = 0; i < table.GetLength(0); i++)
            {
                table[i, 0] = 0;
            }
            for (int j = 0; j < table.GetLength(1); j++)
            {
                table[0, j] = 0;
            }

            for (int i = 1; i < table.GetLength(0); i++)
            {
                for (int j = 1; j < table.GetLength(1); j++)
                {
                    if (str1[i - 1] == str2[j - 1])
                        table[i, j] = table[i - 1, j - 1] + 1;
                    else
                    {
                        if (table[i, j - 1] > table[i - 1, j])
                            table[i, j] = table[i, j - 1];
                        else
                            table[i, j] = table[i - 1, j];
                    }
                }
            }

            return table;
        }
        private static string BackTrackLCS(string s1, string s2, int i, int j, int[,] matrix)
        {
            if (i == 0 || j == 0)
            {
                return string.Empty;
            }
            if (s1[i - 1] == s2[j - 1])
            {
                return BackTrackLCS(s1, s2, i - 1, j - 1, matrix) + s1[i - 1];
            }
            else if (matrix[i - 1, j] > matrix[i, j - 1])
            {
                return BackTrackLCS(s1, s2, i - 1, j, matrix);
            }
            else
            {
                return BackTrackLCS(s1, s2, i, j - 1, matrix);
            }
        }

        private static SortedSet<string> BackTrack(string s1, string s2, int i, int j, int[,] matrix)
        {
            if (i == 0 || j == 0)
            {
                return new SortedSet<string>() { "" };
            }
            else if (s1[i - 1] == s2[j - 1])
            {
                SortedSet<string> temp = new SortedSet<string>();
                SortedSet<string> holder = BackTrack(s1, s2, i - 1, j - 1, matrix);
                foreach (string str in holder)
                {
                    temp.Add(str + s1[i - 1]);
                }
                return temp;
            }
            else
            {
                SortedSet<string> Result = new SortedSet<string>();
                if (matrix[i - 1, j] >= matrix[i, j - 1])
                {
                    SortedSet<string> holder = BackTrack(s1, s2, i - 1, j, matrix);
                    foreach (string s in holder)
                    {
                        Result.Add(s);
                    }
                }

                if (matrix[i, j - 1] >= matrix[i - 1, j])
                {
                    SortedSet<string> holder = BackTrack(s1, s2, i, j - 1, matrix);
                    foreach (string s in holder)
                    {
                        Result.Add(s);
                    }
                }

                return Result;
            }
        }
    }
}


//Reading Out All LCS sorted in lexicographic order
namespace LambdaPractice
{
    class Program
    {
       static int[,] c;
 
       static int max(int a, int b)
        {
            return (a > b) ? a : b;
        }
 
       static int LCS(string s1, string s2)
        {
            for (int i = 1; i <= s1.Length; i++)
                c[i,0] = 0;
            for (int i = 1; i <= s2.Length; i++)
                c[0, i] = 0;
 
            for (int i=1;i<=s1.Length;i++)
                for (int j = 1; j <= s2.Length; j++)
                {
                    if (s1[i-1] == s2[j-1])
                        c[i, j] = c[i - 1, j - 1] + 1;
                    else
                    {
                        c[i, j] = max(c[i - 1, j], c[i, j - 1]);
 
                    }
 
                }
 
            return c[s1.Length, s2.Length]; 
 
        }

       static string BackTrack(string s1, string s2, int i, int j)
        {
 
            if (i == 0 || j == 0)
                return "";
            if (s1[i - 1] == s2[j - 1])
                return BackTrack(s1, s2, i - 1, j - 1) + s1[i - 1];
            else if (c[i - 1, j] > c[i, j - 1])
                return BackTrack(s1, s2, i - 1, j);
            else
                return BackTrack(s1, s2, i, j - 1);
 
        }
 
       static SortedSet<string> backtrack(string s1, string s2, int i, int j)
       {
           Debug.WriteLine(string.Format("Calling backtrack. s1:{0},s2:{1},i:{2},j:{3}",s1, s2, i, j));
           if (i == 0 || j == 0)
               return new SortedSet<string>(){""} ;
           else if (s1[i - 1] == s2[j - 1])
           {
               SortedSet<string> temp = new SortedSet<string>();
               SortedSet<string> holder = backtrack(s1, s2, i - 1, j - 1);
               foreach (string str in holder)
               {
                   temp.Add(str + s1[i - 1]);
                   Debug.WriteLine(string.Format("temp.Add:{0}", str + s1[i - 1]));
               }
 
               return temp;
           }
           else
           {
               SortedSet<string> Result = new SortedSet<string>() ;
               if (c[i - 1, j] >= c[i, j - 1])
               {
                   SortedSet<string> holder = backtrack(s1, s2, i - 1, j);

                   foreach (string s in holder)
                   {
                       Result.Add(s);
                       Debug.WriteLine(string.Format("Result.Add:{0}", s));
                   }
               }
 
               if (c[i, j - 1] >= c[i - 1, j])
               {
                   SortedSet<string> holder = backtrack(s1, s2, i, j - 1);

                   foreach (string s in holder)
                   {
                       Result.Add(s);
                       Debug.WriteLine(string.Format("Result.Add:{0}", s));
                   }
               }
 
 
               return Result;
           }
 
       }
 
        static void Main(string[] args)
        {
                string s1, s2;
                s1 = Console.ReadLine();
                s2 = Console.ReadLine();
                c = new int[s1.Length+1, s2.Length+1];
                LCS(s1, s2);
                Console.WriteLine(BackTrack(s1, s2, s1.Length, s2.Length));
                Console.WriteLine(s1.Length);
                SortedSet<string> st = backtrack(s1, s2, s1.Length, s2.Length);
 
                foreach (string str in st)
                    Console.WriteLine(str);
                GC.Collect();
 
           Console.ReadLine();
        }
    }
}
