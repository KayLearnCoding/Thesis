// 本题为考试多行输入输出规范示例，无需提交，不计分。
using System;
public class Program
{
    public static void Main()
    {
        int n = int.Parse(Console.ReadLine().Trim());
        int N = 0, K = 0;
        int ans = 0;
        for (int i = 0; i < n; i++)
        {
            string[] inputs = Console.ReadLine().Trim().Split(' ');

            N = Convert.ToInt32(inputs[0]);
            K = Convert.ToInt32(inputs[1]);
        }
        Console.WriteLine(N + ',' + K);
    }
}